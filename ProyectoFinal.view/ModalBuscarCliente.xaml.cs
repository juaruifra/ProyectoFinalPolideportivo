using ProyectoFinal.controller;
using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ProyectoFinal.view
{
    /// <summary>
    /// Modal para buscar y seleccionar un cliente
    /// </summary>
    public partial class ModalBuscarCliente : Window
    {
        private ClientesController _controller;
        private List<Clientes> _todosClientes;

        public Clientes ClienteSeleccionado { get; private set; }

        public ModalBuscarCliente()
        {
            InitializeComponent();
            _controller = new ClientesController();
            CargarClientes();
            txtBuscar.Focus();
        }

        private void CargarClientes()
        {
            try
            {
                _todosClientes = _controller.ObtenerTodos();
                dgClientes.ItemsSource = _todosClientes;
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar clientes: {ex.Message}",
                    "Error",
                    ModalMessageType.Error,
                    this
                );
            }
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
            string filtro = txtBuscar.Text.ToLower().Trim();

            if (string.IsNullOrEmpty(filtro))
            {
                dgClientes.ItemsSource = _todosClientes;
            }
            else
            {
                var filtrados = _todosClientes.Where(c =>
                    c.Nombre.ToLower().Contains(filtro) ||
                    c.Email.ToLower().Contains(filtro) ||
                    c.Telefono.Contains(filtro)
                ).ToList();

                dgClientes.ItemsSource = filtrados;
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            dgClientes.ItemsSource = _todosClientes;
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            if (dgClientes.SelectedItem != null)
            {
                ClienteSeleccionado = dgClientes.SelectedItem as Clientes;
                DialogResult = true;
                Close();
            }
            else
            {
                ModalMessage.Show(
                    "Debe seleccionar un cliente de la lista.",
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

        private void DgClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgClientes.SelectedItem != null)
            {
                ClienteSeleccionado = dgClientes.SelectedItem as Clientes;
                DialogResult = true;
                Close();
            }
        }
    }
}