using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace peluqueria.Services
{
	public static class ControllerBaseExtensions
	{
		public static int UsuarioId(this ControllerBase controllerBase)
		{
			var valor = controllerBase.User.FindFirstValue(ClaimTypes.NameIdentifier);
			return int.TryParse(valor, out var id) ? id : 0;
		}
    }
}