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
    /// Modal reutilizable para buscar y seleccionar un socio activo.
    /// Se usa desde VistaCuotas, VistaReservas y cualquier vista que necesite elegir un socio.
    /// </summary>
    public partial class ModalBuscarSocio : Window
    {
        // Controlador de socios.
        private readonly SociosController _controller;

        // Lista completa de socios cargados (para filtrar en memoria).
        private List<Socios> _todosSocios;

        /// <summary>
        /// Socio seleccionado por el usuario. Null si cancela.
        /// </summary>
        public Socios SocioSeleccionado { get; private set; }

        /// <summary>
        /// Constructor: inicializa componentes y carga socios activos.
        /// </summary>
        public ModalBuscarSocio()
        {
            InitializeComponent(); // Inicializamos la ventana.

            _controller = new SociosController(); // Instanciamos controller.

            CargarSocios(); // Cargamos socios en el grid.

            txtBuscar.Focus(); // Foco en el buscador al abrir.
        }

        /// <summary>
        /// Carga todos los socios activos en el DataGrid.
        /// </summary>
        private void CargarSocios()
        {
            try
            {
                // Obtenemos socios activos del controller.
                _todosSocios = _controller.ObtenerTodos(soloActivos: true);

                // Asignamos al grid.
                dgSocios.ItemsSource = _todosSocios;

                // Actualizamos contador.
                txtTotal.Text = $"{_todosSocios.Count} socios";
            }
            catch (Exception ex)
            {
                // Mostramos error.
                ModalMessage.ShowModal($"No se pudieron cargar los socios: {ex.Message}", "Buscar socio", 1);
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
        /// Busca por nombre completo, DNI o telefono.
        /// </summary>
        private void TxtBuscar_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var filtro = (txtBuscar.Text ?? string.Empty).Trim().ToLower(); // Texto del filtro.

            if (string.IsNullOrEmpty(filtro))
            {
                // Sin filtro: mostramos todos.
                dgSocios.ItemsSource = _todosSocios;
                txtTotal.Text = $"{_todosSocios.Count} socios";
                return;
            }

            // Filtramos por nombre completo, DNI o telefono.
            var filtrados = _todosSocios.Where(s =>
                (s.NombreCompleto ?? string.Empty).ToLower().Contains(filtro) ||
                (s.Dni            ?? string.Empty).ToLower().Contains(filtro) ||
                (s.Telefono       ?? string.Empty).ToLower().Contains(filtro) ||
                (s.Email          ?? string.Empty).ToLower().Contains(filtro)
            ).ToList();

            // Actualizamos grid y contador.
            dgSocios.ItemsSource = filtrados;
            txtTotal.Text = $"{filtrados.Count} socios";
        }

        /// <summary>
        /// Boton Limpiar: borra el filtro y muestra todos los socios.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear(); // Limpiamos texto (dispara TxtBuscar_TextChanged).
        }

        /// <summary>
        /// Doble clic en una fila del grid: selecciona directamente ese socio.
        /// </summary>
        private void DgSocios_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ConfirmarSeleccion(); // Reutilizamos logica de seleccion.
        }

        /// <summary>
        /// Boton Seleccionar: confirma la seleccion del socio marcado en el grid.
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
        /// Valida que haya un socio seleccionado y cierra el modal devolviendo true.
        /// Si no hay seleccion muestra un aviso.
        /// </summary>
        private void ConfirmarSeleccion()
        {
            var socio = dgSocios.SelectedItem as Socios; // Obtenemos seleccion del grid.

            if (socio == null)
            {
                // Avisamos si no hay nada seleccionado.
                ModalMessage.ShowModal("Debe seleccionar un socio de la lista.", "Buscar socio", 1);
                return;
            }

            SocioSeleccionado = socio; // Guardamos el socio elegido.
            DialogResult = true; // Indicamos exito.
            Close(); // Cerramos.
        }
    }
}
