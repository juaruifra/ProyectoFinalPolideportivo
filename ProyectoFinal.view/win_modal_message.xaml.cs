using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Lógica de interacción para win_modal_message.xaml
    /// </summary>
    public partial class win_modal_message : Window
    {
        // Resultado del modal
        public bool Result { get; private set; } = false;

        // Constructor del modal
        public win_modal_message(string mensaje, string titulo, ModalMessageType tipo)
        {
            InitializeComponent();

            // Asignamos textos
            txtTitulo.Text = titulo;
            txtMensaje.Text = mensaje;

            // Configuramos el aspecto según el tipo
            ConfigurarTipo(tipo);
        }

        private void ConfigurarTipo(ModalMessageType tipo)
        {
            switch (tipo)
            {
                case ModalMessageType.Info:
                    TopBar.Background = CrearBrush("#005BBB");
                    txtIcono.Text = "ℹ";
                    btnCancelar.Visibility = Visibility.Collapsed;
                    break;

                case ModalMessageType.Error:
                    TopBar.Background = CrearBrush("#DC2626");
                    txtIcono.Text = "✖";
                    btnCancelar.Visibility = Visibility.Collapsed;
                    break;

                case ModalMessageType.Confirmacion:
                    TopBar.Background = CrearBrush("#0D9488");
                    txtIcono.Text = "!";
                    btnCancelar.Visibility = Visibility.Visible;
                    break;
            }
        }

        private Brush CrearBrush(string colorHex)
        {
            return (SolidColorBrush)new BrushConverter().ConvertFromString(colorHex);
        }

        // Permite mover la ventana arrastrando la barra superior
        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void BtnAceptar_Click(object sender, RoutedEventArgs e)
        {
            Result = true;
            Close();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            Result = false;
            Close();
        }
    }
}
