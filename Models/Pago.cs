namespace peluqueria.Models
{
    public class Pago
    {
        public int IdPago { get; set; }
        public DateTime? FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string Estado { get; set; } = "Pendiente";
        public string Metodo { get; set; } = "";
        public int IdTurno { get; set; }
        public Turno? Turno { get; set; }
    }
}