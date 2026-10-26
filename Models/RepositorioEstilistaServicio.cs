using System.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Cms;

namespace peluqueria.Models
{
    public class RepositorioEstilistaServicio: RepositorioBase, IRepositorioEstilistaServicio
    {
        public RepositorioEstilistaServicio(IConfiguration configuration) : base(configuration)
        {
            
        }
        public int Alta(EstilistaServicio s)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Estilista_Servicios
                    (id_estilista, id_Servicio)
                    VALUES (@id_estilista, @id_servicio);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_estilista", s.IdEstilista);
                    command.Parameters.AddWithValue("@id_servicio", s.IdServicio);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    s.IdServicio = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"DELETE FROM Estilista_Servicios
                    WHERE id_estilista_servicios = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(EstilistaServicio s)
        {
            int res = -1;
            return res;
        }

        public List<EstilistaServicio> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<EstilistaServicio> res = new List<EstilistaServicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT e.*, p.nombre AS estilista_nombre, p.apellido AS estilista_apellido,
                    s.nombre AS nombre_servicio, s.precio
                    FROM Estilista_Servicios e
                    INNER JOIN Usuarios p ON e.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON e.id_servicio = s.id_servicio
                    ORDER BY e.id_estilista_servicios
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro -1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EstilistaServicio s = new EstilistaServicio
                        {
                            IdEstilistaServicio = reader.GetInt32("id_estilista_servicios"),
                            IdEstilista = reader.GetInt32("id_estilista"),
                            Estilista = new Usuario
                            {
                                Nombre = reader.GetString("estilista_nombre"),
                                Apellido = reader.GetString("estilista_apellido")
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Precio = reader.GetDecimal("precio"),
                            }
                        };
                        res.Add(s);
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidad()
        {
            int res = 0;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(id_estilista_servicios)
                    FROM estilista_servicios";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        res = reader.GetInt32(0);
                    }
                }
            }
            return res;
        }

        public EstilistaServicio? ObtenerPorId(int id)
        {
            EstilistaServicio? s = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT e.*, p.nombre AS estilista_nombre, p.apellido AS estilista_apellido,
                    s.nombre AS nombre_servicio, s.descripcion
                    FROM Estilista_Servicios e
                    INNER JOIN Usuarios p ON e.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON e.id_servicio = s.id_servicio
                    WHERE e.id_estilista_servicios = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        s = new EstilistaServicio
                        {
                            IdEstilistaServicio = reader.GetInt32("id_estilista_servicios"),
                            IdEstilista = reader.GetInt32("id_estilista"),
                            Estilista = new Usuario
                            {
                                Nombre = reader.GetString("estilista_nombre"),
                                Apellido = reader.GetString("estilista_apellido")
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Descripcion = reader.GetString("descripcion"),
                            }
                        };
                    }
                }
                return s;
            }
        }

        public List<Usuario> BuscarEstilista(string nombre)
        {
            List<Usuario> res = new List<Usuario>();
            nombre = "%" + nombre + "%";
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

        public bool VerificarVinculo(int idServicio, int idEstilista)
        {
            bool existe;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT EXISTS(SELECT 1
                    FROM estilista_servicios
                    WHERE id_estilista = @idEstilista AND id_servicio = @idServicio)";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEstilista", idEstilista);
                    command.Parameters.AddWithValue("@idServicio", idServicio);
                    connection.Open();
                    var resultado = command.ExecuteScalar();
                    existe = Convert.ToBoolean(resultado);
                    connection.Close();
                }
            }
            if (existe)
            {
                return true;
            }
            return false;
        }

        public List<EstilistaServicio> ObtenerEstilista(int idServicio)
        {
            List<EstilistaServicio> estilistas = new List<EstilistaServicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT e.*, p.nombre AS estilista_nombre, p.apellido AS estilista_apellido
                    FROM Estilista_Servicios e
                    INNER JOIN Usuarios p ON e.id_estilista = p.id_usuario
                    WHERE e.id_servicio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = idServicio;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        EstilistaServicio e = new EstilistaServicio()
                        {
                            IdServicio = reader.GetInt32("id_servicio"),
                            IdEstilista = reader.GetInt32("id_estilista"),
                            Estilista = new Usuario()
                            {
                                Nombre = reader.GetString("estilista_nombre"),
                                Apellido = reader.GetString("estilista_apellido")
                            }
                        };
                        estilistas.Add(e);
                    }
                }
                return estilistas;
            }
        }
    }
}