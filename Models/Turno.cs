namespace peluqueria.Models
{
    public class Turno
    {
        public int IdTurno { get; set; }
        public DateTime FechaEmitido { get; set; }
        public DateTime FechaTurno { get; set; }
        public string Estado { get; set; } = "";
        public int IdCliente { get; set; }
        public Usuario? Cliente { get; set; }
        public int IdEstilistaServicio { get; set; }
         public EstilistaServicio? EstilistaServicio { get; set; }
    }
}