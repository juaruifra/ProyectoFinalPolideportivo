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
        private SociosAPI _api;

        public SociosController()
        {
            _api = new SociosAPI();
        }

        public List<Socios> ObtenerTodos(bool soloActivos = false)
        {
            return _api.ObtenerTodos(soloActivos);
        }

        public int ObtenerTotal(bool soloActivos = false)
        {
            return _api.ObtenerTotal(soloActivos);
        }

        public bool Guardar(Socios socio, ref string tituloError, ref string mensajeError)
        {
            try
            {
                if (!ValidarSocio(socio, ref tituloError, ref mensajeError))
                    return false;

                if (_api.DniYaExiste(socio.Dni, socio.SocioId > 0 ? socio.SocioId : (int?)null))
                {
                    tituloError = "DNI duplicado";
                    mensajeError = "Ya existe un socio con el mismo DNI.";
                    return false;
                }

                _api.Guardar(socio);
                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar el socio: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Borra un socio de forma física.
        /// Solo se permite si no está en uso (sin reservas asociadas).
        /// </summary>
        /// <param name="socio">Socio a borrar.</param>
        /// <param name="tituloError">Título (ref).</param>
        /// <param name="mensajeError">Mensaje (ref).</param>
        /// <returns>True si borra.</returns>
        public bool Borrar(Socios socio, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validamos selección.
                if (socio == null || socio.SocioId < 1)
                {
                    tituloError = "Socio no valido";
                    mensajeError = "Debe seleccionar un socio valido.";
                    return false;
                }

                // Validamos uso en reservas.
                if (_api.EstaEnUso(socio.SocioId))
                {
                    tituloError = "Socio en uso";
                    mensajeError = "No se puede borrar el socio porque tiene reservas asociadas.";
                    return false;
                }

                // Borramos.
                _api.Borrar(socio.SocioId);
                return true;
            }
            catch (Exception ex)
            {
                // Error.
                tituloError = "Error al borrar";
                mensajeError = $"No se pudo borrar el socio: {ex.Message}";
                return false;
            }
        }

        private bool ValidarSocio(Socios socio, ref string tituloError, ref string mensajeError)
        {
            if (socio == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "El socio no puede ser nulo.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(socio.Nombre) || socio.Nombre.Trim().Length < 2)
            {
                tituloError = "Nombre requerido";
                mensajeError = "Debe indicar el nombre del socio (min. 2 caracteres).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(socio.Apellidos) || socio.Apellidos.Trim().Length < 2)
            {
                tituloError = "Apellidos requeridos";
                mensajeError = "Debe indicar los apellidos del socio (min. 2 caracteres).";
                return false;
            }

            if (string.IsNullOrWhiteSpace(socio.Dni))
            {
                tituloError = "DNI requerido";
                mensajeError = "Debe indicar el DNI del socio.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(socio.Email))
            {
                tituloError = "Email requerido";
                mensajeError = "Debe ingresar el email del socio.";
                return false;
            }

            if (!Utils.EsEmailValido(socio.Email))
            {
                tituloError = "Email no valido";
                mensajeError = "El formato del email no es correcto.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(socio.Telefono) && !Utils.EsTelefonoValido(socio.Telefono))
            {
                tituloError = "Telefono no valido";
                mensajeError = "El telefono debe tener entre 9 y 15 digitos.";
                return false;
            }

            return true;
        }
    }
}
