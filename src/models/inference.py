import pathlib
import joblib
import numpy as np
import pandas as pd

_HERE = pathlib.Path(__file__).resolve().parent
_modelo = joblib.load(_HERE / "svd_model.joblib")
_df = pd.read_csv(_HERE.parent / "dataset" / "dataset.csv")
_todos_itens = set(_df["item_id"].unique())

# Pré-computado uma vez na inicialização: evita groupby/sort por chamada cold-start
_COLD_START_ITEMS: list[str] = (
    _df.groupby("item_id")["user_id"]
    .count()
    .sort_values(ascending=False)
    .index.tolist()
)


def recomendar(user_id: int, n: int = 10) -> list[dict]:
    """
    Retorna top-n recomendações para um usuário.

    Parâmetros:
        user_id : identificador inteiro do usuário (campo user_id do dataset)
        n       : número de itens a retornar (padrão 10)

    Retorno:
        lista de dicts {"espetaculo": str, "score": float | None}
        ordenada por score decrescente.
        score=None indica fallback por popularidade (cold-start).
        Se n > número de itens não vistos, retorna todos os disponíveis sem erro.

    Exceções:
        ValueError : se n <= 0
        TypeError  : se user_id não for int ou n não for int
    """
    if not isinstance(user_id, (int, np.integer)) or isinstance(user_id, bool):
        raise TypeError(f"user_id deve ser int, recebido {type(user_id).__name__}")
    if not isinstance(n, int) or isinstance(n, bool):
        raise TypeError(f"n deve ser int, recebido {type(n).__name__}")
    if n <= 0:
        raise ValueError("n deve ser maior que zero")

    consumidos = set(_df[_df["user_id"] == user_id]["item_id"])

    # Cold-start: usuário desconhecido ou sem histórico → top-n por popularidade
    if not consumidos:
        return [{"espetaculo": item, "score": None} for item in _COLD_START_ITEMS[:n]]

    ts = _modelo.trainset
    try:
        inner_uid = ts.to_inner_uid(user_id)
    except ValueError:
        return [{"espetaculo": item, "score": None} for item in _COLD_START_ITEMS[:n]]

    # Vetorizado: uma multiplicação matricial BLAS substitui 447 chamadas Cython
    # Nota: usa atributos internos do surprise (pu, qi, bu, bi) — não é API pública
    all_scores = (
        ts.global_mean
        + _modelo.bu[inner_uid]
        + _modelo.bi
        + _modelo.qi @ _modelo.pu[inner_uid]
    ).clip(ts.rating_scale[0], ts.rating_scale[1])

    resultados = [
        (ts.to_raw_iid(i), float(all_scores[i]))
        for i in range(ts.n_items)
        if ts.to_raw_iid(i) not in consumidos
    ]
    resultados.sort(key=lambda x: x[1], reverse=True)
    return [{"espetaculo": item, "score": round(s, 4)} for item, s in resultados[:n]]
