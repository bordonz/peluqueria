using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace peluqueria.Models
{
    public enum EnRoles
	{
		Administrador = 1,
		Empleado = 2,
		Cliente = 3,
	}

	public class Usuario
	{
		[Key]
		[Display(Name = "Código")]
		public int IdUsuario { get; set; }
		[Required]
		public string Nombre { get; set; } = "";
		[Required]
		public string Apellido { get; set; } = "";
		[Required, EmailAddress]
		public string Email { get; set; } = "";
		[Required, DataType(DataType.Password)]
		public string Clave { get; set; } = "";
		public string? Avatar { get; set; }
		[NotMapped]//Para EF
		public IFormFile? AvatarFile { get; set; }
		public int Rol { get; set; }
		[NotMapped]//Para EF
		public string RolNombre => Rol > 0 ? ((EnRoles)Rol).ToString() : "";

		public static IDictionary<int, string> ObtenerRoles()
		{
			SortedDictionary<int, string> roles = new SortedDictionary<int, string>();
			Type tipoEnumRol = typeof(EnRoles);
			foreach (var valor in Enum.GetValues(tipoEnumRol))
			{
				roles.Add((int)valor, Enum.GetName(tipoEnumRol, valor) ?? "");
			}
			return roles;
		}
	}
}