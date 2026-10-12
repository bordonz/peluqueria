using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using peluqueria.Models;

namespace peluqueria.Controllers
{
    public class EstilistaServiciosController : Controller
    {
        private readonly IRepositorioEstilistaServicio repositorio;
        private readonly IRepositorioServicio repositorioServicio;
        private readonly IConfiguration config;
        private readonly ILogger<EstilistaServiciosController> logger;

        public EstilistaServiciosController(IRepositorioEstilistaServicio repo, IRepositorioServicio repoServicio,IConfiguration config, ILogger<EstilistaServiciosController> logger)
        {
            this.repositorio = repo;
            this.repositorioServicio = repoServicio;
            this.config = config;
            this.logger = logger;
        }
        [HttpGet]
        public IActionResult VincularEstilista()
        {
            var servicios = repositorioServicio.ObtenerTodos();
                ViewBag.IdServicio = new SelectList(servicios, "IdServicio", "Nombre");
                return View();
        }

        [Route("EstilistaServicios/BuscarEstilista/{q}")]
        public IActionResult BuscarEstilista(string nombre){
            var res = repositorio.BuscarEstilista(nombre);
            return Json( new { datos = res });
        }

        //POST: Servicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EstilistaServicio s)
        {
            try
            {
                if (repositorio.VerificarVinculo(s.IdServicio, s.IdEstilista))
                {
                    TempData["Error"] = "El estilista ya esta vinculado a un servicio";
                    return RedirectToAction(nameof(VincularEstilista));
                }
                repositorio.Alta(s);
                TempData["Mensaje"] = "Estilista vinculado al servicio con exito";
                return RedirectToAction(nameof(VincularEstilista));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create de EstilistaServicio");
                TempData["Error"] = "Error al vincular el estilista al servicio";
                return RedirectToAction(nameof(VincularEstilista));
            }
        }
    }
}