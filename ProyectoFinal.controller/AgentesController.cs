using Comun;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de agentes
    /// Incluye validaciones y operaciones complejas
    /// </summary>
    public class AgentesController
    {
        // API para comunicarse con los repositorios
        private AgentesAPI _api;
        private OperacionesAPI _operacionesApi;
        private OfertasAPI _ofertasApi;

        /// <summary>
        /// Constructor que inicializa la API
        /// </summary>
        public AgentesController()
        {
            _api = new AgentesAPI();
            _operacionesApi = new OperacionesAPI();
            _ofertasApi = new OfertasAPI();
        }

        /// <summary>
        /// Obtener todos los agentes ordenados alfabeticamente
        /// </summary>
        /// <returns>Lista de todos los agentes</returns>
        public List<Agentes> ObtenerTodos()
        {
            try
            {
                // Obtener todos los agentes a traves de la API
                return _api.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los agentes", ex);
            }
        }

        /// <summary>
        /// Obtener un agente por su identificador
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>Agente encontrado o null</returns>
        public Agentes ObtenerPorId(int idAgente)
        {
            try
            {
                // Obtener el agente por ID a traves de la API
                return _api.ObtenerPorId(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el agente con ID {idAgente}", ex);
            }
        }

        /// <summary>
        /// Buscar agentes por nombre
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre a buscar</param>
        /// <returns>Lista de agentes que coinciden</returns>
        public List<Agentes> BuscarPorNombre(string nombre)
        {
            try
            {
                // Buscar agentes por nombre a traves de la API
                return _api.BuscarPorNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar agentes por nombre", ex);
            }
        }

        /// <summary>
        /// Obtener el total de agentes registrados
        /// </summary>
        /// <returns>Numero total de agentes</returns>
        public int ObtenerTotal()
        {
            try
            {
                // Obtener el total de agentes a traves de la API
                return _api.ObtenerTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de agentes", ex);
            }
        }

        /// <summary>
        /// Guardar un agente con validaciones completas
        /// </summary>
        /// <param name="agente">Objeto agente a guardar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se guardo correctamente, False si hubo error</returns>
        public bool Guardar(Agentes agente, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar datos del agente
                if (!ValidarAgente(agente, ref tituloError, ref mensajeError))
                {
                    return false;
                }

                // Validar que el email no este duplicado
                if (_api.EmailYaExiste(agente.Email, agente.IdAgente))
                {
                    tituloError = "Email duplicado";
                    mensajeError = "Ya existe un agente registrado con ese email.";
                    return false;
                }

                // Guardar en la base de datos
                _api.Guardar(agente);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar el agente: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Eliminar un agente con validaciones
        /// </summary>
        /// <param name="agente">Agente a eliminar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se elimino correctamente, False si hubo error</returns>
        public bool Eliminar(Agentes agente, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar que el agente exista
                if (agente == null || agente.IdAgente < 1)
                {
                    tituloError = "Agente no valido";
                    mensajeError = "No se puede eliminar un agente que no existe.";
                    return false;
                }

                // Verificar si tiene operaciones asociadas
                if (_operacionesApi.ExistenPorAgente(agente.IdAgente))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El agente tiene operaciones asociadas y no se puede eliminar.";
                    return false;
                }

                // Verificar si tiene ofertas asociadas
                if (_ofertasApi.ExistenPorAgente(agente.IdAgente))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El agente tiene ofertas asociadas y no se puede eliminar.";
                    return false;
                }

                // Eliminar de la base de datos
                _api.Eliminar(agente);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al eliminar";
                mensajeError = $"No se pudo eliminar el agente: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Validar los datos de un agente
        /// </summary>
        /// <param name="agente">Agente a validar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si es valido, False si hay errores</returns>
        private bool ValidarAgente(Agentes agente, ref string tituloError, ref string mensajeError)
        {
            // Validar que el objeto no sea nulo
            if (agente == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "El agente no puede ser nulo.";
                return false;
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(agente.Nombre))
            {
                tituloError = "Nombre requerido";
                mensajeError = "Debe ingresar el nombre del agente.";
                return false;
            }

            if (agente.Nombre.Length < 3)
            {
                tituloError = "Nombre muy corto";
                mensajeError = "El nombre debe tener al menos 3 caracteres.";
                return false;
            }

            if (agente.Nombre.Length > 100)
            {
                tituloError = "Nombre muy largo";
                mensajeError = "El nombre no puede superar los 100 caracteres.";
                return false;
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(agente.Email))
            {
                tituloError = "Email requerido";
                mensajeError = "Debe ingresar el email del agente.";
                return false;
            }

            if (!Utils.EsEmailValido(agente.Email))
            {
                tituloError = "Email no valido";
                mensajeError = "El formato del email no es correcto.";
                return false;
            }

            // Validar telefono
            if (string.IsNullOrWhiteSpace(agente.Telefono))
            {
                tituloError = "Telefono requerido";
                mensajeError = "Debe ingresar el telefono del agente.";
                return false;
            }

            if (!Utils.EsTelefonoValido(agente.Telefono))
            {
                tituloError = "Telefono no valido";
                mensajeError = "El telefono debe tener entre 9 y 15 digitos.";
                return false;
            }

            return true;
        }
    }
}
