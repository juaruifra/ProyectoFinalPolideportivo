using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Operaciones en la base de datos
    /// </summary>
    public class OperacionesRepository : RepositoryBase
    {
        /// <summary>
        /// Obtener todas las operaciones de la base de datos
        /// incluyendo los datos relacionados de Cliente, Agente e Inmueble
        /// </summary>
        /// <returns>Lista con todas las operaciones con sus relaciones</returns>
        public List<Operaciones> GetAll()
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .ToList();
        }

        /// <summary>
        /// Buscar una operación por su identificador
        /// incluyendo los datos relacionados
        /// </summary>
        /// <param name="idOperacion">Identificador de la operación</param>
        /// <returns>Operación encontrada o null si no existe</returns>
        public Operaciones GetById(int idOperacion)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .FirstOrDefault(o => o.IdOperacion == idOperacion);
        }

        /// <summary>
        /// Obtener todas las operaciones de un cliente específico
        /// </summary>
        /// <param name="idCliente">Identificador del cliente</param>
        /// <returns>Lista de operaciones del cliente</returns>
        public List<Operaciones> GetPorCliente(int idCliente)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdCliente == idCliente)
                .ToList();
        }

        /// <summary>
        /// Obtener todas las operaciones de un agente específico
        /// </summary>
        /// <param name="idAgente">Identificador del agente</param>
        /// <returns>Lista de operaciones del agente</returns>
        public List<Operaciones> GetPorAgente(int idAgente)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdAgente == idAgente)
                .ToList();
        }

        /// <summary>
        /// Obtener todas las operaciones de un inmueble específico
        /// </summary>
        /// <param name="idInmueble">Identificador del inmueble</param>
        /// <returns>Lista de operaciones del inmueble</returns>
        public List<Operaciones> GetPorInmueble(int idInmueble)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.IdInmueble == idInmueble)
                .ToList();
        }

        /// <summary>
        /// Buscar operaciones por tipo (Venta/Alquiler)
        /// </summary>
        /// <param name="tipoOperacion">Tipo de operación</param>
        /// <returns>Lista de operaciones de ese tipo</returns>
        public List<Operaciones> GetPorTipo(string tipoOperacion)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.TipoOperacion == tipoOperacion)
                .ToList();
        }

        /// <summary>
        /// Buscar operaciones por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial del rango</param>
        /// <param name="fechaFin">Fecha final del rango</param>
        /// <returns>Lista de operaciones en ese rango de fechas</returns>
        public List<Operaciones> GetPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .Where(o => o.FechaOperacion >= fechaInicio && o.FechaOperacion <= fechaFin)
                .ToList();
        }

        /// <summary>
        /// Guardar una operación (insertar o actualizar)
        /// Si el IdOperacion es 0, se inserta un nuevo registro
        /// Si el IdOperacion es mayor que 0, se actualiza el registro existente
        /// </summary>
        /// <param name="operacion">Objeto operación a guardar</param>
        /// <exception cref="Exception">Error al guardar en la base de datos</exception>
        public void Save(Operaciones operacion)
        {
            try
            {
                if (operacion.IdOperacion < 1)
                {
                    // INSERTAR nueva operación
                    Context.Operaciones.Add(new Operaciones
                    {
                        IdCliente = operacion.IdCliente,
                        IdAgente = operacion.IdAgente,
                        IdInmueble = operacion.IdInmueble,
                        TipoOperacion = operacion.TipoOperacion,
                        PrecioFinal = operacion.PrecioFinal,
                        FechaOperacion = operacion.FechaOperacion, // Fecha editable
                        Observaciones = operacion.Observaciones,
                        IdOferta = operacion.IdOferta
                    });
                }
                else
                {
                    // ACTUALIZAR operación existente
                    var o = Context.Operaciones.FirstOrDefault(x => x.IdOperacion == operacion.IdOperacion);

                    if (o != null)
                    {
                        o.IdCliente = operacion.IdCliente;
                        o.IdAgente = operacion.IdAgente;
                        o.IdInmueble = operacion.IdInmueble;
                        o.TipoOperacion = operacion.TipoOperacion;
                        o.PrecioFinal = operacion.PrecioFinal;
                        o.FechaOperacion = operacion.FechaOperacion; // Se puede actualizar la fecha
                        o.Observaciones = operacion.Observaciones;
                    }
                }

                // Guardar cambios en la base de datos
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar operación en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Eliminar una operación de la base de datos
        /// </summary>
        /// <param name="operacion">Objeto operación a eliminar</param>
        /// <exception cref="Exception">Error al eliminar de la base de datos</exception>
        public void Delete(Operaciones operacion)
        {
            try
            {
                // Obtenemos la operación del mismo contexto para evitar problemas
                var o = Context.Operaciones.FirstOrDefault(x => x.IdOperacion == operacion.IdOperacion);

                if (o != null)
                {
                    Context.Operaciones.Remove(o);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar operación de la base de datos.", ex);
            }
        }

        /// <summary>
        /// Obtener el total de operaciones registradas
        /// </summary>
        /// <returns>Número total de operaciones</returns>
        public int GetTotal()
        {
            return Context.Operaciones.Count();
        }

        /// <summary>
        /// Calcular el total de ingresos de todas las operaciones
        /// </summary>
        /// <returns>Suma total de todos los precios finales</returns>
        public decimal GetTotalIngresos()
        {
            return Context.Operaciones.Sum(o => o.PrecioFinal);
        }

        /// <summary>
        /// Calcular el total de ingresos por tipo de operación
        /// </summary>
        /// <param name="tipoOperacion">Tipo de operación (Venta/Alquiler)</param>
        /// <returns>Suma de precios finales de ese tipo</returns>
        public decimal GetIngresosPorTipo(string tipoOperacion)
        {
            return Context.Operaciones
                .Where(o => o.TipoOperacion == tipoOperacion)
                .Sum(o => o.PrecioFinal);
        }

        /// <summary>
        /// Obtener las operaciones más recientes
        /// </summary>
        /// <param name="cantidad">Cantidad de operaciones a devolver</param>
        /// <returns>Lista de operaciones ordenadas por fecha descendente</returns>
        public List<Operaciones> GetMasRecientes(int cantidad = 10)
        {
            return Context.Operaciones
                .Include(o => o.Clientes)
                .Include(o => o.Agentes)
                .Include(o => o.Inmuebles)
                .OrderByDescending(o => o.FechaOperacion)
                .Take(cantidad)
                .ToList();
        }

        /// <summary>
        /// Comprobar si un inmueble ya tiene una operación registrada
        /// para evitar duplicar ventas o alquileres
        /// </summary>
        /// <param name="idInmueble">Identificador del inmueble</param>
        /// <param name="idOperacionExcluir">ID de operación a excluir (para ediciones)</param>
        /// <returns>True si el inmueble ya tiene operación, False si está libre</returns>
        public bool InmuebleYaTieneOperacion(int idInmueble, int? idOperacionExcluir = null)
        {
            var query = Context.Operaciones.Where(o => o.IdInmueble == idInmueble);

            // Si estamos editando, excluimos la propia operación
            if (idOperacionExcluir.HasValue)
            {
                query = query.Where(o => o.IdOperacion != idOperacionExcluir.Value);
            }

            return query.Any();
        }

        /// <summary>
        /// Obtener estadísticas de operaciones por mes
        /// </summary>
        /// <param name="año">Año para las estadísticas</param>
        /// <returns>Diccionario con el mes y el número de operaciones</returns>
        public Dictionary<int, int> GetEstadisticasPorMes(int año)
        {
            return Context.Operaciones
                .Where(o => o.FechaOperacion.Year == año)
                .GroupBy(o => o.FechaOperacion.Month)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        /// <summary>
        /// Verificar si existen operaciones para un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorCliente(int idCliente)
        {
            return Context.Operaciones.Any(o => o.IdCliente == idCliente);
        }

        /// <summary>
        /// Verificar si existen operaciones para un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorAgente(int idAgente)
        {
            return Context.Operaciones.Any(o => o.IdAgente == idAgente);
        }

        /// <summary>
        /// Verificar si existen operaciones para un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorInmueble(int idInmueble)
        {
            return Context.Operaciones.Any(o => o.IdInmueble == idInmueble);
        }
    }
}
