using MySql.Data.MySqlClient;

namespace peluqueria.Models
{
    public class RepositorioPago: RepositorioBase, IRepositorioPago
    {
        public RepositorioPago(IConfiguration configuration) : base(configuration)
        {
            
        }
        public int Alta(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Pagos
                    (id_pago, fecha_pago, monto, estado, metodo, id_turno)
                    VALUES (@id_pago, @fecha_pago, @monto, @estado, @metodo, @id_turno);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id_pago", p.IdPago);
                    command.Parameters.AddWithValue("@fecha_pago", p.FechaPago);
                    command.Parameters.AddWithValue("@monto", p.Monto);
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@metodo", p.Metodo);
                    command.Parameters.AddWithValue("@id_turno", p.IdTurno);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    p.IdPago = res;
                }
            }
            return res;
        }

        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string estado = "Pagado";
                string sql = @"UPDATE Pagos
                    SET estado=@estado 
                    WHERE id_pago = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@estado", estado);
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int Modificacion(Pago p)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Pagos
                    SET estado=@estado
                    WHERE id_pago = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@estado", p.Estado);
                    command.Parameters.AddWithValue("@id", p.IdPago);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public List<Pago> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<Pago> res = new List<Pago>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT p.*, t.id_cliente, t.id_servicio
                    FROM Pagos p
                    INNER JOIN Turnos t ON p.id_turno = t.id_turno
                    ORDER BY p.id_pago
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro -1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Pago p = new Pago
                        {
                            IdPago = reader.GetInt32("id_pago"),
                            FechaPago = reader.GetDateTime("fecha_pago"),
                            Monto = reader.GetDecimal("monto"),
                            Estado = reader.GetString("estado"),
                            Metodo = reader.GetString("metodo"),
                            IdTurno = reader.GetInt32("id_turno"),
                            Turno = new Turno
                            {
                                IdCliente = reader.GetInt32("id_cliente"),
                                IdServicio = reader.GetInt32("id_servicio")
                            }
                        };
                        res.Add(p);
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
                string sql = @"SELECT COUNT(id_pago)
                    FROM Pagos";
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

        public Pago? ObtenerPorId(int id)
        {
            Pago? p = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT p.*, t.id_cliente, t.id_servicio
                    FROM Pagos p
                    INNER JOIN Turnos t ON p.id_turno = t.id_turno
                    WHERE p.id_pago = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        p = new Pago
                        {
                            IdPago = reader.GetInt32("id_pago"),
                            FechaPago = reader.GetDateTime("fecha_pago"),
                            Monto = reader.GetDecimal("monto"),
                            Estado = reader.GetString("estado"),
                            IdTurno = reader.GetInt32("id_turno"),
                            Turno = new Turno
                            {
                                IdCliente = reader.GetInt32("id_cliente"),
                                IdServicio = reader.GetInt32("id_servicio")
                            }
                        };
                    }
                }
            }
            return p;
        }
    }
}