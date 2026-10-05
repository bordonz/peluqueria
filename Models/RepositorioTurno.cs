using MySql.Data.MySqlClient;

namespace peluqueria.Models
{
    public class RepositorioTurno: RepositorioBase, IRepositorioTurno
    {
        public RepositorioTurno(IConfiguration configuration) : base(configuration)
        {
            
        }
        //TODO: Para el servicio implementar el buscador por nombre.
        public int Alta(Turno t)
        {
            t.IdCliente = 1;
            t.Estado = "Ocupado";
            t.FechaEmitido = DateTime.Now;
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                //Definir fecha emitido como today
                string sql = @"INSERT INTO Turnos
                    (fecha_emitido, fecha_turno, estado, id_cliente, id_servicio)
                    VALUES (@fecha_emitido, @fecha_turno, @estado, @id_cliente, @id_servicio);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha_emitido", t.FechaEmitido);
                    command.Parameters.AddWithValue("@fecha_turno", t.FechaTurno);
                    command.Parameters.AddWithValue("@estado", t.Estado);
                    command.Parameters.AddWithValue("@id_cliente", t.IdCliente);
                    command.Parameters.AddWithValue("@id_servicio",t.IdServicio);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.IdTurno = res;
                }
            }
            return res;
        }

        //TODO: Agregar busqueda de servicio por nombre con select2
        public int Baja(int id)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string estado = "Finalizado";
                string sql = @"UPDATE Turnos
                    SET estado=@estado 
                    WHERE id_turno = @id";
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

        //TODO: Quien modifica y que tanto?
        public int Modificacion(Turno t)
        {
            int res = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Turnos
                    SET fecha_turno=@fechaTurno, estado=@estado, id_cliente=@idCliente, id_servicio=@idServicio
                    WHERE id_turno = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha_turno", t.FechaTurno);
                    command.Parameters.AddWithValue("@estado", t.Estado);
                    command.Parameters.AddWithValue("@id_cliente", t.IdCliente);
                    command.Parameters.AddWithValue("@id_servicio",t.IdServicio);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        //TODO: Revisar consulta y armado de t
        public List<Turno> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
        {
            List<Turno> res = new List<Turno>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio,
                    p.nombre AS nombre_peluquero, p.apellido AS apellido_peluquero
                    FROM Turnos t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Servicios s ON t.id_servicio = s.id_servicio
                    INNER JOIN usuarios p ON s.id_peluquero = p.id_usuario
                    ORDER BY t.id_turno
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro -1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Turno t = new Turno
                        {
                            IdTurno = reader.GetInt32("id_turno"),
                            FechaEmitido = reader.GetDateTime("fecha_emitido"),
                            FechaTurno = reader.GetDateTime("fecha_turno"),
                            Estado = reader.GetString("estado"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            Cliente = new Usuario
                            {
                                Nombre = reader.GetString("nombre_cliente"),
                                Apellido = reader.GetString("apellido_cliente")
                                
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_peluquero"),
                                    Apellido = reader.GetString("apellido_peluquero")
                                }
                            }

                        };
                        res.Add(t);
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
                string sql = @"SELECT COUNT(id_turno)
                    FROM Turnos";
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

        //TODO: Revisar consulta y armado de t
        public Turno? ObtenerPorId(int id)
        {
            Turno? t = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio,
                    p.nombre AS nombre_peluquero, p.apellido AS apellido_peluquero
                    FROM Turno t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Servicios s ON t.id_servicio = s.id_servicio
                    INNER JOIN usuarios p ON s.id_peluquero = p.id_usuario
                    WHERE p.id_turno = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.Add("@id", MySqlDbType.Int32).Value = id;
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        t = new Turno
                        {
                            IdTurno = reader.GetInt32("id_turno"),
                            FechaEmitido = reader.GetDateTime("fecha_emitido"),
                            FechaTurno = reader.GetDateTime("fecha_turno"),
                            Estado = reader.GetString("estado"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            Cliente = new Usuario
                            {
                                Nombre = reader.GetString("nombre_cliente"),
                                Apellido = reader.GetString("apellido_cliente")
                                
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_peluquero"),
                                    Apellido = reader.GetString("apellido_peluquero")
                                }
                            }
                        };
                    }
                }
                return t;
            }
        }

        public List<string> ObtenerHorasOcupadas(DateTime fecha, int idServicio)
        {
            List<string> horasOcupadas = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.fecha_turno
                            FROM Turnos t
                            WHERE DATE(t.fecha_turno) = DATE(@fecha)
                                AND t.id_servicio = @idServicio
                                AND t.estado != 'Cancelada'";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@idServicio", idServicio);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime fechaTurno = reader.GetDateTime("fecha_turno");
                            
                            string horaFormateada = fechaTurno.ToString("HH:mm");
                            horasOcupadas.Add(horaFormateada);
                        }
                    }
                }
            }
            return horasOcupadas;
        }

        public List<Turno> ObtenerTurnosDiarios(int paginaNro = 1, int tamPagina = 10)
        {
            List<Turno> res = new List<Turno>();
            DateTime fecha = DateTime.Today;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio,
                    p.nombre AS nombre_peluquero, p.apellido AS apellido_peluquero
                    FROM Turnos t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Servicios s ON t.id_servicio = s.id_servicio
                    INNER JOIN usuarios p ON s.id_peluquero = p.id_usuario
                    WHERE DATE(t.fecha_turno) = DATE(@fecha)
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha);
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (paginaNro -1) * tamPagina);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Turno t = new Turno
                        {
                            IdTurno = reader.GetInt32("id_turno"),
                            FechaEmitido = reader.GetDateTime("fecha_emitido"),
                            FechaTurno = reader.GetDateTime("fecha_turno"),
                            Estado = reader.GetString("estado"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            Cliente = new Usuario
                            {
                                Nombre = reader.GetString("nombre_cliente"),
                                Apellido = reader.GetString("apellido_cliente")
                                
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_peluquero"),
                                    Apellido = reader.GetString("apellido_peluquero")
                                }
                            }

                        };
                        res.Add(t);
                    }
                }
            }
            return res;
        }

        public List<Turno> ObtenerTunosSemanales(int sumaSemana)
        {
            List<Turno> listaTurnos = new List<Turno>();

            int diasASumar = sumaSemana * 7;
            DateTime hoy = DateTime.Today;
            int diasHastaLunes = ((int)hoy.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            DateTime lunes = hoy.AddDays(-diasHastaLunes).AddDays(diasASumar);
            DateTime domingo = lunes.AddDays(6).Date.AddDays(1).AddTicks(-1);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                        c.nombre AS nombre_cliente, 
                        c.apellido AS apellido_cliente,
                        s.nombre AS nombre_servicio,
                        p.nombre AS nombre_peluquero, p.apellido AS apellido_peluquero
                        FROM Turnos t
                        INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                        INNER JOIN Servicios s ON t.id_servicio = s.id_servicio
                        INNER JOIN usuarios p ON s.id_peluquero = p.id_usuario
                        WHERE t.fecha_turno >= @lunes AND t.fecha_turno <= @domingo";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@lunes", lunes);
                    command.Parameters.AddWithValue("@domingo", domingo);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Turno t = new Turno
                        {
                            IdTurno = reader.GetInt32("id_turno"),
                            FechaEmitido = reader.GetDateTime("fecha_emitido"),
                            FechaTurno = reader.GetDateTime("fecha_turno"),
                            Estado = reader.GetString("estado"),
                            IdCliente = reader.GetInt32("id_cliente"),
                            Cliente = new Usuario
                            {
                                Nombre = reader.GetString("nombre_cliente"),
                                Apellido = reader.GetString("apellido_cliente")
                                
                            },
                            IdServicio = reader.GetInt32("id_servicio"),
                            Servicio = new Servicio
                            {
                                Nombre = reader.GetString("nombre_servicio"),
                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_peluquero"),
                                    Apellido = reader.GetString("apellido_peluquero")
                                }
                            }

                        };
                        listaTurnos.Add(t);
                    }
                }
            }
            return listaTurnos;
        }
    }
}