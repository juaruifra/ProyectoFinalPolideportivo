using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Logica de interaccion para gestionar socios del club.
    /// </summary>
    public partial class VistaSocios : UserControl
    {
        private readonly SociosController _controller; // Controlador que gestiona la logica de socios.
        private Socios _socioSeleccionado; // Socio que se esta editando en el formulario.

        /// <summary>
        /// Constructor.
        /// </summary>
        public VistaSocios()
        {
            InitializeComponent(); // Inicializamos los componentes de la vista.
            _controller = new SociosController(); // Creamos el controlador de socios.
            CargarDatos(); // Cargamos el listado al abrir la vista.
        }

        /// <summary>
        /// Carga el listado de socios en el grid.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                var socios = _controller.ObtenerTodos(soloActivos: false); // Obtenemos todos los socios.
                dgSocios.ItemsSource = socios; // Asignamos la lista al grid.

                txtTotalSocios.Text = $"{socios.Count} socios"; // Mostramos el total.

                btnEditarSeleccionado.IsEnabled = dgSocios.SelectedItem != null; // Habilitamos editar solo si hay seleccion.
            }
            catch (Exception ex)
            {
                // Mostramos el error si no se pueden cargar los datos.
                ModalMessage.ShowModal($"No se pudieron cargar los socios: {ex.Message}", "Socios", 1);
            }
        }

        /// <summary>
        /// Limpia el formulario y deja la vista lista para dar de alta un socio nuevo.
        /// </summary>
        private void LimpiarFormulario()
        {
            _socioSeleccionado = null; // Quitamos la referencia al socio en edicion.
            txtNombre.Text = string.Empty; // Vaciamos nombre.
            txtApellidos.Text = string.Empty; // Vaciamos apellidos.
            txtDni.Text = string.Empty; // Vaciamos DNI.
            txtEmail.Text = string.Empty; // Vaciamos email.
            txtTelefono.Text = string.Empty; // Vaciamos telefono.

            chkActivo.IsChecked = true; // Por defecto el socio nuevo estara activo.
            dgSocios.SelectedItem = null; // Quitamos la seleccion del grid.
        }

        /// <summary>
        /// Construye una entidad Socios con los datos que hay en el formulario.
        /// </summary>
        /// <returns>Entidad con los datos del formulario.</returns>
        private Socios CrearSocioDesdeFormulario()
        {
            var socio = _socioSeleccionado ?? new Socios(); // Si estamos editando reutilizamos la entidad.

            socio.Nombre = (txtNombre.Text ?? string.Empty).Trim(); // Asignamos nombre.
            socio.Apellidos = (txtApellidos.Text ?? string.Empty).Trim(); // Asignamos apellidos.
            socio.Dni = (txtDni.Text ?? string.Empty).Trim(); // Asignamos DNI.
            socio.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(); // Asignamos email o null.
            socio.Telefono = string.IsNullOrWhiteSpace(txtTelefono.Text) ? null : txtTelefono.Text.Trim(); // Asignamos telefono o null.
            socio.Activo = chkActivo.IsChecked == true; // Asignamos estado activo.

            return socio; // Devolvemos la entidad lista.
        }

        /// <summary>
        /// Rellena el formulario con los datos de un socio para editarlo.
        /// </summary>
        /// <param name="socio">Socio cuyos datos se cargan en el formulario.</param>
        private void CargarFormularioDesdeSocio(Socios socio)
        {
            if (socio == null) return; // Seguridad por si el objeto viene a null.

            _socioSeleccionado = socio; // Guardamos referencia al socio en edicion.

            txtNombre.Text = socio.Nombre; // Cargamos nombre.
            txtApellidos.Text = socio.Apellidos; // Cargamos apellidos.
            txtDni.Text = socio.Dni; // Cargamos DNI.
            txtEmail.Text = socio.Email; // Cargamos email.
            txtTelefono.Text = socio.Telefono; // Cargamos telefono.
            chkActivo.IsChecked = socio.Activo; // Cargamos estado activo.
        }

        /// <summary>
        /// Evento click del boton Cancelar.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario(); // Limpiamos el formulario y cancelamos la edicion.
        }

        /// <summary>
        /// Evento click del boton Guardar.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty; // Titulo de error vacio por defecto.
                string mensajeError = string.Empty; // Mensaje de error vacio por defecto.

                var socio = CrearSocioDesdeFormulario(); // Construimos la entidad desde el formulario.

                if (_controller.Guardar(socio, ref tituloError, ref mensajeError)) // Intentamos guardar.
                {
                    LimpiarFormulario(); // Si guardamos bien limpiamos el formulario.
                    CargarDatos(); // Recargamos el grid para mostrar el cambio.
                    ModalMessage.ShowModal("Socio guardado correctamente.", "Socios", 2); // Confirmacion.
                    return; // Salimos.
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Mostramos el error de validacion.
            }
            catch (Exception ex)
            {
                // Error inesperado al guardar.
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Socios", 1);
            }
        }

        /// <summary>
        /// Evento click del boton Editar.
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var socio = dgSocios.SelectedItem as Socios; // Obtenemos el socio seleccionado.
            if (socio == null) return; // Guard: si no hay seleccion no hacemos nada.

            CargarFormularioDesdeSocio(socio); // Cargamos el socio en el formulario.
        }

        /// <summary>
        /// Evento click del boton Refrescar.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos el listado.
        }

        /// <summary>
        /// Evento click del boton Borrar.
        /// </summary>
        private void BtnBorrarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var socio = dgSocios.SelectedItem as Socios; // Obtenemos el socio seleccionado.

                if (socio == null) return; // Guard: si no hay seleccion no hacemos nada.

                // Pedimos confirmacion antes de borrar.
                var confirmar = ModalMessage.ShowModal(
                    $"Desea borrar el socio '{socio.Nombre} {socio.Apellidos}'? Esta accion no se puede deshacer.",
                    "Socios",
                    3);

                if (!confirmar) return; // Si el usuario cancela salimos.

                string tituloError = string.Empty; // Titulo de error.
                string mensajeError = string.Empty; // Mensaje de error.

                if (_controller.Borrar(socio, ref tituloError, ref mensajeError)) // Intentamos borrar.
                {
                    LimpiarFormulario(); // Limpiamos el formulario.
                    CargarDatos(); // Recargamos el grid.
                    ModalMessage.ShowModal("Socio borrado correctamente.", "Socios", 2); // Confirmacion.
                    return; // Salimos.
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Mostramos error de validacion o uso.
            }
            catch (Exception ex)
            {
                // Error inesperado al borrar.
                ModalMessage.ShowModal($"Error inesperado al borrar: {ex.Message}", "Socios", 1);
            }
        }

        /// <summary>
        /// Evento de cambio de seleccion en el grid.
        /// </summary>
        private void DgSocios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var haySeleccion = dgSocios.SelectedItem != null; // Comprobamos si hay algo seleccionado.

            btnEditarSeleccionado.IsEnabled = haySeleccion; // Habilitamos o deshabilitamos editar.
            if (btnBorrarSeleccionado != null) btnBorrarSeleccionado.IsEnabled = haySeleccion; // Habilitamos o deshabilitamos borrar.
        }
    }
}
