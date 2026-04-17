using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal.model;
using ProyectoFinal.controller;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports
{
    /// <summary>
    /// Clase responsable de construir el informe de rentabilidad por ciudad.
    /// Se encarga de:
    /// - Obtener las operaciones desde la API.
    /// - Rellenar el DataSet tipado RentabilidadPorCiudadDataSet.
    /// - Crear el informe Crystal (InformeRentabilidadPorCiudad).
    /// - Devolver un ReportDocument listo para visualizar.
    /// Este informe agrupa las operaciones por ciudad y totaliza ingresos.
    /// </summary>
    public class RentabilidadPorCiudadReportBuilder
    {
        // API de operaciones
        private readonly OperacionesAPI _operacionesAPI;

        /// <summary>
        /// Constructor: inicializa la API.
        /// </summary>
        public RentabilidadPorCiudadReportBuilder()
        {
            _operacionesAPI = new OperacionesAPI();
        }

        /// <summary>
        /// Construye el informe de rentabilidad por ciudad:
        /// - Obtiene los datos con navegación incluida.
        /// - Rellena el DataSet.
        /// - Crea y configura el ReportDocument de Crystal.
        /// </summary>
        /// <param name="ciudad">Ciudad a filtrar. Si es null, muestra todas las ciudades.</param>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme(string ciudad = null)
        {
            // Crear una instancia del DataSet tipado
            var ds = new RentabilidadPorCiudadDataSet();

            // Obtener la tabla RentabilidadPorCiudad del DataSet
            var tabla = ds.RentabilidadPorCiudad;

            // Obtener todas las operaciones desde la base de datos
            List<Operaciones> operaciones = _operacionesAPI.ObtenerTodas();

            // Filtrar por ciudad si se especifica
            if (!string.IsNullOrEmpty(ciudad))
            {
                operaciones = operaciones
                    .Where(o => o.Inmuebles != null && 
                               o.Inmuebles.Ciudad != null && 
                               o.Inmuebles.Ciudad.Equals(ciudad, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            // Ordenar por Ciudad y luego por Fecha
            var operacionesOrdenadas = operaciones
                .OrderBy(o => o.Inmuebles?.Ciudad ?? "Sin Ciudad")
                .ThenBy(o => o.FechaOperacion)
                .ToList();

            // Volcar los datos al DataSet tipado
            foreach (var operacion in operacionesOrdenadas)
            {
                // Creamos una nueva fila del DataTable RentabilidadPorCiudad
                var fila = tabla.NewRentabilidadPorCiudadRow();

                // Asignamos los campos
                fila.Ciudad = operacion.Inmuebles?.Ciudad ?? "Sin Ciudad";
                fila.DireccionInmueble = operacion.Inmuebles?.Direccion ?? "Sin Dirección";
                fila.TipoOperacion = operacion.TipoOperacion ?? string.Empty;
                fila.PrecioFinal = operacion.PrecioFinal;
                fila.FechaOperacion = operacion.FechaOperacion;
                fila.NombreCliente = operacion.Clientes?.Nombre ?? "Sin Cliente";
                fila.NombreAgente = operacion.Agentes?.Nombre ?? "Sin Agente";

                // Añadimos la fila a la tabla
                tabla.AddRentabilidadPorCiudadRow(fila);
            }

            // Crear el informe Crystal correspondiente
            var report = new InformeRentabilidadPorCiudad();

            // Asignar el DataSet como origen de datos del informe
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrar
            return report;
        }
    }
}
