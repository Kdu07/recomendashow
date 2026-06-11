// =============================================================================
// upload_imagens.js
// Envia imagens de espetáculos para o MongoDB (GridFS).
//
// Pré-requisitos:
//   node >= 18
//   npm install mongodb dotenv
//
// Uso (de dentro de src/mongodb):
//   node upload_imagens.js
// =============================================================================

import dotenv from "dotenv";
import { MongoClient, GridFSBucket } from "mongodb";
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

// Resolve o .env na raiz do repositório (dois níveis acima de src/mongodb)
const __dirname = path.dirname(fileURLToPath(import.meta.url));
dotenv.config({ path: path.resolve(__dirname, "../../.env") });

const { MONGO_USER, MONGO_PASSWORD, MONGO_HOST, MONGO_PORT } = process.env;

if (!MONGO_USER || !MONGO_PASSWORD || !MONGO_HOST || !MONGO_PORT) {
  console.error("ERRO: Preencha MONGO_USER, MONGO_PASSWORD, MONGO_HOST e MONGO_PORT no .env da raiz do projeto");
  process.exit(1);
}

const MONGO_URI   = `mongodb://${MONGO_USER}:${MONGO_PASSWORD}@${MONGO_HOST}:${MONGO_PORT}`;
const MONGO_DB    = process.env.MONGO_DB    ?? "aventura_ml";
const IMAGENS_DIR = path.resolve(__dirname, process.env.IMAGENS_DIR ?? "./images");

const EXTENSOES_SUPORTADAS = new Set([".jpg", ".jpeg", ".png", ".webp", ".gif"]);

async function main() {
  if (!fs.existsSync(IMAGENS_DIR)) {
    console.error(`ERRO: Diretório '${IMAGENS_DIR}' não encontrado.`);
    console.error("Crie a pasta e coloque as imagens lá, ou ajuste IMAGENS_DIR no .env");
    process.exit(1);
  }

  const client = new MongoClient(MONGO_URI);

  try {
    await client.connect();
    console.log("Conectado ao MongoDB.\n");

    const db     = client.db(MONGO_DB);
    const bucket = new GridFSBucket(db, { bucketName: "fs" });
    const meta   = db.collection("espetaculo_images");

    const arquivos = fs.readdirSync(IMAGENS_DIR)
      .filter(f => EXTENSOES_SUPORTADAS.has(path.extname(f).toLowerCase()));

    if (arquivos.length === 0) {
      console.log("Nenhuma imagem encontrada em", IMAGENS_DIR);
      return;
    }

    console.log(`=== Upload de ${arquivos.length} imagem(ns) para MongoDB GridFS ===\n`);

    let sucesso = 0;
    let pulados = 0;
    let falhas  = 0;

    for (const filename of arquivos) {
      const espetaculoNome = path.basename(filename, path.extname(filename));
      process.stdout.write(`  Enviando '${filename}'... `);

      // Verifica se já existe no GridFS para não duplicar
      const existente = await bucket.find({ filename }).toArray();
      if (existente.length > 0) {
        console.log("já existe, pulando.");
        pulados++;
        continue;
      }

      try {
        const filepath    = path.join(IMAGENS_DIR, filename);
        const contentType = resolveContentType(filename);

        // Upload para GridFS
        await new Promise((resolve, reject) => {
          const readStream   = fs.createReadStream(filepath);
          const uploadStream = bucket.openUploadStream(filename, {
            metadata: { espetaculo_nome: espetaculoNome, content_type: contentType },
          });
          readStream.pipe(uploadStream);
          uploadStream.on("finish", resolve);
          uploadStream.on("error", reject);
        });

        // Registra metadado na coleção espetaculo_images
        await meta.updateOne(
          { espetaculo_nome: espetaculoNome },
          {
            $set: {
              espetaculo_nome: espetaculoNome,
              filename,
              content_type: contentType,
              storage: "gridfs",
              uploaded_at: new Date(),
            },
          },
          { upsert: true }
        );

        console.log("OK");
        sucesso++;
      } catch (err) {
        console.log("FALHA");
        console.error(`    → ${err.message}`);
        falhas++;
      }
    }

    console.log(`\n=== Concluído: ${sucesso} enviadas, ${pulados} puladas, ${falhas} com falha ===`);
  } finally {
    await client.close();
  }
}

function resolveContentType(filename) {
  const ext = path.extname(filename).toLowerCase();
  const map = {
    ".jpg":  "image/jpeg",
    ".jpeg": "image/jpeg",
    ".png":  "image/png",
    ".webp": "image/webp",
    ".gif":  "image/gif",
  };
  return map[ext] ?? "application/octet-stream";
}

main().catch(err => {
  console.error("Erro fatal:", err.message);
  process.exit(1);
});