using Comun;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.controller
{
    /// <summary>
    /// Controlador de instalaciones.
    /// Contiene lógica de negocio y validaciones.
    /// </summary>
    public class InstalacionesController
    {
        private readonly InstalacionesAPI _api;

        /// <summary>
        /// Constructor.
        /// </summary>
        public InstalacionesController()
        {
            // Inicializamos API.
            _api = new InstalacionesAPI();
        }

        /// <summary>
        /// Obtiene todas las instalaciones.
        /// </summary>
        /// <param name="soloDisponibles">Si true, devuelve solo disponibles.</param>
        /// <returns>Lista.</returns>
        public List<Instalaciones> ObtenerTodos(bool soloDisponibles = false)
        {
            // Devolvemos listado desde API.
            return _api.ObtenerTodos(soloDisponibles);
        }

        /// <summary>
        /// Guarda una instalación tras validar.
        /// </summary>
        /// <param name="instalacion">Instalación.</param>
        /// <param name="tituloError">Título del error (ref).</param>
        /// <param name="mensajeError">Mensaje del error (ref).</param>
        /// <returns>True si guarda.</returns>
        public bool Guardar(Instalaciones instalacion, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos.
                if (!ValidarInstalacion(instalacion, ref tituloError, ref mensajeError))
                    return false;

                // Comprobamos nombre único.
                if (_api.NombreYaExiste(instalacion.Nombre, instalacion.InstalacionId > 0 ? instalacion.InstalacionId : (int?)null))
                {
                    tituloError = "Nombre duplicado";
                    mensajeError = "Ya existe una instalación con el mismo nombre.";
                    return false;
                }

                // Guardamos.
                _api.Guardar(instalacion);
                return true;
            }
            catch (Exception ex)
            {
                // Devolvemos error.
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar la instalación: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Borra una instalación de forma física.
        /// Solo se permite si no está en uso (sin reservas asociadas).
        /// </summary>
        /// <param name="instalacion">Instalación a borrar.</param>
        /// <param name="tituloError">Título (ref).</param>
        /// <param name="mensajeError">Mensaje (ref).</param>
        /// <returns>True si borra.</returns>
        public bool Borrar(Instalaciones instalacion, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos selección.
                if (instalacion == null || instalacion.InstalacionId < 1)
                {
                    tituloError = "Instalacion no valida";
                    mensajeError = "Debe seleccionar una instalacion valida.";
                    return false;
                }

                // Validamos uso en reservas.
                if (_api.EstaEnUso(instalacion.InstalacionId))
                {
                    tituloError = "Instalacion en uso";
                    mensajeError = "No se puede borrar la instalacion porque tiene reservas asociadas.";
                    return false;
                }

                // Borramos.
                _api.Borrar(instalacion.InstalacionId);
                return true;
            }
            catch (Exception ex)
            {
                // Error.
                tituloError = "Error al borrar";
                mensajeError = $"No se pudo borrar la instalacion: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Valida los datos básicos.
        /// </summary>
        /// <param name="instalacion">Instalación.</param>
        /// <param name="tituloError">Título.</param>
        /// <param name="mensajeError">Mensaje.</param>
        /// <returns>True si ok.</returns>
        private bool ValidarInstalacion(Instalaciones instalacion, ref string tituloError, ref string mensajeError)
        {
            // Validamos objeto.
            if (instalacion == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "La instalación no puede ser nula.";
                return false;
            }

            // Validamos nombre.
            if (string.IsNullOrWhiteSpace(instalacion.Nombre))
            {
                tituloError = "Nombre requerido";
                mensajeError = "Debe indicar el nombre de la instalación.";
                return false;
            }

            // Validamos longitud.
            if (instalacion.Nombre.Trim().Length < 2)
            {
                tituloError = "Nombre muy corto";
                mensajeError = "El nombre debe tener al menos 2 caracteres.";
                return false;
            }

            // Validamos tipo.
            if (instalacion.TipoInstalacionId < 1)
            {
                tituloError = "Tipo requerido";
                mensajeError = "Debe seleccionar un tipo de instalación.";
                return false;
            }

            // Validamos precio.
            if (instalacion.PrecioHora < 0)
            {
                tituloError = "Precio no válido";
                mensajeError = "El precio por hora no puede ser negativo.";
                return false;
            }

            return true;
        }
    }
}
