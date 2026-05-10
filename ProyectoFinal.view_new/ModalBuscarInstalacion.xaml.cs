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
    /// Modal reutilizable para buscar y seleccionar una instalacion.
    /// Se usa desde VistaReservas y cualquier vista que necesite elegir una instalacion.
    /// </summary>
    public partial class ModalBuscarInstalacion : Window
    {
        // Controlador de instalaciones.
        private readonly InstalacionesController _controller;

        // Lista completa cargada al abrir (para filtrar en memoria).
        private List<Instalaciones> _todasInstalaciones;

        /// <summary>
        /// Instalacion seleccionada por el usuario. Null si cancela.
        /// </summary>
        public Instalaciones InstalacionSeleccionada { get; private set; }

        /// <summary>
        /// Constructor: inicializa componentes y carga instalaciones.
        /// </summary>
        public ModalBuscarInstalacion()
        {
            InitializeComponent(); // Inicializamos la ventana.

            _controller = new InstalacionesController(); // Instanciamos controller.

            CargarInstalaciones(); // Cargamos instalaciones en el grid.

            txtBuscar.Focus(); // Foco en el buscador al abrir.
        }

        /// <summary>
        /// Carga todas las instalaciones disponibles en el DataGrid.
        /// </summary>
        private void CargarInstalaciones()
        {
            try
            {
                // Obtenemos solo las instalaciones disponibles: las no disponibles no deben poder reservarse.
                _todasInstalaciones = _controller.ObtenerTodos(soloDisponibles: true);

                // Asignamos al grid.
                dgInstalaciones.ItemsSource = _todasInstalaciones;

                // Actualizamos contador.
                txtTotal.Text = $"{_todasInstalaciones.Count} instalaciones";
            }
            catch (Exception ex)
            {
                // Mostramos error.
                ModalMessage.ShowModal($"No se pudieron cargar las instalaciones: {ex.Message}", "Buscar instalacion", 1);
            }
        }

        /// <summary>
        /// Permite arrastrar la ventana al hacer clic en la cabecera.
        /// </summary>
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) // Solo boton izquierdo.
                DragMove(); // Arrastramos.
        }

        /// <summary>
        /// Filtrado en tiempo real al escribir en el buscador.
        /// Busca por nombre de instalacion o nombre del tipo.
        /// </summary>
        private void TxtBuscar_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtro = (txtBuscar.Text ?? string.Empty).Trim().ToLower(); // Texto del filtro.

            if (string.IsNullOrEmpty(filtro))
            {
                // Sin filtro: mostramos todas.
                dgInstalaciones.ItemsSource = _todasInstalaciones;
                txtTotal.Text = $"{_todasInstalaciones.Count} instalaciones";
                return;
            }

            // Filtramos por nombre de instalacion o nombre del tipo.
            var filtradas = _todasInstalaciones.Where(i =>
                (i.Nombre ?? string.Empty).ToLower().Contains(filtro) ||
                (i.TiposInstalacion?.Nombre ?? string.Empty).ToLower().Contains(filtro)
            ).ToList();

            // Actualizamos grid y contador.
            dgInstalaciones.ItemsSource = filtradas;
            txtTotal.Text = $"{filtradas.Count} instalaciones";
        }

        /// <summary>
        /// Boton Limpiar: borra el filtro y muestra todas las instalaciones.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpiamos texto (dispara TxtBuscar_TextChanged).
        }

        /// <summary>
        /// Doble clic en una fila del grid: selecciona directamente esa instalacion.
        /// </summary>
        private void DgInstalaciones_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion(); // Reutilizamos logica de seleccion.
        }

        /// <summary>
        /// Boton Seleccionar: confirma la seleccion de la instalacion marcada en el grid.
        /// </summary>
        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            ConfirmarSeleccion(); // Reutilizamos logica de seleccion.
        }

        /// <summary>
        /// Boton Cancelar o X: cierra el modal sin seleccionar nada.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Indicamos cancelacion.
            Close(); // Cerramos.
        }

        /// <summary>
        /// Valida que haya una instalacion seleccionada y cierra el modal devolviendo true.
        /// Si no hay seleccion muestra un aviso.
        /// </summary>
        private void ConfirmarSeleccion()
        {
            var instalacion = dgInstalaciones.SelectedItem as Instalaciones; // Obtenemos seleccion.

            if (instalacion == null)
            {
                // Avisamos si no hay nada seleccionado.
                ModalMessage.ShowModal("Debe seleccionar una instalacion de la lista.", "Buscar instalacion", 1);
                return;
            }

            InstalacionSeleccionada = instalacion; // Guardamos la instalacion elegida.
            DialogResult = true; // Indicamos exito.
            Close(); // Cerramos.
        }
    }
}
