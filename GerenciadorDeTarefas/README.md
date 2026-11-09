# Gerenciador de Tarefas

Projeto simples com **API em C# (.NET 8)** e **front-end em React (Vite)**.

Faz um CRUD completo de tarefas, com filtro por título, paginação e mensagens de erro vindas da API.

## O que tem

**API (`TarefasApi`)**
- Minimal API com EF Core e SQLite (o banco `tarefas.db` é criado sozinho na primeira execução)
- CRUD: `GET /tarefas`, `GET /tarefas/{id}`, `POST /tarefas`, `PUT /tarefas/{id}`, `DELETE /tarefas/{id}`
- Filtro por título e paginação: `GET /tarefas?tituloContem=exemplo&pagina=2&tamanho=5`
- Validação de entrada (`ValidadorDeTarefa`) e erros em Problem Details (400 e 404)
- Camada de serviço (`TarefaService`) separada dos endpoints
- CORS liberado para o front-end e Swagger em desenvolvimento

**Front-end (`frontend`)**
- Lista, cria, edita, remove e marca tarefas como concluídas
- Filtro por título e botões Anterior / Próxima
- Mostra na tela a mensagem de erro que a API devolve

## Como rodar

Pré-requisitos: [.NET 8 SDK](https://dotnet.microsoft.com/download) e [Node.js](https://nodejs.org).

**1. API** (terminal na pasta do projeto)

```bash
cd TarefasApi
dotnet run
```

A API sobe em `http://localhost:5137` e o Swagger abre em `http://localhost:5137/swagger`.

**2. Front-end** (outro terminal)

```bash
cd frontend
npm install
npm run dev
```

Abra `http://localhost:5173`.

## Estrutura

```
GerenciadorDeTarefas/
├── TarefasApi/
│   ├── Program.cs            # configuração e endpoints
│   ├── Tarefa.cs             # modelo e DTOs
│   ├── TarefasContext.cs     # EF Core
│   ├── TarefaService.cs      # regras de acesso aos dados
│   └── ValidadorDeTarefa.cs  # validação
└── frontend/
    └── src/
        ├── App.jsx
        └── lerProblema.js
```

## Autor

Nome do aluno: _preencha aqui_
