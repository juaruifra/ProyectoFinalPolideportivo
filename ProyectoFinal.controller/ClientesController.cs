using Comun;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// Controlador para gestionar la lógica de negocio de clientes
    /// Incluye validaciones y operaciones complejas
    /// </summary>
    public class ClientesController
    {
        // API para comunicarse con los repositorios
        private ClientesAPI _api;
        private OperacionesAPI _operacionesApi;
        private OfertasAPI _ofertasApi;

        /// <summary>
        /// Constructor que inicializa la API
        /// </summary>
        public ClientesController()
        {
            _api = new ClientesAPI();
            _operacionesApi = new OperacionesAPI();
            _ofertasApi = new OfertasAPI();
        }

        /// <summary>
        /// Obtener todos los clientes ordenados alfabeticamente
        /// </summary>
        /// <returns>Lista de todos los clientes</returns>
        public List<Clientes> ObtenerTodos()
        {
            try
            {
                // Obtener todos los clientes a traves de la API
                return _api.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes", ex);
            }
        }

        /// <summary>
        /// Obtener un cliente por su identificador
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>Cliente encontrado o null</returns>
        public Clientes ObtenerPorId(int idCliente)
        {
            try
            {
                // Obtener el cliente por ID a traves de la API
                return _api.ObtenerPorId(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el cliente con ID {idCliente}", ex);
            }
        }

        /// <summary>
        /// Buscar clientes por nombre
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre a buscar</param>
        /// <returns>Lista de clientes que coinciden</returns>
        public List<Clientes> BuscarPorNombre(string nombre)
        {
            try
            {
                // Buscar clientes por nombre a traves de la API
                return _api.BuscarPorNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar clientes por nombre", ex);
            }
        }

        /// <summary>
        /// Obtener el total de clientes registrados
        /// </summary>
        /// <returns>Numero total de clientes</returns>
        public int ObtenerTotal()
        {
            try
            {
                // Obtener el total de clientes a traves de la API
                return _api.ObtenerTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de clientes", ex);
            }
        }

        /// <summary>
        /// Guardar un cliente con validaciones completas
        /// </summary>
        /// <param name="cliente">Objeto cliente a guardar</param>
        /// <param name="tituloError">Título del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se guardó correctamente, False si hubo error</returns>
        public bool Guardar(Clientes cliente, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar datos del cliente
                if (!ValidarCliente(cliente, ref tituloError, ref mensajeError))
                {
                    return false;
                }

                // Guardar en la base de datos
                _api.Guardar(cliente);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar el cliente: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Eliminar un cliente con validaciones
        /// </summary>
        /// <param name="cliente">Cliente a eliminar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se elimino correctamente, False si hubo error</returns>
        public bool Eliminar(Clientes cliente, ref string tituloError, ref string mensajeError)
        {
            try
            {

                // Validar que el cliente exista
                if (cliente == null || cliente.IdCliente < 1)
                {
                    tituloError = "Cliente no válido";
                    mensajeError = "No se puede eliminar un cliente que no existe.";
                    return false;
                }

                // Verificar si tiene operaciones asociadas
                if (_operacionesApi.ExistenPorCliente(cliente.IdCliente))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El cliente tiene operaciones asociadas y no se puede eliminar.";
                    return false;
                }

                // Verificar si tiene ofertas asociadas
                if (_ofertasApi.ExistenPorCliente(cliente.IdCliente))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El cliente tiene ofertas asociadas y no se puede eliminar.";
                    return false;
                }

                // Eliminar de la base de datos
                _api.Eliminar(cliente);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al eliminar";
                mensajeError = $"No se pudo eliminar el cliente: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Validar los datos de un cliente
        /// </summary>
        /// <param name="cliente">Cliente a validar</param>
        /// <param name="tituloError">Título del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si es válido, False si hay errores</returns>
        private bool ValidarCliente(Clientes cliente, ref string tituloError, ref string mensajeError)
        {
            // Validar que el objeto no sea nulo
            if (cliente == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "El cliente no puede ser nulo.";
                return false;
            }

            // Validar nombre
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
            {
                tituloError = "Nombre requerido";
                mensajeError = "Debe ingresar el nombre del cliente.";
                return false;
            }

            if (cliente.Nombre.Length < 3)
            {
                tituloError = "Nombre muy corto";
                mensajeError = "El nombre debe tener al menos 3 caracteres.";
                return false;
            }

            if (cliente.Nombre.Length > 100)
            {
                tituloError = "Nombre muy largo";
                mensajeError = "El nombre no puede superar los 100 caracteres.";
                return false;
            }

            // Validar email
            if (string.IsNullOrWhiteSpace(cliente.Email))
            {
                tituloError = "Email requerido";
                mensajeError = "Debe ingresar el email del cliente.";
                return false;
            }

            if (!Utils.EsEmailValido(cliente.Email))
            {
                tituloError = "Email no válido";
                mensajeError = "El formato del email no es correcto.";
                return false;
            }

            // Validar teléfono
            if (string.IsNullOrWhiteSpace(cliente.Telefono))
            {
                tituloError = "Teléfono requerido";
                mensajeError = "Debe ingresar el teléfono del cliente.";
                return false;
            }

            if (!Utils.EsTelefonoValido(cliente.Telefono))
            {
                tituloError = "Teléfono no válido";
                mensajeError = "El teléfono debe tener entre 9 y 15 dígitos.";
                return false;
            }

            return true;
        }
    }
}
