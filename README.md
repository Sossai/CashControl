# 🚀 CashControl

> Aplicação para controle de fluxo de caixa diário. Registra crédito, débito e calcula saldo diário.

---

## 🛠️ Tecnologias Utilizadas

* [.NET 10.0](https://dotnet.microsoft.com/)
* [ASP.NET Core Web API](https://microsoft.com)
* [Entity Framework Core](https://microsoft.com) (ORM)
* [xUnit Test]
* [PostgreSQL] (Banco de dados)
* [RabbitMQ] (Mensageria)
* [Docker] (Container)

---

## ⚙️ Como Executar o Projeto

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/Sossai/CashControl
   ```

2. **Subir docker e deployar a solução :**
   ```bash
   docker compose up -d --build
   ```

   ![Docker](./assets/docker2.png)

3. **Abrir o Package Manager Console do Visual Studio e aplicar as migrações:**
   ```bash
   Update-Database -Project Transactions.Infrastructure -StartupProject Transactions.Api	
   Update-Database -Project Consolidation.Infrastructure -StartupProject Consolidation.Api
   ```

   ![Estrutura de banco criada](./assets/estrutura_banco.png)
   
4. **Para enviar uma transação, acesse o swagger da aplicação Transactions.Api e clique em "Try it out" em seguida altere os valores como o exemplo abaixo e clique em Execute.**
Swagger : 
- registro das transações => http://localhost:5098/swagger/index.html 
- consulta consolidado : http://localhost:5198/swagger/index.html

   ![Execute](./assets/transactions_api_1.png)

   ```bash
    {
        "date": "2026-08-16",
        "type": 1,
        "amount": 100,
        "description": "Adiconando 100 de Crédito"
    }
   ```

   ```bash
   type = 1 ==> Credit
   type = 2 ==> Debit
   ```

5. **Para consultar um valor consolidado, acesse o swagger da aplicação Consolidation.Api e clique em "Try it out" em seguida preencha o campo com a data que deseja consultar e clique em Execute**

![Execute](./assets/consolidation_api_1.png)

   ```bash
   2026-08-16
   ```

---

## 📦 Estrutura do Projeto

```text
src/Transactions
 ├── Transactions.Api
 ├── Transactions.Application
 ├── Transactions.Domain
 ├── Transactions.Infrastructure
src/Consolidation
 ├── Consolidation.Api
 ├── Consolidation.Application
 ├── Consolidation.Domain
 ├── Consolidation.Infrastructure
 ├── Consolidation.Worker
src/Shared
 ├── Shared
tests/
 └── TransactionTests
 └── ConsolidationTests

```


## 📦 Desenho da solução

```mermaid
flowchart TD
    Client(["Cliente / App"])

    subgraph TX["Transactions context"]
        TxApi["Transactions.Api<br/>POST /transactions"]
        TxDb[("cashcontroldb<br/>Transactions")]
    end

    Queue{{"RabbitMQ<br/>"}}

    subgraph CONS["Consolidation context"]
        Worker["Consolidation.Worker<br/>"]
        ConDb[("cashcontroldb<br/>DailyConsolidate + ProcessedEvent")]
        ConApi["Consolidation.Api<br/>GET /Consolidate"]
    end

    Client -->|"POST /transactions"| TxApi
    TxApi -->|"grava lançamento"| TxDb
    TxApi -->|"publica evento"| Queue
    Queue -->|"consome"| Worker
    Worker -->|"atualiza saldo"| ConDb
    Client -->|"GET /Consolidate"| ConApi

    classDef blue fill:#dbeafe,stroke:#3b82f6,color:#1e3a8a;
    classDef green fill:#dcfce7,stroke:#22c55e,color:#14532d;
    classDef gray fill:#f1f5f9,stroke:#64748b,color:#1e293b;
    classDef red fill:#fee2e2,stroke:#ef4444,color:#7f1d1d,stroke-dasharray: 4 3;

    class TxApi,TxDb,OutboxPub blue
    class Worker,ConDb,Cache,ConApi green
    class Queue gray

```
---

## 📌 Fluxo da solução e recursos utilizados
1. Aplicação Transaction.Api recebe uma requisição **HTTP POST** em /Transactions informando o valor e o tipo de operação (débito ou crédito).
2. Valida a requisição utilizando **FluentValidation**.
3. Registra tanto requisição quanto a mensagem a ser enviada, na mensageria, no banco **Postgrees**. Utilizando **pattern Transactional Outbox**, implementado pelo **RabbitMQ**, garantimos que toda informação inserida no sistema será publicada na mensageria.
4. Aplicação Consolidation.Worker consome a mensagem, valida Idempotência e insere/atualiza o saldo diário no banco.
5. Para recuperar o valor consolidado, podemos fazer uma requisição **HTTP GET** em /Consolidate passando a data desejada.
6. ** Regras de Resiliência implementadas **
- Retry exponencial no RabbitMQ.
- Polly ao inserir registros no banco.

---

## 💻 Healthcheck para verificar status das aplicações e infra
```bash
Transactions Api => http://localhost:5098/health/ready
Copnsolidation Api => http://localhost:5198/health/ready
RabbitMq Consumer => http://localhost:5298/health/ready
```
---
## 📋 Melhorias e débitos técnicos

```text
1. **Implementar uso do Redis:** Salvar o valor consolidado no Redis para um melhor desempenho e não precisar acessar o banco constantemente.
2. **Implementar DLQ:** Implementar uso da Dead Letter Queue.
3. **Logs e Observabilidade:**
4. **Controle de cobertura de código:**
5. **Aplicar regra de TTL no banco para evitar crescimento sem controle:**
6. **Implementar controle de autenticação e autorizacão:** 
```