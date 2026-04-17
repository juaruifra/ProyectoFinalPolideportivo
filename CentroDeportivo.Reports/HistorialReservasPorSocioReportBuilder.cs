using System;
using System.Linq;
using centroDeportivo.Model;
using centroDeportivo.Model.Repositories;
using CrystalDecisions.CrystalReports.Engine;

namespace CentroDeportivo.Reports
{
    /// <summary>
    /// Construye el informe de historial de reservas agrupado por socio.
    /// </summary>
    public class HistorialReservasPorSocioReportBuilder : IDisposable
    {
        private readonly ReservasRepository _reservasRepo;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HistorialReservasPorSocioReportBuilder()
        {
            _reservasRepo = new ReservasRepository();
        }

        /// <summary>
        /// Crea el informe de historial de reservas por socio.
        /// </summary>
        /// <returns>ReportDocument listo para visualizar</returns>
        public ReportDocument CrearInforme()
        {
            // Crear DataSet tipado
            var ds = new HistorialReservasPorSocioDataSet();
            var tabla = ds.HistorialReservas;

            // Obtener todas las reservas, ya cruzadas con socios y actividades
            var reservas = _reservasRepo
                .GetAll()
                .OrderBy(r => r.Socios.Nombre)
                .ThenBy(r => r.Fecha)
                .ToList();

            // Rellenar el DataSet
            foreach (var r in reservas)
            {
                var fila = tabla.NewHistorialReservasRow();

                fila.NombreSocio = r.Socios.Nombre;
                fila.NombreActividad = r.Actividades.Nombre;
                fila.FechaReserva = r.Fecha;

                tabla.AddHistorialReservasRow(fila);
            }

            // Crear el informe Crystal
            var report = new InformeHistorialReservasPorSocio();

            // Asignar DataSet
            report.SetDataSource(ds);

            return report;
        }

        /// <summary>
        /// Liberar recursos.
        /// </summary>
        public void Dispose()
        {
            _reservasRepo?.Dispose();
        }
    }
}

