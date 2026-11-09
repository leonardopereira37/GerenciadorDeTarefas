// ValidadorDeTarefa.cs
// Regras de entrada de uma tarefa, usadas no POST e no PUT.
// Devolve os erros no formato que Results.ValidationProblem espera:
// nome do campo -> lista de mensagens daquele campo.
public static class ValidadorDeTarefa
{
    public const int TituloMaximo = 100;

    public static Dictionary<string, string[]> Validar(string? titulo)
    {
        var erros = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(titulo))
            erros["titulo"] = ["O título é obrigatório."];
        else if (titulo.Length > TituloMaximo)
            erros["titulo"] = [$"O título pode ter no máximo {TituloMaximo} caracteres."];

        return erros;
    }
}
