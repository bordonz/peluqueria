using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using peluqueria.Models;
using peluqueria.Services;

namespace peluqueria.Controllers
{
    public class TurnosController : Controller
    {
        private readonly IRepositorioTurno repositorio;
        private readonly IRepositorioServicio repoServicio;
        private readonly IRepositorioUsuario repoUsuario;
        private readonly IConfiguration config;
        private readonly ILogger<ServiciosController> logger;

        public TurnosController(IRepositorioTurno repo, IRepositorioServicio repoServicio, IRepositorioUsuario repoUsuario, IConfiguration config, ILogger<ServiciosController> logger)
        {
            this.repositorio = repo;
            this.repoServicio = repoServicio;
            this.repoUsuario = repoUsuario;
            this.config = config;
            this.logger = logger;
        }

        //GET: Turnos/Index
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
                logger.LogError(ex, "Error en index de Turnos");
                throw;
            }
        }

        //GET: Turnos/Create
        public ActionResult Create()
        {
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Mensaje = TempData["Error"];
            }

            var servicios = repoServicio.ObtenerTodos();
            ViewBag.Servicio = new SelectList(servicios, "IdServicio", "Nombre");
            return View();
        }

        //POST: Turnos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Turno t, string horaTurno)
        {
            if (string.IsNullOrWhiteSpace(horaTurno))
            {
                ModelState.AddModelError("", "Debe seleccionar un horario para el turno.");
            }

            if (TimeSpan.TryParse(horaTurno, out TimeSpan hora))
            {
                // Combinar la fecha elegida con la hora recibida (ej: "14:30")
                t.FechaTurno = t.FechaTurno.Date.Add(hora);
            }

            if (!ModelState.IsValid)
            {
                var servicios = repoServicio.ObtenerTodos();
                ViewBag.Servicio = new SelectList(servicios, "IdServicio", "Nombre");
                return View(t);
            }


            try
            {
                repositorio.Alta(t);
                TempData["Id"] = t.IdTurno;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create de Turnos");
                var servicios = repoServicio.ObtenerTodos();
                ViewBag.Servicio = new SelectList(servicios, "IdServicio", "Nombre", t.IdServicio);
                ViewBag.Error = "Error al crear el Turno: " + ex.Message;
                
                return View(t);
            }
        }

        //GET: Turnos/Edit
        public ActionResult Edit(int id)
        {
                var entidad = repositorio.ObtenerPorId(id);
                if (TempData.ContainsKey("Error"))
                {
                    ViewBag.Mensaje = TempData["Error"];
                }
                return View(entidad);
        }

        //POST: Turnos/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Turno turno)
        {
            try
            {
                if (id != turno.IdTurno)
                {
                    return NotFound();
                }
                
                var t = repositorio.ObtenerPorId(id);
                if (t == null)
                {
                    return NotFound();
                }

                repositorio.Modificacion(t);
                TempData["Mensaje"] = "Edicion Exitosa";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Edit de Turnos");
                TempData["Error"] = "Error al editar el Turno";
                return RedirectToAction(nameof(Edit));
            }
        }

        //GET: Turnos/Delete
        public ActionResult Delete(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Error"))
            {
                ViewBag.Mensaje = TempData["Error"];
            }
            return View(entidad);
        }

        //POST: Turnos/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Turno turno)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Turno dado de baja";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Delete de Turnos");
                TempData["Error"] = "Error al dar de baja el turno";
                return RedirectToAction(nameof(Delete));
            }
        }

        public JsonResult ObtenerHorariosDisponibles(DateTime fecha, int idServicio)
        {
            List<string> horariosPosibles = new List<string>
            {
                "09:00", "10:00", "11:00", "12:00", "14:00", "15:00", "16:00", "17:00", "18:00"
            };

            // 2. Trae las horas que ya están ocupadas para esa fecha y servicio desde la BD
            List<string> horasOcupadas = repositorio.ObtenerHorasOcupadas(fecha, idServicio);

            // 3. Filtra solo los horarios libres
            var horariosDisponibles = horariosPosibles
                .Where(h => !horasOcupadas.Contains(h))
                .ToList();

            return Json(horariosDisponibles);
        }

        //TODO: Traer en base al usuario(estilista) logueado
        public ActionResult TurnosDiarios(int pagina = 1)
        {
            try
            {
                int user = this.UsuarioId();
                var tamaño = 5;
                var lista = repositorio.ObtenerTurnosDiarios(user, Math.Max(pagina, 1), tamaño);
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
                logger.LogError(ex, "Error al cargar los turnos diarios");
                throw;
            }
        }

        [HttpGet]
        public ActionResult TurnosSemanales(int contadorSemana)
        {
            try {
                List<Turno> turnos = repositorio.ObtenerTunosSemanales(contadorSemana);
                //TODO: Si la lista esta vacia mostrar con el notify un msj y redirigir al index
                return Json(turnos);
            }
            catch (Exception ex)
            {
                // Si hay un error de SQL o NullReferenceException en Include()
                return StatusCode(500, new { error = ex.Message });
            }
            }
    }
}