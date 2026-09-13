namespace CromosList.Models;

public class Publicacion
{
    public long Id { get; set; }

    public string Temporada { get; set; } = string.Empty;

    public string? Nombre { get; set; }

    public DateTime FechaPublicacion { get; set; }

    public long EdicionId { get; set; }

    public Edicion Edicion { get; set; } = null!;

    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
}