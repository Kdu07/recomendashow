import os
import sys
import warnings
import numpy as np
import pandas as pd
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import seaborn as sns
import joblib

from surprise import SVD, Dataset, Reader, accuracy
from surprise.model_selection import train_test_split, GridSearchCV

warnings.filterwarnings("ignore")
np.random.seed(42)
sns.set_theme(style="whitegrid")

print("Bibliotecas carregadas com sucesso.")

# --- Carrega dados ---
PROJECT_ROOT = r"c:\Users\cadus\CADU_FILES\Inteli\Projeto 6 - Aventura\g02"
script_dir = os.path.join(PROJECT_ROOT, "src", "aad")
dataset_path = os.path.join(PROJECT_ROOT, "src", "dataset", "dataset.csv")
df = pd.read_csv(dataset_path)

print(f"Dimensão: {df.shape[0]:,} linhas × {df.shape[1]} colunas")
print(f"\nDistribuição do ranking:\n{df['ranking'].value_counts().sort_index()}")
print(f"\nEscala: {df['ranking'].min()} a {df['ranking'].max()}")
print(f"\nUsuários únicos  : {df['user_id'].nunique():,}")
print(f"Espetáculos únicos: {df['item_id'].nunique():,}")

n_users = df["user_id"].nunique()
n_items = df["item_id"].nunique()
sparsity = (1 - len(df) / (n_users * n_items)) * 100
print(f"Esparsidade da matriz: {sparsity:.2f}%")

# --- Prepara dataset para Surprise ---
reader = Reader(rating_scale=(1, 5))
data = Dataset.load_from_df(df[["user_id", "item_id", "ranking"]], reader)
print("\nDataset carregado no formato Surprise.")

trainset, testset = train_test_split(data, test_size=0.20, random_state=42)
print(f"Treino: {trainset.n_ratings:,} | Teste: {len(testset):,}")

# --- GridSearchCV ---
print("\nIniciando GridSearchCV (n_jobs=1)...")
param_grid = {
    "n_factors": [20, 50, 100],
    "lr_all":    [0.002, 0.005, 0.010],
    "reg_all":   [0.02, 0.05, 0.10],
    "n_epochs":  [20],
}

gs = GridSearchCV(SVD, param_grid, measures=["rmse", "mae"], cv=3, n_jobs=1, joblib_verbose=0, refit=False)
gs.fit(data)

print("Melhores hiperparâmetros (RMSE):")
print(gs.best_params["rmse"])
print(f"\nMelhor RMSE (CV): {gs.best_score['rmse']:.4f}")
print(f"Melhor MAE  (CV): {gs.best_score['mae']:.4f}")

# --- Modelo final ---
best_params = gs.best_params["rmse"]
modelo_svd = SVD(
    n_factors=best_params["n_factors"],
    lr_all=best_params["lr_all"],
    reg_all=best_params["reg_all"],
    n_epochs=50,
    random_state=42,
    verbose=False,
)
full_trainset = data.build_full_trainset()
modelo_svd.fit(full_trainset)

print("\nModelo SVD treinado no dataset completo.")
print(f"  n_factors : {modelo_svd.n_factors}")
print(f"  lr_all    : {best_params['lr_all']}")
print(f"  reg_all   : {best_params['reg_all']}")
print(f"  n_epochs  : {best_params['n_epochs']}")

# --- Salva modelo ---
models_dir = os.path.join(PROJECT_ROOT, "src", "models")
os.makedirs(models_dir, exist_ok=True)
modelo_path = os.path.join(models_dir, "svd_model.joblib")
joblib.dump(modelo_svd, modelo_path)
print(f"\nModelo salvo em: {modelo_path}")

# --- Avaliação ---
modelo_avaliacao = SVD(
    n_factors=best_params["n_factors"],
    lr_all=best_params["lr_all"],
    reg_all=best_params["reg_all"],
    n_epochs=50,
    random_state=42,
    verbose=False,
)
modelo_avaliacao.fit(trainset)
predicoes = modelo_avaliacao.test(testset)

rmse_val = accuracy.rmse(predicoes, verbose=False)
mae_val  = accuracy.mae(predicoes, verbose=False)
print(f"\n=== Métricas no Conjunto de Teste (20%) ===")
print(f"RMSE : {rmse_val:.4f}")
print(f"MAE  : {mae_val:.4f}")

# --- Plots ---
erros = [pred.r_ui - pred.est for pred in predicoes]
fig, axes = plt.subplots(1, 2, figsize=(14, 5))
axes[0].hist(erros, bins=40, color="#4C78A8", edgecolor="white", alpha=0.85)
axes[0].axvline(0, color="red", linestyle="--", linewidth=1.5, label="Erro zero")
axes[0].set_title("Distribuição dos Erros de Predição")
axes[0].set_xlabel("Erro (real − predito)")
axes[0].set_ylabel("Frequência")
axes[0].legend()

reais    = [pred.r_ui for pred in predicoes]
preditos = [pred.est  for pred in predicoes]
axes[1].scatter(reais, preditos, alpha=0.05, s=10, color="#59A14F")
lim = (min(min(reais), min(preditos)) - 0.1, max(max(reais), max(preditos)) + 0.1)
axes[1].plot(lim, lim, "r--", linewidth=1.5, label="Predição perfeita")
axes[1].set_title("Rating Real vs. Predito")
axes[1].set_xlabel("Rating Real")
axes[1].set_ylabel("Rating Predito")
axes[1].legend()

plt.tight_layout()
plots_path = os.path.join(PROJECT_ROOT, "src", "aad", "svd_plots.png")
plt.savefig(plots_path, dpi=100)
print(f"Gráficos salvos em: {plots_path}")

print(f"\nMédia do erro  : {np.mean(erros):.4f}")
print(f"Desvio padrão  : {np.std(erros):.4f}")

# --- Top-N recomendações ---
def recomendar_top_n(modelo, df, user_id, n=10):
    itens_consumidos = set(df[df["user_id"] == user_id]["item_id"])
    itens_nao_vistos = [item for item in df["item_id"].unique() if item not in itens_consumidos]
    preds = [(item, modelo.predict(user_id, item).est) for item in itens_nao_vistos]
    preds.sort(key=lambda x: x[1], reverse=True)
    return pd.DataFrame(preds[:n], columns=["espetáculo", "score_predito"])

usuario_exemplo = (
    df.groupby("user_id").size().reset_index(name="n_interacoes")
    .query("n_interacoes >= 3")
    .sample(1, random_state=42)["user_id"].values[0]
)
historico = df[df["user_id"] == usuario_exemplo][["item_id", "ranking"]]
print(f"\n=== Usuário {usuario_exemplo} — Histórico ({len(historico)} itens) ===")
print(historico.to_string(index=False))
print(f"\nTop-10 Recomendações:")
print(recomendar_top_n(modelo_svd, df, usuario_exemplo, n=10).to_string(index=False))

# --- Fatores latentes ---
qi = modelo_svd.qi
indice_para_item = {v: full_trainset.to_raw_iid(v) for v in full_trainset.all_items()}
print("\n=== Espetáculos com maior projeção por fator latente ===\n")
for fator in range(3):
    scores = [(indice_para_item[i], qi[i, fator]) for i in full_trainset.all_items()]
    scores.sort(key=lambda x: x[1], reverse=True)
    print(f"Fator {fator + 1} — Top 5:")
    for nome, score in scores[:5]:
        print(f"  {nome[:65]:<65} ({score:.3f})")
    print()

print("=== Script concluído com sucesso! ===")
