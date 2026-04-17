using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ProyectoFinal.controller;
using ProyectoFinal.ReportsView;
using ProyectoFinal.Reports;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Lógica de interacción para VistaInformes.xaml
    /// Vista que permite al usuario seleccionar y generar diferentes tipos de informes
    /// </summary>
    public partial class VistaInformes : UserControl
    {
        // APIs para obtener datos (solo para llenar los ComboBox)
        private readonly AgentesAPI _agentesAPI;
        private readonly InmueblesAPI _inmueblesAPI;

        public VistaInformes()
        {
            InitializeComponent();

            // Inicializar APIs
            _agentesAPI = new AgentesAPI();
            _inmueblesAPI = new InmueblesAPI();

            // Cargar datos en los controles
            CargarAgentes();
            CargarCiudades();
        }

        #region Carga de Datos en ComboBox

        /// <summary>
        /// Carga la lista de agentes en el ComboBox
        /// </summary>
        private void CargarAgentes()
        {
            try
            {
                var agentes = _agentesAPI.ObtenerTodos();

                if (agentes != null && agentes.Count > 0)
                {
                    cmbAgentes.ItemsSource = agentes;
                    cmbAgentes.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al cargar agentes: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Carga la lista de ciudades únicas desde los inmuebles
        /// </summary>
        private void CargarCiudades()
        {
            try
            {
                var inmuebles = _inmueblesAPI.ObtenerTodos();

                if (inmuebles != null && inmuebles.Count > 0)
                {
                    // Obtener ciudades únicas y ordenadas
                    var ciudades = inmuebles
                        .Where(i => !string.IsNullOrEmpty(i.Ciudad))
                        .Select(i => i.Ciudad)
                        .Distinct()
                        .OrderBy(c => c)
                        .ToList();

                    cmbCiudades.ItemsSource = ciudades;
                    if (ciudades.Count > 0)
                        cmbCiudades.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al cargar ciudades: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        #endregion

        #region Eventos de Botones - Informe 1: Inmuebles Disponibles

        /// <summary>
        /// Genera el informe de inmuebles disponibles
        /// </summary>
        private void BtnInmueblesDisponibles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear y mostrar el visor con el tipo de informe
                Window1 visor = new Window1(ReportType.InmueblesDisponibles);
                ConfigurarVisor(visor);
                visor.ShowDialog();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al generar el informe: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        #endregion

        #region Eventos de Botones - Informe 2: Operaciones por Agente

        /// <summary>
        /// Genera el informe de todas las operaciones (todos los agentes)
        /// </summary>
        private void BtnOperacionesTodas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear y mostrar el visor sin filtro de agente
                Window1 visor = new Window1(ReportType.OperacionesPorAgente);
                ConfigurarVisor(visor);
                visor.ShowDialog();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al generar el informe: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Genera el informe de operaciones filtrado por el agente seleccionado
        /// </summary>
        private void BtnOperacionesPorAgente_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que hay un agente seleccionado
                if (cmbAgentes.SelectedValue == null)
                {
                    ModalMessage.Show(
                        "Por favor, seleccione un agente.",
                        "Validación",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Obtener el ID del agente seleccionado
                int idAgente = (int)cmbAgentes.SelectedValue;

                // Crear y mostrar el visor con filtro de agente
                Window1 visor = new Window1(ReportType.OperacionesPorAgente, idAgente);
                ConfigurarVisor(visor);
                visor.ShowDialog();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al generar el informe: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        #endregion

        #region Eventos de Botones - Informe 3: Rentabilidad por Ciudad

        /// <summary>
        /// Genera el informe de rentabilidad de todas las ciudades
        /// </summary>
        private void BtnRentabilidadTodas_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Crear y mostrar el visor sin filtro
                Window1 visor = new Window1(ReportType.RentabilidadPorCiudad);
                ConfigurarVisor(visor);
                visor.ShowDialog();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al generar el informe: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        /// <summary>
        /// Genera el informe de rentabilidad filtrado por ciudad
        /// </summary>
        private void BtnRentabilidadPorCiudad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar que hay una ciudad seleccionada
                if (cmbCiudades.SelectedItem == null)
                {
                    ModalMessage.Show(
                        "Por favor, seleccione una ciudad.",
                        "Validación",
                        ModalMessageType.Info,
                        Window.GetWindow(this)
                    );
                    return;
                }

                // Obtener la ciudad seleccionada
                string ciudad = cmbCiudades.SelectedItem.ToString();

                // Crear y mostrar el visor con filtro de ciudad
                Window1 visor = new Window1(ReportType.RentabilidadPorCiudad, null, ciudad);
                ConfigurarVisor(visor);
                visor.ShowDialog();
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    "Error al generar el informe: " + ex.Message,
                    "Error",
                    ModalMessageType.Error,
                    Window.GetWindow(this)
                );
            }
        }

        #endregion

        #region Método Auxiliar

        /// <summary>
        /// Configura las propiedades comunes del visor
        /// </summary>
        private void ConfigurarVisor(Window1 visor)
        {
            visor.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            visor.Width = 1200;
            visor.Height = 700;
        }

        #endregion
    }
}
