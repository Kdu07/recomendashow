"""
Benchmark Python: SVD híbrido equivalente ao modelo C# ML.NET.

Como o ML.NET não expõe os pesos internos do MatrixFactorization diretamente,
este script replica a COMPUTAÇÃO com numpy (mesmas dimensões: 50 fatores, 449 itens).
Os pesos são aleatórios com a mesma distribuição usada pelo ML.NET (normal 0, 0.1).
O objetivo é medir o custo computacional da inferência, não a acurácia.
"""

import json
import time
import numpy as np
import pandas as pd
from pathlib import Path

ROOT       = Path(__file__).parent.parent
DATASET    = ROOT / "dataset" / "dataset.csv"
ITEM_CATS  = ROOT / "models" / "item_categories.json"
CAT_AFF    = ROOT / "models" / "category_affinity.json"
COLD_START = ROOT / "models" / "cold_start.json"

N_FACTORS = 50
N_REQUESTS = 500
TOP_K = 10
ALPHA = 0.7

# ── Startup ──────────────────────────────────────────────────────────────────
t0 = time.perf_counter()

df = pd.read_csv(DATASET)
df["item_id"]     = df["item_id"].str.strip()
df["tipo_evento"] = df["tipo_evento"].str.strip()

with open(ITEM_CATS)  as f: item_categories   = json.load(f)
with open(CAT_AFF)    as f: category_affinity  = json.load(f)
with open(COLD_START) as f: all_items          = json.load(f)  # lista ordenada

all_items_arr = np.array(all_items)
item2idx      = {item: i for i, item in enumerate(all_items)}
item_cat_arr  = np.array([item_categories.get(item, "") for item in all_items])

# Pesos aleatórios equivalentes ao modelo treinado (mesmas dimensões)
rng  = np.random.default_rng(42)
n_items = len(all_items)
n_users = df["user_id"].nunique()

P  = rng.normal(0, 0.1, (n_users, N_FACTORS)).astype(np.float32)  # fatores de usuário
Q  = rng.normal(0, 0.1, (n_items, N_FACTORS)).astype(np.float32)  # fatores de item
bu = rng.normal(0, 0.1, n_users).astype(np.float32)
bi = rng.normal(0, 0.1, n_items).astype(np.float32)
global_mean = float(df["ranking"].mean())

user_history = (
    df.groupby("user_id")["item_id"]
    .apply(set)
    .to_dict()
)

user_ids_with_history = [
    uid for uid, items in user_history.items() if len(items) >= 2
]

# Mapeia user_id real → índice interno (garante bounds corretos)
unique_users = sorted(df["user_id"].unique())
user2idx = {uid: i for i, uid in enumerate(unique_users)}

startup_ms = (time.perf_counter() - t0) * 1000
print(f"=== Benchmark Python — numpy SVD (n={N_REQUESTS}, top-{TOP_K}) ===\n")
print(f"  Startup (load CSV + JSON + alocar matrizes): {startup_ms:.1f} ms")
print(f"  Itens: {n_items}  |  Usuários: {n_users}  |  Fatores: {N_FACTORS}\n")


# ── Função de inferência ──────────────────────────────────────────────────────
def recommend(user_id: int, n: int = TOP_K):
    uidx = user2idx[user_id]
    consumed = user_history.get(user_id, set())

    # Máscara de itens não consumidos
    mask = np.array([item not in consumed for item in all_items])
    candidate_idx = np.where(mask)[0]

    if len(candidate_idx) == 0:
        return []

    # SVD scores: μ + b_u + b_i + p_u · Q[candidates]ᵀ
    svd_scores = (global_mean
                  + bu[uidx]
                  + bi[candidate_idx]
                  + P[uidx] @ Q[candidate_idx].T)

    # Normaliza SVD → [0, 1]
    s_min, s_max = svd_scores.min(), svd_scores.max()
    s_range = s_max - s_min
    norm_svd = (svd_scores - s_min) / s_range if s_range > 1e-6 else np.full_like(svd_scores, 0.5)

    # Afinidade de categoria
    user_aff = category_affinity.get(str(user_id), {})
    cats = item_cat_arr[candidate_idx]
    affinity = np.array([user_aff.get(c, 0.0) for c in cats], dtype=np.float32)

    # Score híbrido
    final_scores = ALPHA * norm_svd + (1 - ALPHA) * affinity

    top_n_idx = np.argpartition(final_scores, -n)[-n:]
    top_n_idx = top_n_idx[np.argsort(final_scores[top_n_idx])[::-1]]
    return [(all_items[candidate_idx[i]], float(final_scores[i])) for i in top_n_idx]


# ── Warmup ───────────────────────────────────────────────────────────────────
sample_users = np.random.default_rng(42).choice(user_ids_with_history, N_REQUESTS + 30, replace=True)
for uid in sample_users[:30]:
    recommend(int(uid))

# ── Benchmark ─────────────────────────────────────────────────────────────────
latencies_us = np.empty(N_REQUESTS)
t_total_start = time.perf_counter()

for i, uid in enumerate(sample_users[30:]):
    t_req = time.perf_counter()
    recommend(int(uid))
    latencies_us[i] = (time.perf_counter() - t_req) * 1_000_000  # µs

total_sec = time.perf_counter() - t_total_start

# ── Resultados ────────────────────────────────────────────────────────────────
latencies_us.sort()
mean_us = latencies_us.mean()
p50_us  = np.percentile(latencies_us, 50)
p95_us  = np.percentile(latencies_us, 95)
p99_us  = np.percentile(latencies_us, 99)
tps     = N_REQUESTS / total_sec

print("  Latência por request:")
print(f"    Média  : {mean_us/1000:.2f} ms  ({mean_us:.0f} µs)")
print(f"    P50    : {p50_us/1000:.2f} ms  ({p50_us:.0f} µs)")
print(f"    P95    : {p95_us/1000:.2f} ms  ({p95_us:.0f} µs)")
print(f"    P99    : {p99_us/1000:.2f} ms  ({p99_us:.0f} µs)")
print(f"  Throughput (single-thread): {tps:.1f} req/s")
print(f"  Tempo total ({N_REQUESTS} reqs):    {total_sec*1000:.0f} ms")
