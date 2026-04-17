using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Clientes en la base de datos
    /// </summary>
    public class ClientesRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todos los clientes de la base de datos
        /// </summary>
        /// <returns>Lista con todos los clientes</returns>
        public List<Clientes> GetAll()
        {
            return Context.Clientes.ToList();
        }

        /// <summary>
        /// Buscar un cliente por su identificador
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Cliente encontrado o null si no existe</returns>
        public Clientes GetById(int idCliente)
        {
            return Context.Clientes.FirstOrDefault(c => c.IdCliente == idCliente);
        }

        /// <summary>
        /// Buscar clientes por nombre (búsqueda parcial)
        /// </summary>
        /// <param name="nombre">Nombre o parte del nombre a buscar</param>
        /// <returns>Lista de clientes que coinciden con el criterio</returns>
        public List<Clientes> BuscarPorNombre(string nombre)
        {
            return Context.Clientes
                .Where(c => c.Nombre.Contains(nombre))
                .ToList();
        }

        /// <summary>
        /// Buscar un cliente por su email
        /// </summary>
        /// <param name="email">Email del cliente</param>
        /// <returns>Cliente encontrado o null si no existe</returns>
        public Clientes BuscarPorEmail(string email)
        {
            return Context.Clientes.FirstOrDefault(c => c.Email == email);
        }

        /// <summary>
        /// Guardar un cliente (insertar o actualizar)
        /// Si el IdCliente es 0, se inserta un nuevo registro
        /// Si el IdCliente es mayor que 0, se actualiza el registro existente
        /// </summary>
        /// <param name="cliente">Objeto cliente a guardar</param>
        /// <exception cref="Exception">Error al guardar en la base de datos</exception>
        public void Save(Clientes cliente)
        {
            try
            {
                if (cliente.IdCliente < 1)
                {
                    // INSERTAR nuevo cliente
                    Context.Clientes.Add(new Clientes
                    {
                        Nombre = cliente.Nombre,
                        Email = cliente.Email,
                        Telefono = cliente.Telefono,
                        FechaAlta = DateTime.Now // Asignar fecha actual al insertar
                    });
                }
                else
                {
                    // ACTUALIZAR cliente existente
                    var c = Context.Clientes.FirstOrDefault(x => x.IdCliente == cliente.IdCliente);

                    if (c != null)
                    {
                        c.Nombre = cliente.Nombre;
                        c.Email = cliente.Email;
                        c.Telefono = cliente.Telefono;
                        // FechaAlta no se modifica, se mantiene la original
                    }
                }

                // Guardar cambios en la base de datos
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar cliente en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Eliminar un cliente de la base de datos
        /// </summary>
        /// <param name="cliente">Objeto cliente a eliminar</param>
        /// <exception cref="Exception">Error al eliminar de la base de datos</exception>
        public void Delete(Clientes cliente)
        {
            try
            {
                // Obtenemos el cliente del mismo contexto para evitar problemas
                var c = Context.Clientes.FirstOrDefault(x => x.IdCliente == cliente.IdCliente);

                if (c != null)
                {
                    Context.Clientes.Remove(c);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar cliente de la base de datos.", ex);
            }
        }

        /// <summary>
        /// Comprobar si ya existe un cliente con el mismo email
        /// </summary>
        /// <param name="email">Email a verificar</param>
        /// <param name="idClienteExcluir">ID del cliente a excluir (para ediciones)</param>
        /// <returns>True si el email ya existe, False si está disponible</returns>
        public bool EmailYaExiste(string email, int? idClienteExcluir = null)
        {
            var query = Context.Clientes.Where(c => c.Email == email);

            // Si estamos editando, excluimos el propio cliente
            if (idClienteExcluir.HasValue)
            {
                query = query.Where(c => c.IdCliente != idClienteExcluir.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Obtener el total de clientes registrados
        /// </summary>
        /// <returns>Número total de clientes</returns>
        public int GetTotal()
        {
            return Context.Clientes.Count();
        }
    }
}
