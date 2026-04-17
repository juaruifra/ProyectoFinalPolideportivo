using ProyectoFinal.model;

namespace ProyectoFinal.model
{
    /// <summary>
    /// Extensión de la clase parcial Clientes para agregar funcionalidad personalizada
    /// </summary>
    public partial class Clientes
    {
        /// <summary>
        /// Override del método ToString para mostrar el nombre del cliente en ComboBox
        /// </summary>
        /// <returns>Nombre del cliente</returns>
        public override string ToString()
        {
            return Nombre ?? string.Empty;
        }
    }
}
