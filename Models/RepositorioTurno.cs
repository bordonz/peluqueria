using System.Security.Claims;
using MySql.Data.MySqlClient;

namespace peluqueria.Models
{
    public class RepositorioTurno: RepositorioBase, IRepositorioTurno
    {
        public RepositorioTurno(IConfiguration configuration) : base(configuration)
        {
            
        }
        //TODO: Para el EstilistaServicio implementar el buscador por nombre.
        public int Alta(Turno t)
        {
            int res = -1;
            if(ValidarDisponibilidad(t))
            {
                return -2;
            }

            t.Estado = "Ocupado";
            t.FechaEmitido = DateTime.Now;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Turnos
                    (fecha_emitido, fecha_turno, estado, id_cliente, id_estilista_servicios)
                    VALUES (@fecha_emitido, @fecha_turno, @estado, @id_cliente, @id_estilista_servicio);
                    SELECT LAST_INSERT_ID();";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha_emitido", t.FechaEmitido);
                    command.Parameters.AddWithValue("@fecha_turno", t.FechaTurno);
                    command.Parameters.AddWithValue("@estado", t.Estado);
                    command.Parameters.AddWithValue("@id_cliente", t.IdCliente);
                    command.Parameters.AddWithValue("@id_estilista_servicio",t.IdEstilistaServicios);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    t.IdTurno = res;
                }
            }
            return res; 
        }

        public bool ValidarDisponibilidad(Turno t)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(1) 
                    FROM Turnos t
                    WHERE t.fecha_turno = @FechaTurno 
                            AND t.id_estilista_servicios = @IdEstilistaServicio
                            AND t.estado = 'Ocupado'";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@FechaTurno", t.FechaTurno);
                    command.Parameters.AddWithValue("@IdEstilistaServicio", t.IdEstilistaServicios);
                    connection.Open();
                    var result = command.ExecuteScalar();

                    return Convert.ToInt64(result) > 0;
                }
            }
        }

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

        public int Modificacion(Turno t)
        {
            int res = -1;
            int IdEstilistaServ = DevolverEstilistaServicio(t.EstilistaServicios.Servicio.IdServicio);
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"UPDATE Turnos
                    SET fecha_turno=@fechaTurno, estado=@estado, id_cliente=@idCliente, id_estilista_servicios=@id_estilista_Servicio
                    WHERE id_turno = @id";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fechaTurno", t.FechaTurno);
                    command.Parameters.AddWithValue("@estado", t.Estado);
                    command.Parameters.AddWithValue("@idCliente", t.IdCliente);
                    command.Parameters.AddWithValue("@id_estilista_Servicio", IdEstilistaServ);
                    command.Parameters.AddWithValue("@id", t.IdTurno);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int DevolverEstilistaServicio(int IdServicio)
		{
			int IdEstilistaServicios = 0;

			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT id_estilista_servicios
							FROM estilista_servicios
							WHERE id_servicio = @id LIMIT 1";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
                    command.Parameters.AddWithValue("@id", IdServicio);
					connection.Open();
					var resultado = command.ExecuteScalar();

					if (resultado != null && int.TryParse(resultado.ToString(), out int id))
                    {
                        IdEstilistaServicios = id;
                    }
				}
			}
			return IdEstilistaServicios;
		}
        public List<Turno> ObtenerLista(bool usuario, int logueado, int paginaNro = 1, int tamPagina = 10)
        {
            DateTime hoy = DateTime.Today;
            List<Turno> res = new List<Turno>();
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio,
                    p.nombre AS nombre_estilista, p.apellido AS apellido_estilista
                    FROM Turnos t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Estilista_Servicios es ON t.id_estilista_servicios = es.id_estilista_servicios
                    INNER JOIN Usuarios p ON es.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON es.id_servicio = s.id_servicio
                    WHERE estado = 'Ocupado'";

                if(!usuario)
                {
                    sql += " AND t.id_cliente = @IdCliente AND fecha_turno >= @FechaHoy";
                }

                sql += " ORDER BY t.id_turno LIMIT @tamPagina OFFSET @offset";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if(!usuario)
                    {
                        command.Parameters.AddWithValue("@IdCliente", logueado);
                        command.Parameters.AddWithValue("@FechaHoy", hoy);
                    }

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
                            EstilistaServicios = new EstilistaServicio
                            {
                                Servicio = new Servicio
                                {
                                    Nombre = reader.GetString("nombre_servicio"),
                                },

                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_estilista"),
                                    Apellido = reader.GetString("apellido_estilista")
                                }
                            }
                        };
                        res.Add(t);
                    }
                }
            }
            return res;
        }

        public int ObtenerCantidad(bool usuario, int logueado)
        {
            int res = 0;
            DateTime hoy = DateTime.Today;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT COUNT(id_turno)
                    FROM Turnos
                    WHERE estado = 'Ocupado'";

                    if (!usuario)
                    {
                        sql += " AND id_cliente = @IdCliente AND fecha_turno >= @FechaHoy";
                    }

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    if(!usuario)
                    {
                        command.Parameters.AddWithValue("@IdCliente", logueado);
                        command.Parameters.AddWithValue("@FechaHoy", hoy);
                    }
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

        public Turno? ObtenerPorId(int id)
        {
            Turno? t = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio, s.id_servicio AS idServicio,
                    p.nombre AS nombre_estilista, p.apellido AS apellido_estilista
                    FROM Turnos t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Estilista_Servicios es ON t.id_estilista_servicios = es.id_estilista_servicios
                    INNER JOIN Usuarios p ON es.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON es.id_servicio = s.id_servicio
                    WHERE t.id_turno = @id";
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
                            EstilistaServicios = new EstilistaServicio
                            {
                                Servicio = new Servicio
                                {
                                    IdServicio = reader.GetInt32("idServicio"),
                                    Nombre = reader.GetString("nombre_servicio"),
                                },

                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_estilista"),
                                    Apellido = reader.GetString("apellido_estilista")
                                }
                            }
                        };
                    }
                }
                return t;
            }
        }

        public List<string> ObtenerHorasOcupadas(DateTime fecha, int IdEstilista)
        {
            List<string> horasOcupadas = new List<string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.fecha_turno
                            FROM Turnos t
                            INNER JOIN estilista_servicios es ON t.id_estilista_servicios = es.id_estilista_servicios
                            WHERE DATE(t.fecha_turno) = DATE(@fecha)
                                AND es.id_estilista = @IdEstilista
                                AND t.estado != 'Cancelada'";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@IdEstilista", IdEstilista);

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

        public List<Turno> ObtenerTurnosDiarios(int user, int paginaNro = 1, int tamPagina = 10)
        {
            List<Turno> res = new List<Turno>();
            DateTime fecha = DateTime.Today;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT t.*, 
                    c.nombre AS nombre_cliente, 
                    c.apellido AS apellido_cliente,
                    s.nombre AS nombre_servicio,
                    p.nombre AS nombre_estilista, p.apellido AS apellido_estilista
                    FROM Turnos t
                    INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                    INNER JOIN Estilista_Servicios es ON t.id_estilista_servicios = es.id_estilista_servicios
                    INNER JOIN Usuarios p ON es.id_estilista = p.id_usuario
                    INNER JOIN Servicios s ON es.id_servicio = s.id_servicio
                    WHERE DATE(t.fecha_turno) = DATE(@fecha) AND p.id_usuario = @user
                    LIMIT @tamPagina OFFSET @offset";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@fecha", fecha);
                    command.Parameters.AddWithValue("@user", user);
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
                            EstilistaServicios = new EstilistaServicio
                            {
                                Servicio = new Servicio
                                {
                                    Nombre = reader.GetString("nombre_servicio"),
                                },

                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_estilista"),
                                    Apellido = reader.GetString("apellido_estilista")
                                }
                            }

                        };
                        res.Add(t);
                    }
                }
            }
            return res;
        }

        public List<Turno> ObtenerTunosSemanales(int sumaSemana, int empleadoEstilista)
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
                        p.nombre AS nombre_estilista, p.apellido AS apellido_estilista
                        FROM Turnos t
                        INNER JOIN usuarios c ON t.id_cliente = c.id_usuario
                        INNER JOIN Estilista_Servicios es ON t.id_estilista_servicios = es.id_estilista_servicios
                        INNER JOIN Usuarios p ON es.id_estilista = p.id_usuario
                        INNER JOIN Servicios s ON es.id_servicio = s.id_servicio
                        WHERE t.fecha_turno >= @lunes AND t.fecha_turno <= @domingo AND p.id_usuario= @empleado";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@lunes", lunes);
                    command.Parameters.AddWithValue("@domingo", domingo);
                    command.Parameters.AddWithValue("@empleado", empleadoEstilista);
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
                            EstilistaServicios = new EstilistaServicio
                            {
                                Servicio = new Servicio
                                {
                                    Nombre = reader.GetString("nombre_servicio"),
                                },

                                Estilista = new Usuario
                                {
                                    Nombre = reader.GetString("nombre_estilista"),
                                    Apellido = reader.GetString("apellido_estilista")
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