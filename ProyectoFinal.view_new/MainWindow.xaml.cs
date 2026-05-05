using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProyectoFinal.view_new
{
    public partial class MainWindow : Window
    {
        private bool menuExpandido = false; // Estado del menú.

        public MainWindow()
        {
            InitializeComponent(); // Inicializamos componentes.
            CargarInicio(); // Cargamos inicio.
        }

        private void BtnHamburguesa_Click(object sender, RoutedEventArgs e)
        {
            if (menuExpandido) // Si está expandido.
            {
                MenuColumn.Width = new GridLength(60); // Colapsamos.
                OcultarTextos(); // Ocultamos textos.
                menuExpandido = false; // Actualizamos flag.
            }
            else // Si está colapsado.
            {
                MenuColumn.Width = new GridLength(220); // Expandimos.
                MostrarTextos(); // Mostramos textos.
                menuExpandido = true; // Actualizamos flag.
            }
        }

        private void OcultarTextos()
        {
            if (TextoInicio != null) TextoInicio.Visibility = Visibility.Collapsed; // Oculta inicio.
            if (TextoTipos != null) TextoTipos.Visibility = Visibility.Collapsed; // Oculta tipos.
            if (TextoClientes != null) TextoClientes.Visibility = Visibility.Collapsed; // Oculta socios.
            if (TextoInstalaciones != null) TextoInstalaciones.Visibility = Visibility.Collapsed; // Oculta instalaciones.
            if (TextoCuotas != null) TextoCuotas.Visibility = Visibility.Collapsed; // Oculta cuotas.
            if (TextoReservas != null) TextoReservas.Visibility = Visibility.Collapsed; // Oculta reservas.
        }

        private void MostrarTextos()
        {
            if (TextoInicio != null) TextoInicio.Visibility = Visibility.Visible; // Muestra inicio.
            if (TextoTipos != null) TextoTipos.Visibility = Visibility.Visible; // Muestra tipos.
            if (TextoClientes != null) TextoClientes.Visibility = Visibility.Visible; // Muestra socios.
            if (TextoInstalaciones != null) TextoInstalaciones.Visibility = Visibility.Visible; // Muestra instalaciones.
            if (TextoCuotas != null) TextoCuotas.Visibility = Visibility.Visible; // Muestra cuotas.
            if (TextoReservas != null) TextoReservas.Visibility = Visibility.Visible; // Muestra reservas.
        }

        private void CargarInicio()
        {
            ResetMenu(); // Resetea estado menú.

            ItemInicio.Tag = "Activo"; // Marca activo.
            tbTitulo.Text = "Inicio"; // Título.
            MainContent.Content = new VistaInicio(); // Carga vista.
        }

        private void AbrirInicio(object sender, MouseButtonEventArgs e)
        {
            CargarInicio(); // Abre inicio.
        }

        private void AbrirSocios(object sender, MouseButtonEventArgs e)
        {
            ResetMenu(); // Resetea.

            ItemClientes.Tag = "Activo"; // Marca socios.
            tbTitulo.Text = "Gestion de Socios"; // Título.
            MainContent.Content = new VistaSocios(); // Vista.
        }

        /// <summary>
        /// Evento click de Instalaciones.
        /// </summary>
        private void AbrirInstalaciones(object sender, MouseButtonEventArgs e)
        {
            ResetMenu(); // Quitamos selección.

            ItemInstalaciones.Tag = "Activo"; // Marcamos menú.
            tbTitulo.Text = "Gestion de Instalaciones"; // Título.
            MainContent.Content = new VistaInstalaciones(); // Cargamos vista.
        }

        private void ResetMenu()
        {
            ItemInicio.Tag = null; // Limpia.
            if (ItemTipos != null) ItemTipos.Tag = null; // Limpia tipos.
            ItemClientes.Tag = null; // Limpia.
            if (ItemInstalaciones != null) ItemInstalaciones.Tag = null; // Limpia.
            if (ItemCuotas != null) ItemCuotas.Tag = null; // Limpia cuotas.
            if (ItemReservas != null) ItemReservas.Tag = null; // Limpia reservas.
        }

        private void BarraSuperior_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed) DragMove(); // Arrastra ventana.
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close(); // Cierra.
        }

        /// <summary>
        /// Evento click de Cuotas.
        /// </summary>
        private void AbrirCuotas(object sender, MouseButtonEventArgs e)
        {
            ResetMenu(); // Quitamos selección.

            ItemCuotas.Tag = "Activo"; // Marcamos menú.
            tbTitulo.Text = "Gestion de Cuotas"; // Título.
            MainContent.Content = new VistaCuotas(); // Cargamos vista.
        }

        /// <summary>
        /// Evento click de Reservas.
        /// </summary>
        private void AbrirReservas(object sender, MouseButtonEventArgs e)
        {
            ResetMenu(); // Quitamos seleccion.

            ItemReservas.Tag = "Activo"; // Marcamos menu.
            tbTitulo.Text = "Gestion de Reservas"; // Titulo.
            MainContent.Content = new VistaReservas(); // Cargamos vista.
        }

        /// <summary>
        /// Evento click de Tipos de Instalacion.
        /// </summary>
        private void AbrirTipos(object sender, MouseButtonEventArgs e)
        {
            ResetMenu(); // Quitamos seleccion.

            ItemTipos.Tag = "Activo"; // Marcamos menu.
            tbTitulo.Text = "Tipos de Instalacion"; // Titulo.
            MainContent.Content = new VistaTiposInstalacion(); // Cargamos vista.
        }
    }
}

