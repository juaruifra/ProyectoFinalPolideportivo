using System;
using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using ProyectoFinal.Reports_new;

namespace ProyectoFinal.ReportsView_new
{
    /// <summary>
    /// Ventana generica de visualizacion de informes Crystal Reports.
    /// Recibe el tipo de informe y los parametros necesarios para generarlo.
    /// </summary>
    public partial class Window1 : Window
    {
        // Tipo de informe a visualizar.
        private readonly ReportType _reportType;

        // Fecha de inicio para el informe filtrado por fechas (informe 4).
        private readonly DateTime? _fechaDesde;

        // Fecha de fin para el informe filtrado por fechas (informe 4).
        private readonly DateTime? _fechaHasta;

        /// <summary>
        /// Constructor que recibe el tipo de informe y los parametros opcionales de filtrado.
        /// </summary>
        /// <param name="reportType">Tipo de informe a cargar.</param>
        /// <param name="fechaDesde">Fecha inicio para filtrar (solo informe de reservas por fecha).</param>
        /// <param name="fechaHasta">Fecha fin para filtrar (solo informe de reservas por fecha).</param>
        public Window1(ReportType reportType, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            // Inicializar los componentes de la ventana.
            InitializeComponent();

            // Asignar el propietario del visor a esta ventana.
            reportViewer.Owner = this;

            // Guardar el tipo de informe recibido.
            _reportType = reportType;

            // Guardar los parametros de fecha opcionales.
            _fechaDesde = fechaDesde;
            _fechaHasta = fechaHasta;

            // Cargar el informe correspondiente al tipo recibido.
            CargarInforme();
        }

        /// <summary>
        /// Decide que informe cargar segun el tipo recibido y lo muestra en el visor.
        /// </summary>
        private void CargarInforme()
        {
            // Variable para guardar el informe generado.
            ReportDocument report = null;

            // Seleccionar el builder correcto segun el tipo de informe.
            switch (_reportType)
            {
                case ReportType.SociosActivos:
                    // Informe 1: listado simple de socios activos.
                    report = CargarInformeSociosActivos();
                    // Poner titulo descriptivo en la ventana.
                    this.Title = "Informe de Socios Activos";
                    break;

                case ReportType.ReservasPorInstalacion:
                    // Informe 2: reservas agrupadas por instalacion.
                    report = CargarInformeReservasPorInstalacion();
                    // Poner titulo descriptivo en la ventana.
                    this.Title = "Informe de Reservas por Instalacion";
                    break;

                case ReportType.IngresosPorTipoInstalacion:
                    // Informe 3: ingresos y estadisticas por tipo de instalacion.
                    report = CargarInformeIngresosPorTipo();
                    // Poner titulo descriptivo en la ventana.
                    this.Title = "Informe de Ingresos por Tipo de Instalacion";
                    break;

                case ReportType.ReservasPorFecha:
                    // Informe 4: reservas filtradas por rango de fechas.
                    report = CargarInformeReservasPorFecha();
                    // Poner titulo con las fechas del filtro si estan disponibles.
                    if (_fechaDesde.HasValue && _fechaHasta.HasValue)
                        this.Title = $"Reservas del {_fechaDesde.Value:dd/MM/yyyy} al {_fechaHasta.Value:dd/MM/yyyy}";
                    else
                        this.Title = "Informe de Reservas por Fecha";
                    break;

                default:
                    // Si el tipo no esta implementado, mostrar error y cerrar.
                    MessageBox.Show("Informe no implementado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
            }

            // Si el informe se genero correctamente, asignarlo al visor.
            if (report != null)
            {
                // Asignar el ReportDocument al visor Crystal Reports WPF.
                reportViewer.ViewerCore.ReportSource = report;
            }
            else
            {
                // Si hubo un error al generar el informe, avisar y cerrar la ventana.
                MessageBox.Show("No se pudo generar el informe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Crea y devuelve el informe de socios activos usando su builder.
        /// </summary>
        /// <returns>ReportDocument del informe de socios activos.</returns>
        private ReportDocument CargarInformeSociosActivos()
        {
            // Instanciar el builder del informe de socios activos.
            var builder = new SociosActivosReportBuilder();
            // Crear y devolver el informe.
            return builder.CrearInforme();
        }

        /// <summary>
        /// Crea y devuelve el informe de reservas agrupadas por instalacion.
        /// Pendiente de implementar cuando se cree el builder correspondiente.
        /// </summary>
        /// <returns>ReportDocument del informe agrupado.</returns>
        private ReportDocument CargarInformeReservasPorInstalacion()
        {
            // Instanciar el builder del informe de reservas por instalacion.
            var builder = new ReservasPorInstalacionReportBuilder();
            // Crear y devolver el informe.
            return builder.CrearInforme();
        }

        /// <summary>
        /// Crea y devuelve el informe de ingresos y estadisticas por tipo de instalacion.
        /// Pendiente de implementar cuando se cree el builder correspondiente.
        /// </summary>
        /// <returns>ReportDocument del informe de totales.</returns>
        private ReportDocument CargarInformeIngresosPorTipo()
        {
            // Instanciar el builder del informe de ingresos por tipo.
            var builder = new IngresosPorTipoInstalacionReportBuilder();
            // Crear y devolver el informe.
            return builder.CrearInforme();
        }

        /// <summary>
        /// Crea y devuelve el informe de reservas filtradas por rango de fechas.
        /// Pendiente de implementar cuando se cree el builder correspondiente.
        /// </summary>
        /// <returns>ReportDocument del informe filtrado por fechas.</returns>
        private ReportDocument CargarInformeReservasPorFecha()
        {
            // Instanciar el builder del informe de reservas por fecha.
            var builder = new ReservasPorFechaReportBuilder();
            // Crear y devolver el informe con las fechas de filtro.
            return builder.CrearInforme(_fechaDesde, _fechaHasta);
        }
    }
}
