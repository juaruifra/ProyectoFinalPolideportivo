using System.Collections.Generic;
using System.Linq;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports_new
{
    /// <summary>
    /// Clase responsable de construir el informe de reservas agrupadas por instalacion.
    /// Se encarga de:
    /// - Obtener todas las reservas desde la API.
    /// - Rellenar el DataSet tipado ReservasPorInstalacionDataSet.
    /// - Crear el informe Crystal (InformeReservasPorInstalacion).
    /// - Devolver un ReportDocument listo para visualizar.
    /// Las reservas se ordenan por instalacion y luego por fecha de inicio,
    /// para que Crystal Reports pueda agruparlas por instalacion en el .rpt.
    /// </summary>
    public class ReservasPorInstalacionReportBuilder
    {
        // API de reservas para obtener los datos de la base de datos.
        private readonly ReservasAPI _reservasAPI;

        /// <summary>
        /// Constructor: inicializa la API de reservas.
        /// </summary>
        public ReservasPorInstalacionReportBuilder()
        {
            // Instanciamos la API de reservas.
            _reservasAPI = new ReservasAPI();
        }

        /// <summary>
        /// Construye el informe de reservas agrupadas por instalacion:
        /// obtiene los datos, rellena el DataSet y crea el ReportDocument.
        /// </summary>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme()
        {
            // Crear una instancia del DataSet tipado para este informe.
            var ds = new ReservasPorInstalacionDataSet();

            // Obtener la tabla del DataSet (generada con guion bajo por el espacio en el nombre original).
            var tabla = ds.ReservasPorInstalacion_;

            // Obtener todas las reservas con sus datos de socio e instalacion.
            List<Reservas> reservas = _reservasAPI.ObtenerTodos();

            // Ordenar las reservas por nombre de instalacion y luego por fecha de inicio.
            var reservasOrdenadas = reservas
                .OrderBy(r => r.Instalaciones != null ? r.Instalaciones.Nombre : string.Empty)
                .ThenBy(r => r.FechaHoraInicio)
                .ToList();

            // Recorrer la lista de reservas para volcar los datos al DataSet.
            foreach (var reserva in reservasOrdenadas)
            {
                // Crear una nueva fila del DataTable ReservasPorInstalacion_.
                var fila = tabla.NewReservasPorInstalacion_Row();

                // Asignar el identificador de la reserva.
                fila.ReservaId = reserva.ReservaId;

                // Asignar el nombre de la instalacion, usando cadena vacia si es null.
                fila.NombreInstalacion = reserva.Instalaciones != null
                    ? reserva.Instalaciones.Nombre ?? string.Empty
                    : string.Empty;

                // Asignar el nombre completo del socio combinando nombre y apellidos.
                fila.NombreSocio = reserva.Socios != null
                    ? $"{reserva.Socios.Nombre} {reserva.Socios.Apellidos}".Trim()
                    : string.Empty;

                // Asignar la fecha y hora de inicio de la reserva.
                fila.FechaHoraInicio = reserva.FechaHoraInicio;

                // Asignar la fecha y hora de fin de la reserva.
                fila.FechaHoraFin = reserva.FechaHoraFin;

                // Asignar el precio total de la reserva.
                fila.PrecioTotal = reserva.PrecioTotal;

                // Asignar el estado de la reserva.
                fila.Estado = reserva.Estado ?? string.Empty;

                // Añadir la fila a la tabla del DataSet.
                tabla.AddReservasPorInstalacion_Row(fila);
            }

            // Crear el informe Crystal Reports correspondiente.
            var report = new InformeReservasPorInstalacion();

            // Asignar el DataSet como origen de datos del informe.
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrarlo en el visor.
            return report;
        }
    }
}
