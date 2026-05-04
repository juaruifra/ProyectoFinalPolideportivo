using Comun;
using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Modal asistido para crear una nueva reserva mostrando los slots libres del dia.
    /// Permite seleccionar uno o varios slots consecutivos de la misma instalacion.
    /// Devuelve instalacion, fecha y horas al cerrarse con DialogResult=true.
    /// </summary>
    public partial class ModalNuevaReserva : Window
    {
        // Controlador de reservas para obtener reservas por fecha.
        private readonly ReservasController _reservasController;

        // Controlador de instalaciones para obtener la lista completa.
        private readonly InstalacionesController _instalacionesController;

        // Flag para evitar recalculo de precio mientras se cargan datos en el panel.
        private bool _cargandoPanel;

        // Lista completa de slots calculados para el dia (sin filtro de instalacion).
        private List<HuecoDiario> _todosLosSlots;

        // Lista de todas las instalaciones para el combo de filtro.
        private List<Instalaciones> _todasLasInstalaciones;

        /// <summary>
        /// Instalacion seleccionada al confirmar. Disponible desde la vista llamante.
        /// </summary>
        public Instalaciones InstalacionSeleccionada { get; private set; }

        /// <summary>
        /// Fecha seleccionada al confirmar.
        /// </summary>
        public DateTime FechaSeleccionada { get; private set; }

        /// <summary>
        /// Hora de inicio seleccionada como TimeSpan al confirmar.
        /// </summary>
        public TimeSpan HoraInicioSeleccionada { get; private set; }

        /// <summary>
        /// Hora de fin seleccionada como TimeSpan al confirmar.
        /// </summary>
        public TimeSpan HoraFinSeleccionada { get; private set; }

        /// <summary>
        /// Constructor: inicializa componentes, carga las instalaciones y los slots del dia de hoy.
        /// </summary>
        public ModalNuevaReserva()
        {
            InitializeComponent(); // Inicializamos la ventana.

            _reservasController = new ReservasController(); // Instanciamos controller de reservas.
            _instalacionesController = new InstalacionesController(); // Instanciamos controller de instalaciones.
            _todosLosSlots = new List<HuecoDiario>(); // Inicializamos lista vacia.

            CargarComboInstalaciones(); // Cargamos el combo de filtro con todas las instalaciones.

            // Asignamos hoy como fecha por defecto (dispara DpFecha_SelectedDateChanged).
            dpFecha.SelectedDate = DateTime.Today;
        }


        /// <summary>
        /// Carga todas las instalaciones en el combo de filtro.
        /// El primer item es "Todas las instalaciones".
        /// </summary>
        private void CargarComboInstalaciones()
        {
            try
            {
                // Obtenemos todas las instalaciones del club.
                _todasLasInstalaciones = _instalacionesController.ObtenerTodos();

                // Creamos un item especial para "Todas".
                var itemTodas = new Instalaciones { InstalacionId = 0, Nombre = "(Todas las instalaciones)" };

                // Construimos la lista del combo con el item Todas al principio.
                var itemsCombo = new List<Instalaciones> { itemTodas };
                itemsCombo.AddRange(_todasLasInstalaciones);

                // Asignamos al combo y seleccionamos "Todas" por defecto.
                cmbFiltroInstalacion.ItemsSource = itemsCombo;
                cmbFiltroInstalacion.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                // Mostramos error si no se pueden cargar las instalaciones.
                ModalMessage.ShowModal($"No se pudieron cargar las instalaciones: {ex.Message}", "Nueva Reserva", 1);
            }
        }

        /// <summary>
        /// Obtiene las reservas del dia, calcula los slots y actualiza el subtitulo y el grid.
        /// </summary>
        /// <param name="fecha">Fecha del dia a analizar.</param>
        private void CargarSlots(DateTime fecha)
        {
            try
            {
                // Actualizamos el subtitulo con la fecha seleccionada.
                ActualizarSubtitulo(fecha);

                // Obtenemos las reservas del dia para todas las instalaciones.
                var reservasDia = _reservasController.ObtenerPorFecha(fecha);

                // Calculamos todos los slots libres del dia para todas las instalaciones.
                _todosLosSlots = CalcularSlots(reservasDia, _todasLasInstalaciones ?? new List<Instalaciones>());

                // Aplicamos el filtro de instalacion actualmente seleccionado.
                AplicarFiltroInstalacion();
            }
            catch (Exception ex)
            {
                // Mostramos error al usuario.
                ModalMessage.ShowModal($"No se pudieron cargar los huecos: {ex.Message}", "Nueva Reserva", 1);
            }
        }

        /// <summary>
        /// Filtra _todosLosSlots segun la instalacion seleccionada en el combo y actualiza el grid.
        /// </summary>
        private void AplicarFiltroInstalacion()
        {
            // Obtenemos la instalacion seleccionada en el combo.
            var instalacionFiltro = cmbFiltroInstalacion.SelectedItem as Instalaciones;

            List<HuecoDiario> slotsFiltrados;

            if (instalacionFiltro == null || instalacionFiltro.InstalacionId == 0)
            {
                // Sin filtro: mostramos todos los slots.
                slotsFiltrados = _todosLosSlots;
            }
            else
            {
                // Filtramos por instalacion seleccionada.
                slotsFiltrados = _todosLosSlots
                    .Where(s => s.Instalacion.InstalacionId == instalacionFiltro.InstalacionId)
                    .ToList();
            }

            // Asignamos al grid.
            dgHuecos.ItemsSource = slotsFiltrados;

            // Actualizamos el contador de slots visibles.
            txtContadorHuecos.Text = $"{slotsFiltrados.Count} slots disponibles";

            // Limpiamos la seleccion y el panel inferior.
            LimpiarPanelAjuste();
            btnUsarHueco.IsEnabled = false;
            MostrarHint("Selecciona uno o varios slots consecutivos de la misma instalacion", false);
        }


        /// <summary>
        /// Calcula los slots libres del dia para cada instalacion.
        /// Si el hueco libre es divisible exactamente por DuracionMinimaMinutos se subdivide
        /// en slots de ese tamano. Si no es divisible, se muestra como un unico slot con
        /// la duracion total del hueco.
        /// Solo se generan slots o huecos con duracion mayor o igual a DuracionMinimaMinutos.
        /// </summary>
        /// <param name="reservasDia">Reservas del dia para todas las instalaciones.</param>
        /// <param name="instalaciones">Lista completa de instalaciones.</param>
        /// <returns>Lista de slots ordenados por instalacion y hora de inicio.</returns>
        private List<HuecoDiario> CalcularSlots(
            List<Reservas> reservasDia,
            List<Instalaciones> instalaciones)
        {
            // Lista acumuladora de todos los slots generados.
            var resultado = new List<HuecoDiario>();

            // Iteramos cada instalacion para calcular sus huecos libres.
            foreach (var instalacion in instalaciones)
            {
                // Filtramos las reservas activas de esta instalacion ordenadas por inicio.
                var reservasInstalacion = reservasDia
                    .Where(r => r.InstalacionId == instalacion.InstalacionId && r.Estado != "Cancelada")
                    .OrderBy(r => r.FechaHoraInicio)
                    .ToList();

                // El cursor empieza en la hora de apertura del club.
                var cursor = ConfiguracionClub.HoraApertura;

                // Recorremos cada reserva para detectar huecos antes de ella.
                foreach (var reserva in reservasInstalacion)
                {
                    var inicioReserva = reserva.FechaHoraInicio.TimeOfDay; // Hora inicio de la reserva.
                    var finReserva = reserva.FechaHoraFin.TimeOfDay; // Hora fin de la reserva.

                    // Si hay tiempo libre entre el cursor y el inicio de la reserva, lo procesamos.
                    if (inicioReserva > cursor)
                        GenerarSlotsDeHueco(resultado, instalacion, cursor, inicioReserva);

                    // Avanzamos el cursor al fin de la reserva.
                    if (finReserva > cursor)
                        cursor = finReserva;
                }

                // Procesamos el hueco final entre el cursor y el cierre del club.
                if (ConfiguracionClub.HoraCierre > cursor)
                    GenerarSlotsDeHueco(resultado, instalacion, cursor, ConfiguracionClub.HoraCierre);
            }

            // Ordenamos por nombre de instalacion y luego por hora de inicio.
            return resultado.OrderBy(s => s.NombreInstalacion).ThenBy(s => s.HoraInicio).ToList();
        }

        /// <summary>
        /// Dado un hueco libre (inicio-fin), genera los slots que lo componen y los añade a la lista.
        /// Si la duracion es divisible exactamente por DuracionMinimaMinutos, subdivide el hueco.
        /// Si no es divisible, lo añade como un unico slot con su duracion total.
        /// Los huecos menores que DuracionMinimaMinutos se descartan.
        /// </summary>
        /// <param name="lista">Lista donde se añaden los slots generados.</param>
        /// <param name="instalacion">Instalacion del hueco.</param>
        /// <param name="inicio">Hora de inicio del hueco libre.</param>
        /// <param name="fin">Hora de fin del hueco libre.</param>
        private void GenerarSlotsDeHueco(
            List<HuecoDiario> lista,
            Instalaciones instalacion,
            TimeSpan inicio,
            TimeSpan fin)
        {
            // Calculamos la duracion total del hueco en minutos.
            int duracionTotal = (int)(fin - inicio).TotalMinutes;

            // Si el hueco no alcanza la duracion minima, lo descartamos por completo.
            if (duracionTotal < ConfiguracionClub.DuracionMinimaMinutos)
                return;

            // Tamanno de cada slot estandar.
            var paso = TimeSpan.FromMinutes(ConfiguracionClub.DuracionMinimaMinutos);

            // Cursor que avanza desde el inicio del hueco.
            var slotInicio = inicio;

            // Generamos slots del tamano estandar mientras quede tiempo suficiente.
            while (slotInicio < fin)
            {
                // Calculamos cuanto tiempo queda desde el cursor hasta el fin del hueco.
                int minutosRestantes = (int)(fin - slotInicio).TotalMinutes;

                // Si el tiempo restante es menor que el minimo, lo descartamos y paramos.
                // Asi una reserva manual que no cuadre no bloquea el resto del dia.
                if (minutosRestantes < ConfiguracionClub.DuracionMinimaMinutos)
                    break;

                // Calculamos el fin del slot actual.
                var slotFin = slotInicio + paso;

                // Anadimos el slot a la lista.
                lista.Add(new HuecoDiario
                {
                    Instalacion = instalacion,
                    HoraInicio = slotInicio,
                    HoraFin = slotFin
                });

                // Avanzamos el cursor al siguiente slot.
                slotInicio = slotFin;
            }
        }

        /// <summary>
        /// Analiza los items seleccionados en el grid y determina si forman una seleccion valida.
        /// Una seleccion es valida si: todos son de la misma instalacion y son consecutivos
        /// (el fin de cada slot coincide con el inicio del siguiente).
        /// Devuelve el rango combinado si es valida.
        /// </summary>
        /// <param name="seleccionados">Lista de slots seleccionados.</param>
        /// <param name="horaInicio">Hora de inicio del rango combinado si es valida.</param>
        /// <param name="horaFin">Hora de fin del rango combinado si es valida.</param>
        /// <param name="instalacion">Instalacion comun si es valida.</param>
        /// <param name="mensajeError">Descripcion del problema si no es valida.</param>
        /// <returns>True si la seleccion es valida.</returns>
        private bool ValidarSeleccionMultiple(
            List<HuecoDiario> seleccionados,
            out TimeSpan horaInicio,
            out TimeSpan horaFin,
            out Instalaciones instalacion,
            out string mensajeError)
        {
            // Inicializamos los parametros de salida.
            horaInicio = TimeSpan.Zero;
            horaFin = TimeSpan.Zero;
            instalacion = null;
            mensajeError = string.Empty;

            // Si no hay seleccion, no es valida.
            if (seleccionados.Count == 0)
            {
                mensajeError = "Selecciona uno o varios slots consecutivos de la misma instalacion";
                return false;
            }

            // Comprobamos que todos los slots sean de la misma instalacion.
            var primeraInstalacion = seleccionados[0].Instalacion;
            bool mismaInstalacion = seleccionados.All(s => s.Instalacion.InstalacionId == primeraInstalacion.InstalacionId);

            if (!mismaInstalacion)
            {
                mensajeError = "Todos los slots seleccionados deben pertenecer a la misma instalacion";
                return false;
            }

            // Ordenamos los slots seleccionados por hora de inicio para comprobar consecutividad.
            var ordenados = seleccionados.OrderBy(s => s.HoraInicio).ToList();

            // Comprobamos que cada slot termina justo donde empieza el siguiente.
            for (int i = 0; i < ordenados.Count - 1; i++)
            {
                if (ordenados[i].HoraFin != ordenados[i + 1].HoraInicio)
                {
                    mensajeError = "Los slots seleccionados no son consecutivos. Selecciona slots sin huecos entre ellos";
                    return false;
                }
            }

            // Seleccion valida: calculamos el rango combinado.
            horaInicio = ordenados.First().HoraInicio;
            horaFin = ordenados.Last().HoraFin;
            instalacion = primeraInstalacion;
            return true;
        }


        /// <summary>
        /// Actualiza el subtitulo de la cabecera con la fecha formateada.
        /// </summary>
        /// <param name="fecha">Fecha a mostrar.</param>
        private void ActualizarSubtitulo(DateTime fecha)
        {
            // Mostramos la fecha en formato largo.
            txtSubtitulo.Text = $"Huecos libres del {fecha:dddd, dd MMMM yyyy}";
        }

        /// <summary>
        /// Limpia los campos del panel de ajuste de horas y precio.
        /// </summary>
        private void LimpiarPanelAjuste()
        {
            _cargandoPanel = true; // Evitamos recalculos durante la limpieza.
            txtHoraInicio.Text = string.Empty; // Vaciamos hora inicio.
            txtHoraFin.Text = string.Empty; // Vaciamos hora fin.
            txtPrecioEstimado.Text = "0,00 EUR"; // Reseteamos precio.
            _cargandoPanel = false; // Reactivamos eventos.
        }

        /// <summary>
        /// Muestra un mensaje de hint o error en el TextBlock inferior del modal.
        /// </summary>
        /// <param name="mensaje">Texto a mostrar.</param>
        /// <param name="esError">Si true muestra en rojo, si false en gris informativo.</param>
        private void MostrarHint(string mensaje, bool esError)
        {
            txtHintSeleccion.Text = mensaje; // Asignamos el texto.
            // Cambiamos el color segun si es error o informacion.
            txtHintSeleccion.Foreground = esError
                ? System.Windows.Media.Brushes.Red
                : System.Windows.Media.Brushes.Gray;
        }

        /// <summary>
        /// Recalcula el precio estimado usando la instalacion activa y las horas del panel.
        /// </summary>
        /// <param name="instalacion">Instalacion cuyo precio/hora se usa.</param>
        private void RecalcularPrecioEstimado(Instalaciones instalacion)
        {
            if (_cargandoPanel) return; // Evitamos recalculo durante la carga del panel.
            if (instalacion == null) return; // Sin instalacion no calculamos.

            try
            {
                // Parseamos hora de inicio del TextBox.
                if (!TimeSpan.TryParseExact(txtHoraInicio.Text.Trim(), "hh\\:mm", null, out TimeSpan horaInicio))
                {
                    txtPrecioEstimado.Text = "0,00 EUR";
                    return;
                }

                // Parseamos hora de fin del TextBox.
                if (!TimeSpan.TryParseExact(txtHoraFin.Text.Trim(), "hh\\:mm", null, out TimeSpan horaFin))
                {
                    txtPrecioEstimado.Text = "0,00 EUR";
                    return;
                }

                // Validamos que el fin sea posterior al inicio.
                if (horaFin <= horaInicio)
                {
                    txtPrecioEstimado.Text = "0,00 EUR";
                    return;
                }

                // Calculamos el precio: horas * precio por hora de la instalacion.
                var horas = (decimal)(horaFin - horaInicio).TotalHours;
                var precio = Math.Round(horas * instalacion.PrecioHora, 2);
                txtPrecioEstimado.Text = $"{precio:F2} EUR";
            }
            catch
            {
                txtPrecioEstimado.Text = "0,00 EUR"; // Si hay error mostramos cero.
            }
        }

        /// <summary>
        /// Arrastramos la ventana al hacer MouseDown en la cabecera.
        /// </summary>
        private void Cabecera_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove(); // Movemos la ventana.
        }

        /// <summary>
        /// Boton X: cierra el modal sin confirmacion.
        /// </summary>
        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Cierre sin datos.
        }

        /// <summary>
        /// Cambio de fecha: recarga los slots del nuevo dia.
        /// </summary>
        private void DpFecha_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dpFecha.SelectedDate == null) return; // Seguridad, por si no se selecciona ninguna fecha
            CargarSlots(dpFecha.SelectedDate.Value); // Recargamos slots.
        }

        /// <summary>
        /// Cambio en el combo de instalacion: filtra el grid sin recargar de BD.
        /// </summary>
        private void CmbFiltroInstalacion_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Solo filtramos si ya tenemos slots calculados (evita ejecuciones durante la carga inicial).
            if (_todosLosSlots == null) return;
            AplicarFiltroInstalacion(); // Filtramos en cliente.
        }

        /// <summary>
        /// Cambio de seleccion en el grid: valida la seleccion multiple y actualiza el panel.
        /// </summary>
        private void DgHuecos_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Recogemos todos los items seleccionados casteandolos a HuecoDiario.
            var seleccionados = dgHuecos.SelectedItems
                .Cast<HuecoDiario>()
                .ToList();

            // Si no hay seleccion, limpiamos y deshabilitamos.
            if (seleccionados.Count == 0)
            {
                LimpiarPanelAjuste();
                btnUsarHueco.IsEnabled = false;
                MostrarHint("Selecciona uno o varios slots consecutivos de la misma instalacion", false);
                return;
            }

            // Validamos la seleccion multiple.
            bool valida = ValidarSeleccionMultiple(
                seleccionados,
                out TimeSpan horaInicio,
                out TimeSpan horaFin,
                out Instalaciones instalacion,
                out string mensajeError);

            if (!valida)
            {
                // Seleccion invalida: mostramos el error y deshabilitamos.
                LimpiarPanelAjuste();
                btnUsarHueco.IsEnabled = false;
                MostrarHint(mensajeError, true);
                return;
            }

            // Seleccion valida: cargamos el rango combinado en el panel.
            _cargandoPanel = true; // Evitamos recalculos prematuros.
            txtHoraInicio.Text = horaInicio.ToString("hh\\:mm"); // Hora inicio del rango.
            txtHoraFin.Text = horaFin.ToString("hh\\:mm"); // Hora fin del rango.
            _cargandoPanel = false; // Reactivamos.

            // Habilitamos el boton y mostramos un hint positivo con el resumen.
            btnUsarHueco.IsEnabled = true;
            int minutosTotales = (int)(horaFin - horaInicio).TotalMinutes;
            MostrarHint($"{seleccionados.Count} slot(s) seleccionado(s) - {instalacion.Nombre} - {minutosTotales} min en total", false);

            // Calculamos el precio estimado.
            RecalcularPrecioEstimado(instalacion);
        }

        /// <summary>
        /// Cambio en los TextBox de hora: evento requerido por el XAML, no hace nada porque los campos son de solo lectura.
        /// El precio se recalcula directamente en DgHuecos_SelectionChanged al cambiar la seleccion.
        /// </summary>
        private void HoraAjustada_Changed(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Los TextBox son de solo lectura, este evento no realiza ninguna accion.
        }

        /// <summary>
        /// Boton Cancelar: cierra el modal sin devolver datos.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Cierre sin confirmacion.
        }

        /// <summary>
        /// Boton Usar este hueco: valida las horas finales y cierra el modal devolviendo los datos.
        /// </summary>
        private void BtnUsarHueco_Click(object sender, RoutedEventArgs e)
        {
            // Recogemos los slots seleccionados.
            var seleccionados = dgHuecos.SelectedItems.Cast<HuecoDiario>().ToList();

            // Revalidamos la seleccion (el usuario pudo modificar las horas manualmente).
            bool valida = ValidarSeleccionMultiple(
                seleccionados,
                out TimeSpan horaInicioBase,
                out TimeSpan horaFinBase,
                out Instalaciones instalacion,
                out string mensajeError);

            if (!valida)
            {
                ModalMessage.ShowModal(mensajeError, "Nueva Reserva", 1);
                return;
            }

            // Parseamos la hora de inicio del TextBox (el usuario puede haberla ajustado).
            if (!TimeSpan.TryParseExact(txtHoraInicio.Text.Trim(), "hh\\:mm", null, out TimeSpan horaInicio))
            {
                ModalMessage.ShowModal("El formato de hora de inicio no es valido. Use HH:mm.", "Nueva Reserva", 1);
                return;
            }

            // Parseamos la hora de fin del TextBox.
            if (!TimeSpan.TryParseExact(txtHoraFin.Text.Trim(), "hh\\:mm", null, out TimeSpan horaFin))
            {
                ModalMessage.ShowModal("El formato de hora de fin no es valido. Use HH:mm.", "Nueva Reserva", 1);
                return;
            }

            // Validamos que el fin sea posterior al inicio.
            if (horaFin <= horaInicio)
            {
                ModalMessage.ShowModal("La hora de fin debe ser posterior a la hora de inicio.", "Nueva Reserva", 1);
                return;
            }

            // Validamos duracion minima.
            if ((horaFin - horaInicio).TotalMinutes < ConfiguracionClub.DuracionMinimaMinutos)
            {
                ModalMessage.ShowModal($"La duracion minima es de {ConfiguracionClub.DuracionMinimaMinutos} minutos.", "Nueva Reserva", 1);
                return;
            }

            // Guardamos los datos en las propiedades publicas.
            InstalacionSeleccionada = instalacion;
            FechaSeleccionada = dpFecha.SelectedDate ?? DateTime.Today;
            HoraInicioSeleccionada = horaInicio;
            HoraFinSeleccionada = horaFin;

            DialogResult = true; // Cerramos con exito.
        }
    }
}
