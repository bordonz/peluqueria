using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using peluqueria.Models;
using peluqueria.Services;

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

        // GET: Usuarios
		[Authorize(Policy = "Administrador")]
		public ActionResult Index(int pagina = 1)
		{
            try
            {               
                var tamaño = 5;
                var usuarios = repositorio.ObtenerLista(Math.Max(pagina, 1), tamaño);
                ViewBag.Pagina = pagina;
                var total = repositorio.ObtenerCantidad();
                ViewBag.TotalPaginas = total % tamaño == 0 ? total / tamaño : total / tamaño + 1;
                ViewBag.Id = TempData["Id"];
                if (TempData.ContainsKey("Mensaje"))
                    ViewBag.Mensaje = TempData["Mensaje"];
                return View(usuarios);
            } catch(Exception ex)
            {
                logger.LogError(ex, "Error en Index de Propietarios");
				throw;
            }
		}

		// GET: Usuarios/Create
		[AllowAnonymous]
		public ActionResult Create()
		{
			ViewBag.Roles = Usuario.ObtenerRoles();
			return View();
		}

		// POST: Usuarios/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[AllowAnonymous]
		public ActionResult Create(Usuario u)
		{
			if (!ModelState.IsValid) 
			{
				ViewBag.Error = "Complete todos los campos requeridos correctamente.";
				ViewBag.Roles = Usuario.ObtenerRoles();
				return View(u);
			}

			if (!User.IsInRole("Administrador"))
			{
				u.Rol = 3;
			}

			try
			{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: u.Clave,
					salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
				u.Clave = hashed;
				int res = repositorio.Alta(u);
				if (u.AvatarFile != null && u.IdUsuario > 0)
				{
					string wwwPath = environment.WebRootPath;
					string path = Path.Combine(wwwPath, "Uploads");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
					string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.AvatarFile.FileName);
					string pathCompleto = Path.Combine(path, fileName);
					u.Avatar = Path.Combine("/Uploads", fileName);
					// Esta operación guarda la foto en memoria en la ruta que necesitamos
					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						u.AvatarFile.CopyTo(stream);
					}
					repositorio.Modificacion(u);
				}
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error al crear el usuario");
				ViewBag.Error = ex.Message;
				ViewBag.Roles = Usuario.ObtenerRoles();
				return View();
			}
		}

		// GET: Usuarios/Edit/5
		[Authorize]
		public ActionResult Perfil()
		{
			if (TempData.ContainsKey("Error"))
			{
				ViewBag.Mensaje = TempData["Error"];
			}else if (TempData.ContainsKey("Mensaje"))
			{
				ViewBag.Mensaje = TempData["Mensaje"];
			}
			ViewData["Title"] = "Mi perfil";
			var u = repositorio.ObtenerPorId(this.UsuarioId());
			ViewBag.Roles = Usuario.ObtenerRoles();
			return View(nameof(Edit), u);
		}

		// GET: Usuarios/Edit/5
		[Authorize(Policy = "Administrador")]
		public ActionResult Edit(int id)
		{
			if (TempData.ContainsKey("Error"))
			{
				ViewBag.Mensaje = TempData["Error"];
			}else if (TempData.ContainsKey("Mensaje"))
			{
				ViewBag.Mensaje = TempData["Mensaje"];
			}

			ViewData["Title"] = "Editar usuario";
			var u = repositorio.ObtenerPorId(id);
			ViewBag.Roles = Usuario.ObtenerRoles();
			return View(u);
		}

		// POST: Usuarios/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public async Task<ActionResult> Edit(int id, Usuario u)
		{
			var vista = nameof(Edit);//de que vista provengo
			try
			{
				if (!User.IsInRole("Administrador"))
				{
					vista = nameof(Perfil);
					// El Id ya viene en la cookie: se compara directo, sin ir a la BD.
					if (this.UsuarioId() != id)
						return RedirectToAction(nameof(Index), "Home");
				}

				if (!ModelState.IsValid)
                {
                    ViewBag.Roles = Usuario.ObtenerRoles();
                    return View("Edit", u);
                }
				bool huboCambios;
                var usuarioE = repositorio.ObtenerPorId(id);
				if (usuarioE == null)
				{
					return NotFound();
				}
				else
				{
					huboCambios = usuarioE.Nombre != u.Nombre ||
						usuarioE.Apellido != u.Apellido ||
						usuarioE.Email != u.Email ||
						(User.IsInRole("Administrador") && usuarioE.Rol != u.Rol) ||
						(u.AvatarFile != null && u.AvatarFile.Length > 0);
				}

				if (!huboCambios)
				{
					ViewBag.Error = "No se realizaron cambios.";
					ViewBag.Roles = Usuario.ObtenerRoles();
					return View("Edit", usuarioE);
				}

                usuarioE.Nombre = u.Nombre;
                usuarioE.Apellido = u.Apellido;
                usuarioE.Email = u.Email;
				if (User.IsInRole("Administrador"))
				{
                	usuarioE.Rol = u.Rol;
				}

                // Si subió avatar nuevo
                if (u.AvatarFile != null && u.AvatarFile.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileName = "avatar_" + id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    usuarioE.Avatar = Path.Combine("/Uploads", fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                }

                repositorio.Modificacion(usuarioE);
				var claims = new List<Claim>
				{
					new Claim(ClaimTypes.NameIdentifier, usuarioE.IdUsuario.ToString()),
					new Claim(ClaimTypes.Name, usuarioE.Email),
					new Claim("FullName", usuarioE.Nombre + " " + usuarioE.Apellido),
					new Claim(ClaimTypes.Role, usuarioE.Rol.ToString())
				};

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                ViewBag.Mensaje = "Datos guardados correctamente.";
				ViewBag.Roles = Usuario.ObtenerRoles();
				
				return View("Edit", usuarioE);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error al editar el usuario");
				ViewBag.Error = "Ocurrió un error al guardar los cambios.";
				ViewBag.Roles = Usuario.ObtenerRoles();
				return View("Edit", u);
			}
		}

		// GET: Usuarios/Delete/5
		[Authorize(Policy = "Administrador")]
		public ActionResult Delete(int id)
		{
			var entidad = repositorio.ObtenerPorId(id);
            if (entidad == null)
            {
                return NotFound();
            }
            return View(entidad);
		}

		// POST: Usuarios/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrador")]
		public ActionResult Delete(int id, Usuario usuario)
		{
			try
			{
				var ruta = Path.Combine(environment.WebRootPath, "Uploads", $"avatar_{id}" + Path.GetExtension(usuario.Avatar));
				if (System.IO.File.Exists(ruta))
					System.IO.File.Delete(ruta);
				repositorio.Baja(id);
				return RedirectToAction(nameof(Index));
			}
			catch(Exception ex)
			{
				logger.LogError(ex, "Error al eliminar el usuario");
				return RedirectToAction(nameof(Delete));
			}
		}

		[Authorize]
		public IActionResult Avatar()
		{
			var u = repositorio.ObtenerPorId(this.UsuarioId());
			if (u == null || string.IsNullOrEmpty(u.Avatar))
				return NotFound();
			string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.Avatar);
			string wwwPath = environment.WebRootPath;
			string path = Path.Combine(wwwPath, "Uploads");
			string pathCompleto = Path.Combine(path, fileName);

			//leer el archivo
			byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
			//devolverlo
			return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
		}

		[Authorize]
		public string AvatarBase64()
		{
			var u = repositorio.ObtenerPorId(this.UsuarioId());
			if (u == null || string.IsNullOrEmpty(u.Avatar))
				return "";
			string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(u.Avatar);
			string wwwPath = environment.WebRootPath;
			string path = Path.Combine(wwwPath, "Uploads");
			string pathCompleto = Path.Combine(path, fileName);

			//leer el archivo
			byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
			//devolverlo
			return Convert.ToBase64String(fileBytes);
		}

		[Authorize]
		[HttpPost("[controller]/[action]/{fileName}")]
		public IActionResult FromBase64([FromBody] string imagen, [FromRoute] string fileName)
		{
			//arma el path
			string wwwPath = environment.WebRootPath;
			string path = Path.Combine(wwwPath, "Uploads");
			string pathCompleto = Path.Combine(path, fileName);
			//convierto a arreglo de bytes
			var bytes = Convert.FromBase64String(imagen);
			//lo escribe
			System.IO.File.WriteAllBytes(pathCompleto, bytes);
			return Ok();
		}

		[Authorize]
		public ActionResult Foto()
		{
			try
			{
				var u = repositorio.ObtenerPorId(this.UsuarioId());
				if (u == null || string.IsNullOrEmpty(u.Avatar))
					return NotFound();
				var stream = System.IO.File.Open(
						Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)),
						FileMode.Open,
						FileAccess.Read);
				var ext = Path.GetExtension(u.Avatar);
				return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error al obtener la foto");
				throw;
			}
		}

		[AllowAnonymous]
		// GET: Usuarios/Login/
		public ActionResult Login(string returnUrl)
		{
			TempData["returnUrl"] = returnUrl;
			return View();
		}

		// POST: Usuarios/Login/
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginView login)
		{
			try
			{
				var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : (TempData["returnUrl"] ?? "").ToString();
				if (ModelState.IsValid)
				{
					string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: login.Clave,
						salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));

					var e = repositorio.ObtenerPorEmail(login.Usuario);
					if (e == null || e.Clave != hashed)
					{
						ModelState.AddModelError("", "El email o la clave no son correctos");
						TempData["returnUrl"] = returnUrl;
						return View();
					}

					var claims = new List<Claim>
					{
						// Identificador estable: es el que se usa para consultar (clave primaria).
						new Claim(ClaimTypes.NameIdentifier, e.IdUsuario.ToString()),
						new Claim(ClaimTypes.Name, e.Email),
						new Claim("FullName", e.Nombre + " " + e.Apellido),
						new Claim(ClaimTypes.Role, e.RolNombre),
					};

					// Los dos últimos parámetros eligen qué claim es el "nombre" (User.Identity.Name)
					// y cuál el "rol" (User.IsInRole). Es el equivalente, para la cookie, de
					// TokenValidationParameters.NameClaimType/RoleClaimType del JWT (ver Program.cs).
					// Con esto User.Identity.Name devuelve el Id, no el email.
					var claimsIdentity = new ClaimsIdentity(
							claims, CookieAuthenticationDefaults.AuthenticationScheme,
							ClaimTypes.NameIdentifier, ClaimTypes.Role);

					await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							new ClaimsPrincipal(claimsIdentity));
					TempData.Remove("returnUrl");
					return Redirect(returnUrl ?? "/");
				}
				TempData["returnUrl"] = returnUrl;
				return View();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		// GET: /salir
		[Route("salir", Name = "logout")]
		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync(
					CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}

		// GET: Usuarios/CambiarClave/5
		[Authorize]
		public ActionResult CambiarClave(int id)
		{
			// Validar permisos: Si no es Admin y tampoco es su propio ID, no se permite.
			if (!User.IsInRole("Administrador") && this.UsuarioId() != id)
			{
				return RedirectToAction(nameof(Index), "Home");
			}

			var usuario = repositorio.ObtenerPorId(id);
			if (usuario == null)
			{
				return NotFound();
			}

			ViewData["Title"] = $"Cambiar contraseña de {usuario.Nombre} {usuario.Apellido}";
			return View(usuario); // Pasamos la entidad Usuario directa
		}

		// POST: Usuarios/CambiarClave/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult CambiarClave(int id, string? claveAntigua, string claveNueva, string confirmarClave)
		{
			bool esAdmin = User.IsInRole("Administrador");

			// Verificar permisos sobre el usuario
			if (!esAdmin && this.UsuarioId() != id)
			{
				return RedirectToAction(nameof(Index), "Home");
			}

			var usuarioBD = repositorio.ObtenerPorId(id);
			if (usuarioBD == null)
			{
				return NotFound();
			}

			// Validar requeridos y coincidencia
			if (string.IsNullOrEmpty(claveNueva))
			{
				ModelState.AddModelError("", "La nueva contraseña es requerida.");
			}

			if (claveNueva != confirmarClave)
			{
				ModelState.AddModelError("", "La nueva contraseña y la confirmación no coinciden.");
			}

			if (!esAdmin)
			{
				if (string.IsNullOrEmpty(claveAntigua))
				{
					ModelState.AddModelError("", "Debes ingresar tu contraseña actual.");
				}
				else
				{
					string claveAntiguaHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
						password: claveAntigua,
						salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
						prf: KeyDerivationPrf.HMACSHA1,
						iterationCount: 1000,
						numBytesRequested: 256 / 8));

					if (usuarioBD.Clave != claveAntiguaHashed)
					{
						ModelState.AddModelError("", "La contraseña actual es incorrecta.");
					}
				}
			}

			if (!ModelState.IsValid)
			{
				ViewData["Title"] = $"Cambiar contraseña de {usuarioBD.Nombre} {usuarioBD.Apellido}";
				return View(usuarioBD);
			}

			// Hashear la nueva contraseña
			string nuevaClaveHashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: claveNueva,
				salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"] ?? ""),
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 1000,
				numBytesRequested: 256 / 8));

			repositorio.CambiarClave(id, nuevaClaveHashed);

			TempData["Mensaje"] = "Contraseña actualizada correctamente.";

			if (esAdmin)
			{
				return RedirectToAction(nameof(Edit), new { id = id });
			}
			return RedirectToAction(nameof(Perfil));
		}
    }
}