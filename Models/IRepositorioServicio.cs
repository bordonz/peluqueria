namespace peluqueria.Models
{
    public interface IRepositorioServicio: IRepositorio<Servicio>
    {
        List<Servicio> ObtenerLista(int paginaNro, int tamPagina, Boolean estado);
        int ObtenerCantidad();
        Servicio? ObtenerPorId(int id);
        List<Servicio> ObtenerTodos();
        int DevolverEstilistaServicio(int IdServicio, int IdEstilista);
    }
}