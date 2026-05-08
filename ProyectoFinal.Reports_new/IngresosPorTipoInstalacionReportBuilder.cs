using System;
using System.Collections.Generic;
using System.Linq;
using CrystalDecisions.CrystalReports.Engine;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;

namespace ProyectoFinal.Reports_new
{
    /// <summary>
    /// Clase responsable de construir el informe de ingresos y estadisticas por tipo de instalacion.
    /// Se encarga de:
    /// - Obtener todas las reservas desde la API.
    /// - Agrupar por tipo de instalacion y calcular totales y media.
    /// - Rellenar el DataSet tipado IngresosPorTipoInstalacionDataSet.
    /// - Crear el informe Crystal (InformeIngresosPorTipoInstalacion).
    /// - Devolver un ReportDocument listo para visualizar.
    /// </summary>
    public class IngresosPorTipoInstalacionReportBuilder
    {
        // API de reservas para obtener los datos de la base de datos.
        private readonly ReservasAPI _reservasAPI;

        /// <summary>
        /// Constructor: inicializa la API de reservas.
        /// </summary>
        public IngresosPorTipoInstalacionReportBuilder()
        {
            // Instanciamos la API de reservas.
            _reservasAPI = new ReservasAPI();
        }

        /// <summary>
        /// Construye el informe de ingresos por tipo de instalacion:
        /// agrupa las reservas, calcula totales y media, rellena el DataSet y crea el ReportDocument.
        /// </summary>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme()
        {
            // Crear una instancia del DataSet tipado para este informe.
            var ds = new IngresosPorTipoInstalacionDataSet();

            // Obtener la tabla IngresosPorTipo del DataSet.
            var tabla = ds.IngresosPorTipo;

            // Obtener todas las reservas con sus datos de instalacion y tipo.
            List<Reservas> reservas = _reservasAPI.ObtenerTodos();

            // Agrupar las reservas por tipo de instalacion y calcular estadisticas.
            var agrupadas = reservas
                .Where(r => r.Instalaciones != null && r.Instalaciones.TiposInstalacion != null)
                .GroupBy(r => new
                {
                    TipoInstalacionId = r.Instalaciones.TiposInstalacion.TipoInstalacionId,
                    NombreTipo = r.Instalaciones.TiposInstalacion.Nombre ?? string.Empty
                })
                .OrderBy(g => g.Key.NombreTipo)
                .ToList();

            // Recorrer cada grupo de tipo de instalacion.
            foreach (var grupo in agrupadas)
            {
                // Crear una nueva fila para este tipo de instalacion.
                var fila = tabla.NewIngresosPorTipoRow();

                // Asignar el identificador del tipo de instalacion.
                fila.TipoInstalacionId = grupo.Key.TipoInstalacionId;

                // Asignar el nombre del tipo de instalacion.
                fila.NombreTipo = grupo.Key.NombreTipo;

                // Calcular el numero total de reservas para este tipo.
                fila.NumReservas = grupo.Count();

                // Calcular la suma total de ingresos para este tipo.
                fila.TotalIngresos = grupo.Sum(r => r.PrecioTotal);

                // Calcular la media de ingresos por reserva para este tipo.
                fila.MediaPorReserva = grupo.Average(r => r.PrecioTotal);

                // Añadir la fila a la tabla del DataSet.
                tabla.AddIngresosPorTipoRow(fila);
            }

            // Crear el informe Crystal Reports correspondiente.
            var report = new InformeIngresosPorTipoInstalacion();

            // Asignar el DataSet como origen de datos del informe.
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrarlo en el visor.
            return report;
        }
    }
}
