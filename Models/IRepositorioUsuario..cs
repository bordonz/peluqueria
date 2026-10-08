namespace peluqueria.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        List<Usuario> ObtenerLista(int paginaNro, int tamPagina);
        public int ObtenerCantidad();
        Usuario? ObtenerPorId(int id);
        Usuario? ObtenerPorEmail(string email);
        List<Usuario> BuscarPorNombre(string nombre);
        int CambiarClave(int idUsuario, string nuevaClave);
    }
}