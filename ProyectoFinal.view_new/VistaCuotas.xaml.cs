using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Logica del UserControl VistaCuotas.
    /// Permite gestionar las cuotas de los socios del club.
    /// </summary>
    public partial class VistaCuotas : UserControl
    {
        // Controlador de cuotas.
        private readonly CuotasController _controller;

        // Controlador de socios (para cargar el ComboBox de socios).
        private readonly SociosController _sociosController;

        // Cuota seleccionada para editar.
        private Cuotas _cuotaSeleccionada;

        /// <summary>
        /// Constructor: inicializa componentes y carga datos.
        /// </summary>
        public VistaCuotas()
        {
            InitializeComponent(); // Inicializamos el UserControl.

            _controller = new CuotasController(); // Instanciamos controller de cuotas.
            _sociosController = new SociosController(); // Instanciamos controller de socios.

            CargarComboSocios(); // Cargamos socios en combos.
            LimpiarFormulario(); // Aplicamos valores por defecto al formulario.
            CargarDatos(); // Cargamos cuotas en grid.
        }



        /// <summary>
        /// Carga todos los socios activos en los ComboBox de la vista.
        /// </summary>
        private void CargarComboSocios()
        {
            try
            {
                // Pedimos socios activos al controller (respeta la arquitectura en capas).
                var socios = _sociosController.ObtenerTodos(soloActivos: true);

                // Creamos item especial "Todos" para el combo de filtro con SocioId=0.
                var itemTodos = new Socios { SocioId = 0, Nombre = "", Apellidos = "(Todos los socios)" };

                // Lista para el combo de filtro: item "Todos" primero.
                var itemsFiltro = new List<Socios> { itemTodos };
                itemsFiltro.AddRange(socios); // Añadimos socios reales.

                // Asignamos al ComboBox de filtro.
                cmbFiltroSocio.ItemsSource = itemsFiltro;
                cmbFiltroSocio.SelectedIndex = 0; // Seleccionamos "Todos" por defecto.

                // Asignamos al ComboBox del formulario sin el item "Todos".
                cmbSocio.ItemsSource = socios;
            }
            catch (Exception ex)
            {
                // Mostramos error.
                ModalMessage.ShowModal($"No se pudieron cargar los socios: {ex.Message}", "Cuotas", 1);
            }
        }

        /// <summary>
        /// Carga las cuotas en el DataGrid segun los filtros activos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Obtenemos el filtro de socio seleccionado.
                int? socioId = null;
                if (cmbFiltroSocio.SelectedValue is int id && id > 0)
                    socioId = id; // Filtramos por socio.

                // Obtenemos si solo pendientes.
                bool soloPendientes = chkSoloPendientes.IsChecked == true;

                // Cargamos cuotas del controlador.
                var cuotas = _controller.ObtenerTodos(socioId, soloPendientes);

                // Asignamos al grid.
                dgCuotas.ItemsSource = cuotas;

                // Actualizamos contador.
                txtTotalCuotas.Text = $"{cuotas.Count} cuotas";

                // Actualizamos estado de botones.
                ActualizarEstadoAcciones();
            }
            catch (Exception ex)
            {
                // Error al cargar.
                ModalMessage.ShowModal($"No se pudieron cargar las cuotas: {ex.Message}", "Cuotas", 1);
            }
        }



        /// <summary>
        /// Limpia el formulario y lo deja listo para una nueva cuota.
        /// Aplica valores por defecto: anio y mes actuales, fecha de pago = hoy.
        /// </summary>
        private void LimpiarFormulario()
        {
            _cuotaSeleccionada = null; // Borramos referencia a cuota en edicion.

            cmbSocio.SelectedIndex = -1; // Sin socio seleccionado.

            txtAnio.Text = DateTime.Today.Year.ToString(); // Anio actual por defecto.
            txtMes.Text = DateTime.Today.Month.ToString(); // Mes actual por defecto.
            txtImporte.Text = string.Empty; // Importe en blanco.

            dpVencimiento.SelectedDate = null; // Vencimiento en blanco: el usuario decide.
            dpFechaPago.SelectedDate = DateTime.Today; // Fecha de pago = hoy por defecto.

            chkPagada.IsChecked = false; // Por defecto no pagada.

            dgCuotas.SelectedItem = null; // Quitamos seleccion del grid.
        }

        /// <summary>
        /// Carga una cuota en el formulario para editar.
        /// </summary>
        /// <param name="cuota">Cuota a cargar.</param>
        private void CargarFormularioDesdeEntidad(Cuotas cuota)
        {
            if (cuota == null) return; // Seguridad por si el objeto viene a null

            _cuotaSeleccionada = cuota; // Guardamos referencia.

            // Seleccionamos el socio buscando el item coincidente en la lista del combo.
            cmbSocio.SelectedItem = (cmbSocio.ItemsSource as List<Socios>)
                ?.Find(s => s.SocioId == cuota.SocioId);

            txtAnio.Text = cuota.Anio.ToString(); // Año.
            txtMes.Text = cuota.Mes.ToString(); // Mes.
            txtImporte.Text = cuota.Importe.ToString(CultureInfo.CurrentCulture); // Importe.

            dpVencimiento.SelectedDate = cuota.FechaVencimiento; // Fecha de vencimiento.
            dpFechaPago.SelectedDate = cuota.FechaPago; // Fecha de pago (puede ser null).

            chkPagada.IsChecked = cuota.Pagada; // Estado pagada.
        }

        /// <summary>
        /// Crea una entidad Cuotas a partir de los datos del formulario.
        /// </summary>
        /// <returns>Entidad Cuotas.</returns>
        private Cuotas CrearEntidadDesdeFormulario()
        {
            var c = _cuotaSeleccionada ?? new Cuotas(); // Reutilizamos en edicion.

            // Socio seleccionado: leemos desde SelectedItem para evitar problemas con ComboBox estilizado.
            c.SocioId = (cmbSocio.SelectedItem as Socios)?.SocioId ?? 0;

            // Año (parseamos).
            c.Anio = int.TryParse(txtAnio.Text, out int anio) ? anio : 0;

            // Mes (parseamos).
            c.Mes = int.TryParse(txtMes.Text, out int mes) ? mes : 0;

            // Importe (aceptamos coma o punto decimal).
            var importeTexto = (txtImporte.Text ?? string.Empty).Trim();
            if (decimal.TryParse(importeTexto, NumberStyles.Any, CultureInfo.CurrentCulture, out var importe) ||
                decimal.TryParse(importeTexto, NumberStyles.Any, CultureInfo.InvariantCulture, out importe))
            {
                c.Importe = importe; // Asignamos importe parseado.
            }
            else
            {
                c.Importe = -1; // Forzamos error de validacion en el controller.
            }

            // Fechas.
            c.FechaVencimiento = dpVencimiento.SelectedDate ?? default(DateTime); // Vencimiento.
            c.FechaPago = dpFechaPago.SelectedDate ?? default(DateTime); // Fecha pago (nullable).
            c.Pagada = chkPagada.IsChecked == true;  // Estado.

            return c; // Devolvemos entidad.
        }

        /// <summary>
        /// Actualiza el estado habilitado/deshabilitado de los botones de accion.
        /// </summary>
        private void ActualizarEstadoAcciones()
        {
            bool haySeleccion = dgCuotas.SelectedItem != null; // Hay fila seleccionada.

            btnEditarSeleccionado.IsEnabled = haySeleccion; // Editar.
            btnMarcarPagada.IsEnabled = haySeleccion; // Marcar pagada.
            if (btnBorrarSeleccionado != null) btnBorrarSeleccionado.IsEnabled = haySeleccion; // Borrar.
        }



        /// <summary>
        /// Cambio de seleccion en el DataGrid: actualiza botones.
        /// </summary>
        private void DgCuotas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoAcciones(); // Refrescamos estado de botones.
        }

        /// <summary>
        /// Cambio en el filtro de socio: recarga el grid.
        /// </summary>
        private void CmbFiltroSocio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CargarDatos(); // Recargamos con el nuevo filtro.
        }

        /// <summary>
        /// Cambio en el CheckBox de pendientes: recarga el grid.
        /// </summary>
        private void FiltroChanged(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos con el filtro actualizado.
        }

        /// <summary>
        /// Boton Editar: carga la cuota seleccionada en el formulario.
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var cuota = dgCuotas.SelectedItem as Cuotas; // Obtenemos seleccion.
            if (cuota == null) return; // Seguridad por si el objeto viene a null.
            CargarFormularioDesdeEntidad(cuota); // Cargamos en formulario.
        }

        /// <summary>
        /// Boton Marcar Pagada: marca la cuota seleccionada como pagada.
        /// </summary>
        private void BtnMarcarPagada_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cuota = dgCuotas.SelectedItem as Cuotas; // Obtenemos seleccion.
                if (cuota == null) return; // Seguridad por si el objeto viene a null.

                // Pedimos confirmacion al usuario.
                var confirmar = ModalMessage.ShowModal(
                    $"Desea marcar como pagada la cuota de {cuota.Socios?.NombreCompleto} ({cuota.Mes}/{cuota.Anio})?",
                    "Cuotas", 3);
                if (!confirmar) return; // Cancelado.

                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                if (_controller.MarcarPagada(cuota, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario(); // Limpiamos formulario.
                    CargarDatos(); // Refrescamos grid.
                    ModalMessage.ShowModal("Cuota marcada como pagada.", "Cuotas", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error de validacion.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado: {ex.Message}", "Cuotas", 1);
            }
        }

        /// <summary>
        /// Boton Cancelar: limpia el formulario.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario(); // Limpiamos y reseteamos valores por defecto.
        }

        /// <summary>
        /// Boton Guardar: valida y guarda la cuota.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                var cuota = CrearEntidadDesdeFormulario(); // Construimos entidad.

                if (_controller.Guardar(cuota, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario(); // Limpiamos formulario.
                    CargarDatos(); // Refrescamos grid.
                    ModalMessage.ShowModal("Cuota guardada correctamente.", "Cuotas", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error de validacion.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Cuotas", 1);
            }
        }

        /// <summary>
        /// Boton Refrescar: recarga la lista de cuotas.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos.
        }

        /// <summary>
        /// Boton Borrar: borra fisicamente la cuota seleccionada (solo si no esta pagada).
        /// </summary>
        private void BtnBorrarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var cuota = dgCuotas.SelectedItem as Cuotas; // Obtenemos seleccion.
                if (cuota == null) return; // Seguridad por si el objeto viene a null.

                // Pedimos confirmacion al usuario.
                var confirmar = ModalMessage.ShowModal(
                    $"Desea borrar la cuota de {cuota.Socios?.NombreCompleto} ({cuota.Mes}/{cuota.Anio})? Esta accion no se puede deshacer.",
                    "Cuotas", 3);
                if (!confirmar) return; // Cancelado.

                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                if (_controller.Borrar(cuota, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario(); // Limpiamos formulario.
                    CargarDatos(); // Refrescamos grid.
                    ModalMessage.ShowModal("Cuota borrada correctamente.", "Cuotas", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error de validacion o negocio.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado al borrar: {ex.Message}", "Cuotas", 1);
            }
        }

        /// <summary>
        /// Boton lupa: abre el modal de busqueda de socios y asigna el seleccionado al combo.
        /// </summary>
        private void BtnBuscarSocio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new ModalBuscarSocio(); // Creamos el modal.
                modal.Owner = Window.GetWindow(this); // Asignamos ventana padre.

                if (modal.ShowDialog() == true && modal.SocioSeleccionado != null)
                {
                    // Buscamos el item en el combo que coincida con el socio elegido.
                    cmbSocio.SelectedItem = (cmbSocio.ItemsSource as List<Socios>)
                        ?.Find(s => s.SocioId == modal.SocioSeleccionado.SocioId);
                }
            }
            catch (Exception ex)
            {
                // Mostramos error inesperado.
                ModalMessage.ShowModal($"Error al buscar socio: {ex.Message}", "Cuotas", 1);
            }
        }
    }
}
