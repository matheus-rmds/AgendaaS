# AgendaaS

![CI](https://github.com/matheus-rmds/AgendaaS/actions/workflows/ci.yml/badge.svg)

SaaS multi-tenant de agendamento, com painel administrativo e página pública de agendamento para o cliente final.

## Stack

**Backend:** ASP.NET Core 8, Entity Framework Core, SQL Server, JWT

**Frontend:** Angular 21, TypeScript

**Infra:** Docker, Docker Compose, GitHub Actions (CI)

## Rodando o projeto

Único pré-requisito: [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
git clone https://github.com/matheus-rmds/AgendaaS.git
cd AgendaaS
docker compose up --build
```

Depois de subir (a primeira vez demora alguns minutos):
- Painel: [http://localhost:4200](http://localhost:4200)
- API + Swagger: [http://localhost:5000/swagger](http://localhost:5000/swagger)

O banco é criado e migrado automaticamente ao subir, não precisa rodar nada manualmente. Crie uma conta pela tela de cadastro pra começar a testar.

## O que dá pra testar

- Cadastro de um salão (cria automaticamente o dono/administrador)
- CRUD de profissionais, serviços e clientes
- Criação de agendamentos, com validação de conflito de horário
- Página pública de agendamento (`/agendar/{slug-do-salão}`), sem necessidade de login para o cliente final

## Decisões técnicas

- **Isolamento multi-tenant**: Banco compartilhado com filtro de tenant aplicado incondicionalmente via EF Core Global Query Filters, resolvido por request (JWT para rotas autenticadas, slug da URL para a página pública), evita o padrão mais caro de banco por tenant sem abrir mão de isolamento real. Coberto por teste automatizado.
- **Detecção de conflito de horário**: Validação de sobreposição de intervalos no backend antes de qualquer criação de agendamento, também coberta por teste automatizado.
- **Cliente final sem conta**: Agendamento público não exige cadastro/login do cliente, mas foi considerada junto com verificação de código e foi deixada fora do escopo.
- **CI simples e rápido**: Os testes usam o provider `InMemory` do EF Core, então o pipeline não depende de subir banco na nuvem.
