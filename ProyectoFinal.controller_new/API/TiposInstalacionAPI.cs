using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API para consultar tipos de instalación.
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
        /// Obtiene todos los tipos.
        /// </summary>
        /// <returns>Lista de tipos.</returns>
        public List<TiposInstalacion> ObtenerTodos()
        {
            try
            {
                // Ordenado por nombre en repositorio.
                return _repo.GetAll().ToList();
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al obtener los tipos de instalación", ex);
            }
        }
    }
}
