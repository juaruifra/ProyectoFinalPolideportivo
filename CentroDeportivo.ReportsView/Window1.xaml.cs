using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using CentroDeportivo.Reports;

namespace CentroDeportivo.ReportsView
{
    /// <summary>
    /// Ventana genérica de visualización de informes Crystal Reports.
    /// </summary>
    public partial class Window1 : Window
    {
        // Informe a visualizar
        private readonly ReportType _reportType;

        // parametro id de actividad para uno de los informes
        private readonly int? _actividadId;

        /// <summary>
        /// Constructor que recibe el tipo de informe a mostrar.
        /// </summary>
        /// <param name="reportType">Tipo de informe a cargar</param>
        /// <param name="actividadId">Id de la actividad (solo para algunos informes)</param>
        public Window1(ReportType reportType, int? actividadId = null)
        {
            InitializeComponent();

            _reportType = reportType;
            _actividadId = actividadId;

            CargarInforme();
        }

        /// <summary>
        /// Decide qué informe cargar según el tipo recibido.
        /// </summary>
        private void CargarInforme()
        {
            ReportDocument report = null;

            switch (_reportType)
            {
                case ReportType.Socios:
                    report = CargarInformeSocios();
                    break;

                case ReportType.ReservasPorActividad:

                    if (!_actividadId.HasValue)
                    {
                        MessageBox.Show("No se ha especificado la actividad.", "Error informe",MessageBoxButton.OK,MessageBoxImage.Error);
                        return;
                    }

                    report = CargarInformeReservasPorActividad(_actividadId.Value);
                    break;

                case ReportType.HistorialReservas:

                    report = CargarInformeHistorialReservas();
                    break;

                default:
                    MessageBox.Show("Informe no implementado");
                    return;
            }

            // Asignar el informe al visor WPF de Crystal
            reportViewer.ViewerCore.ReportSource = report;
        }

        /// <summary>
        /// Crea y devuelve el informe maestro de socios.
        /// </summary>
        /// <returns>Report</returns>
        private ReportDocument CargarInformeSocios()
        {
            using (var builder = new SociosReportBuilder())
            {
                return builder.CrearInformeMaestroSocios();
            }
        }

        /// <summary>
        /// Crea y devuelve el informe de reservas por actividad
        /// </summary>
        /// <param name="actividadId">Id de la actividad a mostrar</param>
        /// <returns>Report</returns>
        private ReportDocument CargarInformeReservasPorActividad(int actividadId)
        {
            using (var builder = new ReservasPorActividadReportBuilder())
            {
                return builder.CrearInforme(actividadId);
            }
        }

        /// <summary>
        /// Crear y devuelve el informe de historiald e reservas
        /// </summary>
        /// <returns>Report</returns>
        private ReportDocument CargarInformeHistorialReservas()
        {
            using (var builder = new HistorialReservasPorSocioReportBuilder())
            {
                return builder.CrearInforme();
            }
        }


    }
}
