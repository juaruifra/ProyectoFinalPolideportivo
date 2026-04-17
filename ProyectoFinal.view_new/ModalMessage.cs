using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProyectoFinal.view_new
{
    public static class ModalMessage
    {
        // Muestra el modal y devuelve true o false según la acción del usuario
        public static bool Show(string mensaje, string titulo, ModalMessageType tipo = ModalMessageType.Error, Window owner = null)
        {
            var win = new win_modal_message(mensaje, titulo, tipo);

            // Asignamos la ventana padre para centrar correctamente
            if (owner != null)
                win.Owner = owner;

            win.ShowDialog();

            // Para Info y Error siempre devolvemos true
            if (tipo != ModalMessageType.Confirmacion)
                return true;

            return win.Result;
        }

        public static bool ShowModal(string msj, string tit, int tipo = 1)
        {

            ModalMessageType tipoMensaje = ModalMessageType.Error;
            switch (tipo)
            {
                case 1: // Error
                    tipoMensaje = ModalMessageType.Error;
                    break;
                case 2: // Error
                    tipoMensaje = ModalMessageType.Info;
                    break;
                case 3: // Error
                    tipoMensaje = ModalMessageType.Confirmacion;
                    break;
            }

            return Show(msj, tit, tipoMensaje);
        }
    }
}
