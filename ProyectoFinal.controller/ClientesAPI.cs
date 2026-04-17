using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// API para gestionar las operaciones sobre clientes
    /// Intermediario entre los repositorios y los controladores
    /// Prepara los datos para que la vista los consuma fácilmente
    /// </summary>
    public class ClientesAPI
    {
        // Repositorio de clientes para acceder a la BBDD
        private ClientesRepository _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public ClientesAPI()
        {
            _repo = new ClientesRepository();
        }

        /// <summary>
        /// Obtener todos los clientes de la base de datos
        /// </summary>
        /// <returns>Lista de clientes ordenada alfabeticamente</returns>
        public List<Clientes> ObtenerTodos()
        {
            try
            {
                return _repo.GetAll().OrderBy(c => c.Nombre).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los clientes", ex);
            }
        }

        /// <summary>
        /// Obtener un cliente por su ID
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Cliente encontrado o null</returns>
        public Clientes ObtenerPorId(int idCliente)
        {
            try
            {
                return _repo.GetById(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el cliente con ID {idCliente}", ex);
            }
        }

        /// <summary>
        /// Buscar clientes por nombre
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre</param>
        /// <returns>Lista de clientes que coinciden</returns>
        public List<Clientes> BuscarPorNombre(string nombre)
        {
            try
            {
                // Si el nombre está vacío, devolvemos la lista vacia
                if (string.IsNullOrWhiteSpace(nombre)) return new List<Clientes>();

                return _repo.BuscarPorNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar clientes por nombre", ex);
            }
        }

        /// <summary>
        /// Verificar si un email ya está registrado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="idClienteExcluir">ID del cliente a excluir (para ediciones)</param>
        /// <returns>True si el email ya existe</returns>
        public bool EmailYaExiste(string email, int? idClienteExcluir = null)
        {
            try
            {
                return _repo.EmailYaExiste(email, idClienteExcluir);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar email", ex);
            }
        }

        /// <summary>
        /// Guardar un cliente (insertar o actualizar)
        /// </summary>
        /// <param name="cliente">Objeto cliente</param>
        public void Guardar(Clientes cliente)
        {
            try
            {
                _repo.Save(cliente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el cliente", ex);
            }
        }

        /// <summary>
        /// Eliminar un cliente
        /// </summary>
        /// <param name="cliente">Objeto cliente a eliminar</param>
        public void Eliminar(Clientes cliente)
        {
            try
            {
                _repo.Delete(cliente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el cliente", ex);
            }
        }

        /// <summary>
        /// Obtener el total de clientes registrados
        /// </summary>
        /// <returns>Número total de clientes</returns>
        public int ObtenerTotal()
        {
            try
            {
                return _repo.GetTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de clientes", ex);
            }
        }
    }
}
