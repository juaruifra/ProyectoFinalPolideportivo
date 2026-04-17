using ProyectoFinal.model;

namespace ProyectoFinal.model
{
    /// <summary>
    /// Extensión de la clase parcial Agentes para agregar funcionalidad personalizada
    /// </summary>
    public partial class Agentes
    {
        /// <summary>
        /// Override del método ToString para mostrar el nombre del agente en ComboBox
        /// </summary>
        /// <returns>Nombre del agente</returns>
        public override string ToString()
        {
            return Nombre ?? string.Empty;
        }
    }
}
