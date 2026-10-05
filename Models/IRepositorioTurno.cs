namespace peluqueria.Models
{
    public interface IRepositorioTurno: IRepositorio<Turno>
    {
        List<Turno> ObtenerLista(int paginaNro, int tamPagina);
        int ObtenerCantidad();
        Turno? ObtenerPorId(int id);
        List<string> ObtenerHorasOcupadas(DateTime fecha, int idServicio);
        List<Turno> ObtenerTurnosDiarios(int paginaNro, int tamPagina);
        public List<Turno> ObtenerTunosSemanales(int sumaSemana);
    }
}