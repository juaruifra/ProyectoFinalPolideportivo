using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Reservas en ClubPolideportivoDB.
    /// Tambien centraliza las validaciones de uso para socios e instalaciones.
    /// </summary>
    public class ReservasRepository : RepositoryBase
    {
        /// <summary>
        /// Devuelve todas las reservas con filtros opcionales.
        /// </summary>
        /// <param name="socioId">Filtra por socio. Null devuelve todas.</param>
        /// <param name="instalacionId">Filtra por instalacion. Null devuelve todas.</param>
        /// <param name="soloPendientes">Si true, devuelve solo las no canceladas.</param>
        /// <returns>Lista de reservas.</returns>
        public List<Reservas> GetAll(int? socioId = null, int? instalacionId = null, bool soloPendientes = false)
        {
            // Comenzamos con todos los registros incluyendo socio e instalacion.
            var q = Context.Reservas
                .Include("Socios")
                .Include("Instalaciones")
                .AsQueryable();

            // Filtramos por socio si se indica.
            if (socioId.HasValue)
                q = q.Where(r => r.SocioId == socioId.Value);

            // Filtramos por instalacion si se indica.
            if (instalacionId.HasValue)
                q = q.Where(r => r.InstalacionId == instalacionId.Value);

            // Filtramos solo activas (no canceladas) si se pide.
            if (soloPendientes)
                q = q.Where(r => r.Estado != "Cancelada");

            // Ordenamos por fecha de inicio descendente para ver las mas recientes primero.
            return q.OrderByDescending(r => r.FechaHoraInicio).ToList();
        }

        /// <summary>
        /// Devuelve una reserva por su ID incluyendo socio e instalacion.
        /// </summary>
        /// <param name="reservaId">Id de la reserva.</param>
        /// <returns>Reserva encontrada o null.</returns>
        public Reservas GetById(int reservaId)
        {
            // Buscamos por clave primaria con includes.
            return Context.Reservas
                .Include("Socios")
                .Include("Instalaciones")
                .FirstOrDefault(r => r.ReservaId == reservaId);
        }

        /// <summary>
        /// Comprueba si existe solape de horario en la misma instalacion.
        /// Excluye la propia reserva al editar.
        /// </summary>
        /// <param name="instalacionId">Id de la instalacion.</param>
        /// <param name="inicio">FechaHoraInicio de la nueva reserva.</param>
        /// <param name="fin">FechaHoraFin de la nueva reserva.</param>
        /// <param name="excludeReservaId">ReservaId a excluir en edicion (0 en alta).</param>
        /// <returns>True si hay solape.</returns>
        public bool ExisteSolape(int instalacionId, DateTime inicio, DateTime fin, int excludeReservaId = 0)
        {
            // Buscamos reservas activas de la misma instalacion que se solapen con el rango.
            return Context.Reservas.Any(r =>
                r.InstalacionId == instalacionId &&
                r.ReservaId != excludeReservaId &&
                r.Estado != "Cancelada" &&
                r.FechaHoraInicio < fin &&
                r.FechaHoraFin > inicio);
        }

        /// <summary>
        /// Guarda una reserva (alta si ReservaId menor que 1, modificacion si existe).
        /// </summary>
        /// <param name="reserva">Entidad Reservas.</param>
        public void Save(Reservas reserva)
        {
            try
            {
                if (reserva.ReservaId < 1)
                {
                    // Alta: añadimos nueva reserva.
                    Context.Reservas.Add(new Reservas
                    {
                        SocioId = reserva.SocioId,
                        InstalacionId = reserva.InstalacionId,
                        FechaHoraInicio = reserva.FechaHoraInicio,
                        FechaHoraFin = reserva.FechaHoraFin,
                        PrecioTotal = reserva.PrecioTotal,
                        Estado = reserva.Estado,
                        Observaciones = reserva.Observaciones
                    });
                }
                else
                {
                    // Modificacion: buscamos y actualizamos.
                    var r = Context.Reservas.FirstOrDefault(x => x.ReservaId == reserva.ReservaId);
                    if (r != null)
                    {
                        r.SocioId = reserva.SocioId;
                        r.InstalacionId = reserva.InstalacionId;
                        r.FechaHoraInicio = reserva.FechaHoraInicio;
                        r.FechaHoraFin = reserva.FechaHoraFin;
                        r.PrecioTotal = reserva.PrecioTotal;
                        r.Estado = reserva.Estado;
                        r.Observaciones = reserva.Observaciones;
                    }
                }

                // Persistimos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos el error.
                throw new Exception("Error al guardar la reserva en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Cancela una reserva cambiando su estado a "Cancelada".
        /// </summary>
        /// <param name="reservaId">Id de la reserva.</param>
        public void Cancelar(int reservaId)
        {
            try
            {
                // Buscamos la reserva.
                var r = Context.Reservas.FirstOrDefault(x => x.ReservaId == reservaId);

                // Si no existe, no hacemos nada.
                if (r == null) return;

                // Marcamos como cancelada.
                r.Estado = "Cancelada";

                // Guardamos.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos el error.
                throw new Exception("Error al cancelar la reserva.", ex);
            }
        }

        /// <summary>
        /// Indica si existe alguna reserva asociada a un socio.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si hay reservas.</returns>
        public bool ExisteReservaParaSocio(int socioId)
        {
            // Buscamos cualquier reserva asociada.
            return Context.Reservas.Any(r => r.SocioId == socioId);
        }

        /// <summary>
        /// Indica si existe alguna reserva asociada a una instalacion.
        /// </summary>
        /// <param name="instalacionId">Id de la instalacion.</param>
        /// <returns>True si hay reservas.</returns>
        public bool ExisteReservaParaInstalacion(int instalacionId)
        {
            // Buscamos cualquier reserva asociada.
            return Context.Reservas.Any(r => r.InstalacionId == instalacionId);
        }
    }
}
