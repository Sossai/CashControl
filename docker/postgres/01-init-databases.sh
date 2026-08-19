#!/bin/bash
# Colocar em: docker/postgres/01-init-databases.sh
#
# Cria o segundo banco lógico (Consolidation). O POSTGRES_DB (cashcontrol_transactions)
# já é criado automaticamente pela imagem oficial do Postgres a partir da env var.
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE cashcontroldb;
EOSQL
