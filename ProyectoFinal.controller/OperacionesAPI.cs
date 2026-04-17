using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// API para gestionar las operaciones sobre operaciones
    /// Intermediario entre los repositorios y los controladores
    /// Prepara los datos para que la vista los consuma facilmente
    /// </summary>
    public class OperacionesAPI
    {
        // Repositorio de operaciones para acceder a la BBDD
        private OperacionesRepository _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public OperacionesAPI()
        {
            _repo = new OperacionesRepository();
        }

        /// <summary>
        /// Obtener todas las operaciones de la base de datos
        /// </summary>
        /// <returns>Lista de operaciones ordenada por fecha descendente</returns>
        public List<Operaciones> ObtenerTodas()
        {
            try
            {
                return _repo.GetAll().OrderByDescending(o => o.FechaOperacion).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las operaciones", ex);
            }
        }

        /// <summary>
        /// Obtener una operacion por su ID
        /// </summary>
        /// <param name="idOperacion">Identificador de la operacion</param>
        /// <returns>Operacion encontrada o null</returns>
        public Operaciones ObtenerPorId(int idOperacion)
        {
            try
            {
                return _repo.GetById(idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la operacion con ID {idOperacion}", ex);
            }
        }

        /// <summary>
        /// Obtener operaciones de un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>Lista de operaciones del cliente</returns>
        public List<Operaciones> ObtenerPorCliente(int idCliente)
        {
            try
            {
                return _repo.GetPorCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener operaciones del cliente {idCliente}", ex);
            }
        }

        /// <summary>
        /// Obtener operaciones de un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>Lista de operaciones del agente</returns>
        public List<Operaciones> ObtenerPorAgente(int idAgente)
        {
            try
            {
                return _repo.GetPorAgente(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener operaciones del agente {idAgente}", ex);
            }
        }

        /// <summary>
        /// Obtener operaciones de un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>Lista de operaciones del inmueble</returns>
        public List<Operaciones> ObtenerPorInmueble(int idInmueble)
        {
            try
            {
                return _repo.GetPorInmueble(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener operaciones del inmueble {idInmueble}", ex);
            }
        }

        /// <summary>
        /// Obtener operaciones por tipo
        /// </summary>
        /// <param name="tipoOperacion">Tipo de operacion (Venta/Alquiler)</param>
        /// <returns>Lista de operaciones de ese tipo</returns>
        public List<Operaciones> ObtenerPorTipo(string tipoOperacion)
        {
            try
            {
                return _repo.GetPorTipo(tipoOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener operaciones de tipo {tipoOperacion}", ex);
            }
        }

        /// <summary>
        /// Obtener operaciones por rango de fechas
        /// </summary>
        /// <param name="fechaInicio">Fecha inicial</param>
        /// <param name="fechaFin">Fecha final</param>
        /// <returns>Lista de operaciones en ese rango</returns>
        public List<Operaciones> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                return _repo.GetPorRangoFechas(fechaInicio, fechaFin);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operaciones por rango de fechas", ex);
            }
        }

        /// <summary>
        /// Verificar si existen operaciones para un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorCliente(int idCliente)
        {
            try
            {
                return _repo.ExistenPorCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar operaciones del cliente", ex);
            }
        }

        /// <summary>
        /// Verificar si existen operaciones para un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorAgente(int idAgente)
        {
            try
            {
                return _repo.ExistenPorAgente(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar operaciones del agente", ex);
            }
        }

        /// <summary>
        /// Verificar si existen operaciones para un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>True si existen operaciones</returns>
        public bool ExistenPorInmueble(int idInmueble)
        {
            try
            {
                return _repo.ExistenPorInmueble(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar operaciones del inmueble", ex);
            }
        }

        /// <summary>
        /// Guardar una operacion (insertar o actualizar)
        /// </summary>
        /// <param name="operacion">Objeto operacion</param>
        public void Guardar(Operaciones operacion)
        {
            try
            {
                _repo.Save(operacion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la operacion", ex);
            }
        }

        /// <summary>
        /// Eliminar una operacion
        /// </summary>
        /// <param name="operacion">Objeto operacion a eliminar</param>
        public void Eliminar(Operaciones operacion)
        {
            try
            {
                _repo.Delete(operacion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la operacion", ex);
            }
        }

        /// <summary>
        /// Obtener el total de operaciones registradas
        /// </summary>
        /// <returns>Numero total de operaciones</returns>
        public int ObtenerTotal()
        {
            try
            {
                return _repo.GetTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de operaciones", ex);
            }
        }

        /// <summary>
        /// Obtener el total de ingresos
        /// </summary>
        /// <returns>Suma total de precios finales</returns>
        public decimal ObtenerTotalIngresos()
        {
            try
            {
                return _repo.GetTotalIngresos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de ingresos", ex);
            }
        }

        /// <summary>
        /// Obtener ingresos por tipo de operacion
        /// </summary>
        /// <param name="tipoOperacion">Tipo de operacion (Venta/Alquiler)</param>
        /// <returns>Suma de precios finales de ese tipo</returns>
        public decimal ObtenerIngresosPorTipo(string tipoOperacion)
        {
            try
            {
                return _repo.GetIngresosPorTipo(tipoOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener ingresos por tipo", ex);
            }
        }

        /// <summary>
        /// Obtener las operaciones mas recientes
        /// </summary>
        /// <param name="cantidad">Cantidad de operaciones</param>
        /// <returns>Lista de operaciones recientes</returns>
        public List<Operaciones> ObtenerMasRecientes(int cantidad = 10)
        {
            try
            {
                return _repo.GetMasRecientes(cantidad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener operaciones recientes", ex);
            }
        }

        /// <summary>
        /// Verificar si un inmueble ya tiene operacion
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <param name="idOperacionExcluir">ID de operacion a excluir (para ediciones)</param>
        /// <returns>True si ya tiene operacion</returns>
        public bool InmuebleYaTieneOperacion(int idInmueble, int? idOperacionExcluir = null)
        {
            try
            {
                return _repo.InmuebleYaTieneOperacion(idInmueble, idOperacionExcluir);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar operacion del inmueble", ex);
            }
        }

        /// <summary>
        /// Obtener estadisticas por mes
        /// </summary>
        /// <param name="año">Año para las estadisticas</param>
        /// <returns>Diccionario con mes y cantidad de operaciones</returns>
        public Dictionary<int, int> ObtenerEstadisticasPorMes(int año)
        {
            try
            {
                return _repo.GetEstadisticasPorMes(año);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener estadisticas por mes", ex);
            }
        }
    }
}
