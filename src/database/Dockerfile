FROM postgres:16

COPY 01_create_database.sql /docker-entrypoint-initdb.d/
COPY 02_initial_data.sql /docker-entrypoint-initdb.d/