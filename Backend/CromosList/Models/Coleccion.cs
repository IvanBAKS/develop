namespace CromosList.Models;

public class Coleccion
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
}
