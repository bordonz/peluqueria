using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using peluqueria.Models;

namespace peluqueria.Controllers
{
    [Authorize(Policy = "Administrador")]
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

        //GET: EstilistoServicios/Index
        public ActionResult Index(int pagina = 1)
        {
            try
            {
                var tamaño = 5;
                var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño +1;
                ViewBag.id = TempData["id"];

                if (TempData.ContainsKey("Mensaje"))
                {
                    ViewBag.Mensaje = TempData["Mensaje"];
                }
                return View(lista);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en index de Servicios");
                throw;
            }
        }

        [HttpGet]
        public IActionResult VincularEstilista()
        {
            var servicios = repositorioServicio.ObtenerTodos();
            ViewBag.IdServicio = new SelectList(servicios, "IdServicio", "Nombre");
            return View();
        }

        [HttpGet]
        public IActionResult DeleteVinculo()
        {
            return View();
        }

        [HttpGet]
        public JsonResult ObtenerServiciosPorEstilista(int idEstilista)
        {
            var servicios = repositorioServicio.ObtenerTodosPorEstilista(idEstilista); 
            return Json(servicios);
        }

        [Route("EstilistaServicios/BuscarEstilista/{q}")]
        public IActionResult BuscarEstilista(string nombre){
            var res = repositorio.BuscarEstilista(nombre);
            return Json( new { datos = res });
        }

        //POST: EstilistaServicios/Create
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

 /*        //GET: EstilistaServicios/Delete
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);
        } */

        //POST: EstilistaServicios/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int idEstilista, int idServicio)
        {
            try
            {
                int idEstilistaServicios = repositorioServicio.DevolverEstilistaServicio(idServicio, idEstilista);

                if (idEstilistaServicios != -1)
                {
                    repositorio.Baja(idEstilistaServicios);
                }

                TempData["Mensaje"] = "Vinculo dado de baja";
                return RedirectToAction("DeleteVinculo");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Delete de EstilistaServiciosController");
                TempData["Error"] = "Error al dar de baja el vinculo";
                return RedirectToAction(nameof(Delete));
            }
        }

        [HttpGet]
        public ActionResult ObtenerEstilistas(int idServicio)
        {
            var estilistas = repositorio.ObtenerEstilista(idServicio);
                return Json(estilistas);
        }
    }
}