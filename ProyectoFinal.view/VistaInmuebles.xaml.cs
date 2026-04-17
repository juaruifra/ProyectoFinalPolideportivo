using Comun;
using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Vista para gestionar los inmuebles de la inmobiliaria
    /// Permite listar, crear, editar y eliminar inmuebles
    /// </summary>
    public partial class VistaInmuebles : UserControl
    {
        // Controlador para gestionar la logica de negocio
        private InmueblesController _controller;

        // Inmueble actualmente seleccionado para edicion
        private Inmuebles _inmuebleSeleccionado;

        /// <summary>
        /// Constructor de la vista
        /// </summary>
        public VistaInmuebles()
        {
            InitializeComponent();

            // Inicializar el controlador
            _controller = new InmueblesController();

            // Cargar los valores del ComboBox de estados
            CargarEstados();

            // Cargar la lista de inmuebles al iniciar
            CargarInmuebles();
        }

        // ================= CARGA DE DATOS =================

        /// <summary>
        /// Cargar los valores del ComboBox de estados
        /// </summary>
        private void CargarEstados()
        {
            // Cargar los estados desde las constantes
            cmbEstado.ItemsSource = InmueblesConstantes.Estados;
            
            // Seleccionar "DISPONIBLE" por defecto
            cmbEstado.SelectedItem = InmueblesConstantes.ESTADO_DISPONIBLE;
        }

        /// <summary>
        /// Cargar todos los inmuebles en el DataGrid
        /// </summary>
        private void CargarInmuebles()
        {
            try
            {
                // Obtener todos los inmuebles a traves del controlador
                List<Inmuebles> inmuebles = _controller.ObtenerTodos();

                // Asignar al DataGrid
                dgInmuebles.ItemsSource = inmuebles;

                // Actualizar contadores
                txtTotalInmuebles.Text = $"{inmuebles.Count} inmueble{(inmuebles.Count != 1 ? "s" : "")}";
                
                // Obtener total de disponibles
                int disponibles = _controller.ObtenerTotalDisponibles();
                txtDisponibles.Text = $"{disponibles} disponible{(disponibles != 1 ? "s" : "")}";
            }
            catch (Exception ex)
            {
                // Mostrar mensaje de error
                ModalMessage.Show(
                    $"Error al cargar los inmuebles: {ex.Message}",
                    "Error de carga",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        // ================= EVENTOS DE BOTONES =================

        /// <summary>
        /// Guardar o actualizar un inmueble
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que el precio sea un numero valido
                decimal precio;
                if (!decimal.TryParse(txtPrecio.Text.Trim(), NumberStyles.Number, CultureInfo.CurrentCulture, out precio))
                {
                    ModalMessage.Show(
                        "El precio debe ser un numero valido.",
                        "Precio no valido",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Obtener el tipo de operacion seleccionado
                string tipoOperacion = rbVenta.IsChecked == true 
                    ? InmueblesConstantes.TIPO_VENTA 
                    : InmueblesConstantes.TIPO_ALQUILER;

                // Obtener el estado seleccionado
                string estado = cmbEstado.SelectedItem?.ToString();

                // Crear objeto inmueble con los datos del formulario
                Inmuebles inmueble = new Inmuebles
                {
                    Direccion = txtDireccion.Text.Trim(),
                    Ciudad = txtCiudad.Text.Trim(),
                    Precio = precio,
                    TipoOperacion = tipoOperacion,
                    Estado = estado
                };

                // Si hay un inmueble seleccionado, estamos editando
                if (_inmuebleSeleccionado != null)
                {
                    inmueble.IdInmueble = _inmuebleSeleccionado.IdInmueble;
                    inmueble.FechaAlta = _inmuebleSeleccionado.FechaAlta;
                }

                // Variables para capturar errores
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                // Intentar guardar el inmueble a traves del controlador
                bool resultado = _controller.Guardar(inmueble, ref tituloError, ref mensajeError);

                if (resultado)
                {
                    // Mostrar mensaje de exito
                    string accion = _inmuebleSeleccionado != null ? "actualizado" : "registrado";
                    ModalMessage.Show(
                        $"El inmueble ha sido {accion} correctamente.",
                        "Operacion exitosa",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );

                    // Recargar la lista
                    CargarInmuebles();

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
        /// Cargar los datos del inmueble seleccionado en el formulario para editar
        /// Si el inmueble tiene ofertas u operaciones, solo permite editar precio, ciudad y direccion
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el inmueble seleccionado del DataGrid
                Inmuebles inmueble = dgInmuebles.SelectedItem as Inmuebles;

                // Validar que hay un inmueble seleccionado
                if (inmueble == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un inmueble de la lista para poder editarlo.",
                        "Ningun inmueble seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Guardar referencia al inmueble seleccionado
                _inmuebleSeleccionado = inmueble;

                // Verificar si el inmueble tiene ofertas u operaciones
                bool tieneOfertasUOperaciones = _controller.TieneOfertasUOperaciones(inmueble.IdInmueble);

                // Si tiene ofertas u operaciones, deshabilitar campos no editables
                if (tieneOfertasUOperaciones)
                {
                    // Deshabilitar campos que no se pueden editar
                    rbVenta.IsEnabled = false;
                    rbAlquiler.IsEnabled = false;
                    cmbEstado.IsEnabled = false;

                    // Habilitar solo los campos permitidos: direccion, ciudad y precio
                    txtDireccion.IsEnabled = true;
                    txtCiudad.IsEnabled = true;
                    txtPrecio.IsEnabled = true;
                }
                else
                {
                    // Habilitar todos los campos
                    rbVenta.IsEnabled = true;
                    rbAlquiler.IsEnabled = true;
                    cmbEstado.IsEnabled = true;
                    txtDireccion.IsEnabled = true;
                    txtCiudad.IsEnabled = true;
                    txtPrecio.IsEnabled = true;
                }

                // Cargar datos en el formulario
                txtDireccion.Text = inmueble.Direccion;
                txtCiudad.Text = inmueble.Ciudad;
                txtPrecio.Text = Utils.FormatearPrecio(inmueble.Precio, true);

                // Seleccionar tipo de operacion
                if (inmueble.TipoOperacion == InmueblesConstantes.TIPO_VENTA)
                {
                    rbVenta.IsChecked = true;
                }
                else
                {
                    rbAlquiler.IsChecked = true;
                }

                // Seleccionar estado
                cmbEstado.SelectedItem = inmueble.Estado;

                // Cambiar el texto del boton
                btnGuardar.Content = "Actualizar";

                // Enfocar el primer campo editable
                txtDireccion.Focus();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar el inmueble: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Eliminar el inmueble seleccionado despues de confirmar
        /// </summary>
        private void BtnEliminarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Obtener el inmueble seleccionado del DataGrid
                Inmuebles inmueble = dgInmuebles.SelectedItem as Inmuebles;

                // Validar que hay un inmueble seleccionado
                if (inmueble == null)
                {
                    ModalMessage.Show(
                        "Debe seleccionar un inmueble de la lista para poder eliminarlo.",
                        "Ningun inmueble seleccionado",
                        ModalMessageType.Error,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Confirmar eliminacion
                bool confirmar = ModalMessage.Show(
                    $"Esta seguro de eliminar el inmueble en '{inmueble.Direccion}'?\n\nEsta accion no se puede deshacer.",
                    "Confirmar eliminacion",
                    ModalMessageType.Confirmacion,
                    Window.GetWindow(this)
                );

                if (confirmar)
                {
                    // Variables para capturar errores
                    string tituloError = string.Empty;
                    string mensajeError = string.Empty;

                    // Intentar eliminar el inmueble a traves del controlador
                    bool resultado = _controller.Eliminar(inmueble, ref tituloError, ref mensajeError);

                    if (resultado)
                    {
                        // Mostrar mensaje de exito
                        ModalMessage.Show(
                            "El inmueble ha sido eliminado correctamente.",
                            "Inmueble eliminado",
                            ModalMessageType.Info,
                            Window.GetWindow(this)
                        );

                        // Recargar la lista
                        CargarInmuebles();

                        // Limpiar formulario si se estaba editando este inmueble
                        if (_inmuebleSeleccionado?.IdInmueble == inmueble.IdInmueble)
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
                    $"Error al eliminar el inmueble: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Refrescar la lista de inmuebles
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            // Recargar inmuebles y limpiar formulario
            CargarInmuebles();
            LimpiarFormulario();
        }

        /// <summary>
        /// Evento cuando se selecciona un inmueble en el DataGrid
        /// </summary>
        private void DgInmuebles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Este evento se puede usar para futuras funcionalidades
            // Por ejemplo, mostrar un resumen del inmueble seleccionado
        }

        // ================= METODOS AUXILIARES =================

        /// <summary>
        /// Limpiar todos los campos del formulario
        /// </summary>
        private void LimpiarFormulario()
        {
            // Limpiar campos de texto
            txtDireccion.Clear();
            txtCiudad.Clear();
            txtPrecio.Clear();

            // Resetear radio buttons
            rbVenta.IsChecked = true;

            // Resetear combobox a DISPONIBLE
            cmbEstado.SelectedItem = InmueblesConstantes.ESTADO_DISPONIBLE;

            // Resetear inmueble seleccionado
            _inmuebleSeleccionado = null;

            // Restaurar texto del boton
            btnGuardar.Content = "Guardar";

            // Limpiar seleccion del DataGrid
            dgInmuebles.SelectedItem = null;

            // Habilitar todos los campos
            rbVenta.IsEnabled = true;
            rbAlquiler.IsEnabled = true;
            cmbEstado.IsEnabled = true;
            txtDireccion.IsEnabled = true;
            txtCiudad.IsEnabled = true;
            txtPrecio.IsEnabled = true;

            // Enfocar el primer campo
            txtDireccion.Focus();
        }
    }
}
