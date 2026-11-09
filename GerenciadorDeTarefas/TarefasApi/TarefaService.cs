using Microsoft.EntityFrameworkCore;

public class TarefaService
{
    private readonly TarefasContext _context;

    public TarefaService(TarefasContext context) => _context = context;

    // Lista uma página de tarefas, com filtro opcional por título.
    public List<TarefaDto> ListarPagina(string? tituloContem, int pagina, int tamanho)
    {
        var consulta = _context.Tarefas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tituloContem))
            consulta = consulta.Where(t => t.Titulo.Contains(tituloContem));

        return consulta
            .OrderBy(t => t.Id)                 // ordem fixa: sem ela, as páginas podem mudar
            .Skip((pagina - 1) * tamanho)       // pula as páginas anteriores
            .Take(tamanho)                      // pega só esta página
            .Select(t => new TarefaDto(t.Id, t.Titulo, t.Concluida))
            .ToList();                          // SÓ AQUI vai ao banco
    }

    // Sem AsNoTracking de propósito: Atualizar e Remover precisam do rastreamento do EF Core.
    public Tarefa? BuscarPorId(int id) => _context.Tarefas.Find(id);

    public Tarefa Criar(Tarefa tarefa)
    {
        _context.Tarefas.Add(tarefa);
        _context.SaveChanges();
        return tarefa;
    }

    public bool Atualizar(int id, Tarefa dados)
    {
        var tarefa = BuscarPorId(id);
        if (tarefa is null) return false;

        tarefa.Titulo = dados.Titulo;
        tarefa.Concluida = dados.Concluida;
        _context.SaveChanges();
        return true;
    }

    public bool Remover(int id)
    {
        var tarefa = BuscarPorId(id);
        if (tarefa is null) return false;

        _context.Tarefas.Remove(tarefa);
        _context.SaveChanges();
        return true;
    }
}
