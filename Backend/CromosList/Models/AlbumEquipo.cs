namespace CromosList.Models;

public class AlbumEquipo
{
    public long Id { get; set; }

    public long AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public long EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public int Orden { get; set; }
}
