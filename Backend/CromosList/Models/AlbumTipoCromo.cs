namespace CromosList.Models;

public class AlbumTipoCromo
{
    public long Id { get; set; }

    public long AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public long TipoCromoId { get; set; }
    public TipoCromo TipoCromo { get; set; } = null!;

    public int Orden { get; set; }
}
