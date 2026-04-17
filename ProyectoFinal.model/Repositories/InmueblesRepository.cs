using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Inmuebles en la base de datos
    /// </summary>
    public class InmueblesRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todos los inmuebles de la base de datos
        /// </summary>
        /// <returns>Lista con todos los inmuebles</returns>
        public List<Inmuebles> GetAll()
        {
            return Context.Inmuebles.ToList();
        }

        /// <summary>
        /// Buscar un inmueble por su identificador
        /// </summary>
        /// <param name="idInmueble">Identificador del inmueble</param>
        /// <returns>Inmueble encontrado o null si no existe</returns>
        public Inmuebles GetById(int idInmueble)
        {
            return Context.Inmuebles.FirstOrDefault(i => i.IdInmueble == idInmueble);
        }

        /// <summary>
        /// Buscar inmuebles por ciudad
        /// </summary>
        /// <param name="ciudad">Nombre de la ciudad</param>
        /// <returns>Lista de inmuebles en esa ciudad</returns>
        public List<Inmuebles> BuscarPorCiudad(string ciudad)
        {
            return Context.Inmuebles
                .Where(i => i.Ciudad.Contains(ciudad))
                .ToList();
        }

        /// <summary>
        /// Buscar inmuebles por tipo de operación (Venta/Alquiler)
        /// </summary>
        /// <param name="tipoOperacion">Tipo de operación (Venta o Alquiler)</param>
        /// <returns>Lista de inmuebles con ese tipo de operación</returns>
        public List<Inmuebles> BuscarPorTipoOperacion(string tipoOperacion)
        {
            return Context.Inmuebles
                .Where(i => i.TipoOperacion == tipoOperacion)
                .ToList();
        }

        /// <summary>
        /// Buscar inmuebles por estado (Disponible/Vendido/Alquilado)
        /// </summary>
        /// <param name="estado">Estado del inmueble</param>
        /// <returns>Lista de inmuebles con ese estado</returns>
        public List<Inmuebles> BuscarPorEstado(string estado)
        {
            return Context.Inmuebles
                .Where(i => i.Estado == estado)
                .ToList();
        }

        /// <summary>
        /// Buscar inmuebles por rango de precio
        /// </summary>
        /// <param name="precioMin">Precio mínimo</param>
        /// <param name="precioMax">Precio máximo</param>
        /// <returns>Lista de inmuebles dentro del rango de precio</returns>
        public List<Inmuebles> BuscarPorRangoPrecio(decimal precioMin, decimal precioMax)
        {
            return Context.Inmuebles
                .Where(i => i.Precio >= precioMin && i.Precio <= precioMax)
                .ToList();
        }

        /// <summary>
        /// Búsqueda avanzada de inmuebles con múltiples filtros
        /// </summary>
        /// <param name="ciudad">Ciudad (opcional)</param>
        /// <param name="tipoOperacion">Tipo de operación (opcional)</param>
        /// <param name="estado">Estado (opcional)</param>
        /// <param name="precioMax">Precio máximo (opcional)</param>
        /// <returns>Lista de inmuebles que cumplen los criterios</returns>
        public List<Inmuebles> BusquedaAvanzada(string ciudad = null, string tipoOperacion = null, 
            string estado = null, decimal? precioMax = null)
        {
            var query = Context.Inmuebles.AsQueryable();

            // Filtrar por ciudad si se especifica
            if (!string.IsNullOrEmpty(ciudad))
            {
                query = query.Where(i => i.Ciudad.Contains(ciudad));
            }

            // Filtrar por tipo de operación si se especifica
            if (!string.IsNullOrEmpty(tipoOperacion))
            {
                query = query.Where(i => i.TipoOperacion == tipoOperacion);
            }

            // Filtrar por estado si se especifica
            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(i => i.Estado == estado);
            }

            // Filtrar por precio máximo si se especifica
            if (precioMax.HasValue)
            {
                query = query.Where(i => i.Precio <= precioMax.Value);
            }

            return query.ToList();
        }

        /// <summary>
        /// Guardar un inmueble (insertar o actualizar)
        /// Si el IdInmueble es 0, se inserta un nuevo registro
        /// Si el IdInmueble es mayor que 0, se actualiza el registro existente
        /// </summary>
        /// <param name="inmueble">Objeto inmueble a guardar</param>
        /// <exception cref="Exception">Error al guardar en la base de datos</exception>
        public void Save(Inmuebles inmueble)
        {
            try
            {
                if (inmueble.IdInmueble < 1)
                {
                    // INSERTAR nuevo inmueble
                    Context.Inmuebles.Add(new Inmuebles
                    {
                        Direccion = inmueble.Direccion,
                        Ciudad = inmueble.Ciudad,
                        Precio = inmueble.Precio,
                        TipoOperacion = inmueble.TipoOperacion,
                        Estado = inmueble.Estado,
                        FechaAlta = DateTime.Now // Asignar fecha actual al insertar
                    });
                }
                else
                {
                    // ACTUALIZAR inmueble existente
                    var i = Context.Inmuebles.FirstOrDefault(x => x.IdInmueble == inmueble.IdInmueble);

                    if (i != null)
                    {
                        i.Direccion = inmueble.Direccion;
                        i.Ciudad = inmueble.Ciudad;
                        i.Precio = inmueble.Precio;
                        i.TipoOperacion = inmueble.TipoOperacion;
                        i.Estado = inmueble.Estado;
                        // FechaAlta no se modifica, se mantiene la original
                    }
                }

                // Guardar cambios en la base de datos
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar inmueble en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Eliminar un inmueble de la base de datos
        /// </summary>
        /// <param name="inmueble">Objeto inmueble a eliminar</param>
        /// <exception cref="Exception">Error al eliminar de la base de datos</exception>
        public void Delete(Inmuebles inmueble)
        {
            try
            {
                // Obtenemos el inmueble del mismo contexto para evitar problemas
                var i = Context.Inmuebles.FirstOrDefault(x => x.IdInmueble == inmueble.IdInmueble);

                if (i != null)
                {
                    Context.Inmuebles.Remove(i);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar inmueble de la base de datos.", ex);
            }
        }

        /// <summary>
        /// Obtener el total de inmuebles registrados
        /// </summary>
        /// <returns>Número total de inmuebles</returns>
        public int GetTotal()
        {
            return Context.Inmuebles.Count();
        }

        /// <summary>
        /// Obtener el total de inmuebles disponibles
        /// </summary>
        /// <returns>Número de inmuebles con estado "Disponible"</returns>
        public int GetTotalDisponibles()
        {
            return Context.Inmuebles.Count(i => i.Estado == "Disponible");
        }

        /// <summary>
        /// Obtener los inmuebles más recientes
        /// </summary>
        /// <param name="cantidad">Cantidad de inmuebles a devolver</param>
        /// <returns>Lista de inmuebles ordenados por fecha de alta descendente</returns>
        public List<Inmuebles> GetMasRecientes(int cantidad = 10)
        {
            return Context.Inmuebles
                .OrderByDescending(i => i.FechaAlta)
                .Take(cantidad)
                .ToList();
        }
    }
}
