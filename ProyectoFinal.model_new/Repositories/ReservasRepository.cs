using ProyectoFinal.model_new;
using System;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio de apoyo para consultas relacionadas con reservas.
    /// Se usa para validaciones de borrado (no permitir borrar entidades en uso).
    /// </summary>
    public class ReservasRepository : RepositoryBase
    {
        /// <summary>
        /// Indica si existe alguna reserva asociada a un socio.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si hay reservas.</returns>
        public bool ExisteReservaParaSocio(int socioId)
        {
            // Buscamos cualquier reserva asociada.
            return Context.Reservas.Any(r => r.SocioId == socioId);
        }

        /// <summary>
        /// Indica si existe alguna reserva asociada a una instalación.
        /// </summary>
        /// <param name="instalacionId">Id de la instalación.</param>
        /// <returns>True si hay reservas.</returns>
        public bool ExisteReservaParaInstalacion(int instalacionId)
        {
            // Buscamos cualquier reserva asociada.
            return Context.Reservas.Any(r => r.InstalacionId == instalacionId);
        }
    }
}
