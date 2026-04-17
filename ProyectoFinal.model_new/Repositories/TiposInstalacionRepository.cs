using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para consultar el catálogo de tipos de instalación.
    /// </summary>
    public class TiposInstalacionRepository : RepositoryBase
    {
        /// <summary>
        /// Obtiene todos los tipos de instalación.
        /// </summary>
        /// <returns>Lista de tipos.</returns>
        public List<TiposInstalacion> GetAll()
        {
            // Devolvemos ordenado alfabéticamente.
            return Context.TiposInstalacion
                .OrderBy(t => t.Nombre)
                .ToList();
        }

        /// <summary>
        /// Obtiene un tipo por id.
        /// </summary>
        /// <param name="idTipo">Id del tipo.</param>
        /// <returns>Tipo o null.</returns>
        public TiposInstalacion GetById(int idTipo)
        {
            // Buscamos por PK.
            return Context.TiposInstalacion.FirstOrDefault(t => t.TipoInstalacionId == idTipo);
        }
    }
}
