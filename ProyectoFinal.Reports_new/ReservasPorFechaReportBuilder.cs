using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports_new
{
    /// <summary>
    /// Clase responsable de construir el informe de reservas filtradas por rango de fechas.
    /// Se encarga de:
    /// - Obtener las reservas comprendidas entre fechaDesde y fechaHasta desde la API.
    /// - Rellenar el DataSet tipado ReservasPorFechaDataSet.
    /// - Crear el informe Crystal (InformeReservasPorFecha).
    /// - Devolver un ReportDocument listo para visualizar.
    /// </summary>
    public class ReservasPorFechaReportBuilder
    {
        // API de reservas para obtener los datos de la base de datos.
        private readonly ReservasAPI _reservasAPI;

        /// <summary>
        /// Constructor: inicializa la API de reservas.
        /// </summary>
        public ReservasPorFechaReportBuilder()
        {
            // Instanciamos la API de reservas.
            _reservasAPI = new ReservasAPI();
        }

        /// <summary>
        /// Construye el informe de reservas filtradas por rango de fechas:
        /// obtiene y filtra los datos, rellena el DataSet y crea el ReportDocument.
        /// </summary>
        /// <param name="fechaDesde">Fecha de inicio del rango. Si es null usa el inicio del mes actual.</param>
        /// <param name="fechaHasta">Fecha de fin del rango. Si es null usa hoy.</param>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme(DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            // Si no se recibe fecha de inicio, usar el primer dia del mes actual.
            DateTime desde = fechaDesde ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // Si no se recibe fecha de fin, usar el dia de hoy a las 23:59:59.
            // Ademas, aunque se reciba fecha de fin, la ajustamos al final del dia (23:59:59)
            // para incluir todas las reservas de ese dia y no solo las de medianoche.
            DateTime hasta = (fechaHasta ?? DateTime.Today).Date.AddDays(1).AddSeconds(-1);

            // Crear una instancia del DataSet tipado para este informe.
            var ds = new ReservasPorFechaDataSet();

            // Obtener la tabla ReservasPorFecha del DataSet.
            var tabla = ds.ReservasPorFecha;

            // Obtener todas las reservas y filtrar por el rango de fechas recibido.
            List<Reservas> reservas = _reservasAPI.ObtenerTodos();

            // Filtrar solo las reservas cuya fecha de inicio este dentro del rango.
            var reservasFiltradas = reservas
                .Where(r => r.FechaHoraInicio >= desde && r.FechaHoraInicio <= hasta)
                .OrderBy(r => r.FechaHoraInicio)
                .ToList();

            // Recorrer la lista de reservas filtradas para volcar los datos al DataSet.
            foreach (var reserva in reservasFiltradas)
            {
                // Crear una nueva fila del DataTable ReservasPorFecha.
                var fila = tabla.NewReservasPorFechaRow();

                // Asignar el identificador de la reserva.
                fila.ReservaId = reserva.ReservaId;

                // Asignar la fecha y hora de inicio de la reserva.
                fila.FechaHoraInicio = reserva.FechaHoraInicio;

                // Asignar la fecha y hora de fin de la reserva.
                fila.FechaHoraFin = reserva.FechaHoraFin;

                // Asignar el nombre completo del socio combinando nombre y apellidos.
                fila.NombreSocio = reserva.Socios != null
                    ? $"{reserva.Socios.Nombre} {reserva.Socios.Apellidos}".Trim()
                    : string.Empty;

                // Asignar el nombre de la instalacion, usando cadena vacia si es null.
                fila.NombreInstalacion = reserva.Instalaciones != null
                    ? reserva.Instalaciones.Nombre ?? string.Empty
                    : string.Empty;

                // Asignar el precio total de la reserva.
                fila.PrecioTotal = reserva.PrecioTotal;

                // Asignar el estado de la reserva.
                fila.Estado = reserva.Estado ?? string.Empty;

                // Añadir la fila a la tabla del DataSet.
                tabla.AddReservasPorFechaRow(fila);
            }

            // Crear el informe Crystal Reports correspondiente.
            var report = new InformeReservasPorFecha();

            // Asignar el DataSet como origen de datos del informe.
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrarlo en el visor.
            return report;
        }
    }
}
