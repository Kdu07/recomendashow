// Inicialização do banco aventura_ml
// Cria coleções e índices para artefatos do modelo e imagens de espetáculos

db = db.getSiblingDB('aventura_ml');

// Artefatos do modelo (pesos SVD, CSVs, zips)
db.createCollection('artifact_metadata');
db.artifact_metadata.createIndex({ artifact_type: 1, uploaded_at: -1 });
db.artifact_metadata.createIndex({ is_active: 1, artifact_type: 1 });

// Imagens dos espetáculos
db.createCollection('espetaculo_images');
db.espetaculo_images.createIndex({ espetaculo_nome: 1 }, { unique: true });

print('MongoDB aventura_ml inicializado.');