using ProyectoFinal.controller_new.controller;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Logica de interaccion para gestionar instalaciones.
    /// </summary>
    public partial class VistaInstalaciones : UserControl
    {
        private readonly InstalacionesController _controller; // Controlador de instalaciones.
        private readonly TiposInstalacionAPI _tiposApi; // API para cargar tipos.

        private Instalaciones _instalacionSeleccionada; // Instalacion actualmente en edicion.
        private System.Collections.Generic.List<Instalaciones> _todasInstalaciones; // Lista completa para filtrar sin ir a la BD.

        /// <summary>
        /// Constructor.
        /// </summary>
        public VistaInstalaciones()
        {
            InitializeComponent(); // Inicializamos componentes.

            _controller = new InstalacionesController(); // Creamos controlador.
            _tiposApi = new TiposInstalacionAPI(); // Creamos API de tipos.

            CargarTipos(); // Cargamos combo de tipos.
            LimpiarFormulario(); // Dejamos formulario en estado "nuevo".
            CargarDatos(); // Cargamos grid.
        }

        /// <summary>
        /// Carga el combo de tipos de instalacion.
        /// </summary>
        private void CargarTipos()
        {
            try
            {
                var tipos = _tiposApi.ObtenerTodos(); // Obtenemos catalogo.
                cmbTipoInstalacion.ItemsSource = tipos; // Asignamos fuente.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"No se pudieron cargar los tipos: {ex.Message}", "Instalaciones", 1); // Error.
            }
        }

        /// <summary>
        /// Carga el listado de instalaciones.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                _todasInstalaciones = _controller.ObtenerTodos(soloDisponibles: false); // Guardamos la lista completa.
                AplicarFiltro(); // Aplicamos el filtro actual.

                btnEditarSeleccionado.IsEnabled = dgInstalaciones.SelectedItem != null; // Habilitamos edicion.
                ActualizarEstadoAcciones(); // Actualizamos acciones.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"No se pudieron cargar las instalaciones: {ex.Message}", "Instalaciones", 1); // Error.
            }
        }

        /// <summary>
        /// Filtra la lista de instalaciones segun el texto del buscador y actualiza el grid y el contador.
        /// </summary>
        private void AplicarFiltro()
        {
            // Si no hay lista cargada todavia, no hacemos nada.
            if (_todasInstalaciones == null) return;

            // Obtenemos el texto de busqueda en minusculas para comparacion sin distincion de mayusculas.
            var texto = (txtBuscar.Text ?? string.Empty).Trim().ToLower();

            // Si no hay texto mostramos todas las instalaciones sin filtrar.
            if (string.IsNullOrEmpty(texto))
            {
                dgInstalaciones.ItemsSource = _todasInstalaciones; // Mostramos todas.
                txtTotalInstalaciones.Text = $"{_todasInstalaciones.Count} instalaciones"; // Contador total.
                return;
            }

            // Filtramos por nombre de instalacion o nombre del tipo que contengan el texto buscado.
            var filtradas = _todasInstalaciones.FindAll(i =>
                (i.Nombre ?? string.Empty).ToLower().Contains(texto) ||
                (i.TiposInstalacion?.Nombre ?? string.Empty).ToLower().Contains(texto));

            dgInstalaciones.ItemsSource = filtradas; // Mostramos el resultado filtrado.
            txtTotalInstalaciones.Text = $"{filtradas.Count} instalaciones"; // Contador filtrado.
        }

        /// <summary>
        /// Limpia formulario para alta.
        /// </summary>
        private void LimpiarFormulario()
        {
            _instalacionSeleccionada = null; // Reseteamos seleccion.

            txtNombre.Text = string.Empty; // Limpiamos nombre.
            txtPrecioHora.Text = string.Empty; // Limpiamos precio.
            txtObservaciones.Text = string.Empty; // Limpiamos observaciones.

            cmbTipoInstalacion.SelectedIndex = -1; // Sin tipo.

            chkDisponible.IsChecked = true; // Por defecto disponible.

            dgInstalaciones.SelectedItem = null; // Quitamos seleccion del grid.
        }

        /// <summary>
        /// Crea entidad a partir del formulario.
        /// </summary>
        /// <returns>Entidad Instalaciones.</returns>
        private Instalaciones CrearEntidadDesdeFormulario()
        {
            var i = _instalacionSeleccionada ?? new Instalaciones(); // Reutilizamos en edicion.

            i.Nombre = (txtNombre.Text ?? string.Empty).Trim(); // Nombre.

            i.TipoInstalacionId = cmbTipoInstalacion.SelectedValue is int ? (int)cmbTipoInstalacion.SelectedValue : 0; // Tipo.

            i.Disponible = chkDisponible.IsChecked == true; // Disponible.

            i.Observaciones = string.IsNullOrWhiteSpace(txtObservaciones.Text) ? null : txtObservaciones.Text.Trim(); // Observaciones.

            // Parseamos precio (coma o punto).
            var precioTexto = (txtPrecioHora.Text ?? string.Empty).Trim(); // Texto.

            if (decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.CurrentCulture, out var precio) ||
                decimal.TryParse(precioTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out precio))
            {
                i.PrecioHora = precio; // Asignamos.
            }
            else
            {
                i.PrecioHora = -1; // Forzamos error de validacion.
            }

            return i; // Devolvemos.
        }

        /// <summary>
        /// Carga una instalacion en el formulario.
        /// </summary>
        /// <param name="instalacion">Instalacion.</param>
        private void CargarFormularioDesdeEntidad(Instalaciones instalacion)
        {
            if (instalacion == null) return; // Seguridad por si el valor viene a null

            _instalacionSeleccionada = instalacion; // Guardamos ref.

            txtNombre.Text = instalacion.Nombre; // Nombre.
            txtPrecioHora.Text = instalacion.PrecioHora.ToString(CultureInfo.CurrentCulture); // Precio.
            txtObservaciones.Text = instalacion.Observaciones; // Obs.

            cmbTipoInstalacion.SelectedValue = instalacion.TipoInstalacionId; // Tipo.

            chkDisponible.IsChecked = instalacion.Disponible; // Disponible.
        }

        /// <summary>
        /// Cancelar.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario(); // Limpiamos.
        }

        /// <summary>
        /// Guardar.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty; // Titulo.
                string mensajeError = string.Empty; // Mensaje.

                var entidad = CrearEntidadDesdeFormulario(); // Entidad.

                if (_controller.Guardar(entidad, ref tituloError, ref mensajeError)) // Guardamos.
                {
                    LimpiarFormulario(); // Limpiamos.
                    CargarDatos(); // Recargamos.
                    ModalMessage.ShowModal("Instalacion guardada correctamente.", "Instalaciones", 2); // Info.
                    return; // Salimos.
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error/validacion.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Instalaciones", 1); // Error.
            }
        }

        /// <summary>
        /// Editar.
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var instalacion = dgInstalaciones.SelectedItem as Instalaciones; // Seleccion.
            if (instalacion == null) return; // Seguridad por si el objeto viene a null.

            CargarFormularioDesdeEntidad(instalacion); // Cargamos.
        }

        /// <summary>
        /// Refrescar.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos.
        }

        /// <summary>
        /// Borrar selección.
        /// </summary>
        private void BtnBorrarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtenemos selección.
                var instalacion = dgInstalaciones.SelectedItem as Instalaciones;

                // Validamos.
                if (instalacion == null) return;

                // Confirmación.
                var confirmar = ModalMessage.ShowModal(
                    $"¿Desea borrar la instalación '{instalacion.Nombre}'? Esta acción no se puede deshacer.",
                    "Instalaciones",
                    3);

                // Si no confirma, salimos.
                if (!confirmar) return;

                // Borrado.
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                if (_controller.Borrar(instalacion, ref tituloError, ref mensajeError))
                {
                    // Recargamos.
                    LimpiarFormulario();
                    CargarDatos();
                    ModalMessage.ShowModal("Instalación borrada correctamente.", "Instalaciones", 2);
                    return;
                }

                // Error de validación/uso.
                ModalMessage.ShowModal(mensajeError, tituloError, 1);
            }
            catch (Exception ex)
            {
                // Error inesperado.
                ModalMessage.ShowModal($"Error inesperado al borrar: {ex.Message}", "Instalaciones", 1);
            }
        }

        /// <summary>
        /// Habilita o deshabilita los botones de acciones segun la seleccion.
        /// </summary>
        private void ActualizarEstadoAcciones()
        {
            // Calculamos si hay seleccion.
            var haySeleccion = dgInstalaciones != null && dgInstalaciones.SelectedItem != null;

            // Boton editar.
            if (btnEditarSeleccionado != null) btnEditarSeleccionado.IsEnabled = haySeleccion;

            // Boton borrar (buscamos por nombre).
            var btnBorrar = FindName("btnBorrarSeleccionado") as Button;
            if (btnBorrar != null) btnBorrar.IsEnabled = haySeleccion;
        }

        /// <summary>
        /// Cambio de seleccion.
        /// </summary>
        private void DgInstalaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Actualizamos botones.
            ActualizarEstadoAcciones();
        }

        /// <summary>
        /// Evento TextChanged del buscador: aplica el filtro en tiempo real.
        /// </summary>
        private void TxtBuscar_TextChanged(object sender, TextChangedEventArgs e)
        {
            AplicarFiltro(); // Filtramos cada vez que cambia el texto.
        }
    }
}
