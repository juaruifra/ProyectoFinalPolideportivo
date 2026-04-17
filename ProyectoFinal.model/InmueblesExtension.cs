using ProyectoFinal.model;

namespace ProyectoFinal.model
{
    public partial class Inmuebles
    {
        // Propiedad calculada para mostrar en ComboBox
        public string DireccionCompleta
        {
            get
            {
                if (string.IsNullOrEmpty(Direccion))
                    return string.Empty;
                    
                if (string.IsNullOrEmpty(Ciudad))
                    return Direccion;
                    
                return $"{Direccion} ({Ciudad})";
            }
        }

        /// <summary>
        /// Override del método ToString para mostrar la dirección completa en ComboBox
        /// </summary>
        /// <returns>Dirección completa del inmueble</returns>
        public override string ToString()
        {
            return DireccionCompleta;
        }
    }
}
