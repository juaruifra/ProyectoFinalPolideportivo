using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API para gestionar tipos de instalacion.
    /// </summary>
    public class TiposInstalacionAPI
    {
        private readonly TiposInstalacionRepository _repo;

        /// <summary>
        /// Constructor.
        /// </summary>
        public TiposInstalacionAPI()
        {
            // Inicializamos el repositorio.
            _repo = new TiposInstalacionRepository();
        }

        /// <summary>
        /// Obtiene todos los tipos de instalacion.
        /// </summary>
        /// <returns>Lista de tipos.</returns>
        public List<TiposInstalacion> ObtenerTodos()
        {
            try
            {
                // Delegamos al repositorio.
                return _repo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al obtener los tipos de instalacion", ex);
            }
        }

        /// <summary>
        /// Comprueba si el nombre ya existe para otro tipo.
        /// </summary>
        /// <param name="nombre">Nombre a comprobar.</param>
        /// <param name="excluirId">Id a excluir (edicion).</param>
        /// <returns>True si duplicado.</returns>
        public bool NombreYaExiste(string nombre, int? excluirId = null)
        {
            // Delegamos al repositorio.
            return _repo.NombreYaExiste(nombre, excluirId);
        }

        /// <summary>
        /// Comprueba si el tipo esta siendo usado por alguna instalacion.
        /// </summary>
        /// <param name="idTipo">Id del tipo.</param>
        /// <returns>True si esta en uso.</returns>
        public bool EstaEnUso(int idTipo)
        {
            // Delegamos al repositorio.
            return _repo.EstaEnUso(idTipo);
        }

        /// <summary>
        /// Guarda o actualiza un tipo de instalacion.
        /// </summary>
        /// <param name="tipo">Tipo a guardar.</param>
        public void Guardar(TiposInstalacion tipo)
        {
            try
            {
                // Delegamos al repositorio.
                _repo.Save(tipo);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al guardar el tipo de instalacion", ex);
            }
        }

        /// <summary>
        /// Borra un tipo de instalacion por su id.
        /// </summary>
        /// <param name="idTipo">Id del tipo a borrar.</param>
        public void Borrar(int idTipo)
        {
            try
            {
                // Delegamos al repositorio.
                _repo.Delete(idTipo);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al borrar el tipo de instalacion", ex);
            }
        }
    }
}
