using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Comun
{
    public class Utils
    {

        /// <summary>
        /// Función para unificar la gestión de los modales con mensajes
        /// </summary>
        /// <param name="msj">Cuerpo del mensaje</param>
        /// <param name="tit">Titulo de la ventana</param>
        /// <param name="op">Opcion de ventana</param>
        /// <returns>La respuesta en caso de necesitarla</returns>
        public static DialogResult showModal(string msj, string tit, int op = 1)
        {
            DialogResult result = new DialogResult();
            switch (op) {

                case 1: // Mensaje de error
                    result = MessageBox.Show(msj, tit, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case 2: // Mensaje de exito
                    result = MessageBox.Show(msj, tit, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case 3: // Mensaje de confirmacion
                    result = MessageBox.Show(msj, tit, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    break;

            }

            
            return result;
        }


        /// <summary>
        /// Validar formato de email
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si el formato es válido</returns>
        public static bool EsEmailValido(string email)
        {
            try
            {
                // Expresión regular para validar email
                string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, patron, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validar formato de teléfono
        /// </summary>
        /// <param name="telefono">Teléfono a validar</param>
        /// <returns>True si el formato es válido</returns>
        public static bool EsTelefonoValido(string telefono)
        {
            // Eliminar espacios y guiones
            string telefonoLimpio = telefono.Replace(" ", "").Replace("-", "").Replace("+", "");

            // Validar que contenga solo dígitos y tenga entre 9 y 15 caracteres
            return Regex.IsMatch(telefonoLimpio, @"^\d{9,15}$");
        }

        /// <summary>
        /// Formatear precio con punto como separador de miles
        /// Ejemplos: 
        /// - 250000 -> "250.000" (sin decimales)
        /// - 250000.50 -> "250.000,50" (con decimales)
        /// </summary>
        /// <param name="precio">Precio a formatear</param>
        /// <param name="mostrarDecimales">True para mostrar decimales, False para ocultarlos</param>
        /// <returns>String con el precio formateado</returns>
        public static string FormatearPrecio(decimal precio, bool mostrarDecimales = false)
        {
            // Creamos una cultura personalizada con punto como separador de miles
            System.Globalization.CultureInfo culturaPersonalizada =
                (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();

            // Configuramos el punto como separador de miles
            culturaPersonalizada.NumberFormat.NumberGroupSeparator = ".";

            // Configuramos la coma como separador de decimales (estándar español)
            culturaPersonalizada.NumberFormat.NumberDecimalSeparator = ",";

            // Configuramos cantidad de decimales según el parámetro
            if (mostrarDecimales)
            {
                culturaPersonalizada.NumberFormat.NumberDecimalDigits = 2; // 2 decimales
            }
            else
            {
                culturaPersonalizada.NumberFormat.NumberDecimalDigits = 0; // Sin decimales
            }

            // Devolvemos el número formateado
            return precio.ToString("N", culturaPersonalizada);
        }


    }
}
