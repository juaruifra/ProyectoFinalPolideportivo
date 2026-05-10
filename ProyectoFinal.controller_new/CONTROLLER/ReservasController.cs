using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de las reservas.
    /// Incluye validaciones y operaciones sobre ClubPolideportivoDB.
    /// </summary>
    public class ReservasController
    {
        // API de reservas.
        private readonly ReservasAPI _api;

        // API de cuotas: para verificar si el socio tiene cuotas vencidas.
        private readonly CuotasAPI _cuotasApi;

        // API de instalaciones: para obtener el precio por hora y verificar disponibilidad.
        private readonly InstalacionesAPI _instalacionesApi;

        // API de socios: para verificar que el socio esta activo.
        private readonly SociosAPI _sociosApi;

        /// <summary>
        /// Constructor: inicializa las APIs necesarias.
        /// </summary>
        public ReservasController()
        {
            // Instanciamos las APIs.
            _api = new ReservasAPI();
            _cuotasApi = new CuotasAPI();
            _instalacionesApi = new InstalacionesAPI();
            _sociosApi = new SociosAPI(); // Instanciamos la API de socios.
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
            // Delegamos en la API.
            return _api.ObtenerTodos(socioId, instalacionId, soloPendientes);
        }

        /// <summary>
        /// Obtiene las reservas de un dia concreto con filtro opcional de instalacion.
        /// </summary>
        /// <param name="fecha">Fecha del dia a consultar.</param>
        /// <param name="instalacionId">Id de instalacion. Null devuelve todas.</param>
        /// <returns>Lista de reservas del dia.</returns>
        public List<Reservas> ObtenerPorFecha(DateTime fecha, int? instalacionId = null)
        {
            // Delegamos en la API.
            return _api.ObtenerPorFecha(fecha, instalacionId);
        }

        /// <summary>
        /// Calcula el precio total de una reserva en base al precio/hora de la instalacion.
        /// </summary>
        /// <param name="instalacionId">Id de la instalacion.</param>
        /// <param name="inicio">FechaHoraInicio.</param>
        /// <param name="fin">FechaHoraFin.</param>
        /// <returns>Precio total calculado o 0 si no se puede calcular.</returns>
        public decimal CalcularPrecio(int instalacionId, DateTime inicio, DateTime fin)
        {
            try
            {
                // Obtenemos la instalacion para leer su precio/hora.
                var instalacion = _instalacionesApi.ObtenerPorId(instalacionId);

                // Si no existe o las fechas no son validas, devolvemos 0.
                if (instalacion == null || fin <= inicio)
                    return 0;

                // Calculamos horas y multiplicamos por precio/hora.
                var horas = (decimal)(fin - inicio).TotalHours;
                return Math.Round(horas * instalacion.PrecioHora, 2);
            }
            catch
            {
                // Si hay error al calcular, devolvemos 0 sin explotar.
                return 0;
            }
        }

        /// <summary>
        /// Guarda una reserva (alta o modificacion) con validaciones previas.
        /// </summary>
        /// <param name="reserva">Entidad Reservas.</param>
        /// <param name="tituloError">Titulo del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si guarda correctamente.</returns>
        public bool Guardar(Reservas reserva, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos la reserva antes de guardar.
                if (!ValidarReserva(reserva, ref tituloError, ref mensajeError))
                    return false;

                // Guardamos.
                _api.Guardar(reserva);
                return true;
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar la reserva: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Borra una reserva fisicamente con validacion previa.
        /// </summary>
        /// <param name="reserva">Reserva a borrar.</param>
        /// <param name="tituloError">Titulo del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si borra correctamente.</returns>
        public bool Borrar(Reservas reserva, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos que la reserva sea valida.
                if (reserva == null || reserva.ReservaId < 1)
                {
                    tituloError = "Reserva no valida";
                    mensajeError = "Debe seleccionar una reserva valida.";
                    return false;
                }

                // Borramos fisicamente.
                _api.Borrar(reserva.ReservaId);
                return true;
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al borrar";
                mensajeError = $"No se pudo borrar la reserva: {ex.Message}";
                return false;
            }
        }

        /// <summary>
     /// Valida los datos de una reserva antes de guardar.
     /// Comprueba: campos obligatorios, socio activo, cuotas vencidas,
     /// instalacion disponible, fechas coherentes y solapes de horario.
     /// </summary>
     /// <param name="reserva">Entidad Reservas.</param>
     /// <param name="tituloError">Titulo del error (ref).</param>
     /// <param name="mensajeError">Mensaje del error (ref).</param>
     /// <returns>True si es valida.</returns>
     private bool ValidarReserva(Reservas reserva, ref string tituloError, ref string mensajeError)
        {
            bool ok = true; // Empezamos asumiendo que los datos son correctos.

            // Null guard: comprobamos que el objeto no sea nulo.
            if (reserva == null)
            {
                tituloError = "Datos incompletos"; // Titulo.
                mensajeError = "La reserva no puede ser nula."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // El socio debe estar seleccionado.
            if (ok && reserva.SocioId < 1)
            {
                tituloError = "Socio requerido"; // Titulo.
                mensajeError = "Debe seleccionar un socio para la reserva."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // El socio debe estar activo en el club para poder reservar.
            if (ok)
            {
                var socio = _sociosApi.ObtenerPorId(reserva.SocioId); // Obtenemos el socio de la BD.
                if (socio == null || !socio.Activo) // Si no existe o esta inactivo, rechazamos.
                {
                    tituloError = "Socio inactivo"; // Titulo.
                    mensajeError = "El socio seleccionado no esta activo en el club y no puede realizar reservas."; // Mensaje.
                    ok = false; // Marcamos error.
                }
            }

            // La instalacion debe estar seleccionada.
            if (ok && reserva.InstalacionId < 1)
            {
                tituloError = "Instalacion requerida"; // Titulo.
                mensajeError = "Debe seleccionar una instalacion para la reserva."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // La instalacion debe estar marcada como disponible.
            if (ok)
            {
                var instalacion = _instalacionesApi.ObtenerPorId(reserva.InstalacionId); // Obtenemos la instalacion de la BD.
                if (instalacion == null || !instalacion.Disponible) // Si no existe o no esta disponible, rechazamos.
                {
                    tituloError = "Instalacion no disponible"; // Titulo.
                    mensajeError = "La instalacion seleccionada no esta disponible para reservas."; // Mensaje.
                    ok = false; // Marcamos error.
                }
            }

            // La fecha y hora de inicio es obligatoria.
            if (ok && reserva.FechaHoraInicio == default(DateTime))
            {
                tituloError = "Fecha de inicio requerida"; // Titulo.
                mensajeError = "Debe indicar la fecha y hora de inicio."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // La fecha y hora de fin es obligatoria.
            if (ok && reserva.FechaHoraFin == default(DateTime))
            {
                tituloError = "Fecha de fin requerida"; // Titulo.
                mensajeError = "Debe indicar la fecha y hora de fin."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // El fin debe ser posterior al inicio.
            if (ok && reserva.FechaHoraFin <= reserva.FechaHoraInicio)
            {
                tituloError = "Fechas no validas"; // Titulo.
                mensajeError = "La hora de fin debe ser posterior a la hora de inicio."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // La duracion minima es de 30 minutos.
            if (ok && (reserva.FechaHoraFin - reserva.FechaHoraInicio).TotalMinutes < 30)
            {
                tituloError = "Duracion minima"; // Titulo.
                mensajeError = "La reserva debe durar al menos 30 minutos."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // El socio debe tener al menos una cuota pagada vigente para la fecha de la reserva.
            if (ok && !_cuotasApi.EstaAlCorriente(reserva.SocioId, reserva.FechaHoraInicio))
            {
                tituloError = "Socio sin cobertura de cuota"; // Titulo.
                mensajeError = "El socio no tiene ninguna cuota pagada vigente para la fecha de la reserva."; // Mensaje.
                ok = false; // Marcamos error.
            }

            // Comprobamos que no exista otra reserva en el mismo horario para la misma instalacion.
            if (ok && _api.ExisteSolape(reserva.InstalacionId, reserva.FechaHoraInicio, reserva.FechaHoraFin, reserva.ReservaId))
            {
                tituloError = "Horario ocupado"; // Titulo.
                mensajeError = "La instalacion ya tiene una reserva en ese horario."; // Mensaje.
                ok = false; // Marcamos error.
            }

            return ok; // Devolvemos el resultado de la validacion.
        }
    }
}
