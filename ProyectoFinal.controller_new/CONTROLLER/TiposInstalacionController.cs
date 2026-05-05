using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.controller
{
    /// <summary>
    /// Controlador de tipos de instalacion.
    /// Contiene logica de negocio y validaciones.
    /// </summary>
    public class TiposInstalacionController
    {
        private readonly TiposInstalacionAPI _api; // API de tipos de instalacion.

        /// <summary>
        /// Constructor.
        /// </summary>
        public TiposInstalacionController()
        {
            // Inicializamos la API.
            _api = new TiposInstalacionAPI();
        }

        /// <summary>
        /// Obtiene todos los tipos de instalacion.
        /// </summary>
        /// <returns>Lista de tipos.</returns>
        public List<TiposInstalacion> ObtenerTodos()
        {
            // Delegamos a la API.
            return _api.ObtenerTodos();
        }

        /// <summary>
        /// Guarda un tipo de instalacion tras validar los datos.
        /// </summary>
        /// <param name="tipo">Tipo a guardar.</param>
        /// <param name="tituloError">Titulo del error en caso de fallo (ref).</param>
        /// <param name="mensajeError">Mensaje del error en caso de fallo (ref).</param>
        /// <returns>True si se guarda correctamente.</returns>
        public bool Guardar(TiposInstalacion tipo, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos el nombre obligatorio.
                if (tipo == null || string.IsNullOrWhiteSpace(tipo.Nombre))
                {
                    tituloError = "Nombre requerido"; // Titulo.
                    mensajeError = "Debe indicar el nombre del tipo de instalacion."; // Mensaje.
                    return false; // Error.
                }

                // Validamos longitud minima del nombre.
                if (tipo.Nombre.Trim().Length < 2)
                {
                    tituloError = "Nombre muy corto"; // Titulo.
                    mensajeError = "El nombre debe tener al menos 2 caracteres."; // Mensaje.
                    return false; // Error.
                }

                // Comprobamos si el nombre ya existe en otro tipo distinto.
                var excluirId = tipo.TipoInstalacionId > 0 ? tipo.TipoInstalacionId : (int?)null; // Id a excluir.

                if (_api.NombreYaExiste(tipo.Nombre.Trim(), excluirId))
                {
                    tituloError = "Nombre duplicado"; // Titulo.
                    mensajeError = "Ya existe un tipo de instalacion con el mismo nombre."; // Mensaje.
                    return false; // Error.
                }

                // Guardamos a traves de la API.
                _api.Guardar(tipo);
                return true; // Exito.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al guardar"; // Titulo.
                mensajeError = $"No se pudo guardar el tipo de instalacion: {ex.Message}"; // Mensaje.
                return false; // Error.
            }
        }

        /// <summary>
        /// Borra un tipo de instalacion tras validar que no esta en uso.
        /// </summary>
        /// <param name="tipo">Tipo a borrar.</param>
        /// <param name="tituloError">Titulo del error en caso de fallo (ref).</param>
        /// <param name="mensajeError">Mensaje del error en caso de fallo (ref).</param>
        /// <returns>True si se borra correctamente.</returns>
        public bool Borrar(TiposInstalacion tipo, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos que haya tipo seleccionado.
                if (tipo == null || tipo.TipoInstalacionId < 1)
                {
                    tituloError = "Tipo no valido"; // Titulo.
                    mensajeError = "Debe seleccionar un tipo de instalacion valido."; // Mensaje.
                    return false; // Error.
                }

                // Comprobamos si alguna instalacion lo esta usando.
                if (_api.EstaEnUso(tipo.TipoInstalacionId))
                {
                    tituloError = "Tipo en uso"; // Titulo.
                    mensajeError = "No se puede borrar el tipo porque hay instalaciones que lo utilizan."; // Mensaje.
                    return false; // Error.
                }

                // Borramos a traves de la API.
                _api.Borrar(tipo.TipoInstalacionId);
                return true; // Exito.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                tituloError = "Error al borrar"; // Titulo.
                mensajeError = $"No se pudo borrar el tipo de instalacion: {ex.Message}"; // Mensaje.
                return false; // Error.
            }
        }
    }
}
