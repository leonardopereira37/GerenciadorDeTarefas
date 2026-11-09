using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

const string CorsFrontEndLocal = "FrontEndLocal";

// ---------------------------------------------------------
// Fase de preparação: registrar serviços
// ---------------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TarefasContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TarefasDb")));

builder.Services.AddScoped<TarefaService>();

// Libera o front-end (Vite, porta 5173) para chamar a API.
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsFrontEndLocal, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Todo erro sai em Problem Details (application/problem+json).
builder.Services.AddProblemDetails();

// JSON mal escrito no corpo do pedido responde 400, e não 500.
builder.Services.Configure<RouteHandlerOptions>(opcoes => opcoes.ThrowOnBadRequest = false);

var app = builder.Build();

// ---------------------------------------------------------
// Banco: cria o arquivo tarefas.db na primeira execução
// e coloca 12 tarefas de exemplo (para ver a paginação funcionando).
// ---------------------------------------------------------
using (var escopo = app.Services.CreateScope())
{
    var db = escopo.ServiceProvider.GetRequiredService<TarefasContext>();
    db.Database.EnsureCreated();

    if (!db.Tarefas.Any())
    {
        for (var i = 1; i <= 12; i++)
            db.Tarefas.Add(new Tarefa { Titulo = $"Tarefa de exemplo {i:00}", Concluida = i % 4 == 0 });
        db.SaveChanges();
    }
}

// ---------------------------------------------------------
// Fase de execução: montar o pipeline, EM ORDEM
// ---------------------------------------------------------
app.UseExceptionHandler();   // exceção não tratada -> 500 em Problem Details
app.UseStatusCodePages();    // erro sem corpo (404, 405...) ganha corpo em Problem Details
app.UseCors(CorsFrontEndLocal);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// O mesmo 404 para GET, PUT e DELETE.
static IResult NaoEncontrada(int id) =>
    Results.Problem(
        statusCode: StatusCodes.Status404NotFound,
        title: "Tarefa não encontrada",
        detail: $"Não existe tarefa com id {id}.");

// ---------------------------------------------------------
// Endpoints (Minimal API) — CRUD completo de Tarefa
// ---------------------------------------------------------

// GET — listar (filtro opcional por título + paginação, via query string)
app.MapGet("/tarefas", (TarefaService s, string? tituloContem, int pagina = 1, int tamanho = 10) =>
{
    if (pagina < 1 || tamanho < 1 || tamanho > 50)
        return Results.ValidationProblem(new Dictionary<string, string[]>
        {
            ["paginacao"] = ["Use pagina >= 1 e tamanho entre 1 e 50."]
        });

    return Results.Ok(s.ListarPagina(tituloContem, pagina, tamanho));
})
.WithSummary("Lista tarefas, com filtro por título e paginação")
.Produces<List<TarefaDto>>(StatusCodes.Status200OK)
.ProducesValidationProblem();

// GET — buscar por id
app.MapGet("/tarefas/{id}", (int id, TarefaService s) =>
{
    var t = s.BuscarPorId(id);
    return t is not null
        ? Results.Ok(new TarefaDto(t.Id, t.Titulo, t.Concluida))
        : NaoEncontrada(id);
})
.WithSummary("Busca uma tarefa pelo id")
.Produces<TarefaDto>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);

// POST — criar
app.MapPost("/tarefas", (TarefaEntradaDto dto, TarefaService s) =>
{
    var erros = ValidadorDeTarefa.Validar(dto.Titulo);
    if (erros.Count > 0)
        return Results.ValidationProblem(erros, title: "Dados da tarefa inválidos");

    var criada = s.Criar(new Tarefa { Titulo = dto.Titulo!.Trim(), Concluida = dto.Concluida });
    var criadaDto = new TarefaDto(criada.Id, criada.Titulo, criada.Concluida);
    return Results.Created($"/tarefas/{criada.Id}", criadaDto);
})
.WithSummary("Cria uma tarefa")
.Produces<TarefaDto>(StatusCodes.Status201Created)
.ProducesValidationProblem();

// PUT — atualizar
app.MapPut("/tarefas/{id}", (int id, TarefaEntradaDto dto, TarefaService s) =>
{
    var erros = ValidadorDeTarefa.Validar(dto.Titulo);
    if (erros.Count > 0)
        return Results.ValidationProblem(erros, title: "Dados da tarefa inválidos");

    var ok = s.Atualizar(id, new Tarefa { Titulo = dto.Titulo!.Trim(), Concluida = dto.Concluida });
    return ok ? Results.NoContent() : NaoEncontrada(id);
})
.WithSummary("Substitui os dados de uma tarefa")
.Produces(StatusCodes.Status204NoContent)
.ProducesValidationProblem()
.ProducesProblem(StatusCodes.Status404NotFound);

// DELETE — remover
app.MapDelete("/tarefas/{id}", (int id, TarefaService s) =>
{
    var ok = s.Remover(id);
    return ok ? Results.NoContent() : NaoEncontrada(id);
})
.WithSummary("Remove uma tarefa")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status404NotFound);

app.Run();
