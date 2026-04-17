using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para gestionar operaciones CRUD de instalaciones.
    /// </summary>
    public class InstalacionesRepository : RepositoryBase
    {
        /// <summary>
        /// Obtiene todas las instalaciones.
        /// </summary>
        /// <param name="soloDisponibles">Si es true, devuelve solo las disponibles.</param>
        /// <returns>Lista de instalaciones.</returns>
        public List<Instalaciones> GetAll(bool soloDisponibles = false)
        {
            // Construimos la consulta incluyendo el tipo para mostrarlo en el grid.
            var query = Context.Instalaciones
                .Include(i => i.TiposInstalacion)
                .AsQueryable();

            // Filtramos si se solicita.
            if (soloDisponibles)
                query = query.Where(i => i.Disponible);

            // Ejecutamos la consulta.
            return query.ToList();
        }

        /// <summary>
        /// Obtiene una instalación por su id.
        /// </summary>
        /// <param name="idInstalacion">Id de la instalación.</param>
        /// <returns>Instalación o null.</returns>
        public Instalaciones GetById(int idInstalacion)
        {
            // Buscamos por PK.
            return Context.Instalaciones
                .Include(i => i.TiposInstalacion)
                .FirstOrDefault(i => i.InstalacionId == idInstalacion);
        }

        /// <summary>
        /// Guarda una instalación (alta o modificación).
        /// </summary>
        /// <param name="instalacion">Instalación a guardar.</param>
        public void Save(Instalaciones instalacion)
        {
            try
            {
                // Alta.
                if (instalacion.InstalacionId < 1)
                {
                    // Insertamos una nueva entidad.
                    Context.Instalaciones.Add(new Instalaciones
                    {
                        // Asignamos los campos.
                        TipoInstalacionId = instalacion.TipoInstalacionId,
                        Nombre = instalacion.Nombre,
                        PrecioHora = instalacion.PrecioHora,
                        Disponible = instalacion.Disponible,
                        Observaciones = instalacion.Observaciones
                    });
                }
                else
                {
                    // Modificación.
                    var i = Context.Instalaciones.FirstOrDefault(x => x.InstalacionId == instalacion.InstalacionId);

                    // Si existe, actualizamos campos.
                    if (i != null)
                    {
                        i.TipoInstalacionId = instalacion.TipoInstalacionId;
                        i.Nombre = instalacion.Nombre;
                        i.PrecioHora = instalacion.PrecioHora;
                        i.Disponible = instalacion.Disponible;
                        i.Observaciones = instalacion.Observaciones;
                    }
                }

                // Persistimos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Propagamos una excepción más clara.
                throw new Exception("Error al guardar la instalación en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Cambia el estado de disponibilidad de una instalación.
        /// </summary>
        /// <param name="idInstalacion">Id de instalación.</param>
        /// <param name="disponible">Nuevo estado.</param>
        public void SetDisponible(int idInstalacion, bool disponible)
        {
            try
            {
                // Buscamos la instalación.
                var i = Context.Instalaciones.FirstOrDefault(x => x.InstalacionId == idInstalacion);

                // Si existe, cambiamos el flag.
                if (i != null)
                {
                    i.Disponible = disponible;
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Propagamos.
                throw new Exception("Error al cambiar la disponibilidad de la instalación.", ex);
            }
        }

        /// <summary>
        /// Devuelve el total de instalaciones.
        /// </summary>
        /// <param name="soloDisponibles">Si true, cuenta solo disponibles.</param>
        /// <returns>Total.</returns>
        public int GetTotal(bool soloDisponibles = false)
        {
            // Contamos según filtro.
            return soloDisponibles ? Context.Instalaciones.Count(i => i.Disponible) : Context.Instalaciones.Count();
        }

        /// <summary>
        /// Comprueba si ya existe una instalación con el mismo nombre.
        /// </summary>
        /// <param name="nombre">Nombre a comprobar.</param>
        /// <param name="idExcluir">Id a excluir (en edición).</param>
        /// <returns>True si existe.</returns>
        public bool NombreYaExiste(string nombre, int? idExcluir = null)
        {
            // Normalizamos.
            if (string.IsNullOrWhiteSpace(nombre)) return false;

            // Consulta por nombre exacto.
            var query = Context.Instalaciones.Where(i => i.Nombre == nombre);

            // Excluimos si editamos.
            if (idExcluir.HasValue)
                query = query.Where(i => i.InstalacionId != idExcluir.Value);

            // Existe si hay alguno.
            return query.Any();
        }

        // NOTA: La validacion de "en uso" se centraliza en ReservasRepository.

        /// <summary>
        /// Elimina físicamente una instalación.
        /// IMPORTANTE: solo debe llamarse si no está en uso.
        /// </summary>
        /// <param name="instalacionId">Id de instalación.</param>
        public void Delete(int instalacionId)
        {
            try
            {
                // Buscamos la instalación.
                var i = Context.Instalaciones.FirstOrDefault(x => x.InstalacionId == instalacionId);

                // Si no existe, no hacemos nada.
                if (i == null) return;

                // Eliminamos.
                Context.Instalaciones.Remove(i);

                // Guardamos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos error.
                throw new Exception("Error al borrar la instalación en la base de datos.", ex);
            }
        }
    }
}
