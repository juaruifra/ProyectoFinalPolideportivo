using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Agentes en la base de datos
    /// </summary>
    public class AgentesRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todos los agentes de la base de datos
        /// </summary>
        /// <returns>Lista con todos los agentes</returns>
        public List<Agentes> GetAll()
        {
            return Context.Agentes.ToList();
        }

        /// <summary>
        /// Buscar un agente por su identificador
        /// </summary>
        /// <param name="idAgente">Identificador del agente</param>
        /// <returns>Agente encontrado o null si no existe</returns>
        public Agentes GetById(int idAgente)
        {
            return Context.Agentes.FirstOrDefault(a => a.IdAgente == idAgente);
        }

        /// <summary>
        /// Buscar agentes por nombre (búsqueda parcial)
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre a buscar</param>
        /// <returns>Lista de agentes que coinciden con el criterio</returns>
        public List<Agentes> BuscarPorNombre(string nombre)
        {
            return Context.Agentes
                .Where(a => a.Nombre.Contains(nombre))
                .ToList();
        }

        /// <summary>
        /// Buscar un agente por su email
        /// </summary>
        /// <param name="email">Email del agente</param>
        /// <returns>Agente encontrado o null si no existe</returns>
        public Agentes BuscarPorEmail(string email)
        {
            return Context.Agentes.FirstOrDefault(a => a.Email == email);
        }

        /// <summary>
        /// Guardar un agente (insertar o actualizar)
        /// Si el IdAgente es 0, se inserta un nuevo registro
        /// Si el IdAgente es mayor que 0, se actualiza el registro existente
        /// </summary>
        /// <param name="agente">Objeto agente a guardar</param>
        /// <exception cref="Exception">Error al guardar en la base de datos</exception>
        public void Save(Agentes agente)
        {
            try
            {
                if (agente.IdAgente < 1)
                {
                    // INSERTAR nuevo agente
                    Context.Agentes.Add(new Agentes
                    {
                        Nombre = agente.Nombre,
                        Email = agente.Email,
                        Telefono = agente.Telefono,
                        FechaAlta = DateTime.Now // Asignar fecha actual al insertar
                    });
                }
                else
                {
                    // ACTUALIZAR agente existente
                    var a = Context.Agentes.FirstOrDefault(x => x.IdAgente == agente.IdAgente);

                    if (a != null)
                    {
                        a.Nombre = agente.Nombre;
                        a.Email = agente.Email;
                        a.Telefono = agente.Telefono;
                        // FechaAlta no se modifica, se mantiene la original
                    }
                }

                // Guardar cambios en la base de datos
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar agente en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Eliminar un agente de la base de datos
        /// </summary>
        /// <param name="agente">Objeto agente a eliminar</param>
        /// <exception cref="Exception">Error al eliminar de la base de datos</exception>
        public void Delete(Agentes agente)
        {
            try
            {
                // Obtenemos el agente del mismo contexto para evitar problemas
                var a = Context.Agentes.FirstOrDefault(x => x.IdAgente == agente.IdAgente);

                if (a != null)
                {
                    Context.Agentes.Remove(a);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar agente de la base de datos.", ex);
            }
        }

        /// <summary>
        /// Comprobar si ya existe un agente con el mismo email
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="idAgenteExcluir">ID del agente a excluir (para ediciones)</param>
        /// <returns>True si el email ya existe, False si está disponible</returns>
        public bool EmailYaExiste(string email, int? idAgenteExcluir = null)
        {
            var query = Context.Agentes.Where(a => a.Email == email);

            // Si estamos editando, excluimos el propio agente
            if (idAgenteExcluir.HasValue)
            {
                query = query.Where(a => a.IdAgente != idAgenteExcluir.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Obtener el total de agentes registrados
        /// </summary>
        /// <returns>Número total de agentes</returns>
        public int GetTotal()
        {
            return Context.Agentes.Count();
        }

        /// <summary>
        /// Obtener los agentes más activos (con más operaciones)
        /// </summary>
        /// <param name="cantidad">Cantidad de agentes a devolver</param>
        /// <returns>Lista de agentes ordenados por número de operaciones</returns>
        public List<Agentes> GetMasActivos(int cantidad = 5)
        {
            return Context.Agentes
                .OrderByDescending(a => a.Operaciones.Count)
                .Take(cantidad)
                .ToList();
        }
    }
}
