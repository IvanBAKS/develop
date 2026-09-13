namespace CromosList.Models;

public class TipoCromo
{
    public long Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
}