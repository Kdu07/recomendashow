# MongoDB — Aventura ML

Guia de referência do banco de dados utilizado pelo sistema de recomendação: estrutura, scripts e como rodar.

---

## Estrutura da pasta

```
src/mongodb/
  Dockerfile            # imagem customizada do MongoDB com inicialização automática
  mongo-init.js         # cria coleções e índices ao subir o container pela primeira vez
  upload_images.js     # script para enviar imagens ao GridFS
  images/              # imagens dos espetáculos (não commitadas)
  package.json          # dependências do script Node.js
  node_modules/         # não commitado
```

---

## Ambiente

O MongoDB roda em um container Docker hospedado em uma máquina Oracle Cloud. As credenciais ficam no `.env` da raiz do projeto — **nunca commite esse arquivo**.

```
Sua máquina local
       │
       │  IP público Oracle
       ▼
Oracle Cloud VM
  └── Docker
        └── container MongoDB (porta 27017)
              └── banco: aventura_ml
                    ├── artifact_metadata
                    ├── espetaculo_images
                    └── GridFS (fs.files + fs.chunks)
```

---

## Credenciais

O `.env` fica na raiz do projeto (`../../.env` relativo a esta pasta) com o seguinte formato:

```env
MONGO_USER=
MONGO_PASSWORD=
MONGO_HOST=
MONGO_PORT=
MONGO_DB=aventura_ml
```

Certifique-se que `.env` está no `.gitignore` da raiz.

---

## Container — Dockerfile

```dockerfile
FROM mongo:7

COPY mongo-init.js /docker-entrypoint-initdb.d/
```

O `mongo-init.js` é copiado para `/docker-entrypoint-initdb.d/`, diretório especial do container oficial do MongoDB. **Tudo que estiver nessa pasta é executado automaticamente na primeira vez que o container sobe** — ou seja, as coleções e índices são criados sem nenhuma intervenção manual.

> Nas execuções seguintes (container reiniciado com dados já existentes) o script é ignorado.

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
Armazena os metadados das imagens. O binário da imagem fica no GridFS.

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
Criado automaticamente pelo driver MongoDB ao fazer o primeiro upload. Armazena os binários das imagens fragmentados em chunks de 255KB.

---

## Por que essa divisão de armazenamento?

| O que | Onde | Por quê |
|---|---|---|
| `model.zip` | Volume Docker (`/app/storage/models/`) | Comprimido já tem ~12MB — ultrapassa o limite de documento do Mongo (16MB) descomprimido. Lido direto do disco na inicialização |
| `dataset.csv` | Volume Docker (`/app/storage/dataset/`) | Pode crescer muito; leitura sequencial é mais eficiente em disco |
| JSONs de artefatos | MongoDB (`artifact_metadata`) | Pequenos, consultados pela API, beneficiam de índice e query |
| Imagens | MongoDB GridFS | Tamanho moderado, acesso por nome, sem necessidade de filesystem extra |

### Como enviar o `model.zip` e o `dataset.csv`

Como esses arquivos ficam em volume Docker, o envio é via `scp` para o servidor Oracle:

```bash
scp data/models/model.zip opc@<IP_ORACLE>:/app/storage/models/model.zip
scp data/dataset/dataset.csv opc@<IP_ORACLE>:/app/storage/dataset/dataset.csv
```

Ou copiando direto para dentro do container:

```bash
docker cp data/models/model.zip <container_ml>:/app/storage/models/model.zip
docker cp data/dataset/dataset.csv <container_ml>:/app/storage/dataset/dataset.csv
```

---

## Script de upload de imagens — `upload_images.js`

Envia imagens para o GridFS e registra os metadados em `espetaculo_images`. Lê as credenciais do `.env` da raiz automaticamente. Suporta `.jpg`, `.jpeg`, `.png`, `.webp` e `.gif`.

O nome do arquivo vira o `espetaculo_nome` — nomeie como `nome-do-espetaculo.jpg`.

**Pré-requisitos:**

```bash
# Dentro de src/mongodb
npm install
```

**Como rodar:**

```bash
# 1. Coloque as imagens na pasta
mkdir images
cp circo-das-maravilhas.jpg festival-de-jazz.png images/

# 2. Execute
node upload_images.js
```

O script pula imagens que já existem no GridFS — pode rodar várias vezes sem duplicar.

---

## Verificando o que foi salvo

Conecte via `mongosh` no servidor Oracle:

```bash
mongosh "mongodb://<MONGO_USER>:<MONGO_PASSWORD>@<IP_ORACLE>:27017/aventura_ml"
```

```js
// Metadados das imagens
db.espetaculo_images.find().pretty()

// Arquivos no GridFS
db["fs.files"].find({}, { filename: 1, length: 1, uploadDate: 1 }).pretty()

// Artefatos ativos do modelo
db.artifact_metadata.find({ is_active: true }, { artifact_type: 1, filename: 1, uploaded_at: 1 }).pretty()
```

---

## Dicas de segurança

- Nunca exponha a porta 27017 publicamente sem autenticação.
- Use um túnel SSH para operações administrativas pontuais: `ssh -L 27017:localhost:27017 opc@<IP_ORACLE> -N`
- Use um usuário MongoDB com permissões mínimas (`readWrite` apenas no `aventura_ml`).
- O `.env` nunca entra no repositório.