# Projeto Conta Bancária

Sistema de conta bancária desenvolvido em **C#**, usando os conceitos de **Programação Orientada a Objetos** e persistência em **PostgreSQL**.

O projeto possui menu interativo no terminal, cadastro de contas, movimentações financeiras, validações de regras de negócio e testes unitários.

## Funcionalidades

- Criar conta corrente.
- Criar conta poupança.
- Listar todas as contas cadastradas.
- Consultar conta pelo número.
- Realizar depósito.
- Realizar saque.
- Realizar transferência entre contas.
- Encerrar conta.
- Exibir relatório financeiro geral.
- Persistir os dados no PostgreSQL.

## Conceitos abordados

- Classes, atributos e métodos.
- Classe abstrata `Conta`.
- Herança com `ContaCorrente` e `ContaPoupanca`.
- Interface `IContaRepository`.
- Encapsulamento das regras de negócio.
- Controller para centralizar operações do sistema.
- Validações de entrada e regras bancárias.
- Persistência com PostgreSQL usando `Npgsql`.
- Testes unitários com xUnit.

## Estrutura do projeto

```text
ContaBancaria/
├── src/
│   └── ContaBancaria/
│       ├── Controllers/
│       ├── Models/
│       ├── Repositories/
│       ├── Utils/
│       ├── Program.cs
│       ├── appsettings.json
│       └── ContaBancaria.csproj
│
├── tests/
│   └── ContaBancaria.Tests/
│       ├── ContaCorrenteTests.cs
│       ├── ContaPoupancaTests.cs
│       ├── TransferenciaTests.cs
│       └── ContaBancaria.Tests.csproj
│
├── database/
│   ├── schema.sql
│   └── docker-compose.yml
│
├── ContaBancaria.sln
├── .gitignore
└── README.md
```

## Requisitos

Antes de rodar o projeto, instale:

- .NET SDK 8.0 ou superior.
- PostgreSQL.
- Um editor, como Visual Studio Code ou Visual Studio.

O Docker **não é obrigatório**. Ele foi deixado apenas como alternativa para quem não quiser instalar o PostgreSQL diretamente na máquina.

## Como rodar o projeto

### 1. Baixe ou clone o projeto

Entre na pasta raiz do projeto, onde está o arquivo `ContaBancaria.sln`.

```bash
cd ContaBancaria
```

### 2. Crie o banco de dados no PostgreSQL

Acesse o PostgreSQL e crie o banco:

```sql
CREATE DATABASE bancodb;
```

Depois, execute o script localizado em:

```text
database/schema.sql
```

Esse script cria a tabela `contas`, usada pelo sistema.

### 3. Configure a conexão com o banco

Abra o arquivo:

```text
src/ContaBancaria/appsettings.json
```

Confira se a conexão está correta:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=bancodb;Username=postgres;Password=SUA_SENHA"
  }
}
```

Troque `SUA_SENHA` pela senha do seu usuário `postgres`.

### 4. Restaure as dependências

Na raiz do projeto, execute:

```bash
dotnet restore
```

### 5. Compile o projeto

```bash
dotnet build
```

### 6. Rode o sistema

```bash
dotnet run --project src/ContaBancaria/ContaBancaria.csproj
```

O menu do sistema será exibido no terminal.

## Como rodar os testes

Na raiz do projeto, execute:

```bash
dotnet test
```

Os testes validam regras como:

- Depósito com valor válido;
- Bloqueio de depósito inválido;
- Saque com saldo suficiente;
- Bloqueio de saque sem saldo suficiente;
- Uso de limite em conta corrente;
- Saque em conta poupança;
- Simulação de transferência válida e inválida.

## Uso opcional com Docker

Use o Docker apenas se você não tiver o PostgreSQL instalado localmente ou caso prefira não instalá-lo.

Para subir um PostgreSQL com Docker, execute:

```bash
cd database
docker compose up -d
```

Nesse caso, a conexão padrão será:

```text
Host=localhost;Port=5432;Database=bancodb;Username=postgres;Password=postgres
```

Se o banco já tiver sido criado antes com outra senha, execute:

```bash
docker compose down -v
docker compose up -d
```

## Observação importante

Se aparecer erro de autenticação, como:

```text
autenticação do tipo senha falhou para o usuário "postgres"
```

verifique se a senha no arquivo `appsettings.json` é a mesma senha configurada no seu PostgreSQL.
