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
    /// Modal para buscar y seleccionar un agente
    /// </summary>
    public partial class ModalBuscarAgente : Window
    {
        private AgentesController _controller;
        private List<Agentes> _todosAgentes;

        public Agentes AgenteSeleccionado { get; private set; }

        public ModalBuscarAgente()
        {
            InitializeComponent();
            _controller = new AgentesController();
            CargarAgentes();
            txtBuscar.Focus();
        }

        private void CargarAgentes()
        {
            try
            {
                _todosAgentes = _controller.ObtenerTodos();
                dgAgentes.ItemsSource = _todosAgentes;
            }
            catch (Exception ex)
            {
                ModalMessage.Show(
                    $"Error al cargar agentes: {ex.Message}",
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
                dgAgentes.ItemsSource = _todosAgentes;
            }
            else
            {
                var filtrados = _todosAgentes.Where(a =>
                    a.Nombre.ToLower().Contains(filtro) ||
                    a.Email.ToLower().Contains(filtro) ||
                    a.Telefono.Contains(filtro)
                ).ToList();

                dgAgentes.ItemsSource = filtrados;
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            txtBuscar.Clear();
            dgAgentes.ItemsSource = _todosAgentes;
        }

        private void BtnSeleccionar_Click(object sender, RoutedEventArgs e)
        {
            if (dgAgentes.SelectedItem != null)
            {
                AgenteSeleccionado = dgAgentes.SelectedItem as Agentes;
                DialogResult = true;
                Close();
            }
            else
            {
                ModalMessage.Show(
                    "Debe seleccionar un agente de la lista.",
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

        private void DgAgentes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgAgentes.SelectedItem != null)
            {
                AgenteSeleccionado = dgAgentes.SelectedItem as Agentes;
                DialogResult = true;
                Close();
            }
        }
    }
}
