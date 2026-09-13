namespace CromosList.Models;

public class UsuarioCromo
{
    public long Id { get; set; }

    public long UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public long CromoId { get; set; }
    public Cromo Cromo { get; set; } = null!;
}