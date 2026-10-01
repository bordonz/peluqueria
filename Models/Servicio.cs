namespace peluqueria.Models
{
    public class Servicio
    {
        public int IdServicio { get; set; }
        public string Nombre { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Duracion { get; set; } = "";
        public decimal Precio { get; set; }
        public int IdPeluquero { get; set; }
        public Usuario? Peluquero { get; set; } 
        public bool Activo { get; set; } = true;
    }
}