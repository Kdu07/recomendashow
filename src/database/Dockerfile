FROM postgres:16.9

COPY 01_create_database.sql /docker-entrypoint-initdb.d/
COPY 03_sprint4_improvements.sql /docker-entrypoint-initdb.d/
COPY 03_initial_data.sql /docker-entrypoint-initdb.d/
COPY 04_espetaculo_dataset_mapping.sql /docker-entrypoint-initdb.d/
