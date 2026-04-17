using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// API para gestionar las operaciones sobre agentes
    /// Intermediario entre los repositorios y los controladores
    /// Prepara los datos para que la vista los consuma facilmente
    /// </summary>
    public class AgentesAPI
    {
        // Repositorio de agentes para acceder a la BBDD
        private AgentesRepository _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public AgentesAPI()
        {
            _repo = new AgentesRepository();
        }

        /// <summary>
        /// Obtener todos los agentes de la base de datos
        /// </summary>
        /// <returns>Lista de agentes ordenada alfabeticamente</returns>
        public List<Agentes> ObtenerTodos()
        {
            try
            {
                return _repo.GetAll().OrderBy(a => a.Nombre).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los agentes", ex);
            }
        }

        /// <summary>
        /// Obtener un agente por su ID
        /// </summary>
        /// <param name="idAgente">Identificador del agente</param>
        /// <returns>Agente encontrado o null</returns>
        public Agentes ObtenerPorId(int idAgente)
        {
            try
            {
                return _repo.GetById(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el agente con ID {idAgente}", ex);
            }
        }

        /// <summary>
        /// Buscar agentes por nombre
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre</param>
        /// <returns>Lista de agentes que coinciden</returns>
        public List<Agentes> BuscarPorNombre(string nombre)
        {
            try
            {
                // Si el nombre esta vacio, devolvemos la lista vacia
                if (string.IsNullOrWhiteSpace(nombre)) return new List<Agentes>();

                return _repo.BuscarPorNombre(nombre);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar agentes por nombre", ex);
            }
        }

        /// <summary>
        /// Verificar si un email ya esta registrado
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="idAgenteExcluir">ID del agente a excluir (para ediciones)</param>
        /// <returns>True si el email ya existe</returns>
        public bool EmailYaExiste(string email, int? idAgenteExcluir = null)
        {
            try
            {
                return _repo.EmailYaExiste(email, idAgenteExcluir);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar email", ex);
            }
        }

        /// <summary>
        /// Guardar un agente (insertar o actualizar)
        /// </summary>
        /// <param name="agente">Objeto agente</param>
        public void Guardar(Agentes agente)
        {
            try
            {
                _repo.Save(agente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el agente", ex);
            }
        }

        /// <summary>
        /// Eliminar un agente
        /// </summary>
        /// <param name="agente">Objeto agente a eliminar</param>
        public void Eliminar(Agentes agente)
        {
            try
            {
                _repo.Delete(agente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el agente", ex);
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
                return _repo.GetTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de agentes", ex);
            }
        }
    }
}
