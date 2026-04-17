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
    /// Vista para gestionar las ofertas de la inmobiliaria
    /// Permite listar, crear, editar y eliminar ofertas
    /// </summary>
    public partial class VistaOfertas : UserControl
    {
        // Controladores para gestionar la logica de negocio
        private OfertasController _controller;
        private InmueblesController _inmueblesController;
        private ClientesController _clientesController;
        private AgentesController _agentesController;

        // Oferta actualmente seleccionada para edicion
        private Ofertas _ofertaSeleccionada;

        /// <summary>
        /// Constructor de la vista
        /// </summary>
        public VistaOfertas()
        {
            InitializeComponent();

            // Inicializar los controladores
            _controller = new OfertasController();
            _inmueblesController = new InmueblesController();
            _clientesController = new ClientesController();
            _agentesController = new AgentesController();

            // Configurar fecha actual por defecto
            dpFechaOferta.SelectedDate = DateTime.Now;

            // Cargar los valores del ComboBox de estados
            CargarEstados();

            // Cargar los ComboBox
            CargarClientes();
            CargarAgentes();
            CargarInmuebles();

            // Cargar la lista de ofertas al iniciar
            CargarOfertas();
        }

        /// <summary>
        /// Cargar los valores del ComboBox de estados
        /// </summary>
        private void CargarEstados()
        {
            cmbEstado.ItemsSource = OfertasConstantes.Estados;

            // Estado inicial pendiente que es lo normal
            cmbEstado.SelectedItem = OfertasConstantes.ESTADO_PENDIENTE;
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
                
                // Seleccionar el primer agente por defecto si existe
                //if (agentes.Count > 0)
                //{
                //    cmbAgente.SelectedIndex = 0;
                //}
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
        /// Cargar inmuebles filtrados por tipo de operacion
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
        /// Cargar todas las ofertas en el DataGrid
        /// </summary>
        private void CargarOfertas()
        {
            try
            {
                List<Ofertas> ofertas = _controller.ObtenerTodas();
                dgOfertas.ItemsSource = ofertas;
                txtTotalOfertas.Text = $"{ofertas.Count} oferta{(ofertas.Count != 1 ? "s" : "")}";
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar las ofertas: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }


        /// <summary>
        /// Guardar o actualizar una oferta
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                decimal precio;
                if (!decimal.TryParse(txtPrecioOferta.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out precio))
                {
                    ModalMessage.Show(
                        "El precio ofertado debe ser un numero valido.",
                        "RPecio no valido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (!dpFechaOferta.SelectedDate.HasValue)
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
                string estado = cmbEstado.SelectedItem?.ToString();

                // Validar que se haya seleccionado un agente
                if (agente == null || agente.IdAgente == 0)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un agente.",
                        "Agente requerido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                Ofertas oferta = new Ofertas
                {
                    IdCliente = cliente.IdCliente,
                    IdAgente = agente.IdAgente,
                    IdInmueble = inmueble.IdInmueble,
                    PrecioOfertado = precio,
                    Estado = estado,
                    FechaOferta = dpFechaOferta.SelectedDate.Value,
                    Observaciones = txtObservaciones.Text.Trim()
                };

                if (_ofertaSeleccionada != null)
                {
                    oferta.IdOferta = _ofertaSeleccionada.IdOferta;
                }

                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                bool resultado = _controller.Guardar(oferta, ref tituloError, ref mensajeError);

                if (resultado)
                {
                    string accion = _ofertaSeleccionada != null ? "actualizada" : "registrada";
                    ModalMessage.Show(
                        $"La oferta ha sido {accion} correctamente.",
                        "Operacion exitosa",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );

                    CargarOfertas();
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
        /// Cargar los datos de la oferta seleccionada en el formulario para editar
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ofertas oferta = dgOfertas.SelectedItem as Ofertas;

                if (oferta == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una oferta de la lista para poder editarla.",
                        "Ninguna oferta seleccionada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (!_controller.PuedeEditarse(oferta.IdOferta))
                {
                    ModalMessage.Show(
                        "Esta oferta no se puede editar porque el inmueble ya tiene una operacion asociada.",
                        "Oferta bloqueada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                _ofertaSeleccionada = oferta;

                // Establecer el tipo de operación para cargar los inmuebles correctos
                if (oferta.Inmuebles.TipoOperacion == InmueblesConstantes.TIPO_VENTA)
                {
                    rbVenta.IsChecked = true;
                }
                else
                {
                    rbAlquiler.IsChecked = true;
                }

                // Asignar los valores después de que los inmuebles estén cargados
                cmbCliente.SelectedValue = oferta.IdCliente;
                cmbAgente.SelectedValue = oferta.IdAgente;
                cmbInmueble.SelectedValue = oferta.IdInmueble;
                txtPrecioOferta.Text = Utils.FormatearPrecio(oferta.PrecioOfertado, true);
                dpFechaOferta.SelectedDate = oferta.FechaOferta;
                cmbEstado.SelectedItem = oferta.Estado;
                txtObservaciones.Text = oferta.Observaciones;

                cmbEstado.IsEnabled = true;
                btnGuardar.Content = "Actualizar";
                txtPrecioOferta.Focus();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar la oferta: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Eliminar la oferta seleccionada despues de confirmar
        /// </summary>
        private void BtnEliminarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ofertas oferta = dgOfertas.SelectedItem as Ofertas;

                if (oferta == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una oferta de la lista para poder eliminarla.",
                        "Ninguna oferta seleccionada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (!_controller.PuedeEliminarse(oferta.IdOferta))
                {
                    ModalMessage.Show(
                        "Esta oferta no se puede eliminar porque el inmueble ya tiene una operacion asociada.",
                        "Oferta bloqueada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                bool confirmar = ModalMessage.Show(
                    $"¿Esta seguro de eliminar la oferta de '{oferta.Clientes.Nombre}' por el inmueble en '{oferta.Inmuebles.Direccion}'?\n\nEsta accion no se puede deshacer.",
                    "Confirmar eliminacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    bool resultado = _controller.Eliminar(oferta, ref tituloError, ref mensajeError);

                    if (resultado)
                    {
                        ModalMessage.Show(
                            "La oferta ha sido eliminada correctamente.",
                            "Oferta eliminada",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        CargarOfertas();

                        if (_ofertaSeleccionada?.IdOferta == oferta.IdOferta)
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
                    $"Error al eliminar la oferta: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Generar una operacion a partir de la oferta seleccionada
        /// </summary>
        private void BtnGenerarOperacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Ofertas oferta = dgOfertas.SelectedItem as Ofertas;

                if (oferta == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar una oferta de la lista.",
                        "Ninguna oferta seleccionada",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                if (!_controller.PuedeGenerarOperacion(oferta))
                {
                    ModalMessage.Show(
                        "Solo se pueden generar operaciones desde ofertas ACEPTADAS sin operacion existente.",
                        "Accion no permitida",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                bool confirmar = ModalMessage.Show(
                    $"¿Desea generar una operacion desde esta oferta?\n\nCliente: {oferta.Clientes.Nombre}\nInmueble: {oferta.Inmuebles.Direccion}\nPrecio: {Utils.FormatearPrecio(oferta.PrecioOfertado)}",
                    "Confirmar generacion de operacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    Operaciones operacion = _controller.GenerarOperacion(oferta, ref tituloError, ref mensajeError);

                    if (operacion != null)
                    {
                        ModalMessage.Show(
                            "La operacion ha sido generada correctamente.",
                            "Operacion creada",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        CargarOfertas();
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
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al generar la operacion: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Refrescar la lista de ofertas
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarOfertas();
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
            if (cmbInmueble != null)
            {
                CargarInmuebles();
            }
        }

        /// <summary>
        /// Evento cuando se selecciona una oferta en el DataGrid
        /// </summary>
        private void DgOfertas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Ofertas oferta = dgOfertas.SelectedItem as Ofertas;

            if (oferta != null)
            {
                btnGenerarOperacion.IsEnabled = _controller.PuedeGenerarOperacion(oferta);
                btnEliminarSeleccionado.IsEnabled = _controller.PuedeEliminarse(oferta.IdOferta);
                btnEditarSeleccionado.IsEnabled = _controller.PuedeEditarse(oferta.IdOferta);
            }
            else btnGenerarOperacion.IsEnabled = false;


            
        }

        /// <summary>
        /// Limpiar todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            txtPrecioOferta.Clear();
            txtObservaciones.Clear();
            dpFechaOferta.SelectedDate = DateTime.Now;
            rbVenta.IsChecked = true;
            cmbCliente.SelectedIndex = -1;
            cmbAgente.SelectedIndex = -1;//cmbAgente.Items.Count > 0 ? 0 : -1;
            cmbInmueble.SelectedIndex = -1;
            cmbEstado.SelectedItem = OfertasConstantes.ESTADO_PENDIENTE;
            _ofertaSeleccionada = null;
            btnGuardar.Content = "Guardar";
            dgOfertas.SelectedItem = null;
            btnGenerarOperacion.IsEnabled = false;
            btnEditarSeleccionado.IsEnabled = false;
            btnEliminarSeleccionado.IsEnabled = false;
            txtPrecioOferta.Focus();
            cmbEstado.IsEnabled = false;
        }
    }
}
