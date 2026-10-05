namespace peluqueria.Models
{
    public interface IRepositorioServicio: IRepositorio<Servicio>
    {
        List<Servicio> ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        Servicio? ObtenerPorId(int id);
        public List<Servicio> ObtenerTodos();
    }
}