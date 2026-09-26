using MySql.Data.MySqlClient;

namespace peluqueria.Models
{
    public class RepositorioServicio: RepositorioBase, IRepositorioServicio
    {
        public RepositorioServicio(IConfiguration configuration) : base(configuration)
        {
            
        }
        public int Alta(Servicio s)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Servicios
                    (nombre, descripcion, duracion, precio, activo)
                    VALUES (@nombre, @descripcion, @duracion, @precio, @activo);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", s.Nombre);
                    command.Parameters.AddWithValue("@descripcion", s.Descripcion);
                    command.Parameters.AddWithValue("@duracion", s.Duracion);
                    command.Parameters.AddWithValue("@precio", s.Precio);
                    command.Parameters.AddWithValue("@activo", s.Activo);
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
                bool estado = false;
                string sql = @"UPDATE Servicios
                    SET activo=@activo
                    WHERE id_serivcio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@activo", estado);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Servicio s)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Servicios                 
                    SET nombre=@nombre, descripcion=@descripcion, duracion=@duracion, precio=@precio, activo=@activo
                    WHERE id_servicio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@nombre", s.Nombre);
                    command.Parameters.AddWithValue("@descripcion", s.Descripcion);
                    command.Parameters.AddWithValue("@duracion", s.Duracion);
                    command.Parameters.AddWithValue("@precio", s.Precio);
                    command.Parameters.AddWithValue("@activo", s.Activo);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public List<Servicio> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<Servicio> res = new List<Servicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT s.*
                    FROM Servicios s
                    ORDER BY s.id_servicio
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro -1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Servicio s = new Servicio
                        {
                            IdServicio = reader.GetInt32("id_servicio"),
                            Nombre = reader.GetString("nombre"),
                            Descripcion = reader.GetString("descripcion"),
                            Duracion = reader.GetString("duracion"),
                            Precio = reader.GetDecimal("precio"),
                            Activo = reader.GetBoolean("activo"),
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
                string sql = @"SELECT COUNT(id_servicio)
                    FROM Servicios";
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

        public Servicio? ObtenerPorId(int id)
        {
            Servicio? s = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT s.*
                    FROM Servicios s
                    WHERE s.id_servicio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        s = new Servicio
                        {
                            IdServicio = reader.GetInt32("id_servicio"),
                            Nombre = reader.GetString("nombre"),
                            Descripcion = reader.GetString("descripcion"),
                            Duracion = reader.GetString("duracion"),
                            Precio = reader.GetDecimal("precio"),
                            Activo = reader.GetBoolean("activo"),
                        };
                    }
                }
                return s;
            }
        }
    }
}