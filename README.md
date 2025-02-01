# MyDigitalWallet

Este projeto consiste em uma aplicação **.NET** para gerenciar carteiras digitais e transações financeiras, utilizando o **Entity Framework Core** em modo **Code-First** com **PostgreSQL** e autenticação via **JWT**.

---

## Sumário
1. [Visão Geral](#visao-geral)
2. [Pré-requisitos](#pre-requisitos)
3. [Configurações](#configuracoes)
4. [Executando com Docker](#executando-com-docker)
5. [Testando a API](#testando-a-api)

---

## Visão Geral

Este repositório contém:

- **MyDigitalWallet.Domain**: Entidades (User, Transaction), Value Objects (Wallet, Password) e regras de negócio do domínio (modelo rico).
- **MyDigitalWallet.Application**: Serviços e interfaces de aplicação (ex.: `IUserService`, `ITransactionService`).
- **MyDigitalWallet.Infrastructure**: Persistência (EF Core, repositórios), migrations e configurações de banco.
- **MyDigitalWallet.API**: Endpoints expostos via ASP.NET Core Web API, controllers e configuração de autenticação JWT.

---

## Pré-requisitos

- **Docker** instalado e em execução (versão 20+)
- **Docker Compose** (geralmente incluso nas instalações mais recentes de Docker)

> **Observação**: Se você já possui o Docker Desktop, provavelmente já tem também o Docker Compose.

---

## Configurações

Existem algumas configurações que podem ser definidas via **variáveis de ambiente** ou diretamente no `appsettings.json` da API. Exemplo de `appsettings.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db;Port=5432;Database=MyDigitalWalletDb;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "zUk3jAshGu7FD9qy5Gg6XQbm4f9Spf0c", // Deve ter pelo menos 32 caracteres
    "Issuer": "MyDigitalWallet",
    "Audience": "MyDigitalWalletUsers",
    "ExpiresInMinutes": 120
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

No Docker Compose, a connection string já está configurada para apontar para o serviço de banco chamado db (conforme definido no docker-compose.yml).

## Executando com Docker

1. Clone ou baixe o repositório:

```bash
git clone https://github.com/Gabriel-Moya/WLConsultings.git
cd WLConsultings
```

2. Verifique o arquivo `docker-compose.yml`

Ele deve conter pelo menos dois serviços:
- `db`: container com o **PostgreSQL**
- `api`: container com a aplicação .NET

3. Execute o comando para subir os contêineres (compila e cria a imagem da API e executa ambos os serviços):

```bash
docker compose up --build
```

4. Verifique os logs
- Se tudo ocorrer bem, você será capaz de ver os logs do PostgreSQL e da API no terminal.
- A API deve ficar disponível em `http://localhost:5000`
- A documentação, feita com o `scalar`, deverá estar disponível em `http://localhost:5000/scalar`

## Seed de dados

Durante a inicialização, o sistema já popula dois usuários no banco de dados, assim você já poderá testar as operações de login, transferências e saldo sem precisar criar um usuário anteriormente.

1. User: `admin`, Password: `UltraPowerSecretPwd`
2. User: `William`, Password: `senhaS3creta`
