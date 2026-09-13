namespace CromosList.Models;
public class Edicion
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
    public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
}