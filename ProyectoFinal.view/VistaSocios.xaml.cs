using ProyectoFinal.controller.controller;
using ProyectoFinal.model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view
{
    public partial class VistaSocios : UserControl
    {
        private readonly SociosController _controller;
        private Socios _socioSeleccionado;

        public VistaSocios()
        {
            InitializeComponent();
            _controller = new SociosController();
            CargarDatos();
        }

        private void CargarDatos()
        {
            try
            {
                var socios = _controller.ObtenerTodos(soloActivos: false);
                dgSocios.ItemsSource = socios;

                txtTotalSocios.Text = $"{socios.Count} socios";

                btnEditarSeleccionado.IsEnabled = dgSocios.SelectedItem != null;
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"No se pudieron cargar los socios: {ex.Message}", "Socios", 1);
            }
        }

        private void LimpiarFormulario()
        {
            _socioSeleccionado = null;
            txtNombre.Text = string.Empty;
            txtApellidos.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefono.Text = string.Empty;

            // Por defecto, todo socio nuevo debe quedar activo.
            chkActivo.IsChecked = true;

            dgSocios.SelectedItem = null;
        }

        private Socios CrearSocioDesdeFormulario()
        {
            var socio = _socioSeleccionado ?? new Socios();

            socio.Nombre = (txtNombre.Text ?? string.Empty).Trim();
            socio.Apellidos = (txtApellidos.Text ?? string.Empty).Trim();
            socio.Dni = (txtDni.Text ?? string.Empty).Trim();
            socio.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
            socio.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim();
            socio.Activo = chkActivo.IsChecked == true;

            return socio;
        }

        private void CargarFormularioDesdeSocio(Socios socio)
        {
            if (socio == null) return;

            _socioSeleccionado = socio;

            txtNombre.Text = socio.Nombre;
            txtApellidos.Text = socio.Apellidos;
            txtDni.Text = socio.Dni;
            txtEmail.Text = socio.Email;
            txtTelefono.Text = socio.Telefono;
            chkActivo.IsChecked = socio.Activo;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                var socio = CrearSocioDesdeFormulario();

                if (_controller.Guardar(socio, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario();
                    CargarDatos();
                    ModalMessage.ShowModal("Socio guardado correctamente.", "Socios", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1);
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Socios", 1);
            }
        }

        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var socio = dgSocios.SelectedItem as Socios;
            if (socio == null) return;

            CargarFormularioDesdeSocio(socio);
        }

        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void DgSocios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnEditarSeleccionado.IsEnabled = dgSocios.SelectedItem != null;
        }
    }
}
