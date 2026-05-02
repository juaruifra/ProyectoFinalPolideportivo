using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Logica del UserControl VistaReservas.
    /// Permite gestionar las reservas de instalaciones del club.
    /// </summary>
    public partial class VistaReservas : UserControl
    {
        // Controlador de reservas.
        private readonly ReservasController _controller;

        // Controlador de socios (para combo de filtro).
        private readonly SociosController _sociosController;

        // Controlador de instalaciones (para combo de filtro).
        private readonly InstalacionesController _instalacionesController;

        // Socio actualmente seleccionado en el formulario.
        private Socios _socioSeleccionado;

        // Instalacion actualmente seleccionada en el formulario.
        private Instalaciones _instalacionSeleccionada;

        // Reserva que se esta editando. Null si es alta nueva.
        private Reservas _reservaSeleccionada;

        // Flag para evitar recalculos de precio mientras cargamos el formulario.
        private bool _cargandoFormulario;

        /// <summary>
        /// Constructor: inicializa componentes y carga datos.
        /// </summary>
        public VistaReservas()
        {
            InitializeComponent(); // Inicializamos el UserControl.

            _controller = new ReservasController(); // Instanciamos controller de reservas.
            _sociosController = new SociosController(); // Instanciamos controller de socios.
            _instalacionesController = new InstalacionesController(); // Instanciamos controller de instalaciones.

            CargarCombosFiltro(); // Cargamos socios e instalaciones en los combos de filtro.
            LimpiarFormulario(); // Aplicamos valores por defecto al formulario.
            CargarDatos(); // Cargamos reservas en el grid.
        }

        /// <summary>
        /// Carga los combos de filtro (socios e instalaciones).
        /// </summary>
        private void CargarCombosFiltro()
        {
            try
            {
                // Obtenemos socios activos.
                var socios = _sociosController.ObtenerTodos(soloActivos: true);

                // Creamos item "Todos" para socios.
                var itemTodosSocios = new Socios { SocioId = 0, Nombre = "", Apellidos = "(Todos los socios)" };
                var itemsFiltroSocios = new List<Socios> { itemTodosSocios };
                itemsFiltroSocios.AddRange(socios); // Anadimos socios reales.

                // Asignamos al combo de filtro de socios.
                cmbFiltroSocio.ItemsSource = itemsFiltroSocios;
                cmbFiltroSocio.SelectedIndex = 0; // Seleccionamos "Todos".

                // Obtenemos todas las instalaciones.
                var instalaciones = _instalacionesController.ObtenerTodos();

                // Creamos item "Todas" para instalaciones.
                var itemTodasInstalaciones = new Instalaciones { InstalacionId = 0, Nombre = "(Todas las instalaciones)" };
                var itemsFiltroInstalaciones = new List<Instalaciones> { itemTodasInstalaciones };
                itemsFiltroInstalaciones.AddRange(instalaciones); // Anadimos instalaciones reales.

                // Asignamos al combo de filtro de instalaciones.
                cmbFiltroInstalacion.ItemsSource = itemsFiltroInstalaciones;
                cmbFiltroInstalacion.SelectedIndex = 0; // Seleccionamos "Todas".
            }
            catch (Exception ex)
            {
                // Mostramos error.
                ModalMessage.ShowModal($"No se pudieron cargar los filtros: {ex.Message}", "Reservas", 1);
            }
        }

        /// <summary>
        /// Carga las reservas en el DataGrid segun los filtros activos.
        /// </summary>
        private void CargarDatos()
        {
            try
            {
                // Obtenemos filtro de socio seleccionado.
                int? socioId = null;
                var socioFiltro = cmbFiltroSocio.SelectedItem as Socios;
                if (socioFiltro != null && socioFiltro.SocioId > 0)
                    socioId = socioFiltro.SocioId; // Filtramos por socio.

                // Obtenemos filtro de instalacion seleccionada.
                int? instalacionId = null;
                var instalacionFiltro = cmbFiltroInstalacion.SelectedItem as Instalaciones;
                if (instalacionFiltro != null && instalacionFiltro.InstalacionId > 0)
                    instalacionId = instalacionFiltro.InstalacionId; // Filtramos por instalacion.

                // Cargamos reservas del controlador sin filtro de estado.
                var reservas = _controller.ObtenerTodos(socioId, instalacionId);

                // Asignamos al grid.
                dgReservas.ItemsSource = reservas;

                // Actualizamos contador.
                txtTotalReservas.Text = $"{reservas.Count} reservas";

                // Actualizamos estado de botones.
                ActualizarEstadoAcciones();
            }
            catch (Exception ex)
            {
                // Error al cargar.
                ModalMessage.ShowModal($"No se pudieron cargar las reservas: {ex.Message}", "Reservas", 1);
            }
        }

        /// <summary>
        /// Limpia el formulario y lo deja listo para una nueva reserva.
        /// </summary>
        private void LimpiarFormulario()
        {
            _reservaSeleccionada = null; // Borramos referencia a reserva en edicion.
            _socioSeleccionado = null; // Limpiamos socio seleccionado.
            _instalacionSeleccionada = null; // Limpiamos instalacion seleccionada.

            txtSocio.Text = string.Empty; // Limpiamos campo socio.
            txtInstalacion.Text = string.Empty; // Limpiamos campo instalacion.

            dpFechaInicio.SelectedDate = null; // Sin fecha inicio.
            txtHoraInicio.Text = "09:00"; // Hora por defecto.
            dpFechaFin.SelectedDate = null; // Sin fecha fin.
            txtHoraFin.Text = "10:00"; // Hora fin por defecto.

            txtPrecio.Text = "0,00"; // Precio en cero.

            dgReservas.SelectedItem = null; // Quitamos seleccion del grid.
        }

        /// <summary>
        /// Carga una reserva en el formulario para editar.
        /// </summary>
        /// <param name="reserva">Reserva a cargar.</param>
        private void CargarFormularioDesdeEntidad(Reservas reserva)
        {
            if (reserva == null) return; // Guard.

            _cargandoFormulario = true; // Activamos flag para evitar recalculo prematuro.

            _reservaSeleccionada = reserva; // Guardamos referencia.

            // Guardamos socio e instalacion seleccionados.
            _socioSeleccionado = reserva.Socios;
            _instalacionSeleccionada = reserva.Instalaciones;

            // Mostramos nombre del socio y la instalacion en los TextBox de solo lectura.
            txtSocio.Text = reserva.Socios?.NombreCompleto ?? string.Empty;
            txtInstalacion.Text = reserva.Instalaciones?.Nombre ?? string.Empty;

            // Cargamos fecha y hora de inicio.
            dpFechaInicio.SelectedDate = reserva.FechaHoraInicio.Date;
            txtHoraInicio.Text = reserva.FechaHoraInicio.ToString("HH:mm");

            // Cargamos fecha y hora de fin.
            dpFechaFin.SelectedDate = reserva.FechaHoraFin.Date;
            txtHoraFin.Text = reserva.FechaHoraFin.ToString("HH:mm");

            // Mostramos precio total formateado.
            txtPrecio.Text = reserva.PrecioTotal.ToString("F2");

            _cargandoFormulario = false; // Desactivamos flag.
        }

        /// <summary>
        /// Crea la entidad Reservas a partir de los datos del formulario.
        /// </summary>
        /// <returns>Entidad Reservas con los datos del formulario.</returns>
        private Reservas CrearEntidadDesdeFormulario()
        {
            var r = _reservaSeleccionada ?? new Reservas(); // Reutilizamos en edicion.

            // Asignamos socio e instalacion desde las variables privadas.
            r.SocioId = _socioSeleccionado?.SocioId ?? 0;
            r.InstalacionId = _instalacionSeleccionada?.InstalacionId ?? 0;

            // Construimos FechaHoraInicio combinando DatePicker + TextBox.
            r.FechaHoraInicio = CombinarFechaHora(dpFechaInicio.SelectedDate, txtHoraInicio.Text);

            // Construimos FechaHoraFin combinando DatePicker + TextBox.
            r.FechaHoraFin = CombinarFechaHora(dpFechaFin.SelectedDate, txtHoraFin.Text);

            // Calculamos precio total usando el controller.
            if (r.InstalacionId > 0 && r.FechaHoraInicio != default(DateTime) && r.FechaHoraFin > r.FechaHoraInicio)
                r.PrecioTotal = _controller.CalcularPrecio(r.InstalacionId, r.FechaHoraInicio, r.FechaHoraFin);

            // Estado siempre "Activa" al guardar (no hay canceladas, se borran directamente).
            r.Estado = "Activa";

            return r; // Devolvemos entidad.
        }

        /// <summary>
        /// Combina una fecha de un DatePicker con una hora de un TextBox (formato HH:mm).
        /// </summary>
        /// <param name="fecha">Fecha seleccionada en el DatePicker. Null si no hay.</param>
        /// <param name="horaTexto">Texto de la hora en formato HH:mm.</param>
        /// <returns>DateTime combinado, o default si los datos no son validos.</returns>
        private DateTime CombinarFechaHora(DateTime? fecha, string horaTexto)
        {
            if (fecha == null) return default(DateTime); // Sin fecha: devolvemos default.

            // Intentamos parsear la hora.
            if (!TimeSpan.TryParseExact((horaTexto ?? string.Empty).Trim(), "hh\\:mm", null, out TimeSpan hora))
                return default(DateTime); // Hora invalida: devolvemos default.

            return fecha.Value.Date + hora; // Combinamos fecha y hora.
        }

        /// <summary>
        /// Recalcula y muestra el precio en el TextBox de precio.
        /// Solo lo hace si tenemos instalacion y las fechas/horas son validas.
        /// </summary>
        private void RecalcularPrecio()
        {
            if (_cargandoFormulario) return; // Evitamos recalculo durante la carga del formulario.
            if (_instalacionSeleccionada == null) return; // Sin instalacion no calculamos.

            try
            {
                // Construimos las fechas.
                var inicio = CombinarFechaHora(dpFechaInicio.SelectedDate, txtHoraInicio.Text);
                var fin = CombinarFechaHora(dpFechaFin.SelectedDate, txtHoraFin.Text);

                if (inicio == default(DateTime) || fin == default(DateTime) || fin <= inicio)
                {
                    txtPrecio.Text = "0,00"; // Fechas no validas: precio cero.
                    return;
                }

                // Calculamos precio usando el controller.
                var precio = _controller.CalcularPrecio(_instalacionSeleccionada.InstalacionId, inicio, fin);
                txtPrecio.Text = precio.ToString("F2"); // Mostramos con 2 decimales.
            }
            catch
            {
                txtPrecio.Text = "0,00"; // Si hay error, precio cero.
            }
        }

        /// <summary>
        /// Actualiza el estado habilitado/deshabilitado de los botones de accion.
        /// </summary>
        private void ActualizarEstadoAcciones()
        {
            bool haySeleccion = dgReservas.SelectedItem != null; // Hay fila seleccionada.

            btnEditarSeleccionado.IsEnabled = haySeleccion; // Editar.
            btnCancelarReserva.IsEnabled = haySeleccion; // Cancelar reserva.
        }

        // ??? EVENTOS ???????????????????????????????????????????????????????????????

        /// <summary>
        /// Cambio de seleccion en el DataGrid: actualiza botones.
        /// </summary>
        private void DgReservas_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoAcciones(); // Refrescamos estado.
        }

        /// <summary>
        /// Cambio en cualquier filtro del panel superior: recarga el grid.
        /// </summary>
        private void FiltroChanged(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos con los filtros actuales.
        }

        /// <summary>
        /// Cambio en un DatePicker de fecha: recalcula precio.
        /// </summary>
        private void FechaHoraChanged(object sender, SelectionChangedEventArgs e)
        {
            RecalcularPrecio(); // Recalculamos.
        }

        /// <summary>
        /// Cambio en un TextBox de hora: recalcula precio.
        /// </summary>
        private void FechaHoraChangedText(object sender, TextChangedEventArgs e)
        {
            RecalcularPrecio(); // Recalculamos.
        }

        /// <summary>
        /// Boton Editar: carga la reserva seleccionada en el formulario.
        /// </summary>
        private void BtnEditarSeleccionado_Click(object sender, RoutedEventArgs e)
        {
            var reserva = dgReservas.SelectedItem as Reservas; // Obtenemos seleccion.
            if (reserva == null) return; // Guard.
            CargarFormularioDesdeEntidad(reserva); // Cargamos en formulario.
        }

        /// <summary>
        /// Boton Borrar Reserva: borra fisicamente la reserva seleccionada tras confirmacion.
        /// </summary>
        private void BtnCancelarReserva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var reserva = dgReservas.SelectedItem as Reservas; // Obtenemos seleccion.
                if (reserva == null) return; // Guard.

                // Pedimos confirmacion al usuario.
                var confirmar = ModalMessage.ShowModal(
                    $"Desea borrar la reserva de {reserva.Socios?.NombreCompleto} en {reserva.Instalaciones?.Nombre} ({reserva.FechaHoraInicio:dd/MM/yyyy HH:mm})? Esta accion no se puede deshacer.",
                    "Reservas", 3);
                if (!confirmar) return; // Cancelado.

                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                if (_controller.Borrar(reserva, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario(); // Limpiamos formulario.
                    CargarDatos(); // Refrescamos grid.
                    ModalMessage.ShowModal("Reserva borrada correctamente.", "Reservas", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error de validacion.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado: {ex.Message}", "Reservas", 1);
            }
        }

        /// <summary>
        /// Boton Refrescar: recarga la lista de reservas.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarDatos(); // Recargamos.
        }

        /// <summary>
        /// Boton Cancelar del formulario: limpia el formulario sin guardar.
        /// </summary>
        private void BtnCancelarFormulario_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario(); // Limpiamos y reseteamos valores.
        }

        /// <summary>
        /// Boton Guardar: valida y guarda la reserva.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tituloError = string.Empty;
                string mensajeError = string.Empty;

                var reserva = CrearEntidadDesdeFormulario(); // Construimos entidad.

                if (_controller.Guardar(reserva, ref tituloError, ref mensajeError))
                {
                    LimpiarFormulario(); // Limpiamos formulario.
                    CargarDatos(); // Refrescamos grid.
                    ModalMessage.ShowModal("Reserva guardada correctamente.", "Reservas", 2);
                    return;
                }

                ModalMessage.ShowModal(mensajeError, tituloError, 1); // Error de validacion.
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error inesperado al guardar: {ex.Message}", "Reservas", 1);
            }
        }

        /// <summary>
        /// Boton lupa de socio: abre el modal de busqueda y asigna el socio seleccionado.
        /// </summary>
        private void BtnBuscarSocio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new ModalBuscarSocio(); // Creamos el modal.
                modal.Owner = Window.GetWindow(this); // Asignamos ventana padre.

                if (modal.ShowDialog() == true && modal.SocioSeleccionado != null)
                {
                    _socioSeleccionado = modal.SocioSeleccionado; // Guardamos referencia.
                    txtSocio.Text = _socioSeleccionado.NombreCompleto; // Mostramos nombre.
                }
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error al buscar socio: {ex.Message}", "Reservas", 1);
            }
        }

        /// <summary>
        /// Boton lupa de instalacion: abre el modal de busqueda y asigna la instalacion seleccionada.
        /// </summary>
        private void BtnBuscarInstalacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var modal = new ModalBuscarInstalacion(); // Creamos el modal.
                modal.Owner = Window.GetWindow(this); // Asignamos ventana padre.

                if (modal.ShowDialog() == true && modal.InstalacionSeleccionada != null)
                {
                    _instalacionSeleccionada = modal.InstalacionSeleccionada; // Guardamos referencia.
                    txtInstalacion.Text = _instalacionSeleccionada.Nombre; // Mostramos nombre.
                    RecalcularPrecio(); // Recalculamos precio con la nueva instalacion.
                }
            }
            catch (Exception ex)
            {
                ModalMessage.ShowModal($"Error al buscar instalacion: {ex.Message}", "Reservas", 1);
            }
        }
    }
}
