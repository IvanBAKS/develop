namespace CromosList.Dtos;

public class AuthResponseDto
{
    public string Token { get; set; } = string.Empty;

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EsAdmin { get; set; }
}
