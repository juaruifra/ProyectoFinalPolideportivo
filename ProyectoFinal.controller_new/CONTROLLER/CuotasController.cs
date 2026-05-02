using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.controller
{
    /// <summary>
    /// Controlador para gestionar la lógica de negocio de las cuotas.
    /// Incluye validaciones y operaciones sobre ClubPolideportivoDB.
    /// </summary>
    public class CuotasController
    {
        // API de cuotas.
        private readonly CuotasAPI _api;

        /// <summary>
        /// Constructor: inicializa la API.
        /// </summary>
        public CuotasController()
        {
            // Instanciamos la API.
            _api = new CuotasAPI();
        }

        /// <summary>
        /// Obtiene todas las cuotas con filtros opcionales.
        /// </summary>
        /// <param name="socioId">Filtro por socio. Null devuelve todas.</param>
        /// <param name="soloPendientes">Si true, solo devuelve las no pagadas.</param>
        /// <returns>Lista de cuotas.</returns>
        public List<Cuotas> ObtenerTodos(int? socioId = null, bool soloPendientes = false)
        {
            // Delegamos en la API.
            return _api.ObtenerTodos(socioId, soloPendientes);
        }

        /// <summary>
        /// Guarda una cuota (alta o modificación) con validaciones previas.
        /// </summary>
        /// <param name="cuota">Entidad Cuotas.</param>
        /// <param name="tituloError">Título del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si guarda correctamente.</returns>
        public bool Guardar(Cuotas cuota, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos la cuota antes de guardar.
                if (!ValidarCuota(cuota, ref tituloError, ref mensajeError))
                    return false; // Devolvemos false si no pasa validacion.

                // Guardamos.
                _api.Guardar(cuota);
                return true; // Éxito.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar la cuota: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Marca una cuota como pagada con fecha de hoy.
        /// </summary>
        /// <param name="cuota">Cuota a marcar.</param>
        /// <param name="tituloError">Título del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si se marca correctamente.</returns>
        public bool MarcarPagada(Cuotas cuota, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos que la cuota sea válida.
                if (cuota == null || cuota.CuotaId < 1)
                {
                    tituloError = "Cuota no válida";
                    mensajeError = "Debe seleccionar una cuota válida.";
                    return false;
                }

                // Validamos que no esté ya pagada.
                if (cuota.Pagada)
                {
                    tituloError = "Cuota ya pagada";
                    mensajeError = "Esta cuota ya está marcada como pagada.";
                    return false;
                }

                // Marcamos como pagada con la fecha de hoy.
                _api.MarcarPagada(cuota.CuotaId, DateTime.Today);
                return true; // Éxito.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al marcar pagada";
                mensajeError = $"No se pudo marcar la cuota como pagada: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Borra fisicamente una cuota con validacion previa.
        /// No se puede borrar una cuota ya pagada.
        /// </summary>
        /// <param name="cuota">Cuota a borrar.</param>
        /// <param name="tituloError">Titulo del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si borra correctamente.</returns>
        public bool Borrar(Cuotas cuota, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos que la cuota sea valida.
                if (cuota == null || cuota.CuotaId < 1)
                {
                    tituloError = "Cuota no valida";
                    mensajeError = "Debe seleccionar una cuota valida.";
                    return false;
                }

                // No permitimos borrar cuotas ya pagadas: forman parte del historial.
                if (cuota.Pagada)
                {
                    tituloError = "Cuota pagada";
                    mensajeError = "No se puede borrar una cuota que ya esta pagada. Forma parte del historial.";
                    return false;
                }

                // Borramos.
                _api.Borrar(cuota.CuotaId);
                return true; // Exito.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al borrar";
                mensajeError = $"No se pudo borrar la cuota: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Valida los datos de una cuota antes de guardar.
        /// </summary>
        /// <param name="cuota">Entidad Cuotas.</param>
        /// <param name="tituloError">Título del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si es válida.</returns>
        private bool ValidarCuota(Cuotas cuota, ref string tituloError, ref string mensajeError)
        {
            // Null guard.
            if (cuota == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "La cuota no puede ser nula.";
                return false;
            }

            // El socio debe estar seleccionado.
            if (cuota.SocioId < 1)
            {
                tituloError = "Socio requerido";
                mensajeError = "Debe seleccionar un socio para la cuota.";
                return false;
            }

            // El año debe ser razonable.
            if (cuota.Anio < 2000 || cuota.Anio > 2100)
            {
                tituloError = "Año no válido";
                mensajeError = "El año debe estar entre 2000 y 2100.";
                return false;
            }

            // El mes debe estar entre 1 y 12.
            if (cuota.Mes < 1 || cuota.Mes > 12)
            {
                tituloError = "Mes no válido";
                mensajeError = "El mes debe estar entre 1 y 12.";
                return false;
            }

            // El importe no puede ser negativo.
            if (cuota.Importe < 0)
            {
                tituloError = "Importe no válido";
                mensajeError = "El importe no puede ser negativo.";
                return false;
            }

            // La fecha de vencimiento no puede ser nula.
            if (cuota.FechaVencimiento == default(DateTime))
            {
                tituloError = "Fecha de vencimiento requerida";
                mensajeError = "Debe indicar la fecha de vencimiento.";
                return false;
            }

            // La fecha de pago no puede ser nula.
            if (cuota.FechaPago == default(DateTime))
            {
                tituloError = "Fecha de pago requerida";
                mensajeError = "Debe indicar la fecha de pago.";
                return false;
            }

            // La fecha de vencimiento no puede ser anterior a hoy (solo en alta nueva).
            if (cuota.CuotaId == 0 && cuota.FechaVencimiento.Date < DateTime.Today)
            {
                tituloError = "Fecha de vencimiento no valida";
                mensajeError = "La fecha de vencimiento no puede ser anterior a hoy.";
                return false;
            }

            // No puede existir otra cuota del mismo socio para el mismo anio y mes.
            if (_api.ExisteDuplicado(cuota.SocioId, cuota.Anio, cuota.Mes, cuota.CuotaId))
            {
                tituloError = "Cuota duplicada";
                mensajeError = $"Ya existe una cuota para este socio en el mes {cuota.Mes}/{cuota.Anio}.";
                return false;
            }

            return true; // Todo correcto.
        }
    }
}
