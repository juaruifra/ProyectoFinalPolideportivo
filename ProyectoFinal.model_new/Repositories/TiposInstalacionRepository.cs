using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para el catalogo de tipos de instalacion.
    /// </summary>
    public class TiposInstalacionRepository : RepositoryBase
    {
        /// <summary>
        /// Obtiene todos los tipos de instalacion ordenados por nombre.
        /// </summary>
        /// <returns>Lista de tipos.</returns>
        public List<TiposInstalacion> GetAll()
        {
            // Devolvemos ordenado alfabeticamente.
            return Context.TiposInstalacion
                .OrderBy(t => t.Nombre)
                .ToList();
        }

        /// <summary>
        /// Obtiene un tipo por su identificador.
        /// </summary>
        /// <param name="idTipo">Id del tipo.</param>
        /// <returns>Tipo o null.</returns>
        public TiposInstalacion GetById(int idTipo)
        {
            // Buscamos por clave primaria.
            return Context.TiposInstalacion.FirstOrDefault(t => t.TipoInstalacionId == idTipo);
        }

        /// <summary>
        /// Comprueba si ya existe un tipo con el mismo nombre, excluyendo opcionalmente un id.
        /// </summary>
        /// <param name="nombre">Nombre a comprobar.</param>
        /// <param name="excluirId">Id a excluir en la busqueda (para edicion).</param>
        /// <returns>True si el nombre ya existe.</returns>
        public bool NombreYaExiste(string nombre, int? excluirId = null)
        {
            // Consultamos si existe algun tipo con ese nombre distinto al id indicado.
            return Context.TiposInstalacion.Any(t =>
                t.Nombre.ToLower() == nombre.ToLower() &&
                (excluirId == null || t.TipoInstalacionId != excluirId.Value));
        }

        /// <summary>
        /// Comprueba si el tipo esta siendo usado por alguna instalacion.
        /// </summary>
        /// <param name="idTipo">Id del tipo.</param>
        /// <returns>True si esta en uso.</returns>
        public bool EstaEnUso(int idTipo)
        {
            // Buscamos si alguna instalacion referencia este tipo.
            return Context.Instalaciones.Any(i => i.TipoInstalacionId == idTipo);
        }

        /// <summary>
        /// Guarda o actualiza un tipo de instalacion.
        /// </summary>
        /// <param name="tipo">Tipo a guardar.</param>
        public void Save(TiposInstalacion tipo)
        {
            // Si el id es 0, es nuevo y lo agregamos.
            if (tipo.TipoInstalacionId == 0)
            {
                Context.TiposInstalacion.Add(tipo); // Insertamos.
            }
            else
            {
                // Buscamos la entidad existente para actualizarla.
                var existente = Context.TiposInstalacion.Find(tipo.TipoInstalacionId);

                if (existente != null)
                {
                    existente.Nombre = tipo.Nombre; // Actualizamos nombre.
                    existente.Descripcion = tipo.Descripcion; // Actualizamos descripcion.
                }
            }

            Context.SaveChanges(); // Persistimos cambios.
        }

        /// <summary>
        /// Borra fisicamente un tipo de instalacion por su id.
        /// </summary>
        /// <param name="idTipo">Id del tipo a borrar.</param>
        public void Delete(int idTipo)
        {
            // Buscamos el tipo.
            var tipo = Context.TiposInstalacion.Find(idTipo);

            // Si existe, lo eliminamos.
            if (tipo != null)
            {
                Context.TiposInstalacion.Remove(tipo); // Marcamos para borrar.
                Context.SaveChanges(); // Persistimos.
            }
        }
    }
}
