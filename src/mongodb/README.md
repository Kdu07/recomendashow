# MongoDB — Aventura ML

Guia de referência do banco de dados utilizado pelo sistema de recomendação: estrutura, scripts e como rodar.

---

## Estrutura da pasta

```
src/mongodb/
  Dockerfile              # imagem customizada do MongoDB com inicialização automática
  mongo-init.js           # cria coleções e índices ao subir o container pela primeira vez
  upload-images.js       # envia imagens dos espetáculos ao GridFS
  deploy-artifacts.js     # envia model.zip/dataset.csv ao servidor ML e JSONs ao MongoDB
  images/                # imagens dos espetáculos (não commitadas)
  package.json
  node_modules/           # não commitado
```

---

## Ambiente

Cada serviço roda em um servidor separado na Oracle Cloud:

```
Sua máquina local
       │
       ├──scp──────────────────→ Oracle VM: servidor ML
       │                              └── volume Docker
       │                                    ├── /app/storage/models/model.zip
       │                                    └── /app/storage/dataset/dataset.csv
       │
       └──MongoDB driver────────→ Oracle VM: servidor MongoDB
                                        └── banco: aventura_ml
                                              ├── artifact_metadata (JSONs)
                                              ├── espetaculo_images (metadados)
                                              └── GridFS (binários das imagens)
```

---

## Credenciais

O `.env` fica na raiz do projeto. Copie o `.env.example` e preencha:

```bash
cp .env.example .env
```

```env
# MongoDB
MONGO_USER=
MONGO_PASSWORD=
MONGO_HOST=
MONGO_PORT=
MONGO_DB=aventura_ml

# Servidor ML
ML_SSH_USER=opc
ML_SSH_HOST=
ML_STORAGE_PATH=/app/storage

# Imagens
IMAGENS_DIR=./images
```

Certifique-se que `.env` está no `.gitignore` da raiz.

---

## Container — Dockerfile

```dockerfile
FROM mongo:7

COPY mongo-init.js /docker-entrypoint-initdb.d/
```

O `mongo-init.js` é copiado para `/docker-entrypoint-initdb.d/`, diretório especial do container oficial do MongoDB. **Tudo que estiver nessa pasta é executado automaticamente na primeira vez que o container sobe** — as coleções e índices são criados sem nenhuma intervenção manual. Nas execuções seguintes o script é ignorado.

---

## Inicialização — `mongo-init.js`

Cria o banco `aventura_ml` com duas coleções e seus índices:

### Coleção `artifact_metadata`
Armazena os artefatos JSON gerados pelo treino do modelo.

```json
{
  "artifact_type": "cold_start",
  "filename": "cold_start.json",
  "content": { "data": ["Espetáculo A", "Espetáculo B"] },
  "is_active": true,
  "uploaded_at": "2024-01-15T10:30:00Z"
}
```

Tipos de `artifact_type`: `cold_start`, `item_categories`, `category_affinity`, `cold_start_by_category`.

Índices:
- `{ artifact_type: 1, uploaded_at: -1 }` — busca por tipo ordenada do mais recente
- `{ is_active: 1, artifact_type: 1 }` — filtra apenas artefatos ativos

### Coleção `espetaculo_images`
Metadados das imagens. O binário fica no GridFS.

```json
{
  "espetaculo_nome": "circo-das-maravilhas",
  "filename": "circo-das-maravilhas.jpg",
  "content_type": "image/jpeg",
  "storage": "gridfs",
  "uploaded_at": "2024-01-15T10:30:00Z"
}
```

Índice: `{ espetaculo_nome: 1 }` único — cada espetáculo tem no máximo uma entrada.

### GridFS (`fs.files` + `fs.chunks`)
Criado automaticamente pelo driver ao primeiro upload. Armazena binários fragmentados em chunks de 255KB.

---

## Por que essa divisão de armazenamento?

| O que | Onde | Por quê |
|---|---|---|
| `model.zip` | Volume Docker do servidor ML | Comprimido já tem ~12MB — ultrapassa o limite de documento do Mongo (16MB) descomprimido. Lido direto do disco na inicialização |
| `dataset.csv` | Volume Docker do servidor ML | Pode crescer muito; leitura sequencial é mais eficiente em disco |
| JSONs de artefatos | MongoDB (`artifact_metadata`) | Pequenos, consultados pela API, beneficiam de índice e query |
| Imagens | MongoDB GridFS | Tamanho moderado, acesso por nome, sem necessidade de filesystem extra |

---

## Scripts

### `deploy-artifacts.js`
Script **manual** — você roda da sua máquina após cada retreino. Envia cada artefato para o destino correto:

| Artefato | Destino |
|---|---|
| `data/models/model.zip` | Servidor ML via `scp` |
| `data/dataset/dataset.csv` | Servidor ML via `scp` |
| `data/models/cold_start.json` | MongoDB (`artifact_metadata`) |
| `data/models/item_categories.json` | MongoDB (`artifact_metadata`) |
| `data/models/category_affinity.json` | MongoDB (`artifact_metadata`) |
| `data/models/cold_start_by_category.json` | MongoDB (`artifact_metadata`) |

> **Pré-requisito:** o servidor ML precisa estar no ar e acessível via SSH para o `scp` funcionar. Preencha `ML_SSH_USER`, `ML_SSH_HOST` e `ML_STORAGE_PATH` no `.env` antes de rodar.

O script detecta automaticamente se está rodando no próprio servidor Oracle — nesse caso copia os arquivos localmente sem precisar de `scp`.

O retreino não é automatizado — o `RecommendationBatchJob` do inference-service roda a cada 24h, mas apenas gera recomendações com o modelo já carregado em memória. O retreino com dados novos é sempre iniciado manualmente.

**Como rodar (a partir da raiz do projeto):**

```bash
# 1. Treina o modelo
cd src/ml
dotnet run preprocess
dotnet run train

# 2. Faz o deploy
cd ../mongodb
node deploy-artifacts.js
```

---

### `upload-images.js`
Envia imagens para o GridFS e registra metadados em `espetaculo_images`. O nome do arquivo vira o `espetaculo_nome` — nomeie como `nome-do-espetaculo.jpg`. Suporta `.jpg`, `.jpeg`, `.png`, `.webp` e `.gif`. Pula imagens já existentes.

**Como rodar:**

```bash
# 1. Coloque as imagens na pasta
mkdir src/mongodb/images
cp circo-das-maravilhas.jpg festival-de-jazz.png src/mongodb/images/

# 2. Execute
cd src/mongodb && node upload-images.js
```

---

### Instalação das dependências

```bash
cd src/mongodb
npm install
```

---

## Verificando o que foi salvo

```bash
mongosh "mongodb://<MONGO_USER>:<MONGO_PASSWORD>@<IP_ORACLE>:<MONGO_PORT>/aventura_ml"

# Artefatos ativos do modelo
db.artifact_metadata.find({ is_active: true }, { artifact_type: 1, filename: 1, uploaded_at: 1 }).pretty()

# Metadados das imagens
db.espetaculo_images.find().pretty()

# Arquivos no GridFS
db["fs.files"].find({}, { filename: 1, length: 1, uploadDate: 1 }).pretty()
```

---

## Dicas de segurança

- Use um usuário MongoDB com permissões mínimas (`readWrite` apenas no `aventura_ml`).
- O `.env` nunca entra no repositório — use `.env.example` como template.