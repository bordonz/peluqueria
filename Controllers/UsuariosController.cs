using Microsoft.AspNetCore.Mvc;
using peluqueria.Models;

namespace peluqueria.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> logger;
		private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;
		private readonly IRepositorioUsuario repositorio;

		public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio, ILogger<UsuariosController> logger)
		{
			this.configuration = configuration;
			this.environment = environment;
			this.repositorio = repositorio;
			this.logger = logger;
		}

        //GET: Usuarios/Buscar/5
        [Route("[controller]/Buscar/{q}", Name = "Buscar")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorNombre(q);
                return Json( new { datos = res });
            }
            catch (Exception ex)
            {
                return Json(new { datos = new List<Usuario>(), error = ex.Message });
            }
        }
    }
}