using Comun;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de socios.
    /// Incluye validaciones y operaciones sobre ClubPolideportivoDB.
    /// </summary>
    public class SociosController
    {
        private SociosAPI _api; // API de socios que comunica con el repositorio.

        /// <summary>
        /// Constructor.
        /// </summary>
        public SociosController()
        {
            // Inicializamos la API de socios.
            _api = new SociosAPI();
        }

        /// <summary>
        /// Obtiene todos los socios, opcionalmente solo los activos.
        /// </summary>
        /// <param name="soloActivos">Si true, devuelve solo socios activos.</param>
        /// <returns>Lista de socios.</returns>
        public List<Socios> ObtenerTodos(bool soloActivos = false)
        {
            // Delegamos directamente a la API.
            return _api.ObtenerTodos(soloActivos);
        }

        /// <summary>
        /// Devuelve el numero total de socios.
        /// </summary>
        /// <param name="soloActivos">Si true, cuenta solo los activos.</param>
        /// <returns>Total de socios.</returns>
        public int ObtenerTotal(bool soloActivos = false)
        {
            // Delegamos el conteo a la API.
            return _api.ObtenerTotal(soloActivos);
        }

        /// <summary>
        /// Guarda o actualiza un socio tras validar sus datos.
        /// </summary>
        /// <param name="socio">Socio a guardar.</param>
        /// <param name="tituloError">Titulo del error en caso de fallo (ref).</param>
        /// <param name="mensajeError">Mensaje del error en caso de fallo (ref).</param>
        /// <returns>True si se guarda correctamente.</returns>
        public bool Guardar(Socios socio, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos los datos basicos del socio antes de continuar.
                if (!ValidarSocio(socio, ref tituloError, ref mensajeError))
                    return false; // Salimos si la validacion falla.

                // Comprobamos que el DNI no este ya registrado en otro socio.
                if (_api.DniYaExiste(socio.Dni, socio.SocioId > 0 ? socio.SocioId : (int?)null))
                {
                    tituloError = "DNI duplicado"; // Titulo del error.
                    mensajeError = "Ya existe un socio con el mismo DNI."; // Mensaje al usuario.
                    return false; // Error de duplicado.
                }

                // Si todo es correcto guardamos el socio.
                _api.Guardar(socio);
                return true; // Exito.
            }
            catch (Exception ex)
            {
                // Error inesperado al guardar.
                tituloError = "Error al guardar"; // Titulo.
                mensajeError = $"No se pudo guardar el socio: {ex.Message}"; // Detalle del error.
                return false; // Error.
            }
        }

        /// <summary>
        /// Borra un socio de forma fisica.
        /// Solo se permite si el socio no tiene reservas asociadas.
        /// </summary>
        /// <param name="socio">Socio a borrar.</param>
        /// <param name="tituloError">Titulo del error en caso de fallo (ref).</param>
        /// <param name="mensajeError">Mensaje del error en caso de fallo (ref).</param>
        /// <returns>True si se borra correctamente.</returns>
        public bool Borrar(Socios socio, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Comprobamos que el socio sea valido antes de intentar borrar.
                if (socio == null || socio.SocioId < 1)
                {
                    tituloError = "Socio no valido"; // Titulo.
                    mensajeError = "Debe seleccionar un socio valido."; // Mensaje.
                    return false; // Error.
                }

                // Comprobamos que el socio no tenga reservas asociadas.
                if (_api.EstaEnUso(socio.SocioId))
                {
                    tituloError = "Socio en uso"; // Titulo.
                    mensajeError = "No se puede borrar el socio porque tiene reservas asociadas."; // Mensaje.
                    return false; // Error.
                }

                // Si no esta en uso procedemos a borrarlo.
                _api.Borrar(socio.SocioId);
                return true; // Exito.
            }
            catch (Exception ex)
            {
                // Error inesperado al borrar.
                tituloError = "Error al borrar"; // Titulo.
                mensajeError = $"No se pudo borrar el socio: {ex.Message}"; // Detalle del error.
                return false; // Error.
            }
        }

        /// <summary>
        /// Valida los datos obligatorios de un socio antes de guardarlo.
        /// </summary>
        /// <param name="socio">Socio a validar.</param>
        /// <param name="tituloError">Titulo del error si la validacion falla (ref).</param>
        /// <param name="mensajeError">Mensaje del error si la validacion falla (ref).</param>
        /// <returns>True si los datos son correctos.</returns>
        private bool ValidarSocio(Socios socio, ref string tituloError, ref string mensajeError)
        {
            // Comprobamos que el objeto no sea nulo.
            if (socio == null)
            {
                tituloError = "Datos incompletos"; // Titulo.
                mensajeError = "El socio no puede ser nulo."; // Mensaje.
                return false; // Error.
            }

            // Comprobamos que el nombre tenga al menos 2 caracteres.
            if (string.IsNullOrWhiteSpace(socio.Nombre) || socio.Nombre.Trim().Length < 2)
            {
                tituloError = "Nombre requerido"; // Titulo.
                mensajeError = "Debe indicar el nombre del socio (min. 2 caracteres)."; // Mensaje.
                return false; // Error.
            }

            // Comprobamos que los apellidos tengan al menos 2 caracteres.
            if (string.IsNullOrWhiteSpace(socio.Apellidos) || socio.Apellidos.Trim().Length < 2)
            {
                tituloError = "Apellidos requeridos"; // Titulo.
                mensajeError = "Debe indicar los apellidos del socio (min. 2 caracteres)."; // Mensaje.
                return false; // Error.
            }

            // Comprobamos que el DNI no este vacio.
            if (string.IsNullOrWhiteSpace(socio.Dni))
            {
                tituloError = "DNI requerido"; // Titulo.
                mensajeError = "Debe indicar el DNI del socio."; // Mensaje.
                return false; // Error.
            }

            // Comprobamos que el email no este vacio.
            if (string.IsNullOrWhiteSpace(socio.Email))
            {
                tituloError = "Email requerido"; // Titulo.
                mensajeError = "Debe ingresar el email del socio."; // Mensaje.
                return false; // Error.
            }

            // Comprobamos que el formato del email sea valido usando la utilidad comun.
            if (!Utils.EsEmailValido(socio.Email))
            {
                tituloError = "Email no valido"; // Titulo.
                mensajeError = "El formato del email no es correcto."; // Mensaje.
                return false; // Error.
            }

            // Si hay telefono, comprobamos que tenga un formato valido.
            if (!string.IsNullOrWhiteSpace(socio.Telefono) && !Utils.EsTelefonoValido(socio.Telefono))
            {
                tituloError = "Telefono no valido"; // Titulo.
                mensajeError = "El telefono debe tener entre 9 y 15 digitos."; // Mensaje.
                return false; // Error.
            }

            return true; // Todos los datos son correctos.
        }
    }
}
