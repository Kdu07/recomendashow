"""Busca e exibe os dados da tabela `usuarios` do banco PostgreSQL.

Lê as credenciais do arquivo .env na raiz do projeto. Uso:
    python scripts/fetch_usuarios.py
"""
import os
import sys
from pathlib import Path

import psycopg2
from psycopg2.extras import RealDictCursor


def load_env(env_path: Path) -> dict:
    """Lê pares CHAVE=VALOR de um arquivo .env simples."""
    env = {}
    for line in env_path.read_text(encoding="utf-8").splitlines():
        line = line.strip()
        if not line or line.startswith("#") or "=" not in line:
            continue
        key, _, value = line.partition("=")
        env[key.strip()] = value.strip()
    return env


def main() -> int:
    root = Path(__file__).resolve().parents[1]
    env = load_env(root / ".env")

    conn = psycopg2.connect(
        host=env["POSTGRES_HOST"],
        port=env.get("POSTGRES_PORT", "5432"),
        dbname=env["POSTGRES_DB"],
        user=env["POSTGRES_USER"],
        password=env["POSTGRES_PASSWORD"],
        connect_timeout=10,
    )
    try:
        with conn.cursor(cursor_factory=RealDictCursor) as cur:
            cur.execute(
                "SELECT id, nome, email, idade, ml_id, data_cadastro "
                "FROM usuarios ORDER BY data_cadastro;"
            )
            rows = cur.fetchall()

        print(f"Total de usuarios: {len(rows)}\n")
        for r in rows:
            print(
                f"- {r['nome']} <{r['email']}> | idade={r['idade']} "
                f"| ml_id={r['ml_id']} | cadastro={r['data_cadastro']}"
            )
            print(f"  id: {r['id']}")
    finally:
        conn.close()
    return 0


if __name__ == "__main__":
    sys.exit(main())
