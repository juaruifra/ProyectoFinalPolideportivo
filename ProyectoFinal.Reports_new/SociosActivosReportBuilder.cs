using System;
using System.Collections.Generic;
using ProyectoFinal.controller_new.api;
using ProyectoFinal.model_new;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports_new
{
    /// <summary>
    /// Clase responsable de construir el informe de socios activos.
    /// Se encarga de:
    /// - Obtener los socios activos desde la API.
    /// - Rellenar el DataSet tipado SociosActivosDataSet.
    /// - Crear el informe Crystal (InformeSociosActivos).
    /// - Devolver un ReportDocument listo para visualizar.
    /// </summary>
    public class SociosActivosReportBuilder
    {
        // API de socios para obtener los datos de la base de datos.
        private readonly SociosAPI _sociosAPI;

        /// <summary>
        /// Constructor: inicializa la API de socios.
        /// </summary>
        public SociosActivosReportBuilder()
        {
            // Instanciamos la API de socios.
            _sociosAPI = new SociosAPI();
        }

        /// <summary>
        /// Construye el informe de socios activos:
        /// obtiene los datos, rellena el DataSet y crea el ReportDocument.
        /// </summary>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme()
        {
            // Crear una instancia del DataSet tipado para este informe.
            var ds = new SociosActivosDataSet();

            // Obtener la tabla SociosActivos del DataSet.
            var tabla = ds.SociosActivos;

            // Obtener todos los socios activos desde la base de datos.
            List<Socios> socios = _sociosAPI.ObtenerTodos(soloActivos: true);

            // Recorrer la lista de socios para volcar los datos al DataSet.
            foreach (var socio in socios)
            {
                // Crear una nueva fila del DataTable SociosActivos.
                var fila = tabla.NewSociosActivosRow();

                // Asignar el identificador del socio.
                fila.SocioId = socio.SocioId;

                // Asignar el nombre, usando cadena vacia si es null.
                fila.Nombre = socio.Nombre ?? string.Empty;

                // Asignar los apellidos, usando cadena vacia si es null.
                fila.Apellidos = socio.Apellidos ?? string.Empty;

                // Asignar el DNI, usando cadena vacia si es null.
                fila.DNI = socio.Dni ?? string.Empty;

                // Asignar el telefono, usando cadena vacia si es null.
                fila.Telefono = socio.Telefono ?? string.Empty;

                // Asignar el email, usando cadena vacia si es null.
                fila.Email = socio.Email ?? string.Empty;

                // Asignar la fecha de alta del socio.
                fila.FechaAlta = socio.FechaAlta;

                // Añadir la fila a la tabla del DataSet.
                tabla.AddSociosActivosRow(fila);
            }

            // Crear el informe Crystal Reports correspondiente.
            var report = new InformeSociosActivos();

            // Asignar el DataSet como origen de datos del informe.
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrarlo en el visor.
            return report;
        }
    }
}
