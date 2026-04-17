using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal.model;
using ProyectoFinal.controller;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports
{
    /// <summary>
    /// Clase responsable de construir el informe de inmuebles disponibles.
    /// Se encarga de:
    /// - Obtener los inmuebles desde la API.
    /// - Rellenar el DataSet tipado InmueblesDisponiblesDataSet.
    /// - Crear el informe Crystal (InformeInmueblesDisponibles).
    /// - Devolver un ReportDocument listo para visualizar.
    /// </summary>
    public class InmueblesDisponiblesReportBuilder
    {
        // API de inmuebles
        private readonly InmueblesAPI _inmueblesAPI;

        /// <summary>
        /// Constructor: inicializa la API.
        /// </summary>
        public InmueblesDisponiblesReportBuilder()
        {
            _inmueblesAPI = new InmueblesAPI();
        }

        /// <summary>
        /// Construye el informe de inmuebles disponibles:
        /// - Obtiene los datos.
        /// - Rellena el DataSet.
        /// - Crea y configura el ReportDocument de Crystal.
        /// </summary>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme()
        {
            // Crear una instancia del DataSet tipado
            var ds = new InmueblesDisponiblesDataSet();

            // Obtener la tabla InmueblesDisponibles del DataSet
            var tabla = ds.InmueblesDisponibles;

            // Obtener todos los inmuebles desde la base de datos
            List<Inmuebles> inmuebles = _inmueblesAPI.ObtenerTodos();

            // Filtrar solo los disponibles y ordenar por Ciudad y Precio
            var inmueblesDisponibles = inmuebles
                .Where(i => i.Estado == "DISPONIBLE")
                .OrderBy(i => i.Ciudad)
                .ThenBy(i => i.Precio)
                .ToList();

            // Volcar los datos al DataSet tipado
            foreach (var inmueble in inmueblesDisponibles)
            {
                // Creamos una nueva fila del DataTable InmueblesDisponibles
                var fila = tabla.NewInmueblesDisponiblesRow();

                // Asignamos los campos
                fila.IdInmueble = inmueble.IdInmueble;
                fila.Direccion = inmueble.Direccion ?? string.Empty;
                fila.Ciudad = inmueble.Ciudad ?? string.Empty;
                fila.Precio = inmueble.Precio;
                fila.TipoOperacion = inmueble.TipoOperacion ?? string.Empty;
                fila.Estado = inmueble.Estado ?? string.Empty;
                fila.FechaAlta = inmueble.FechaAlta;

                // Añadimos la fila a la tabla
                tabla.AddInmueblesDisponiblesRow(fila);
            }

            // Crear el informe Crystal correspondiente
            var report = new InformeInmueblesDisponibles();

            // Asignar el DataSet como origen de datos del informe
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrar
            return report;
        }
    }
}
