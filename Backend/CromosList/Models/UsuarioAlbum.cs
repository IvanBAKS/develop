namespace CromosList.Models;

public class UsuarioAlbum
{
    public long Id { get; set; }

    public long UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public long AlbumId { get; set; }
    public Album Album { get; set; } = null!;
}