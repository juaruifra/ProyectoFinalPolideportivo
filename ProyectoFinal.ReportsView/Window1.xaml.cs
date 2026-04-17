using System.Windows;
using CrystalDecisions.CrystalReports.Engine;
using ProyectoFinal.Reports;

namespace ProyectoFinal.ReportsView
{
    /// <summary>
    /// Ventana genérica de visualización de informes Crystal Reports.
    /// </summary>
    public partial class Window1 : Window
    {
        // Informe a visualizar
        private readonly ReportType _reportType;

        // Parámetros opcionales para filtrado
        private readonly int? _idAgente;
        private readonly string _ciudad;

        /// <summary>
        /// Constructor que recibe el tipo de informe a mostrar.
        /// </summary>
        /// <param name="reportType">Tipo de informe a cargar</param>
        /// <param name="idAgente">Id del agente (solo para informe de operaciones filtrado)</param>
        /// <param name="ciudad">Ciudad (solo para informe de rentabilidad filtrado por ciudad)</param>
        public Window1(ReportType reportType, int? idAgente = null, string ciudad = null)
        {
            InitializeComponent();
            reportViewer.Owner = this;

            _reportType = reportType;
            _idAgente = idAgente;
            _ciudad = ciudad;

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
                case ReportType.InmueblesDisponibles:
                    report = CargarInformeInmueblesDisponibles();
                    this.Title = "Informe de Inmuebles Disponibles";
                    break;

                case ReportType.OperacionesPorAgente:
                    report = CargarInformeOperacionesPorAgente();
                    if (_idAgente.HasValue)
                        this.Title = "Informe de Operaciones por Agente";
                    else
                        this.Title = "Informe de Operaciones - Todos los Agentes";
                    break;

                case ReportType.RentabilidadPorCiudad:
                    report = CargarInformeRentabilidadPorCiudad();
                    if (!string.IsNullOrEmpty(_ciudad))
                        this.Title = $"Informe de Rentabilidad - {_ciudad}";
                    else
                        this.Title = "Informe de Rentabilidad por Ciudad";
                    break;

                default:
                    MessageBox.Show("Informe no implementado", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                    return;
            }

            if (report != null)
            {
                // Asignar el informe al visor WPF de Crystal
                reportViewer.ViewerCore.ReportSource = report;
            }
            else
            {
                MessageBox.Show("No se pudo generar el informe", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Crea y devuelve el informe de inmuebles disponibles.
        /// </summary>
        private ReportDocument CargarInformeInmueblesDisponibles()
        {
            var builder = new InmueblesDisponiblesReportBuilder();
            return builder.CrearInforme();
        }

        /// <summary>
        /// Crea y devuelve el informe de operaciones por agente.
        /// Puede filtrarse por un agente específico o mostrar todos.
        /// </summary>
        private ReportDocument CargarInformeOperacionesPorAgente()
        {
            var builder = new OperacionesPorAgenteReportBuilder();
            return builder.CrearInforme(_idAgente);
        }

        /// <summary>
        /// Crea y devuelve el informe de rentabilidad por ciudad.
        /// Puede filtrarse por ciudad específica o mostrar todas.
        /// </summary>
        private ReportDocument CargarInformeRentabilidadPorCiudad()
        {
            var builder = new RentabilidadPorCiudadReportBuilder();
            return builder.CrearInforme(_ciudad);
        }
    }
}
