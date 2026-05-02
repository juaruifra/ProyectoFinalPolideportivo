using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API para gestionar las operaciones sobre reservas.
    /// Intermediario entre los repositorios y los controladores.
    /// </summary>
    public class ReservasAPI
    {
        // Repositorio de reservas.
        private readonly ReservasRepository _repo;

        /// <summary>
        /// Constructor: inicializa el repositorio.
        /// </summary>
        public ReservasAPI()
        {
            // Instanciamos el repositorio.
            _repo = new ReservasRepository();
        }

        /// <summary>
        /// Obtiene todas las reservas con filtros opcionales.
        /// </summary>
        /// <param name="socioId">Filtra por socio. Null devuelve todas.</param>
        /// <param name="instalacionId">Filtra por instalacion. Null devuelve todas.</param>
        /// <param name="soloPendientes">Si true, solo las no canceladas.</param>
        /// <returns>Lista de reservas.</returns>
        public List<Reservas> ObtenerTodos(int? socioId = null, int? instalacionId = null, bool soloPendientes = false)
        {
            try
            {
                // Delegamos en el repositorio.
                return _repo.GetAll(socioId, instalacionId, soloPendientes);
            }
            catch (Exception ex)
            {
                // Envolvemos el error.
                throw new Exception("Error al obtener las reservas.", ex);
            }
        }

        /// <summary>
        /// Obtiene una reserva por su ID.
        /// </summary>
        /// <param name="reservaId">Id de la reserva.</param>
        /// <returns>Reserva encontrada o null.</returns>
        public Reservas ObtenerPorId(int reservaId)
        {
            try
            {
                // Delegamos.
                return _repo.GetById(reservaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception($"Error al obtener la reserva con ID {reservaId}.", ex);
            }
        }

        /// <summary>
        /// Comprueba si existe solape de horario en la misma instalacion.
        /// </summary>
        /// <param name="instalacionId">Id de la instalacion.</param>
        /// <param name="inicio">FechaHoraInicio.</param>
        /// <param name="fin">FechaHoraFin.</param>
        /// <param name="excludeReservaId">ReservaId a excluir en edicion.</param>
        /// <returns>True si hay solape.</returns>
        public bool ExisteSolape(int instalacionId, DateTime inicio, DateTime fin, int excludeReservaId = 0)
        {
            try
            {
                // Delegamos.
                return _repo.ExisteSolape(instalacionId, inicio, fin, excludeReservaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al verificar solape de reservas.", ex);
            }
        }

        /// <summary>
        /// Guarda una reserva (alta o modificacion).
        /// </summary>
        /// <param name="reserva">Entidad Reservas.</param>
        public void Guardar(Reservas reserva)
        {
            try
            {
                // Delegamos.
                _repo.Save(reserva);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al guardar la reserva.", ex);
            }
        }

        /// <summary>
        /// Cancela una reserva por su ID.
        /// </summary>
        /// <param name="reservaId">Id de la reserva.</param>
        public void Cancelar(int reservaId)
        {
            try
            {
                // Delegamos.
                _repo.Cancelar(reservaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al cancelar la reserva.", ex);
            }
        }
    }
}
