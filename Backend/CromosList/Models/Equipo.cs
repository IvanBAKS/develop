namespace CromosList.Models;

public class Equipo
    {
        public long Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Jugador> Jugadores { get; set; } = new List<Jugador>();
        public ICollection<Cromo> Cromos { get; set; } = new List<Cromo>();
    }
