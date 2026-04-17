using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API de instalaciones.
    /// Intermediario entre repositorios y controladores.
    /// </summary>
    public class InstalacionesAPI
    {
        private readonly InstalacionesRepository _repo;
        private readonly ReservasRepository _reservasRepo;

        /// <summary>
        /// Constructor.
        /// </summary>
        public InstalacionesAPI()
        {
            // Inicializamos el repositorio.
            _repo = new InstalacionesRepository();
            _reservasRepo = new ReservasRepository();
        }

        /// <summary>
        /// Obtiene el listado de instalaciones.
        /// </summary>
        /// <param name="soloDisponibles">Filtra por disponibilidad.</param>
        /// <returns>Lista de instalaciones.</returns>
        public List<Instalaciones> ObtenerTodos(bool soloDisponibles = false)
        {
            try
            {
                // Ordenamos por tipo y nombre.
                return _repo.GetAll(soloDisponibles)
                    .OrderBy(i => i.TiposInstalacion != null ? i.TiposInstalacion.Nombre : string.Empty)
                    .ThenBy(i => i.Nombre)
                    .ToList();
            }
            catch (Exception ex)
            {
                // Envolvemos error.
                throw new Exception("Error al obtener las instalaciones", ex);
            }
        }

        /// <summary>
        /// Obtiene una instalación por id.
        /// </summary>
        /// <param name="idInstalacion">Id.</param>
        /// <returns>Instalación o null.</returns>
        public Instalaciones ObtenerPorId(int idInstalacion)
        {
            try
            {
                // Delegamos.
                return _repo.GetById(idInstalacion);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception($"Error al obtener la instalación con ID {idInstalacion}", ex);
            }
        }

        /// <summary>
        /// Valida si un nombre ya existe.
        /// </summary>
        /// <param name="nombre">Nombre.</param>
        /// <param name="idExcluir">Id a excluir.</param>
        /// <returns>True si existe.</returns>
        public bool NombreYaExiste(string nombre, int? idExcluir = null)
        {
            try
            {
                // Delegamos.
                return _repo.NombreYaExiste(nombre, idExcluir);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al verificar el nombre de la instalación", ex);
            }
        }

        /// <summary>
        /// Guarda una instalación.
        /// </summary>
        /// <param name="instalacion">Instalación.</param>
        public void Guardar(Instalaciones instalacion)
        {
            try
            {
                // Delegamos.
                _repo.Save(instalacion);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al guardar la instalación", ex);
            }
        }

        /// <summary>
        /// Cambia el estado disponible/no disponible.
        /// </summary>
        /// <param name="idInstalacion">Id.</param>
        /// <param name="disponible">Estado.</param>
        public void CambiarDisponible(int idInstalacion, bool disponible)
        {
            try
            {
                // Delegamos.
                _repo.SetDisponible(idInstalacion, disponible);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al cambiar la disponibilidad", ex);
            }
        }

        /// <summary>
        /// Obtiene el total.
        /// </summary>
        /// <param name="soloDisponibles">Filtra disponibles.</param>
        /// <returns>Total.</returns>
        public int ObtenerTotal(bool soloDisponibles = false)
        {
            try
            {
                // Delegamos.
                return _repo.GetTotal(soloDisponibles);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al obtener el total de instalaciones", ex);
            }
        }

        /// <summary>
        /// Indica si la instalación está en uso (tiene reservas asociadas).
        /// </summary>
        /// <param name="instalacionId">Id de instalación.</param>
        /// <returns>True si está en uso.</returns>
        public bool EstaEnUso(int instalacionId)
        {
            try
            {
                // Delegamos en reservas.
                return _reservasRepo.ExisteReservaParaInstalacion(instalacionId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar uso de la instalación", ex);
            }
        }

        /// <summary>
        /// Borra físicamente una instalación.
        /// </summary>
        /// <param name="instalacionId">Id de instalación.</param>
        public void Borrar(int instalacionId)
        {
            try
            {
                // Delegamos.
                _repo.Delete(instalacionId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al borrar la instalación", ex);
            }
        }
    }
}
