using System;
using System.Collections.Generic;
using System.Linq;
using ProyectoFinal.model;
using ProyectoFinal.controller;
using CrystalDecisions.CrystalReports.Engine;

namespace ProyectoFinal.Reports
{
    /// <summary>
    /// Clase responsable de construir el informe de operaciones por agente.
    /// Se encarga de:
    /// - Obtener las operaciones desde la API.
    /// - Rellenar el DataSet tipado OperacionesPorAgenteDataSet.
    /// - Crear el informe Crystal (InformeOperacionesPorAgente).
    /// - Devolver un ReportDocument listo para visualizar.
    /// Este informe agrupa las operaciones por agente y totaliza por tipo.
    /// </summary>
    public class OperacionesPorAgenteReportBuilder
    {
        // API de operaciones
        private readonly OperacionesAPI _operacionesAPI;

        /// <summary>
        /// Constructor: inicializa la API.
        /// </summary>
        public OperacionesPorAgenteReportBuilder()
        {
            _operacionesAPI = new OperacionesAPI();
        }

        /// <summary>
        /// Construye el informe de operaciones por agente:
        /// - Obtiene los datos con navegación incluida.
        /// - Rellena el DataSet.
        /// - Crea y configura el ReportDocument de Crystal.
        /// </summary>
        /// <param name="idAgente">ID del agente a filtrar. Si es null, muestra todas las operaciones.</param>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInforme(int? idAgente = null)
        {
            // Crear una instancia del DataSet tipado
            var ds = new OperacionesPorAgenteDataSet();

            // Obtener la tabla OperacionesPorAgente del DataSet
            var tabla = ds.OperacionesPorAgente;

            // Obtener las operaciones desde la base de datos
            List<Operaciones> operaciones;
            
            if (idAgente.HasValue)
            {
                // Filtrar por agente específico
                operaciones = _operacionesAPI.ObtenerPorAgente(idAgente.Value);
            }
            else
            {
                // Obtener todas las operaciones
                operaciones = _operacionesAPI.ObtenerTodas();
            }

            // Ordenar por Agente y luego por Fecha
            var operacionesOrdenadas = operaciones
                .OrderBy(o => o.Agentes?.Nombre ?? string.Empty)
                .ThenBy(o => o.FechaOperacion)
                .ToList();

            // Volcar los datos al DataSet tipado
            foreach (var operacion in operacionesOrdenadas)
            {
                // Creamos una nueva fila del DataTable OperacionesPorAgente
                var fila = tabla.NewOperacionesPorAgenteRow();

                // Asignamos los campos
                fila.IdAgente = operacion.IdAgente;
                fila.NombreAgente = operacion.Agentes?.Nombre ?? "Sin Agente";
                fila.IdOperacion = operacion.IdOperacion;
                fila.FechaOperacion = operacion.FechaOperacion;
                fila.NombreCliente = operacion.Clientes?.Nombre ?? "Sin Cliente";
                
                // Incluir ciudad entre paréntesis en la dirección
                string direccion = operacion.Inmuebles?.Direccion ?? "Sin Dirección";
                string ciudad = operacion.Inmuebles?.Ciudad;
                fila.DireccionInmueble = !string.IsNullOrEmpty(ciudad) 
                    ? $"{direccion} ({ciudad})" 
                    : direccion;
                
                fila.TipoOperacion = operacion.TipoOperacion ?? string.Empty;
                fila.PrecioFinal = operacion.PrecioFinal;

                // Añadimos la fila a la tabla
                tabla.AddOperacionesPorAgenteRow(fila);
            }

            // Crear el informe Crystal correspondiente
            var report = new InformeOperacionesPorAgente();

            // Asignar el DataSet como origen de datos del informe
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrar
            return report;
        }
    }
}
