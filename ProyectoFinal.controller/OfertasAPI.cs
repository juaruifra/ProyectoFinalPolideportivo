using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// API para gestionar las operaciones sobre ofertas
    /// Intermediario entre los repositorios y los controladores
    /// Prepara los datos para que la vista los consuma facilmente
    /// </summary>
    public class OfertasAPI
    {
        // Repositorio de ofertas para acceder a la BBDD
        private OfertasRepository _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public OfertasAPI()
        {
            _repo = new OfertasRepository();
        }

        /// <summary>
        /// Obtener todas las ofertas de la base de datos
        /// </summary>
        /// <returns>Lista de ofertas ordenada por fecha descendente</returns>
        public List<Ofertas> ObtenerTodas()
        {
            try
            {
                return _repo.GetAll().OrderByDescending(o => o.FechaOferta).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ofertas", ex);
            }
        }

        /// <summary>
        /// Obtener una oferta por su ID
        /// </summary>
        /// <param name="idOferta">Identificador de la oferta</param>
        /// <returns>Oferta encontrada o null</returns>
        public Ofertas ObtenerPorId(int idOferta)
        {
            try
            {
                return _repo.GetById(idOferta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la oferta con ID {idOferta}", ex);
            }
        }

        /// <summary>
        /// Obtener ofertas de un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>Lista de ofertas del cliente</returns>
        public List<Ofertas> ObtenerPorCliente(int idCliente)
        {
            try
            {
                return _repo.GetPorCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ofertas del cliente {idCliente}", ex);
            }
        }

        /// <summary>
        /// Obtener ofertas de un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>Lista de ofertas del agente</returns>
        public List<Ofertas> ObtenerPorAgente(int idAgente)
        {
            try
            {
                return _repo.GetPorAgente(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ofertas del agente {idAgente}", ex);
            }
        }

        /// <summary>
        /// Obtener ofertas de un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>Lista de ofertas del inmueble</returns>
        public List<Ofertas> ObtenerPorInmueble(int idInmueble)
        {
            try
            {
                return _repo.GetPorInmueble(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ofertas del inmueble {idInmueble}", ex);
            }
        }

        /// <summary>
        /// Obtener ofertas por estado
        /// </summary>
        /// <param name="estado">Estado de la oferta (Pendiente/Aceptada/Rechazada)</param>
        /// <returns>Lista de ofertas con ese estado</returns>
        public List<Ofertas> ObtenerPorEstado(string estado)
        {
            try
            {
                return _repo.GetPorEstado(estado);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ofertas con estado {estado}", ex);
            }
        }

        /// <summary>
        /// Verificar si existen ofertas para un cliente especifico
        /// </summary>
        /// <param name="idCliente">ID del cliente</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorCliente(int idCliente)
        {
            try
            {
                return _repo.ExistenPorCliente(idCliente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar ofertas del cliente", ex);
            }
        }

        /// <summary>
        /// Verificar si existen ofertas para un agente especifico
        /// </summary>
        /// <param name="idAgente">ID del agente</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorAgente(int idAgente)
        {
            try
            {
                return _repo.ExistenPorAgente(idAgente);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar ofertas del agente", ex);
            }
        }

        /// <summary>
        /// Verificar si existen ofertas para un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>True si existen ofertas</returns>
        public bool ExistenPorInmueble(int idInmueble)
        {
            try
            {
                return _repo.ExistenPorInmueble(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar ofertas del inmueble", ex);
            }
        }

        /// <summary>
        /// Guardar una oferta (insertar o actualizar)
        /// </summary>
        /// <param name="oferta">Objeto oferta</param>
        public void Guardar(Ofertas oferta)
        {
            try
            {
                _repo.Save(oferta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar la oferta", ex);
            }
        }

        /// <summary>
        /// Eliminar una oferta
        /// </summary>
        /// <param name="oferta">Objeto oferta a eliminar</param>
        public void Eliminar(Ofertas oferta)
        {
            try
            {
                _repo.Delete(oferta);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la oferta", ex);
            }
        }

        /// <summary>
        /// Obtener el total de ofertas registradas
        /// </summary>
        /// <returns>Numero total de ofertas</returns>
        public int ObtenerTotal()
        {
            try
            {
                return _repo.GetTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de ofertas", ex);
            }
        }
    }
}
