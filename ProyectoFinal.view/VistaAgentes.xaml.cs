using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Vista para gestionar los agentes de la inmobiliaria
    /// Permite listar, crear, editar y eliminar agentes
    /// </summary>
    public partial class VistaAgentes : UserControl
    {
        // Controlador para gestionar la logica de negocio
        private AgentesController _controller;

        // Agente actualmente seleccionado para edicion
        private Agentes _agenteSeleccionado;

        /// <summary>
        /// Constructor de la vista
        /// </summary>
        public VistaAgentes()
        {
            InitializeComponent();

            // Inicializar el controlador
            _controller = new AgentesController();

            // Cargar la lista de agentes al iniciar
            CargarAgentes();
        }

        // ================= CARGA DE DATOS =================

        /// <summary>
        /// Cargar todos los agentes en el DataGrid
        /// </summary>
        private void CargarAgentes()
        {
            try
            {
                // Obtener todos los agentes a traves del controlador
                List<Agentes> agentes = _controller.ObtenerTodos();

                // Asignar al DataGrid
                dgAgentes.ItemsSource = agentes;

                // Actualizar contador
                txtTotalAgentes.Text = $"{agentes.Count} agente{(agentes.Count != 1 ? "s" : "")}";
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error
                ModalMessage.Show(
                    $"Error al cargar los agentes: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        // ================= EVENTOS DE BOTONES =================

        /// <summary>
        /// Guardar o actualizar un agente
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear objeto agente con los datos del formulario
                Agentes agente = new Agentes
                {
                    Nombre = txtNombre.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Telefono = txtTelefono.Text.Trim()
                };

                // Si hay un agente seleccionado, estamos editando
                if (_agenteSeleccionado != null)
                {
                    agente.IdAgente = _agenteSeleccionado.IdAgente;
                    agente.FechaAlta = _agenteSeleccionado.FechaAlta;
                }

                // Variables para capturar errores
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                // Intentar guardar el agente a traves del controlador
                bool resultado = _controller.Guardar(agente, ref tituloError, ref mensajeError);

                if (resultado)
                {
                    // Mostrar mensaje de exito
                    string accion = _agenteSeleccionado != null ? "actualizado" : "registrado";
                    ModalMessage.Show(
                        $"El agente ha sido {accion} correctamente.",
                        "Operacion exitosa",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );

                    // Recargar la lista
                    CargarAgentes();

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
        /// Cargar los datos del agente seleccionado en el formulario para editar
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el agente seleccionado del DataGrid
                Agentes agente = dgAgentes.SelectedItem as Agentes;

                // Validar que hay un agente seleccionado
                if (agente == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un agente de la lista para poder editarlo.",
                        "Ningun agente seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Guardar referencia al agente seleccionado
                _agenteSeleccionado = agente;

                // Cargar datos en el formulario
                txtNombre.Text = agente.Nombre;
                txtEmail.Text = agente.Email;
                txtTelefono.Text = agente.Telefono;

                // Cambiar el texto del boton
                btnGuardar.Content = "Actualizar";

                // Enfocar el primer campo
                txtNombre.Focus();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar el agente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Eliminar el agente seleccionado despues de confirmar
        /// </summary>
        private void BtnEliminarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el agente seleccionado del DataGrid
                Agentes agente = dgAgentes.SelectedItem as Agentes;

                // Validar que hay un agente seleccionado
                if (agente == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un agente de la lista para poder eliminarlo.",
                        "Ningun agente seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Confirmar eliminacion
                bool confirmar = ModalMessage.Show(
                    $"Esta seguro de eliminar al agente '{agente.Nombre}'?\n\nEsta accion no se puede deshacer.",
                    "Confirmar eliminacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    // Variables para capturar errores
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    // Intentar eliminar el agente a traves del controlador
                    bool resultado = _controller.Eliminar(agente, ref tituloError, ref mensajeError);

                    if (resultado)
                    {
                        // Mostrar mensaje de exito
                        ModalMessage.Show(
                            "El agente ha sido eliminado correctamente.",
                            "Agente eliminado",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        // Recargar la lista
                        CargarAgentes();

                        // Limpiar formulario si se estaba editando este agente
                        if (_agenteSeleccionado?.IdAgente == agente.IdAgente)
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
                    $"Error al eliminar el agente: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Refrescar la lista de agentes
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar agentes y limpiar formulario
            CargarAgentes();
            LimpiarFormulario();
        }

        /// <summary>
        /// Evento cuando se selecciona un agente en el DataGrid
        /// </summary>
        private void DgAgentes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Este evento se puede usar para futuras funcionalidades
            // Por ejemplo, mostrar un resumen del agente seleccionado
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

            // Resetear agente seleccionado
            _agenteSeleccionado = null;

            // Restaurar texto del boton
            btnGuardar.Content = "Guardar";

            // Limpiar seleccion del DataGrid
            dgAgentes.SelectedItem = null;

            // Enfocar el primer campo
            txtNombre.Focus();
        }
    }
}
