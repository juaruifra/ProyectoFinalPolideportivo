using System;
using System.Windows;
using System.Windows.Controls;
using ProyectoFinal.Reports_new;
using ProyectoFinal.ReportsView_new;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// Vista del selector de informes del club polideportivo.
    /// Permite al usuario elegir que informe quiere visualizar
    /// y lanzar la ventana de Crystal Reports correspondiente.
    /// </summary>
    public partial class VistaInformes : UserControl
    {
        /// <summary>
        /// Constructor: inicializa los componentes de la vista.
        /// </summary>
        public VistaInformes()
        {
            // Inicializamos los componentes definidos en el XAML.
            InitializeComponent();

            // Establecer el primer dia del mes actual como fecha desde por defecto.
            DpFechaDesde.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // Establecer hoy como fecha hasta por defecto.
            DpFechaHasta.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Abre el informe de socios activos al pulsar su boton.
        /// </summary>
        private void BtnSociosActivos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear la ventana del visor pasando el tipo de informe de socios activos.
                var ventana = new Window1(ReportType.SociosActivos);

                // Mostrar la ventana como dialogo modal.
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                // Mostrar error si no se puede abrir el informe.
                MessageBox.Show($"Error al abrir el informe: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre el informe de reservas agrupadas por instalacion al pulsar su boton.
        /// </summary>
        private void BtnReservasPorInstalacion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear la ventana del visor pasando el tipo de informe de reservas por instalacion.
                var ventana = new Window1(ReportType.ReservasPorInstalacion);

                // Mostrar la ventana como dialogo modal.
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                // Mostrar error si no se puede abrir el informe.
                MessageBox.Show($"Error al abrir el informe: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre el informe de ingresos por tipo de instalacion al pulsar su boton.
        /// </summary>
        private void BtnIngresosPorTipo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear la ventana del visor pasando el tipo de informe de ingresos por tipo.
                var ventana = new Window1(ReportType.IngresosPorTipoInstalacion);

                // Mostrar la ventana como dialogo modal.
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                // Mostrar error si no se puede abrir el informe.
                MessageBox.Show($"Error al abrir el informe: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Abre el informe de reservas filtradas por rango de fechas al pulsar su boton.
        /// Valida que se hayan seleccionado ambas fechas y que desde sea anterior a hasta.
        /// </summary>
        private void BtnReservasPorFecha_Click(object sender, RoutedEventArgs e)
        {
            // Comprobar que se ha seleccionado la fecha de inicio.
            if (!DpFechaDesde.SelectedDate.HasValue)
            {
                // Avisar al usuario que debe seleccionar la fecha de inicio.
                MessageBox.Show("Selecciona la fecha de inicio.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Comprobar que se ha seleccionado la fecha de fin.
            if (!DpFechaHasta.SelectedDate.HasValue)
            {
                // Avisar al usuario que debe seleccionar la fecha de fin.
                MessageBox.Show("Selecciona la fecha de fin.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Leer las fechas seleccionadas en los DatePicker.
            DateTime fechaDesde = DpFechaDesde.SelectedDate.Value;
            DateTime fechaHasta = DpFechaHasta.SelectedDate.Value;

            // Comprobar que la fecha de inicio no es posterior a la de fin.
            if (fechaDesde > fechaHasta)
            {
                // Avisar al usuario del error en el rango de fechas.
                MessageBox.Show("La fecha de inicio no puede ser posterior a la fecha de fin.", "Aviso",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Crear la ventana del visor pasando el tipo y las fechas de filtro.
                var ventana = new Window1(ReportType.ReservasPorFecha, fechaDesde, fechaHasta);

                // Mostrar la ventana como dialogo modal.
                ventana.ShowDialog();
            }
            catch (Exception ex)
            {
                // Mostrar error si no se puede abrir el informe.
                MessageBox.Show($"Error al abrir el informe: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
