namespace CromosList.Models;

public class Album
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Temporada { get; set; } = string.Empty;

    public long EditorialId { get; set; }
    public Editorial Editorial { get; set; } = null!;

    public ICollection<AlbumEquipo> AlbumEquipos { get; set; } = new List<AlbumEquipo>();
    public ICollection<AlbumTipoCromo> AlbumTiposCromo { get; set; } = new List<AlbumTipoCromo>();
    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
}
