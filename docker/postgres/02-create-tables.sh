#!/bin/bash
# Colocar em: docker/postgres/02-create-tables.sh
#
# Roda depois de 01-init-databases.sh (ordem alfabética garantida pelo
# entrypoint oficial do Postgres). Cria as tabelas de cada contexto no
# banco correto, a partir dos scripts fornecidos:
#   script_Transactions.txt       -> cashcontrol_transactions
#   script_DailyConsolidate.txt   -> cashcontrol_consolidation
#   script_ProcessedEvent.txt     -> cashcontrol_consolidation
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "cashcontrol" <<'EOSQL'
CREATE TABLE IF NOT EXISTS public."Transactions"
(
    "Id" uuid NOT NULL,
    "Date" date NOT NULL,
    "Type" integer NOT NULL,
    "Amount" numeric NOT NULL,
    "Description" text COLLATE pg_catalog."default" NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Transactions" PRIMARY KEY ("Id")
)
TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."Transactions"
    OWNER to cashcontrol;

CREATE TABLE IF NOT EXISTS public."DailyConsolidate"
(
    "Date" date NOT NULL,
    "AccumulatedBalance" numeric NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_DailyConsolidate" PRIMARY KEY ("Date")
)
TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."DailyConsolidate"
    OWNER to cashcontrol;

CREATE TABLE IF NOT EXISTS public."ProcessedEvent"
(
    "EventId" uuid NOT NULL,
    "ProcessedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ProcessedEvent" PRIMARY KEY ("EventId")
)
TABLESPACE pg_default;

ALTER TABLE IF EXISTS public."ProcessedEvent"
    OWNER to cashcontrol;
EOSQL
