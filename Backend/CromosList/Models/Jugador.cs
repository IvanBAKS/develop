namespace CromosList.Models;

public class Jugador
{
    public long Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string NombreCompleto { get; set; } = string.Empty;
    public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
}