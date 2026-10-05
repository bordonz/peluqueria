using System.Data;
using MySql.Data.MySqlClient;

namespace peluqueria.Models
{
    public class RepositorioUsuario : RepositorioBase, IRepositorioUsuario
    {
        public RepositorioUsuario(IConfiguration configuration) : base(configuration)
        {
            
        }

        public int Alta(Usuario e)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Usuarios 
					(nombre, apellido, avatar, email, clave, rol) 
					VALUES (@nombre, @apellido, @avatar, @email, @clave, @rol);
                    SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					if (String.IsNullOrEmpty(e.Avatar))
						command.Parameters.AddWithValue("@avatar", DBNull.Value);
					else
						command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					e.IdUsuario = res;
					connection.Close();
				}
			}
			return res;
		}

		public int Baja(int id)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = "DELETE FROM Usuarios WHERE id_usuario = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Usuario e)
		{
			int res = -1;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"UPDATE Usuarios 
					SET nombre=@nombre, apellido=@apellido, avatar=@avatar, email=@email, clave=@clave, rol=@rol
					WHERE id_usuario = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", e.Nombre);
					command.Parameters.AddWithValue("@apellido", e.Apellido);
					if (String.IsNullOrEmpty(e.Avatar))
						command.Parameters.AddWithValue("@avatar", DBNull.Value);
					else
                		command.Parameters.AddWithValue("@avatar", e.Avatar);
					command.Parameters.AddWithValue("@email", e.Email);
					command.Parameters.AddWithValue("@clave", e.Clave);
					command.Parameters.AddWithValue("@rol", e.Rol);
					command.Parameters.AddWithValue("@id", e.IdUsuario);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public List<Usuario> ObtenerLista(int paginaNro, int tamPagina)
		{
			List<Usuario> res = new List<Usuario>();
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT id_usuario, nombre, apellido, avatar, email, clave, rol
					FROM Usuarios
					ORDER BY id_usuario
					LIMIT @tamPagina OFFSET @offset";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro - 1) * tamPagina);
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario e = new Usuario
						{
							IdUsuario = reader.GetInt32("id_usuario"),
							Nombre = reader.GetString("nombre"),
							Apellido = reader.GetString("apellido"),
							Avatar = reader["avatar"] is DBNull ? null : reader.GetString("avatar"),
							Email = reader.GetString("email"),
							Clave = reader.GetString("clave"),
							Rol = reader.GetInt32("rol"),
						};
						res.Add(e);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT COUNT(id_usuario)
					FROM Usuarios";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					connection.Close();
				}
			}
			return res;
		}

		public Usuario? ObtenerPorId(int id)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT id_usuario, nombre, apellido, avatar, email, clave, rol 
					FROM Usuarios
					WHERE id_usuario=@id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							IdUsuario = reader.GetInt32("id_usuario"),
							Nombre = reader.GetString("nombre"),
							Apellido = reader.GetString("apellido"),
							Avatar = reader["avatar"] is DBNull ? null : reader.GetString("avatar"),
							Email = reader.GetString("email"),
							Clave = reader.GetString("clave"),
							Rol = reader.GetInt32("rol"),
						};
					}
					connection.Close();
				}
			}
			return e;
		}

		public Usuario? ObtenerPorEmail(string email)
		{
			Usuario? e = null;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT id_usuario, nombre, apellido, avatar, email, clave, rol
                    FROM Usuarios
					WHERE email=@email";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						e = new Usuario
						{
							IdUsuario = reader.GetInt32("id_usuario"),
							Nombre = reader.GetString("nombre"),
							Apellido = reader.GetString("apellido"),
							Avatar = reader["avatar"] is DBNull ? null : reader.GetString("avatar"),
							Email = reader.GetString("email"),
							Clave = reader.GetString("clave"),
							Rol = reader.GetInt32("rol"),
						};
					}
					connection.Close();
				}
			}
			return e;
		}

		public List<Usuario> BuscarPorNombre(string nombre)
        {
            List<Usuario> res = new List<Usuario>();
            nombre = "%" + nombre + "%"; //TODO: Optimizar
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT u.*
                    FROM Usuarios u
                    WHERE nombre LIKE @nombre OR apellido LIKE @nombre";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@nombre", MySqlDbType.VarChar).Value = nombre;;
                    command.CommandType = CommandType.Text;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var u = new Usuario
                        {    
                            IdUsuario = reader.GetInt32("id_usuario"),
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido"),
                            Email = reader.GetString("email"),
                        };
                        res.Add(u);
                    }
                    connection.Close();
                }
            }
            return res;
        }
	}
}