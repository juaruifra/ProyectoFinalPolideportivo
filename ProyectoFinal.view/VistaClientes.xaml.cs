using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Vista para gestionar los clientes de la inmobiliaria
    /// Permite listar, crear, editar y eliminar clientes
    /// </summary>
    public partial class VistaClientes : UserControl
    {
        // Controlador para gestionar la logica de negocio
        private ClientesController _controller;

        // Cliente actualmente seleccionado para edicion
        private Clientes _clienteSeleccionado;

        /// <summary>
        /// Constructor de la vista
        /// </summary>
        public VistaClientes()
        {
            InitializeComponent();

            // Inicializar el controlador
            _controller = new ClientesController();

            // Cargar la lista de clientes al iniciar
            CargarClientes();
        }

        // ================= CARGA DE DATOS =================

        /// <summary>
        /// Cargar todos los clientes en el DataGrid
        /// </summary>
        private void CargarClientes()
        {
            try
            {
                // Obtener todos los clientes a traves del controlador
                List<Clientes> clientes = _controller.ObtenerTodos();

                // Asignar al DataGrid
                dgClientes.ItemsSource = clientes;

                // Actualizar contador
                txtTotalClientes.Text = $"{clientes.Count} cliente{(clientes.Count != 1 ? "s" : "")}";
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error
                ModalMessage.Show(
                    $"Error al cargar los clientes: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        // ================= EVENTOS DE BOTONES =================

        /// <summary>
        /// Guardar o actualizar un cliente
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear objeto cliente con los datos del formulario
                Clientes cliente = new Clientes
                {
                    Nombre = txtNombre.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim()
                };

                // Si hay un cliente seleccionado, estamos editando
                if (_clienteSeleccionado != null)
                {
                    cliente.IdCliente = _clienteSeleccionado.IdCliente;
                    cliente.FechaAlta = _clienteSeleccionado.FechaAlta;
                }

                // Variables para capturar errores
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                // Intentar guardar el cliente a traves del controlador
                bool resultado = _controller.Guardar(cliente, ref tituloError, ref mensajeError);

                if (resultado)
                {
                    // Mostrar mensaje de exito
                    string accion = _clienteSeleccionado != null ? "actualizado" : "registrado";
                    ModalMessage.Show(
                        $"El cliente ha sido {accion} correctamente.",
                        "Operacion exitosa",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );

                    // Recargar la lista
                    CargarClientes();

                    // Limpiar el formulario
                    LimpiarFormulario();
                }
                else
                {
                    // Mostrar mensaje de error con validaciones del controlador
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
                // Mostrar error inesperado
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
        /// Cargar los datos del cliente seleccionado en el formulario para editar
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el cliente seleccionado del DataGrid
                Clientes cliente = dgClientes.SelectedItem as Clientes;

                // Validar que hay un cliente seleccionado
                if (cliente == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un cliente de la lista para poder editarlo.",
                        "Ningun cliente seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Guardar referencia al cliente seleccionado
                _clienteSeleccionado = cliente;

                // Cargar datos en el formulario
                txtNombre.Text = cliente.Nombre;
                txtEmail.Text = cliente.Email;
                txtTelefono.Text = cliente.Telefono;

                // Cambiar el texto del boton
                btnGuardar.Content = "Actualizar";

                // Enfocar el primer campo
                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar el cliente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Eliminar el cliente seleccionado despues de confirmar
        /// </summary>
        private void BtnEliminarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el cliente seleccionado del DataGrid
                Clientes cliente = dgClientes.SelectedItem as Clientes;

                // Validar que hay un cliente seleccionado
                if (cliente == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un cliente de la lista para poder eliminarlo.",
                        "Ningun cliente seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Confirmar eliminacion
                bool confirmar = ModalMessage.Show(
                    $"Esta seguro de eliminar al cliente '{cliente.Nombre}'?\n\nEsta accion no se puede deshacer.",
                    "Confirmar eliminacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    // Variables para capturar errores
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    // Intentar eliminar el cliente a traves del controlador
                    bool resultado = _controller.Eliminar(cliente, ref tituloError, ref mensajeError);

                    if (resultado)
                    {
                        // Mostrar mensaje de exito
                        ModalMessage.Show(
                            "El cliente ha sido eliminado correctamente.",
                            "Cliente eliminado",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        // Recargar la lista
                        CargarClientes();

                        // Limpiar formulario si se estaba editando este cliente
                        if (_clienteSeleccionado?.IdCliente == cliente.IdCliente)
                        {
                            LimpiarFormulario();
                        }
                    }
                    else
                    {
                        // Mostrar mensaje de error con validaciones del controlador
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
                    $"Error al eliminar el cliente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Refrescar la lista de clientes
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar clientes y limpiar formulario
            CargarClientes();
            LimpiarFormulario();
        }

        /// <summary>
        /// Evento cuando se selecciona un cliente en el DataGrid
        /// </summary>
        private void DgClientes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Este evento se puede usar para futuras funcionalidades
            // Por ejemplo, mostrar un resumen del cliente seleccionado
        }

        // ================= METODOS AUXILIARES =================

        /// <summary>
        /// Limpiar todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            // Limpiar campos de texto
            txtNombre.Clear();
            txtEmail.Clear();
            txtTelefono.Clear();

            // Resetear cliente seleccionado
            _clienteSeleccionado = null;

            // Restaurar texto del boton
            btnGuardar.Content = "Guardar";

            // Limpiar seleccion del DataGrid
            dgClientes.SelectedItem = null;

            // Enfocar el primer campo
            txtNombre.Focus();
        }
    }
}
