using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Modal para buscar y seleccionar un inmueble
    /// </summary>
    public partial class ModalBuscarInmueble : Window
    {
        private InmueblesController _controller;
        private List<Inmuebles> _todosInmuebles;
        private string _tipoOperacion;

        public Inmuebles InmuebleSeleccionado { get; private set; }

        public ModalBuscarInmueble(string tipoOperacion)
        {
            InitializeComponent();
            _controller = new InmueblesController();
            _tipoOperacion = tipoOperacion;
            
            // Mostrar el tipo de operación en el título entre paréntesis
            txtTipoOperacion.Text = $"({tipoOperacion})";
            
            CargarInmuebles();
            txtBuscar.Focus();
        }

        private void CargarInmuebles()
        {
            try
            {
                _todosInmuebles = _controller.ObtenerTodos()
                    .Where(i => i.TipoOperacion == _tipoOperacion && 
                                i.Estado == InmueblesConstantes.ESTADO_DISPONIBLE)
                    .ToList();

                dgInmuebles.ItemsSource = _todosInmuebles;
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar inmuebles: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    this
                );
            }
        }

        private void AplicarFiltros()
        {
            string filtroTexto = txtBuscar.Text.ToLower().Trim();
            string precioDesdeTexto = txtPrecioDesde.Text.Trim();
            string precioHastaTexto = txtPrecioHasta.Text.Trim();

            var filtrados = _todosInmuebles.AsEnumerable();

            // Filtro por texto (dirección o ciudad)
            if (!string.IsNullOrEmpty(filtroTexto))
            {
                filtrados = filtrados.Where(i =>
                    i.Direccion.ToLower().Contains(filtroTexto) ||
                    i.Ciudad.ToLower().Contains(filtroTexto)
                );
            }

            // Filtro por precio desde
            if (!string.IsNullOrEmpty(precioDesdeTexto))
            {
                if (decimal.TryParse(precioDesdeTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precioDesde))
                {
                    filtrados = filtrados.Where(i => i.Precio >= precioDesde);
                }
            }

            // Filtro por precio hasta
            if (!string.IsNullOrEmpty(precioHastaTexto))
            {
                if (decimal.TryParse(precioHastaTexto, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal precioHasta))
                {
                    filtrados = filtrados.Where(i => i.Precio <= precioHasta);
                }
            }

            dgInmuebles.ItemsSource = filtrados.ToList();
        }

        /// <summary>
        /// Permite arrastrar la ventana al hacer clic en la cabecera
        /// </summary>
        private void Header_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void TxtBuscar_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void TxtPrecio_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            txtPrecioDesde.Clear();
            txtPrecioHasta.Clear();
            dgInmuebles.ItemsSource = _todosInmuebles;
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            if (dgInmuebles.SelectedItem != null)
            {
                InmuebleSeleccionado = dgInmuebles.SelectedItem as Inmuebles;
                DialogResult = true;
                Close();
            }
            else
            {
                ModalMessage.Show(
                    "Debe seleccionar un inmueble de la lista.",
                    "Seleccion requerida",
                    ModalMessageType.Error,
                    this
                );
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DgInmuebles_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgInmuebles.SelectedItem != null)
            {
                InmuebleSeleccionado = dgInmuebles.SelectedItem as Inmuebles;
                DialogResult = true;
                Close();
            }
        }
    }
}
