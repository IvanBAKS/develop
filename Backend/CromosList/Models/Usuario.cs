namespace CromosList.Models;

public class Usuario
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;

    public bool EsAdmin { get; set; }

    public ICollection<UsuarioCromo> Cromos { get; set; } = new List<UsuarioCromo>();

    public ICollection<UsuarioAlbum> Albumes { get; set; } = new List<UsuarioAlbum>();
}