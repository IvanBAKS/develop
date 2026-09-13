namespace CromosList.Models;

public class Editorial
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public ICollection<Album> Albums { get; set; } = new List<Album>();
}
