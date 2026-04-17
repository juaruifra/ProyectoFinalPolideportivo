using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ProyectoFinal.view
{
    public partial class MainWindow : Window
    {
        // Estado del menú (true = expandido, false = colapsado)
        // Inicia en false porque el XAML ya tiene Width="60"
        private bool menuExpandido = false;

        public MainWindow()
        {
            InitializeComponent();

            // Al iniciar la aplicación cargamos la vista inicial
            CargarInicio();
        }

        // ================= CONTROL DEL MENÚ HAMBURGUESA =================

        /// <summary>
        /// Toggle del menú: expande o colapsa según el estado actual
        /// </summary>
        private void BtnHamburguesa_Click(object sender, RoutedEventArgs e)
        {
            if (menuExpandido)
            {
                // Colapsar menú
                MenuColumn.Width = new GridLength(60);
                OcultarTextos();
                menuExpandido = false;
            }
            else
            {
                // Expandir menú
                MenuColumn.Width = new GridLength(220);
                MostrarTextos();
                menuExpandido = true;
            }
        }

        /// <summary>
        /// Oculta los textos del menú
        /// </summary>
        private void OcultarTextos()
        {
            if (TextoInicio != null) TextoInicio.Visibility = Visibility.Collapsed;
            if (TextoClientes != null) TextoClientes.Visibility = Visibility.Collapsed; // Socios
            if (TextoAgentes != null) TextoAgentes.Visibility = Visibility.Collapsed;
            if (TextoInmuebles != null) TextoInmuebles.Visibility = Visibility.Collapsed;
            if (TextoOperaciones != null) TextoOperaciones.Visibility = Visibility.Collapsed;
            if (TextoOfertas != null) TextoOfertas.Visibility = Visibility.Collapsed;
            if (TextoInformes != null) TextoInformes.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Muestra los textos del menú
        /// </summary>
        private void MostrarTextos()
        {
            if (TextoInicio != null) TextoInicio.Visibility = Visibility.Visible;
            if (TextoClientes != null) TextoClientes.Visibility = Visibility.Visible; // Socios
            if (TextoAgentes != null) TextoAgentes.Visibility = Visibility.Visible;
            if (TextoInmuebles != null) TextoInmuebles.Visibility = Visibility.Visible;
            if (TextoOperaciones != null) TextoOperaciones.Visibility = Visibility.Visible;
            if (TextoOfertas != null) TextoOfertas.Visibility = Visibility.Visible;
            if (TextoInformes != null) TextoInformes.Visibility = Visibility.Visible;
        }

        // ================= CARGA DE VISTAS =================

        // Muestra la vista de inicio
        private void CargarInicio()
        {
            ResetMenu();

            // Marcamos el elemento activo del menú
            ItemInicio.Tag = "Activo";

            // Cambiamos el título superior
            tbTitulo.Text = "Inicio";

            // Cargamos el UserControl correspondiente
            MainContent.Content = new VistaInicio();
        }

        // Evento click de Inicio
        private void AbrirInicio(object sender, MouseButtonEventArgs e)
        {
            CargarInicio();
        }

        // Evento click de Clientes (ahora Socios)
        private void AbrirSocios(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();

            ItemClientes.Tag = "Activo";
            tbTitulo.Text = "Gestión de Socios";
            MainContent.Content = new VistaSocios();
        }

        // Evento click de Agentes
        private void AbrirAgentes(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();
            ItemAgentes.Tag = "Activo";
            tbTitulo.Text = "Gestión de Agentes";
            MainContent.Content = new VistaAgentes();
        }

        // Evento click de Inmuebles
        private void AbrirInmuebles(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();
            ItemInmuebles.Tag = "Activo";
            tbTitulo.Text = "Gestión de Inmuebles";
            MainContent.Content = new VistaInmuebles();
        }

        private void AbrirOfertas(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();
            ItemOfertas.Tag = "Activo";
            tbTitulo.Text = "Gestión de Ofertas";
            MainContent.Content = new VistaOfertas();
        }

        private void AbrirOperaciones(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();
            ItemOperaciones.Tag = "Activo";
            tbTitulo.Text = "Gestión de Operaciones";
            MainContent.Content = new VistaOperaciones();
        }

        // Evento click de Informes
        private void AbrirInformes(object sender, MouseButtonEventArgs e)
        {
            ResetMenu();
            ItemInformes.Tag = "Activo";
            tbTitulo.Text = "Informes";
            MainContent.Content = new VistaInformes();
        }

        // Limpia el estado visual del menú
        private void ResetMenu()
        {
            ItemInicio.Tag = null;
            ItemClientes.Tag = null; // Socios
            ItemAgentes.Tag = null;
            ItemInmuebles.Tag = null;
            ItemOperaciones.Tag = null;
            ItemOfertas.Tag = null;
            ItemInformes.Tag = null;
        }



        /// <summary>
        /// Permite arrastrar pulsando en la barra superior
        /// </summary>
        private void BarraSuperior_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        /// <summary>
        /// Permite cerrar la aplicación
        /// </summary>
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

