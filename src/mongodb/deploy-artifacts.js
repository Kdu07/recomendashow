// =============================================================================
// deploy_artifacts.js
// Após o treino, envia os artefatos para os destinos corretos:
//
//   model.zip + dataset.csv → servidor ML via scp (ou local se já estiver na Oracle)
//   JSONs (cold_start, item_categories, category_affinity,
//          cold_start_by_category) → MongoDB (coleção artifact_metadata)
//
// Pré-requisitos:
//   node >= 18
//   npm install mongodb dotenv
//   scp disponível no PATH (nativo no macOS/Linux; no Windows use Git Bash ou WSL)
//
// Uso:
//   node deploy_artifacts.js
// =============================================================================

import dotenv from "dotenv";
import { MongoClient } from "mongodb";
import fs from "fs";
import path from "path";
import { execSync } from "child_process";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
dotenv.config({ path: path.resolve(__dirname, "../../.env") });

// ---------------------------------------------------------------------------
// Configuração
// ---------------------------------------------------------------------------
const {
  MONGO_USER, MONGO_PASSWORD, MONGO_HOST, MONGO_PORT,
  ML_SSH_USER, ML_SSH_HOST, ML_STORAGE_PATH,
} = process.env;

if (!MONGO_USER || !MONGO_PASSWORD || !MONGO_HOST || !MONGO_PORT) {
  console.error("ERRO: Preencha MONGO_USER, MONGO_PASSWORD, MONGO_HOST e MONGO_PORT no .env");
  process.exit(1);
}

if (!ML_SSH_USER || !ML_SSH_HOST || !ML_STORAGE_PATH) {
  console.error("ERRO: Preencha ML_SSH_USER, ML_SSH_HOST e ML_STORAGE_PATH no .env");
  process.exit(1);
}

const MONGO_URI = `mongodb://${MONGO_USER}:${MONGO_PASSWORD}@${MONGO_HOST}:${MONGO_PORT}`;
const MONGO_DB  = process.env.MONGO_DB ?? "aventura_ml";

// Caminhos locais dos artefatos (relativos à raiz do repositório)
const ROOT = path.resolve(__dirname, "../../");
const ARTIFACTS = {
  // Arquivos que vão pro servidor ML via scp
  scp: [
    { local: path.join(ROOT, "data/models/model.zip"),       remote: `${ML_STORAGE_PATH}/models/model.zip` },
    { local: path.join(ROOT, "data/dataset/dataset.csv"),    remote: `${ML_STORAGE_PATH}/dataset/dataset.csv` },
  ],
  // JSONs que vão pro MongoDB
  mongo: [
    { type: "cold_start",             file: path.join(ROOT, "data/models/cold_start.json") },
    { type: "item_categories",        file: path.join(ROOT, "data/models/item_categories.json") },
    { type: "category_affinity",      file: path.join(ROOT, "data/models/category_affinity.json") },
    { type: "cold_start_by_category", file: path.join(ROOT, "data/models/cold_start_by_category.json") },
  ],
};

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------
async function main() {
  console.log("=== Deploy de artefatos ===\n");

  await deployViaScp();
  await deployToMongo();

  console.log("\n=== Deploy concluído ===");
}

// ---------------------------------------------------------------------------
// SCP — model.zip e dataset.csv → servidor ML
// ---------------------------------------------------------------------------
async function deployViaScp() {
  console.log("--- Enviando arquivos para o servidor ML via scp ---\n");

  // Detecta se já está rodando na própria Oracle (mesmo host)
  const isLocal = ML_SSH_HOST === "localhost" || ML_SSH_HOST === "127.0.0.1";

  for (const { local, remote } of ARTIFACTS.scp) {
    if (!fs.existsSync(local)) {
      console.warn(`  AVISO: '${local}' não encontrado, pulando.`);
      continue;
    }

    const filename = path.basename(local);
    process.stdout.write(`  Enviando '${filename}'... `);

    try {
      if (isLocal) {
        // Já está no servidor — copia direto
        fs.mkdirSync(path.dirname(remote), { recursive: true });
        fs.copyFileSync(local, remote);
      } else {
        // Envia via scp
        execSync(`scp "${local}" ${ML_SSH_USER}@${ML_SSH_HOST}:"${remote}"`, { stdio: "pipe" });
      }
      console.log("OK");
    } catch (err) {
      console.log("FALHA");
      console.error(`    → ${err.message}`);
    }
  }
}

// ---------------------------------------------------------------------------
// MongoDB — JSONs → coleção artifact_metadata
// ---------------------------------------------------------------------------
async function deployToMongo() {
  console.log("\n--- Salvando artefatos JSON no MongoDB ---\n");

  const client = new MongoClient(MONGO_URI);

  try {
    await client.connect();
    const collection = client.db(MONGO_DB).collection("artifact_metadata");

    for (const { type, file } of ARTIFACTS.mongo) {
      if (!fs.existsSync(file)) {
        console.warn(`  AVISO: '${file}' não encontrado, pulando.`);
        continue;
      }

      process.stdout.write(`  Salvando '${type}'... `);

      try {
        const raw     = fs.readFileSync(file, "utf-8");
        const parsed  = JSON.parse(raw);
        const content = Array.isArray(parsed) ? { data: parsed } : parsed;

        // Desativa versão anterior e insere a nova
        await collection.updateMany(
          { artifact_type: type, is_active: true },
          { $set: { is_active: false } }
        );

        await collection.insertOne({
          artifact_type: type,
          filename:      path.basename(file),
          content,
          is_active:     true,
          uploaded_at:   new Date(),
        });

        console.log("OK");
      } catch (err) {
        console.log("FALHA");
        console.error(`    → ${err.message}`);
      }
    }
  } finally {
    await client.close();
  }
}

main().catch(err => {
  console.error("Erro fatal:", err.message);
  process.exit(1);
});