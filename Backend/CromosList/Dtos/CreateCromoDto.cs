namespace CromosList.Dtos;

public class CreateCromoDto
{
    public string Numero { get; set; } = string.Empty;

    public long JugadorId { get; set; }

    public long EquipoId { get; set; }

    public long EdicionId { get; set; }

    public long? PublicacionId { get; set; }

    public long TipoCromoId { get; set; }

    public long? ColeccionId { get; set; }

    public long AlbumId { get; set; }
}