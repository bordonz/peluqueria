namespace peluqueria.Models
{
    public class EstilistaServicio
    {
        public int IdEstilistaServicio { get; set; } // Clave primaria individual

        public int IdEstilista { get; set; }
        public Usuario? Estilista { get; set; }

        public int IdServicio { get; set; }
        public Servicio? Servicio { get; set; }
    
    }
}