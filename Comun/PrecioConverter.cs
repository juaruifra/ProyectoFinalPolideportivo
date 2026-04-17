using System;
using System.Globalization;
using System.Windows.Data;

namespace Comun
{
    /// <summary>
    /// Convertidor para formatear precios en DataGrid
    /// Utiliza el método FormatearPrecio de Utils
    /// </summary>
    public class PrecioConverter : IValueConverter
    {
        /// <summary>
        /// Convierte el precio a formato con punto como separador
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si el valor no es válido, devolvemos vacío
            if (value == null || !(value is decimal))
                return string.Empty;

            // Usamos el método de Utils para formatear (sin decimales por defecto)
            return Utils.FormatearPrecio((decimal)value,true);
        }

        /// <summary>
        /// No necesitamos convertir de vuelta
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}