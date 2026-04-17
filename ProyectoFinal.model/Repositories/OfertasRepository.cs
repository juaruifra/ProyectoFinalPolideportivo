using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Ofertas en la base de datos
    /// </summary>
    public class OfertasRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todas las ofertas de la base de datos
        /// incluyendo los datos relacionados de Cliente, Agente e Inmueble
        /// </summary>
        /// <returns>Lista con todas las ofertas con sus relaciones</returns>
        public List<Ofertas> GetAll()
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .ToList();
        }

        /// <summary>
        /// Buscar una oferta por su identificador
        /// incluyendo los datos relacionados
        /// </summary>
        /// <param name="idOferta">Identificador de la oferta</param>
        /// <returns>Oferta encontrada o null si no existe</returns>
        public Ofertas GetById(int idOferta)
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .FirstOrDefault(o => o.IdOferta == idOferta);
        }

        /// <summary>
        /// Obtener todas las ofertas de un cliente especifico
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Lista de ofertas del cliente</returns>
        public List<Ofertas> GetPorCliente(int idCliente)
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdCliente == idCliente)
                .ToList();
        }

        /// <summary>
        /// Obtener todas las ofertas de un agente especifico
        /// </summary>
        /// <param name="idAgente">Identificador del agente</param>
        /// <returns>Lista de ofertas del agente</returns>
        public List<Ofertas> GetPorAgente(int idAgente)
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdAgente == idAgente)
                .ToList();
        }

        /// <summary>
        /// Obtener todas las ofertas de un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">Identificador del inmueble</param>
        /// <returns>Lista de ofertas del inmueble</returns>
        public List<Ofertas> GetPorInmueble(int idInmueble)
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdInmueble == idInmueble)
                .ToList();
        }

        /// <summary>
        /// Buscar ofertas por estado (Pendiente/Aceptada/Rechazada)
        /// </summary>
        /// <param name="estado">Estado de la oferta</param>
        /// <returns>Lista de ofertas con ese estado</returns>
        public List<Ofertas> GetPorEstado(string estado)
        {
            return Context.Ofertas
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.Estado == estado)
                .ToList();
        }

        /// <summary>
        /// Guardar una oferta (insertar o actualizar)
        /// Si el IdOferta es 0, se inserta un nuevo registro
        /// Si el IdOferta es mayor que 0, se actualiza el registro existente
        /// </summary>
        /// <param name="oferta">Objeto oferta a guardar</param>
        /// <exception cref="Exception">Error al guardar en la base de datos</exception>
        public void Save(Ofertas oferta)
        {
            try
            {
                if (oferta.IdOferta < 1)
                {
                    // INSERTAR nueva oferta
                    Context.Ofertas.Add(new Ofertas
                    {
                        IdCliente = oferta.IdCliente,
                        IdAgente = oferta.IdAgente,
                        IdInmueble = oferta.IdInmueble,
                        PrecioOfertado = oferta.PrecioOfertado,
                        Estado = oferta.Estado,
                        FechaOferta = oferta.FechaOferta,
                        Observaciones = oferta.Observaciones
                    });
                }
                else
                {
                    // ACTUALIZAR oferta existente
                    var o = Context.Ofertas.FirstOrDefault(x => x.IdOferta == oferta.IdOferta);

                    if (o != null)
                    {
                        o.IdCliente = oferta.IdCliente;
                        o.IdAgente = oferta.IdAgente;
                        o.IdInmueble = oferta.IdInmueble;
                        o.PrecioOfertado = oferta.PrecioOfertado;
                        o.Estado = oferta.Estado;
                        o.FechaOferta = oferta.FechaOferta;
                        o.Observaciones = oferta.Observaciones;
                    }
                }

                // Guardar cambios en la base de datos
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar oferta en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Eliminar una oferta de la base de datos
        /// </summary>
        /// <param name="oferta">Objeto oferta a eliminar</param>
        /// <exception cref="Exception">Error al eliminar de la base de datos</exception>
        public void Delete(Ofertas oferta)
        {
            try
            {
                // Obtenemos la oferta del mismo contexto para evitar problemas
                var o = Context.Ofertas.FirstOrDefault(x => x.IdOferta == oferta.IdOferta);

                if (o != null)
                {
                    Context.Ofertas.Remove(o);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar oferta de la base de datos.", ex);
            }
        }

        /// <summary>
        /// Obtener el total de ofertas registradas
        /// </summary>
        /// <returns>Numero total de ofertas</returns>
        public int GetTotal()
        {
            return Context.Ofertas.Count();
        }

        /// <summary>
        /// Verificar si existen ofertas para un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorCliente(int idCliente)
        {
            return Context.Ofertas.Any(o => o.IdCliente == idCliente);
        }

        /// <summary>
        /// Verificar si existen ofertas para un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorAgente(int idAgente)
        {
            return Context.Ofertas.Any(o => o.IdAgente == idAgente);
        }

        /// <summary>
        /// Verificar si existen ofertas para un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorInmueble(int idInmueble)
        {
            return Context.Ofertas.Any(o => o.IdInmueble == idInmueble);
        }
    }
}
