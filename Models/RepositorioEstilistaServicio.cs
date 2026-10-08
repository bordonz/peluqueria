using MySql.Data.MySqlClient;

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
                    WHERE id_estilista_serivcio = @id";
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
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Estilista_Servicios                 
                    SET id_estilista=@idEstilista, id_servicio=@idServicio
                    WHERE id_estilista_servicio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEstilista", s.IdEstilista);
                    command.Parameters.AddWithValue("@idServicio", s.IdServicio);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public List<EstilistaServicio> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<EstilistaServicio> res = new List<EstilistaServicio>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT e.*, p.nombre AS estilista_nombre, p.apellido AS estilista_apellido, s.*
                    FROM Estilista_Servicios e
                    INNER JOIN Usuarios p ON e.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON e.id_servicio = s.id_servicio
                    ORDER BY e.id_estilista_servicio
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
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.GetString("descripcion"),
                                Duracion = reader.GetString("duracion"),
                                Precio = reader.GetDecimal("precio"),
                            },
                            IdEstilista = reader.GetInt32("id_peluquero"),
                            Estilista = new Usuario
                            {
                                Nombre = reader.GetString("estilista_nombre"),
                                Apellido = reader.GetString("estilista_apellido")
                            },
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

        //TODO: Corregir join
        public EstilistaServicio? ObtenerPorId(int id)
        {
            EstilistaServicio? s = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT e.*, p.nombre AS estilista_nombre, p.apellido AS estilista_apellido, s.*
                    FROM Estilista_Servicios e
                    INNER JOIN Usuarios p ON e.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON e.id_servicio = s.id_servicio
                    WHERE s.id_servicio = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        s = new EstilistaServicio
                        {
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre"),
                                Descripcion = reader.GetString("descripcion"),
                                Duracion = reader.GetString("duracion"),
                                Precio = reader.GetDecimal("precio"),
                            },
                            IdEstilista = reader.GetInt32("id_peluquero"),
                            Estilista = new Usuario
                            {
                                Nombre = reader.GetString("estilista_nombre"),
                                Apellido = reader.GetString("estilista_apellido")
                            },
                        };
                    }
                }
                return s;
            }
        }
    }
}