using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using peluqueria.Models;

namespace peluqueria.Controllers
{
    public class ServiciosController : Controller
    {
        private readonly IRepositorioServicio repositorio;
        private readonly IConfiguration config;
        private readonly ILogger<ServiciosController> logger;

        public ServiciosController(IRepositorioServicio repo, IConfiguration config, ILogger<ServiciosController> logger)
        {
            this.repositorio = repo;
            this.config = config;
            this.logger = logger;
        }

        //GET: Servicios/Index
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

        //GET: Servicios/Create
        public ActionResult Create()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Mensaje = TempData["Error"];
            }
            return View();
        }

        //POST: Servicios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Servicio s)
        {
            try
            {
                repositorio.Alta(s);
                TempData["Id"] = s.IdServicio;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create de Servicios");
                TempData["Error"] = "Error al crear Servicios";
                return RedirectToAction(nameof(Create));
            }
        }

        //GET: Servicios/Edit
        public ActionResult Edit(int id)
        {
                var entidad = repositorio.ObtenerPorId(id);
                if (TempData.ContainsKey("Error"))
                {
                    ViewBag.Mensaje = TempData["Error"];
                }
                return View(entidad);
        }

        //POST: Servicios/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Servicio servicio)
        {
            try
            {
                if (id != servicio.IdServicio)
                {
                    return NotFound();
                }
                
                var s = repositorio.ObtenerPorId(id);
                if (s == null)
                {
                    return NotFound();
                }

                repositorio.Modificacion(s);
                TempData["Mensaje"] = "Exito...";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit de ServiciosController");
                TempData["Error"] = "Error al editar el servicio";
                return RedirectToAction(nameof(Edit));
            }
        }

        //GET: Servicios/Delete
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Mensaje = TempData["Error"];
            }
            return View(entidad);
        }

        //POST: Servicios/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Servicio servicio)
        {
            try
            {
                //TODO: Validar que nos devuelva un servicio
                repositorio.Baja(id);
                TempData["Mensaje"] = "Cambio de estado del pago exitoso";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Delete de ServiciosController");
                TempData["Error"] = "Error al dar de baja el servicio";
                return RedirectToAction(nameof(Delete));
            }
        }
    }
}