// Tarefa.cs
// O modelo (tabela do banco) e os DTOs (o que entra e sai pela API).

public class Tarefa
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public bool Concluida { get; set; }
}

// O que a API devolve.
public record TarefaDto(int Id, string Titulo, bool Concluida);

// O que a API recebe no POST e no PUT.
public record TarefaEntradaDto(string? Titulo, bool Concluida);
