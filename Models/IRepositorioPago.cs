namespace peluqueria.Models
{
    public interface IRepositorioPago: IRepositorio<Pago>
    {
        List<Pago> ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        Pago? ObtenerPorId(int id);
    }
}