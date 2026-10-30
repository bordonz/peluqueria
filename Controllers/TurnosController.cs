using System.Security.Claims;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using peluqueria.Models;
using peluqueria.Services;

namespace peluqueria.Controllers
{
    [Authorize]
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

        // GET: Turnos
        public IActionResult Index()
        {
            return View(); 
        }

        // GET: Turnos/Index
        public ActionResult ObtenerTurnos(int pagina = 1, int tamPagina = 5)
        {
            try
            {
                bool usuario = User.IsInRole("Administrador");
                int logueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var lista = repositorio.ObtenerLista(usuario, logueado, Math.Max(pagina, 1), tamPagina);
                var total = repositorio.ObtenerCantidad(usuario, logueado);
                var totalPaginas = total % tamPagina == 0 ? total / tamPagina : (total / tamPagina) + 1;

                return Json(new {
                    items = lista,
                    totalPaginas = totalPaginas,
                    paginaActual = pagina
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en index de Turnos");
                return BadRequest(ex.Message);
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
        public ActionResult Create(Turno t, string horaTurno)
        {
            if (string.IsNullOrWhiteSpace(horaTurno))
            {
                ModelState.AddModelError("", "Debe seleccionar un horario para el turno.");
            }

            if (TimeSpan.TryParse(horaTurno, out TimeSpan hora))
            {
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
                t.IdCliente = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                t.IdEstilistaServicios = repoServicio.DevolverEstilistaServicio(t.EstilistaServicios.IdServicio, t.EstilistaServicios.IdEstilista);

                if (t.IdEstilistaServicios == -1)
                {
                    return BadRequest("La combinación de estilista y servicio seleccionada no es válida.");
                }

                int resultado = repositorio.Alta(t);
                if(resultado == -2)
                {
                    return BadRequest("El horario ya esta ocupado");
                }
                
                if(resultado == -1)
                {
                    return BadRequest();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Create de Turnos");
                var servicios = repoServicio.ObtenerTodos();    
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

        [HttpGet]
        public JsonResult EditPorId(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null)
            {
                return Json(new { success = false, message = "No encontrado" });
            }
            
            return Json(entidad);
        }
        
        //POST: Turnos/Edit
        [HttpPost]
        public ActionResult Edit(int id, Turno turno, [FromForm] int idServicio, [FromForm] string horaTurno)
        {
            int IdDelEditor = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var t = repositorio.ObtenerPorId(id);

            try
            {
                if (id != turno.IdTurno)
                {
                    return NotFound();
                }

                if (t == null)
                {
                    return NotFound();
                }
                DateTime nuevaFechaTurno = turno.FechaTurno;
                if (!string.IsNullOrEmpty(horaTurno) && TimeSpan.TryParse(horaTurno, out TimeSpan horaParsed))
                {
                    t.FechaTurno = turno.FechaTurno.Date.Add(horaParsed);
                }
                else
                {
                    t.FechaTurno = turno.FechaTurno;
                }
                
                if (idServicio > 0)
                {
                    t.EstilistaServicios = new EstilistaServicio
                    {
                        Servicio = new Servicio { IdServicio = idServicio }
                    };
                }

                if(User.IsInRole("Administrador") || User.IsInRole("Empleado"))
                {
                    repositorio.Modificacion(t);
                    return Ok();
                } else if (User.IsInRole("Cliente"))
                {
                    if(t.IdCliente != IdDelEditor)
                    {
                        return StatusCode(403);
                    }

                    if(nuevaFechaTurno <= DateTime.Now.AddDays(1))
                    {
                        return BadRequest();
                    }
                }

                t.FechaTurno = nuevaFechaTurno;
                repositorio.Modificacion(t);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //POST: Turnos/Delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                var turno = repositorio.ObtenerPorId(id);
                if (turno == null)
                {
                    return NotFound();
                }

                if (User.IsInRole("Administrador") || User.IsInRole("Empleado"))
                {
                    repositorio.Baja(id);
                    return Ok();
                } else if (User.IsInRole("Cliente") && turno.FechaTurno > DateTime.Now.AddDays(1))
                {
                    repositorio.Baja(id);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error en Delete de Turnos");
                return StatusCode(403, "No autorizado");
            }
        }

        public JsonResult ObtenerHorariosDisponibles(DateTime fecha, int IdEstilista, int idServicio)
        {
            List<string> horariosPosibles = new List<string>
            {
                "09:00", "10:00", "11:00", "12:00", "14:00", "15:00", "16:00", "17:00", "18:00"
            };

            List<string> horasOcupadas = repositorio.ObtenerHorasOcupadas(fecha, IdEstilista);
            int duracion = repositorio.ObtenerDuracionServicio(idServicio);
            List<string> horariosDisponibles = new List<string>();

            foreach (var hora in horariosPosibles)
            {
                bool espacioSuficiente = true;

                for(int i = 0; i < duracion; i++)
                {
                    TimeSpan horaActual = TimeSpan.Parse(hora).Add(TimeSpan.FromHours(i));
                    string horaString = horaActual.ToString(@"hh\:mm");
                        if (!horariosPosibles.Contains(horaString) || horasOcupadas.Contains(horaString))
                    {
                        espacioSuficiente = false;
                        break;
                    }
                }

                if (espacioSuficiente)
                {
                    horariosDisponibles.Add(hora);
                }
            }

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

                bool usuario = User.IsInRole("Administrador");
                int logueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var total = repositorio.ObtenerCantidad(usuario, logueado);
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
                int logueado = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                List<Turno> turnos = repositorio.ObtenerTunosSemanales(contadorSemana, logueado);
                return Json(turnos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            }
    }
}