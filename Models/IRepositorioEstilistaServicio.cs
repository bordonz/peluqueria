namespace peluqueria.Models
{
    public interface IRepositorioEstilistaServicio: IRepositorio<EstilistaServicio>
    {
        List<EstilistaServicio> ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        EstilistaServicio? ObtenerPorId(int id);
    }
}