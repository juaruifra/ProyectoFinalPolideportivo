using ProyectoFinal.model_new;

namespace ProyectoFinal.model_new
{
    /// <summary>
    /// Clase base para los repositorios del escenario Club Polideportivo.
    /// </summary>
    public abstract class RepositoryBase
    {
        // Contexto EF compartido por repositorios.
        protected ClubPolideportivoDBEntities Context;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected RepositoryBase()
        {
            // Inicializamos el contexto.
            Context = new ClubPolideportivoDBEntities();
        }
    }
}
