# URL Shortener

API para encurtar URLs e redirecionar links curtos, desenvolvida com ASP.NET Core 8 e PostgreSQL.

## Visão geral

O projeto recebe uma URL original, gera um código curto e registra os acessos realizados por meio desse código. A aplicação inclui persistência com Entity Framework Core, documentação OpenAPI/Swagger e suporte a execução em container.

## Tecnologias

- C# e .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL
- Swagger / OpenAPI
- Docker

## Como executar

### Pré-requisitos

- .NET SDK 8
- PostgreSQL

Configure a conexão com o banco por variável de ambiente:

```bash
ConnectionStrings__DefaultConnection="Host=localhost;Port=5432;Database=url_shortener;Username=postgres;Password=postgres"
```

Depois, restaure as dependências e inicie a API:

```bash
dotnet restore
dotnet run
```

As migrations são aplicadas automaticamente durante a inicialização. Em ambiente de desenvolvimento, a documentação Swagger fica disponível no endereço exibido pelo terminal, normalmente em `/swagger`.

## Endpoints principais

| Método | Rota | Descrição |
| --- | --- | --- |
| `POST` | `/api/Shortener` | Cria uma URL curta |
| `GET` | `/{shortCode}` | Redireciona para a URL original e registra o acesso |

Exemplo de requisição:

```http
POST /api/Shortener
Content-Type: application/json

{
  "originalUrl": "https://example.com"
}
```

## Docker

```bash
docker build -t url-shortener .
docker run --rm -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=url_shortener;Username=postgres;Password=postgres" \
  url-shortener
```

## Estrutura

- `Controllers/`: endpoints HTTP
- `Services/`: regras de encurtamento e redirecionamento
- `Data/`: contexto do Entity Framework
- `Models/` e `Dto/`: modelos de domínio e contratos da API
- `Migrations/`: histórico do banco de dados

## Autor

Desenvolvido por [Willian Henrique](https://github.com/willianhenri).
