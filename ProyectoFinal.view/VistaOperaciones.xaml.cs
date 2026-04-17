using Comun;
using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Vista para gestionar las operaciones de la inmobiliaria
    /// Permite listar, crear, editar y eliminar operaciones
    /// </summary>
    public partial class VistaOperaciones : UserControl
    {
        // Controladores para gestionar la logica de negocio
        private OperacionesController _controller;
        private InmueblesController _inmueblesController;
        private ClientesController _clientesController;
        private AgentesController _agentesController;

        // Operacion actualmente seleccionada para edicion
        private Operaciones _operacionSeleccionada;

        /// <summary>
        /// Constructor de la vista
        /// </summary>
        public VistaOperaciones()
        {
            InitializeComponent();

            // Inicializar los controladores
            _controller = new OperacionesController();
            _inmueblesController = new InmueblesController();
            _clientesController = new ClientesController();
            _agentesController = new AgentesController();

            // Configurar fecha actual por defecto
            dpFechaOperacion.SelectedDate = DateTime.Now;

            // Cargar los ComboBox
            CargarClientes();
            CargarAgentes();
            CargarInmuebles();

            // Cargar la lista de operaciones al iniciar
            CargarOperaciones();
        }

        /// <summary>
        /// Cargar todos los clientes en el ComboBox
        /// </summary>
        private void CargarClientes()
        {
            try
            {
                List<Clientes> clientes = _clientesController.ObtenerTodos();
                cmbCliente.ItemsSource = clientes;
                cmbCliente.SelectedValuePath = "IdCliente";
                cmbCliente.DisplayMemberPath = "Nombre";
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar clientes: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Cargar todos los agentes en el ComboBox
        /// </summary>
        private void CargarAgentes()
        {
            try
            {
                List<Agentes> agentes = _agentesController.ObtenerTodos();

                cmbAgente.ItemsSource = agentes;
                cmbAgente.SelectedValuePath = "IdAgente";
                cmbAgente.DisplayMemberPath = "Nombre";
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar agentes: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Cargar inmuebles filtrados por tipo de operacion y disponibilidad
        /// </summary>
        private void CargarInmuebles()
        {
            try
            {
                string tipoOperacion = rbVenta.IsChecked == true ? InmueblesConstantes.TIPO_VENTA : InmueblesConstantes.TIPO_ALQUILER;

                List<Inmuebles> inmuebles = _inmueblesController.ObtenerTodos()
                    .Where(i => i.TipoOperacion == tipoOperacion &&
                                i.Estado == InmueblesConstantes.ESTADO_DISPONIBLE)
                    .ToList();

                cmbInmueble.ItemsSource = inmuebles;
                cmbInmueble.SelectedValuePath = "IdInmueble";
                cmbInmueble.DisplayMemberPath = "DireccionCompleta";
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar inmuebles: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Cargar todas las operaciones en el DataGrid
        /// </summary>
        private void CargarOperaciones()
        {
            try
            {
                List<Operaciones> operaciones = _controller.ObtenerTodas();
                dgOperaciones.ItemsSource = operaciones;
                txtTotalOperaciones.Text = $"{operaciones.Count} operacion{(operaciones.Count != 1 ? "es" : "")}";
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar las operaciones: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Guardar o actualizar una operacion
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validaciones básicas de campos
                if (cmbCliente.SelectedItem == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un cliente.",
                        "Cliente requerido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (cmbInmueble.SelectedItem == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un inmueble.",
                        "Inmueble requerido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (cmbAgente.SelectedItem == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un agente.",
                        "Agente requerido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                decimal precio;
                if (!decimal.TryParse(txtPrecioFinal.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out precio))
                {
                    ModalMessage.Show(
                        "El precio final debe ser un numero valido.",
                        "Precio no valido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (!dpFechaOperacion.SelectedDate.HasValue)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una fecha.",
                        "Fecha requerida",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                Clientes cliente = cmbCliente.SelectedItem as Clientes;
                Agentes agente = cmbAgente.SelectedItem as Agentes;
                Inmuebles inmueble = cmbInmueble.SelectedItem as Inmuebles;
                string tipoOperacion = rbVenta.IsChecked == true ? InmueblesConstantes.TIPO_VENTA : InmueblesConstantes.TIPO_ALQUILER;

                Operaciones operacion = new Operaciones
                {
                    IdCliente = cliente.IdCliente,
                    IdAgente = agente.IdAgente,
                    IdInmueble = inmueble.IdInmueble,
                    TipoOperacion = tipoOperacion,
                    PrecioFinal = precio,
                    FechaOperacion = dpFechaOperacion.SelectedDate.Value,
                    Observaciones = txtObservaciones.Text.Trim()
                };

                // Si estamos editando una operación
                if (_operacionSeleccionada != null)
                {
                    operacion.IdOperacion = _operacionSeleccionada.IdOperacion;
                    
                    // Si la operación viene de una oferta, mantener el IdOferta
                    if (_controller.VieneDeOferta(_operacionSeleccionada))
                    {
                        operacion.IdOferta = _operacionSeleccionada.IdOferta;
                        
                        // Si viene de oferta, solo se pueden editar fecha y precio
                        // Mantener los valores originales de los demás campos
                        operacion.IdCliente = _operacionSeleccionada.IdCliente;
                        operacion.IdAgente = _operacionSeleccionada.IdAgente;
                        operacion.IdInmueble = _operacionSeleccionada.IdInmueble;
                        operacion.TipoOperacion = _operacionSeleccionada.TipoOperacion;
                    }
                }

                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                bool resultado = _controller.Guardar(operacion, ref tituloError, ref mensajeError);

                if (resultado)
                {
                    string accion = _operacionSeleccionada != null ? "actualizada" : "registrada";
                    ModalMessage.Show(
                        $"La operacion ha sido {accion} correctamente.",
                        "Operacion exitosa",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );

                    CargarOperaciones();
                    LimpiarFormulario();
                }
                else
                {
                    ModalMessage.Show(
                        mensajeError,
                        tituloError,
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error inesperado: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Cancelar la operacion actual y limpiar el formulario
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
        }

        /// <summary>
        /// Cargar los datos de la operacion seleccionada en el formulario para editar
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Operaciones operacion = dgOperaciones.SelectedItem as Operaciones;

                if (operacion == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una operacion de la lista para poder editarla.",
                        "Ninguna operacion seleccionada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                _operacionSeleccionada = operacion;

                bool vieneDeOferta = _controller.VieneDeOferta(operacion);

                // Si viene de oferta, bloquear campos excepto fecha y precio
                if (vieneDeOferta)
                {
                    // Deshabilitar campos que no se pueden editar
                    rbVenta.IsEnabled = false;
                    rbAlquiler.IsEnabled = false;
                    cmbCliente.IsEnabled = false;
                    cmbAgente.IsEnabled = false;
                    cmbInmueble.IsEnabled = false;
                    btnBuscarCliente.IsEnabled = false;
                    btnBuscarAgente.IsEnabled = false;
                    btnBuscarInmueble.IsEnabled = false;
                }
                else
                {
                    // Habilitar todos los campos
                    rbVenta.IsEnabled = true;
                    rbAlquiler.IsEnabled = true;
                    cmbCliente.IsEnabled = true;
                    cmbAgente.IsEnabled = true;
                    cmbInmueble.IsEnabled = true;
                    btnBuscarCliente.IsEnabled = true;
                    btnBuscarAgente.IsEnabled = true;
                    btnBuscarInmueble.IsEnabled = true;
                }

                // Establecer el tipo de operación
                if (operacion.TipoOperacion == InmueblesConstantes.TIPO_VENTA)
                {
                    rbVenta.IsChecked = true;
                }
                else
                {
                    rbAlquiler.IsChecked = true;
                }

                // Cargar inmuebles antes de asignar el valor
                if (!vieneDeOferta)
                {
                    CargarInmuebles();
                }
                else
                {
                    // Si viene de oferta, cargar todos los inmuebles para mostrar el actual
                    List<Inmuebles> inmuebles = _inmueblesController.ObtenerTodos()
                        .Where(i => i.TipoOperacion == operacion.TipoOperacion)
                        .ToList();
                    cmbInmueble.ItemsSource = inmuebles;
                }

                // Asignar los valores
                cmbCliente.SelectedValue = operacion.IdCliente;
                cmbAgente.SelectedValue = operacion.IdAgente;
                cmbInmueble.SelectedValue = operacion.IdInmueble;
                txtPrecioFinal.Text = Utils.FormatearPrecio(operacion.PrecioFinal, true);
                dpFechaOperacion.SelectedDate = operacion.FechaOperacion;
                txtObservaciones.Text = operacion.Observaciones;

                btnGuardar.Content = "Actualizar";
                txtPrecioFinal.Focus();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar la operacion: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Eliminar la operacion seleccionada despues de confirmar
        /// </summary>
        private void BtnEliminarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Operaciones operacion = dgOperaciones.SelectedItem as Operaciones;

                if (operacion == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una operacion de la lista para poder eliminarla.",
                        "Ninguna operacion seleccionada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                bool confirmar = ModalMessage.Show(
                    $"¿Esta seguro de eliminar esta operacion?\n\nCliente: {operacion.Clientes.Nombre}\nInmueble: {operacion.Inmuebles.Direccion}\nPrecio: {Utils.FormatearPrecio(operacion.PrecioFinal)}\n\nEsta accion no se puede deshacer y el inmueble volvera a estar DISPONIBLE.",
                    "Confirmar eliminacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    bool resultado = _controller.Eliminar(operacion, ref tituloError, ref mensajeError);

                    if (resultado)
                    {
                        ModalMessage.Show(
                            "La operacion ha sido eliminada correctamente.",
                            "Operacion eliminada",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        CargarOperaciones();

                        if (_operacionSeleccionada?.IdOperacion == operacion.IdOperacion)
                        {
                            LimpiarFormulario();
                        }
                    }
                    else
                    {
                        ModalMessage.Show(
                            mensajeError,
                            tituloError,
                            ModalMessageType.Error,
                            Window.GetWindow(this)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al eliminar la operacion: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Refrescar la lista de operaciones
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarOperaciones();
            CargarClientes();
            CargarAgentes();
            CargarInmuebles();
            LimpiarFormulario();
        }

        /// <summary>
        /// Abrir modal para buscar un cliente
        /// </summary>
        private void BtnBuscarCliente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new ModalBuscarCliente();
                modal.Owner = Window.GetWindow(this);

                if (modal.ShowDialog() == true && modal.ClienteSeleccionado != null)
                {
                    cmbCliente.SelectedValue = modal.ClienteSeleccionado.IdCliente;
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al buscar cliente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Abrir modal para buscar un agente
        /// </summary>
        private void BtnBuscarAgente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new ModalBuscarAgente();
                modal.Owner = Window.GetWindow(this);

                if (modal.ShowDialog() == true && modal.AgenteSeleccionado != null)
                {
                    cmbAgente.SelectedValue = modal.AgenteSeleccionado.IdAgente;
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al buscar agente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Abrir modal para buscar un inmueble
        /// Se le pasa el tipo de operación para que filtre por ella
        /// </summary>
        private void BtnBuscarInmueble_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tipoOperacion = rbVenta.IsChecked == true
                    ? InmueblesConstantes.TIPO_VENTA
                    : InmueblesConstantes.TIPO_ALQUILER;

                var modal = new ModalBuscarInmueble(tipoOperacion);
                modal.Owner = Window.GetWindow(this);

                if (modal.ShowDialog() == true && modal.InmuebleSeleccionado != null)
                {
                    cmbInmueble.SelectedValue = modal.InmuebleSeleccionado.IdInmueble;
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al buscar inmueble: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Evento cuando cambia el tipo de operacion
        /// </summary>
        private void RbTipoOperacion_Changed(object sender, RoutedEventArgs e)
        {
            if (cmbInmueble != null && _operacionSeleccionada == null)
            {
                CargarInmuebles();
            }
        }

        /// <summary>
        /// Evento cuando se selecciona una operacion en el DataGrid
        /// </summary>
        private void DgOperaciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Operaciones operacion = dgOperaciones.SelectedItem as Operaciones;

            if (operacion != null)
            {
                btnEditarSeleccionado.IsEnabled = true;
                btnEliminarSeleccionado.IsEnabled = true;
            }
            else
            {
                btnEditarSeleccionado.IsEnabled = false;
                btnEliminarSeleccionado.IsEnabled = false;
            }
        }

        /// <summary>
        /// Limpiar todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            txtPrecioFinal.Clear();
            txtObservaciones.Clear();
            dpFechaOperacion.SelectedDate = DateTime.Now;
            rbVenta.IsChecked = true;
            cmbCliente.SelectedIndex = -1;
            cmbAgente.SelectedIndex = -1;
            cmbInmueble.SelectedIndex = -1;
            _operacionSeleccionada = null;
            btnGuardar.Content = "Guardar";
            dgOperaciones.SelectedItem = null;
            btnEditarSeleccionado.IsEnabled = false;
            btnEliminarSeleccionado.IsEnabled = false;
            txtPrecioFinal.Focus();

            // Habilitar todos los campos
            rbVenta.IsEnabled = true;
            rbAlquiler.IsEnabled = true;
            cmbCliente.IsEnabled = true;
            cmbAgente.IsEnabled = true;
            cmbInmueble.IsEnabled = true;
            btnBuscarCliente.IsEnabled = true;
            btnBuscarAgente.IsEnabled = true;
            btnBuscarInmueble.IsEnabled = true;

            // Recargar inmuebles disponibles
            CargarInmuebles();
        }
    }
}
