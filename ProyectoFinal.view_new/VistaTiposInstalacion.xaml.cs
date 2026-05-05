using Comun;
using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Logica de interaccion para gestionar tipos de instalacion.
    /// </summary>
    public partial class VistaTiposInstalacion : UserControl
    {
        private readonly TiposInstalacionController _controller; // Controlador de tipos.

        private TiposInstalacion _tipoSeleccionado; // Tipo actualmente en edicion.

        /// <summary>
        /// Constructor.
        /// </summary>
        public VistaTiposInstalacion()
        {
            InitializeComponent(); // Inicializamos componentes.

            _controller = new TiposInstalacionController(); // Creamos controlador.

            LimpiarFormulario(); // Dejamos formulario listo para nuevo.
            CargarDatos(); // Cargamos el grid.
        }

        /// <summary>
        /// Carga el listado de tipos de instalacion en el grid.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                var tipos = _controller.ObtenerTodos(); // Obtenemos todos los tipos.
                dgTipos.ItemsSource = tipos; // Asignamos al grid.

                txtTotalTipos.Text = $"{tipos.Count} tipos"; // Mostramos total.

                ActualizarEstadoAcciones(); // Actualizamos botones.
            }
            catch (Exception ex)
            {
                // Mostramos error.
                ModalMessage.ShowModal($"No se pudieron cargar los tipos: {ex.Message}", "Tipos de instalacion", 1);
            }
        }

        /// <summary>
        /// Limpia el formulario para alta de nuevo tipo.
        /// </summary>
        private void LimpiarFormulario()
        {
            _tipoSeleccionado = null; // Limpiamos referencia.

            txtNombre.Text = string.Empty; // Vaciamos nombre.
            txtDescripcion.Text = string.Empty; // Vaciamos descripcion.

            dgTipos.SelectedItem = null; // Quitamos seleccion del grid.
        }

        /// <summary>
        /// Construye una entidad TiposInstalacion a partir del formulario.
        /// </summary>
        /// <returns>Entidad con los datos del formulario.</returns>
        private TiposInstalacion CrearEntidadDesdeFormulario()
        {
            var t = _tipoSeleccionado ?? new TiposInstalacion(); // Reutilizamos en edicion.

            t.Nombre = (txtNombre.Text ?? string.Empty).Trim(); // Asignamos nombre.

            // Si la descripcion esta vacia guardamos null, de lo contrario el texto.
            t.Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text)
                ? null
                : txtDescripcion.Text.Trim();

            return t; // Devolvemos entidad.
        }

        /// <summary>
        /// Carga un tipo en el formulario para editarlo.
        /// </summary>
        /// <param name="tipo">Tipo a cargar.</param>
        private void CargarFormularioDesdeEntidad(TiposInstalacion tipo)
        {
            if (tipo == null) return; // Seguridad por si el tipo es null

            _tipoSeleccionado = tipo; // Guardamos referencia.

            txtNombre.Text = tipo.Nombre; // Asignamos nombre.
            txtDescripcion.Text = tipo.Descripcion; // Asignamos descripcion.
        }

        /// <summary>
        /// Habilita o deshabilita los botones de accion segun si hay seleccion.
        /// </summary>
        private void ActualizarEstadoAcciones()
        {
            var haySeleccion = dgTipos != null && dgTipos.SelectedItem != null; // Comprobamos seleccion.

            if (btnEditarSeleccionado != null) btnEditarSeleccionado.IsEnabled = haySeleccion; // Boton editar.
            if (btnBorrarSeleccionado != null) btnBorrarSeleccionado.IsEnabled = haySeleccion; // Boton borrar.
        }

        /// <summary>
        /// Evento click del boton Cancelar.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario(); // Limpiamos formulario.
        }

        /// <summary>
        /// Evento click del boton Guardar.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty; // Titulo de error.
                string mensajeError = string.Empty; // Mensaje de error.

                var entidad = CrearEntidadDesdeFormulario(); // Construimos entidad.

                if (_controller.Guardar(entidad, ref tituloError, ref mensajeError)) // Intentamos guardar.
                {
                    LimpiarFormulario(); // Limpiamos si OK.
                    CargarDatos(); // Recargamos grid.
                    ModalMessage.ShowModal("Tipo de instalacion guardado correctamente.", "Tipos de instalacion", 2); // Confirmacion.
                    return; // Salimos.
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Mostramos error de validacion.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Tipos de instalacion", 1);
            }
        }

        /// <summary>
        /// Evento click del boton Editar.
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var tipo = dgTipos.SelectedItem as TiposInstalacion; // Obtenemos seleccion.
            if (tipo == null) return; // Seguridad por si el objeto viene a null.

            CargarFormularioDesdeEntidad(tipo); // Cargamos en formulario.
        }

        /// <summary>
        /// Evento click del boton Borrar.
        /// </summary>
        private void BtnBorrarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tipo = dgTipos.SelectedItem as TiposInstalacion; // Obtenemos seleccion.
                if (tipo == null) return; // Seguridad por si el objeto viene a null.

                // Pedimos confirmacion antes de borrar.
                var confirmar = ModalMessage.ShowModal(
                    $"Desea borrar el tipo '{tipo.Nombre}'? Esta accion no se puede deshacer.",
                    "Tipos de instalacion",
                    3);

                if (!confirmar) return; // Si cancela, salimos.

                string tituloError = string.Empty; // Titulo de error.
                string mensajeError = string.Empty; // Mensaje de error.

                if (_controller.Borrar(tipo, ref tituloError, ref mensajeError)) // Intentamos borrar.
                {
                    LimpiarFormulario(); // Limpiamos.
                    CargarDatos(); // Recargamos.
                    ModalMessage.ShowModal("Tipo de instalacion borrado correctamente.", "Tipos de instalacion", 2); // OK.
                    return; // Salimos.
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Mostramos error.
            }
            catch (Exception ex)
            {
                // Error inesperado.
                ModalMessage.ShowModal($"Error inesperado al borrar: {ex.Message}", "Tipos de instalacion", 1);
            }
        }

        /// <summary>
        /// Evento click del boton Refrescar.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos el grid.
        }

        /// <summary>
        /// Evento de cambio de seleccion en el grid.
        /// </summary>
        private void DgTipos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoAcciones(); // Actualizamos botones.
        }
    }
}
