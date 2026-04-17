using System;
using System.Linq;
using centroDeportivo.Model;
using centroDeportivo.Model.Repositories;
using CrystalDecisions.CrystalReports.Engine;

namespace CentroDeportivo.Reports
{
    /// <summary>
    /// Construye el informe de reservas filtrado por actividad.
    /// </summary>
    public class ReservasPorActividadReportBuilder : IDisposable
    {
        private readonly ReservasRepository _reservasRepo;
        private readonly ActividadesRepository _actividadesRepo;

        /// <summary>
        /// Constructor: inicializa los repositorios necesarios.
        /// </summary>
        public ReservasPorActividadReportBuilder()
        {
            _reservasRepo = new ReservasRepository();
            _actividadesRepo = new ActividadesRepository();
        }

        /// <summary>
        /// Crea el informe de reservas para una actividad concreta.
        /// </summary>
        /// <param name="actividadId">Id de la actividad seleccionada</param>
        /// <returns>ReportDocument listo para visualizar</returns>
        public ReportDocument CrearInforme(int actividadId)
        {
            // Crear la instancia del DataSet tipado
            var ds = new ReservasPorActividadDataSet();
            var tabla = ds.ReservasPorActividad;

            // Obtener la actividad (para nombre y aforo)
            // según la que se pasa por parámetro
            var actividad = _actividadesRepo.GetById(actividadId);

            // validacion existencia de actividad
            if (actividad == null) throw new Exception("La actividad seleccionada no existe.");

            // Obtener las reservas de esa actividad
            var reservas = _reservasRepo
                .GetAll()
                .Where(r => r.ActividadId == actividadId)
                .OrderBy(r => r.Fecha)
                .ToList();

            // Rellenar el DataSet
            foreach (var r in reservas)
            {
                var fila = tabla.NewReservasPorActividadRow();

                fila.NombreActividad = actividad.Nombre;
                fila.AforoMaximo = actividad.AforoMaximo;
                fila.FechaReserva = r.Fecha;
                fila.NombreSocio = r.Socios.Nombre;

                tabla.AddReservasPorActividadRow(fila);
            }

            // Crear el informe
            var report = new InformeReservasPorActividad();

            // Asignar DataSet
            report.SetDataSource(ds);

            return report;
        }

        /// <summary>
        /// Libera los recursos de los repositorios.
        /// </summary>
        public void Dispose()
        {
            _reservasRepo?.Dispose();
            _actividadesRepo?.Dispose();
        }
    }
}

