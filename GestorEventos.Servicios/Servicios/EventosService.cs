using GestorEventos.Servicios.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Dapper;

namespace GestorEventos.Servicios.Servicios
{
    public interface IEventoService
    {
        bool DeleteEvento(int idEvento);
        IEnumerable<Evento> GetAllEventos();
        IEnumerable<EventoViewModel> GetAllEventosViewModel();
        IEnumerable<EventoViewModel> GetMisEventos(int IdUsuario);
        Evento GetEventoPorId(int IdEvento);
        int PostNuevoEvento(Evento evento);
        bool PutNuevoEvento(int idEvento, Evento evento);
        public bool CambiarEstadoEvento(int idEvento, int idEstado);
    }

    public class EventoService : IEventoService
    {
        private string _connectionString;



        public EventoService()
        {

            //Connection string 
            _connectionString = "Server=localhost\\SQLEXPRESS;Database=prueba;Trusted_Connection=True;";

        }


        public IEnumerable<Evento> GetAllEventos()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                List<Evento> eventos = db.Query<Evento>("SELECT * FROM Eventos WHERE Borrado = 0").ToList();

                return eventos;

            }
        }

        public IEnumerable<EventoViewModel> GetMisEventos(int idUsuario)
        {

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                List<EventoViewModel> eventos = db.Query<EventoViewModel>("select eventos.*, EstadosEventos.Descripcion EstadoEvento from eventos left join EstadosEventos on EstadosEventos.IdEstadoEvento = eventos.idEstadoEvento WHERE Eventos.IdUsuario =" + idUsuario.ToString()).ToList();

                return eventos;

            } 
        }


        public IEnumerable<EventoViewModel> GetAllEventosViewModel()
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                List<EventoViewModel> eventos = db.Query<EventoViewModel>("select eventos.*, EstadosEventos.Descripcion EstadoEvento from eventos left join EstadosEventos on EstadosEventos.IdEstadoEvento = eventos.idEstadoEvento").ToList();

                return eventos;

            }
        }


        public Evento GetEventoPorId(int IdEvento)
        {

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                Evento eventos = db.Query<Evento>("SELECT * FROM Eventos WHERE Borrado = 0").First();

                return eventos;

            }
        }

        public int PostNuevoEvento(Evento evento)
        {

            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string query = "insert into Eventos (NombreEvento, FechaEvento, CantidadPersonas, IdPersonaAgasajada, IdTipoEvento, IdUsuario, IdEstadoEvento, Visible, Borrado) values ( @NombreEvento, @FechaEvento, @CantidadPersonas, @IdPersonaAgasajada, @IdTipoEvento, @IdUsuario, @IdEstadoEvento, @Visible, @Borrado);" +
                    "select  CAST(SCOPE_IDENTITY() AS INT) ";
                    evento.IdEvento = db.QuerySingle<int>(query, evento);
                    //db.QuerySingle(query, evento);


                    return evento.IdEvento;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

        /*public int PostNuevoEvento(Evento evento)
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string query = @"
                insert into Eventos (NombreEvento, FechaEvento, CantidadPersonas, IdPersonaAgasajada, IdTipoEvento, Visible, Borrado, IdUsuario, IdEstadoEvento) 
                values (@NombreEvento, @FechaEvento, @CantidadPersonas, @IdPersonaAgasajada, @IdTipoEvento, @Visible, @Borrado, @IdUsuario, @IdEstadoEvento);
                select CAST(SCOPE_IDENTITY() AS INT);";

                    evento.IdEvento = db.QuerySingle<int>(query, evento);

                    return evento.IdEvento;
                }
            }
            catch (Exception ex)
            {
                // Registro del error para diagnóstico
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                // Puedes utilizar un mecanismo de registro más robusto, como log4net o NLog, en lugar de Console.WriteLine

                return 0;
            }
        }*/


        public bool CambiarEstadoEvento(int idEvento, int idEstado) 
        {
            try
            {
                using (IDbConnection db = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Eventos SET IdEstadoEvento = " + idEstado.ToString() + " WHERE IdEvento = " + idEvento.ToString();
                    db.Execute(query);
                    //evento.IdEvento = db.QuerySingle<int>(query, evento);
                    //db.QuerySingle(query, evento);


                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool PutNuevoEvento(int idEvento, Evento evento)
        {

            try
            {
             /*   var eventoDeLista = this.Eventos.Where(x => x.IdEvento == idEvento).First(); //LINQ

                eventoDeLista.FechaEvento = evento.FechaEvento;
                eventoDeLista.NombreEvento = evento.NombreEvento;
                eventoDeLista.CantidadPersonas = evento.CantidadPersonas;
                eventoDeLista.IdUsuario= evento.IdUsuario;
                eventoDeLista.IdPersonaAgasajada = evento.IdPersonaAgasajada;
             */

                /*Update de la base*/

                /*Variable 
				 
					Nombre 
					Valor 
					Espacio en memoria 
					puntero de referencia a ese espacio en memoria 


				 */

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
 

        public bool DeleteEvento(int idEvento)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                string query = "UPDATE Eventos SET Borrado = 1 where IdEvento = " + idEvento.ToString();
                db.Execute(query);

                return true;
            }
        }
        /*
		public void PostNuevoEventoCompleto(EventoModel eventoModel)
		{
			PersonaService personaService = new PersonaService();
			int idPersonaAgasajada = personaService.AgregarNuevaPersona(eventoModel.PersonaAgasajada);
			int idPersonaContacto = personaService.AgregarNuevaPersona(eventoModel.PersonaContacto);


			eventoModel.evento.IdPersonaAgasajada = idPersonaAgasajada;
			eventoModel.evento.IdPersonaContacto = idPersonaContacto;
			eventoModel.evento.Visible = true;

			this.PostNuevoEvento(eventoModel.evento);

			foreach(Servicio servicio in eventoModel.ListaDeServiciosContratados)
			{
				ServicioService servicioService = new ServicioService();
				servicioService.AgregarNuevoServicio(servicio);
			}



		}*/
    }
}
