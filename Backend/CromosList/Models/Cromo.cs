namespace CromosList.Models;

public class Cromo
{
    public long Id { get; set; }

    public string Numero { get; set; } = string.Empty;

    public long JugadorId { get; set; }
    public Jugador Jugador { get; set; } = null!;

    public long EquipoId { get; set; }
    public Equipo Equipo { get; set; } = null!;

    public long EdicionId { get; set; }
    public Edicion Edicion { get; set; } = null!;

    public long? PublicacionId { get; set; }
    public Publicacion? Publicacion { get; set; }

    public long TipoCromoId { get; set; }
    public TipoCromo TipoCromo { get; set; } = null!;

    public long? ColeccionId { get; set; }
    public Coleccion? Coleccion { get; set; }

    public long AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public ICollection<UsuarioCromo> UsuariosCromo { get; set; } = new List<UsuarioCromo>();
}