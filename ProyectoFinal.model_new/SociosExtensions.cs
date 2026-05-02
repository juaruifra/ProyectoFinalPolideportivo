namespace ProyectoFinal.model_new
{
    /// <summary>
    /// Extensiones de la entidad Socios generada por Entity Framework.
    /// Se implementan en un fichero parcial separado para no ser sobreescritas al regenerar el modelo.
    /// </summary>
    public partial class Socios
    {
        /// <summary>
        /// Devuelve el nombre completo del socio en formato "Apellidos, Nombre".
        /// Util para mostrar en ComboBox y columnas de DataGrid.
        /// </summary>
        public string NombreCompleto
        {
            get
            {
                // Si no hay nombre, devolvemos solo los apellidos sin coma.
                if (string.IsNullOrWhiteSpace(Nombre))
                    return Apellidos;

                // Formato normal: "Apellidos, Nombre".
                return $"{Apellidos}, {Nombre}";
            }
        }
    }
}
