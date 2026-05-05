using Comun;
using ProyectoFinal.controller_new.controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Vista de inicio con estadisticas resumidas del club y reservas del dia.
    /// </summary>
    public partial class VistaInicio : UserControl
    {
        // Controladores necesarios para obtener los datos de cada seccion.
        private readonly SociosController _sociosController;
        private readonly ReservasController _reservasController;
        private readonly CuotasController _cuotasController;
        private readonly InstalacionesController _instalacionesController; // Controlador de instalaciones para calcular ocupacion.

        // Mes que se esta mostrando en la tarjeta de ingresos (empieza en el mes actual).
        private DateTime _mesIngresos;

        /// <summary>
        /// Constructor: inicializa controladores y carga todos los datos.
        /// </summary>
        public VistaInicio()
        {
            InitializeComponent(); // Inicializamos el UserControl.

            _sociosController = new SociosController(); // Controlador de socios.
            _reservasController = new ReservasController(); // Controlador de reservas.
            _cuotasController = new CuotasController(); // Controlador de cuotas.
            _instalacionesController = new InstalacionesController(); // Controlador de instalaciones.

            _mesIngresos = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1); // Mes actual.

            CargarTodo(); // Cargamos todos los datos de la pantalla.
        }

        /// <summary>
        /// Carga todos los bloques de la pantalla de inicio.
        /// </summary>
        private void CargarTodo()
        {
            CargarCabecera(); // Saludo y fecha.
            CargarTarjetas(); // Las 4 tarjetas de metricas.
            CargarReservasHoy(); // Tabla de reservas del dia.
        }

        /// <summary>
        /// Rellena el saludo y la fecha de hoy en la cabecera.
        /// </summary>
        private void CargarCabecera()
        {
            // Calculamos el saludo segun la hora del dia.
            int hora = DateTime.Now.Hour;
            string saludo;
            if (hora < 13)
                saludo = "Buenos dias"; // Por la manana.
            else if (hora < 21)
                saludo = "Buenas tardes"; // Por la tarde.
            else
                saludo = "Buenas noches"; // Por la noche.

            // Mostramos el saludo y la fecha completa formateada.
            txtSaludo.Text = saludo;
            txtFechaHoy.Text = DateTime.Today.ToString("dddd, d 'de' MMMM 'de' yyyy");
        }

        /// <summary>
        /// Carga los datos de las cuatro tarjetas de metricas.
        /// </summary>
        private void CargarTarjetas()
        {
            CargarSociosActivos(); // Tarjeta 1.
            CargarReservasContadorHoy(); // Tarjeta 2.
            CargarIngresosMes(); // Tarjeta 3.
            CargarCuotasPendientes(); // Tarjeta 4.
        }

        /// <summary>
        /// Cuenta los socios activos y los muestra en la tarjeta.
        /// </summary>
        private void CargarSociosActivos()
        {
            try
            {
                // Pedimos solo los socios activos y contamos cuantos hay.
                int total = _sociosController.ObtenerTodos(soloActivos: true).Count;
                txtSociosActivos.Text = total.ToString(); // Mostramos el numero.
            }
            catch
            {
                txtSociosActivos.Text = "-"; // Si hay error mostramos guion.
            }
        }

        /// <summary>
        /// Cuenta las reservas de hoy y las muestra en la tarjeta.
        /// </summary>
        private void CargarReservasContadorHoy()
        {
            try
            {
                // Obtenemos las reservas del dia de hoy y contamos.
                int total = _reservasController.ObtenerPorFecha(DateTime.Today).Count;
                txtReservasHoy.Text = total.ToString(); // Mostramos el numero.
            }
            catch
            {
                txtReservasHoy.Text = "-"; // Si hay error mostramos guion.
            }
        }

        /// <summary>
        /// Suma los ingresos del mes seleccionado y los muestra en la tarjeta.
        /// </summary>
        private void CargarIngresosMes()
        {
            try
            {
                // Calculamos inicio y fin del mes para filtrar reservas.
                var iniciomes = _mesIngresos;
                var finMes = _mesIngresos.AddMonths(1);

                // Obtenemos todas las reservas y filtramos las del mes en codigo.
                var reservas = _reservasController.ObtenerTodos();
                var delMes = reservas.Where(r =>
                    r.FechaHoraInicio >= iniciomes &&
                    r.FechaHoraInicio < finMes).ToList();

                // Sumamos el precio total de todas las reservas del mes.
                decimal ingresos = delMes.Sum(r => r.PrecioTotal);

                // Mostramos el total con formato de euros.
                txtIngresosMes.Text = $"{ingresos:F2} EUR";

                // Actualizamos la etiqueta del mes navegado.
                txtMesSeleccionado.Text = _mesIngresos.ToString("MMMM yyyy");
                txtLabelIngresos.Text = "Ingresos del mes";
            }
            catch
            {
                txtIngresosMes.Text = "-"; // Si hay error mostramos guion.
            }
        }

        /// <summary>
        /// Cuenta las cuotas sin pagar y las muestra en la tarjeta de alerta.
        /// </summary>
        private void CargarCuotasPendientes()
        {
            try
            {
                // Obtenemos todas las cuotas y filtramos las no pagadas.
                var cuotas = _cuotasController.ObtenerTodos();
                int pendientes = cuotas.Count(c => !c.Pagada);
                txtCuotasPendientes.Text = pendientes.ToString(); // Mostramos el numero.
            }
            catch
            {
                txtCuotasPendientes.Text = "-"; // Si hay error mostramos guion.
            }
        }

        /// <summary>
        /// Carga la tabla de reservas de hoy y actualiza el indicador de estado del dia.
        /// </summary>
        private void CargarReservasHoy()
        {
            try
            {
                // Obtenemos las reservas de hoy ordenadas por hora.
                var reservasHoy = _reservasController.ObtenerPorFecha(DateTime.Today);

                // Asignamos al grid.
                dgReservasHoy.ItemsSource = reservasHoy;

                // Actualizamos el subtitulo con el total.
                if (reservasHoy.Count == 0)
                    txtSubtituloReservasHoy.Text = "No hay ninguna reserva registrada para hoy";
                else if (reservasHoy.Count == 1)
                    txtSubtituloReservasHoy.Text = "1 reserva registrada para hoy";
                else
                    txtSubtituloReservasHoy.Text = $"{reservasHoy.Count} reservas registradas para hoy";

                // Calculamos el porcentaje de ocupacion y actualizamos el chip.
                double porcentaje = CalcularPorcentajeOcupacion(reservasHoy);
                ActualizarEstadoDia(porcentaje);
            }
            catch
            {
                // Si hay error mostramos mensaje informativo.
                txtSubtituloReservasHoy.Text = "No se pudieron cargar las reservas de hoy";
            }
        }

        /// <summary>
        /// Calcula el porcentaje de ocupacion del club para hoy.
        /// Formula: horas reservadas / (num instalaciones * horas apertura) * 100.
        /// </summary>
        /// <param name="reservasHoy">Lista de reservas del dia.</param>
        /// <returns>Porcentaje de ocupacion entre 0 y 100 (puede superar 100 si hay solapamientos).</returns>
        private double CalcularPorcentajeOcupacion(List<ProyectoFinal.model_new.Reservas> reservasHoy)
        {
            // Obtenemos el total de instalaciones disponibles del club.
            var instalaciones = _instalacionesController.ObtenerTodos(soloDisponibles: false);
            int numInstalaciones = instalaciones.Count; // Total de instalaciones.

            // Si no hay instalaciones no podemos calcular, devolvemos 0.
            if (numInstalaciones == 0) return 0;

            // Calculamos las horas que el club esta abierto al dia.
            double horasApertura = (ConfiguracionClub.HoraCierre - ConfiguracionClub.HoraApertura).TotalHours;

            // Capacidad maxima del club en horas (todas las instalaciones todo el dia).
            double capacidadTotal = numInstalaciones * horasApertura;

            // Sumamos la duracion en horas de cada reserva de hoy.
            double horasReservadas = reservasHoy.Sum(r =>
                (r.FechaHoraFin - r.FechaHoraInicio).TotalHours);

            // Calculamos el porcentaje redondeado.
            return (horasReservadas / capacidadTotal) * 100.0;
        }

        /// <summary>
        /// Cambia el color y el texto del indicador de estado segun el porcentaje de ocupacion del club hoy.
        /// 0%: Dia tranquilo. 1-30%: Actividad moderada. 31-60%: Dia activo. 61%+: Dia muy ocupado.
        /// </summary>
        /// <param name="porcentajeOcupacion">Porcentaje de ocupacion calculado.</param>
        private void ActualizarEstadoDia(double porcentajeOcupacion)
        {
            if (porcentajeOcupacion <= 0)
            {
                // Sin ocupacion: dia tranquilo en verde claro.
                bdEstadoDia.Background = new SolidColorBrush(Color.FromRgb(209, 250, 229));
                txtEstadoDia.Foreground = new SolidColorBrush(Color.FromRgb(6, 95, 70));
                txtEstadoDia.Text = "Dia tranquilo";
            }
            else if (porcentajeOcupacion <= 30)
            {
                // Hasta el 30% de ocupacion: actividad moderada en azul claro.
                bdEstadoDia.Background = new SolidColorBrush(Color.FromRgb(219, 234, 254));
                txtEstadoDia.Foreground = new SolidColorBrush(Color.FromRgb(30, 58, 138));
                txtEstadoDia.Text = "Actividad moderada";
            }
            else if (porcentajeOcupacion <= 60)
            {
                // Entre 31% y 60%: dia activo en amarillo claro.
                bdEstadoDia.Background = new SolidColorBrush(Color.FromRgb(254, 243, 199));
                txtEstadoDia.Foreground = new SolidColorBrush(Color.FromRgb(146, 64, 14));
                txtEstadoDia.Text = "Dia activo";
            }
            else
            {
                // Mas del 60% de ocupacion: dia muy ocupado en rojo claro.
                bdEstadoDia.Background = new SolidColorBrush(Color.FromRgb(254, 226, 226));
                txtEstadoDia.Foreground = new SolidColorBrush(Color.FromRgb(153, 27, 27));
                txtEstadoDia.Text = "Dia muy ocupado";
            }
        }

        /// <summary>
        /// Boton actualizar: recarga todos los datos de la pantalla.
        /// </summary>
        private void BtnRefrescar_Click(object sender, RoutedEventArgs e)
        {
            CargarTodo(); // Recargamos todo.
        }

        /// <summary>
        /// Flecha izquierda: retrocede un mes en la tarjeta de ingresos.
        /// </summary>
        private void BtnMesAnterior_Click(object sender, RoutedEventArgs e)
        {
            _mesIngresos = _mesIngresos.AddMonths(-1); // Restamos un mes.
            CargarIngresosMes(); // Recargamos la tarjeta de ingresos.
        }

        /// <summary>
        /// Flecha derecha: avanza un mes en la tarjeta de ingresos.
        /// </summary>
        private void BtnMesSiguiente_Click(object sender, RoutedEventArgs e)
        {
            _mesIngresos = _mesIngresos.AddMonths(1); // Sumamos un mes.
            CargarIngresosMes(); // Recargamos la tarjeta de ingresos.
        }
    }
}
