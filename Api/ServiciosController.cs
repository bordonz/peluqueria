using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using peluqueria.Models;
using peluqueria.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using peluqueria.Controllers;

namespace peluqueria.Api
{
    [ApiController]
	[Route("api/Servicios")]
	[Authorize(AuthenticationSchemes = "Cookies,JwtBearer")]
	public class ServiciosController : Controller
	{
		private readonly IRepositorioServicio repositorio;
		private readonly IWebHostEnvironment environment;

		public ServiciosController(IRepositorioServicio repositorio, IWebHostEnvironment environment)
		{
			this.repositorio = repositorio;
			this.environment = environment;
		}

		// GET: api/Servicios
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> Get(int paginaNro = 1, int tamPagina = 10, Boolean activo = true)
		{
			try
			{
				if(!activo && !User.IsInRole("Administrador"))
				{
					return Forbid();
				}

                var servicios = repositorio.ObtenerLista(paginaNro, tamPagina, activo);
				return Ok(servicios);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// GET api/Servicios/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			try
			{
				var servicio = repositorio.ObtenerPorId(id);
				return Ok(servicio);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// POST api/Servicios
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] Servicio entidad)
		{
			try
			{
				if(!User.IsInRole("Administrador"))
				{
					return Forbid();
				}

				if (string.IsNullOrWhiteSpace(entidad.Nombre) ||
					string.IsNullOrWhiteSpace(entidad.Descripcion) ||
					string.IsNullOrWhiteSpace(entidad.Duracion) ||
					entidad.Precio <= 0 ||
           		 	entidad.AvatarFile == null || entidad.AvatarFile.Length == 0)
				{
					return BadRequest("Todos los campos son obligatorios y el precio debe ser mayor a 0.");
				}
				
				//Al avatar/portada lo controlamos mediante el ModelState
				if (ModelState.IsValid)
				{
					int ServicioCreado = repositorio.Alta(entidad);
                    if(ServicioCreado > 0)
                    {
                        entidad.IdServicio = ServicioCreado;
						if (entidad.AvatarFile != null)
						{
							string wwwPath = environment.WebRootPath;
							string path = Path.Combine(wwwPath, "Uploads");
							if (!Directory.Exists(path))
							{
								Directory.CreateDirectory(path);
							}
							//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
							string fileName = "avatar_" + entidad.IdServicio + Path.GetExtension(entidad.AvatarFile.FileName);
							string pathCompleto = Path.Combine(path, fileName);
							entidad.Avatar = Path.Combine("/Uploads", fileName);
							// Esta operación guarda la foto en memoria en la ruta que necesitamos
							using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
							{
								await entidad.AvatarFile.CopyToAsync(stream);
							}
							repositorio.Modificacion(entidad);
						}
					    return CreatedAtAction(nameof(Get), new { id = ServicioCreado }, entidad);
                    }
                    return BadRequest("No se pudo registrar el servicio.");
				}
				return BadRequest(ModelState);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// PUT api/Servicios/5
		[HttpPut("{id}")]
		public async Task<IActionResult> Put(int id, [FromForm] Servicio entidad)
		{
			try
			{
				if(!User.IsInRole("Administrador"))
				{
					return Forbid();
				}

				if (id != entidad.IdServicio)
				{
					return BadRequest("El id de la ruta no coincide con el objeto");
				}
				
				bool huboCambios;
                var servicioE = repositorio.ObtenerPorId(id);
				if (servicioE == null)
				{
					return BadRequest();
				}
				else
				{
					huboCambios = servicioE.Nombre != entidad.Nombre ||
						servicioE.Descripcion != entidad.Descripcion ||
						servicioE.Duracion != entidad.Duracion ||
						servicioE.Precio != entidad.Precio ||
						servicioE.AvatarFile != entidad.AvatarFile;
				}

				if (!huboCambios)
				{
					return BadRequest(); //TODO: Revisar si la respuesta tiene que ser BadRequest()
				}

				//TODO: Si el usuario borra los campos precargados se cargan vacios???
                servicioE.Nombre = entidad.Nombre;
                servicioE.Descripcion = entidad.Descripcion;
                servicioE.Duracion = entidad.Duracion;
				servicioE.Precio = entidad.Precio;

                // Si subió avatar nuevo
                if (entidad.AvatarFile != null && entidad.AvatarFile.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileName = "avatar_" + id + Path.GetExtension(entidad.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    servicioE.Avatar = Path.Combine("/Uploads", fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        await entidad.AvatarFile.CopyToAsync(stream);
                    }
                }

				if (ModelState.IsValid)
				{
					repositorio.Modificacion(servicioE);
					return Ok(entidad);
				}
				return BadRequest(ModelState);
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		// DELETE api/Servicios/5
		[HttpDelete("{id}/BajaAlta")]
		public async Task<IActionResult> BajaAlta(int id)
		{
			try
			{
				if(!User.IsInRole("Administrador"))
				{
					return Forbid();
				}

				repositorio.Baja(id);
				return Ok();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}