
namespace Application.Services.Common;

public static class Validators
{
    public static bool EsEmailValido(string email)
        => !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.IndexOf('@') > 0;

    public static bool SoloDigitos(string? texto)
        => !string.IsNullOrWhiteSpace(texto) && texto.All(char.IsDigit);

    public static bool TipoUsuarioValido(string? tipo)
        => tipo == "Cliente" || tipo == "Vendedor";
}
