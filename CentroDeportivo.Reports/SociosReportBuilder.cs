using System;
using System.Collections.Generic;
using centroDeportivo.Model;
using centroDeportivo.Model.Repositories;
using CrystalDecisions.CrystalReports.Engine;

namespace CentroDeportivo.Reports
{
    /// <summary>
    /// Clase responsable de construir el informe maestro de socios.
    /// Se encarga de:
    /// Obtener los socios desde el repositorio (EF).
    /// Rellenar el DataSet tipado SociosReportDataSet.
    /// Crear el informe Crystal (InformeMaestroSocios).
    /// Devolver un ReportDocument listo para visualizar.
    /// </summary>
    public class SociosReportBuilder : IDisposable
    {
        // Repositorio de socios, usa EF internamente.
        private readonly SociosRepository _socioRepo;

        /// <summary>
        /// Constructor: inicializa el repositorio.
        /// </summary>
        public SociosReportBuilder()
        {
            // Creamos el repositorio que usa el DbContext
            _socioRepo = new SociosRepository();
        }

        /// <summary>
        /// Construye el informe maestro de socios:
        /// - Obtiene los datos.
        /// - Rellena el DataSet.
        /// - Crea y configura el ReportDocument de Crystal.
        /// </summary>
        /// <returns>ReportDocument listo para asignarlo al visor WPF.</returns>
        public ReportDocument CrearInformeMaestroSocios()
        {
            // Crear una instancia del DataSet tipado
            var ds = new SociosReportDataSet();

            // Obtener la tabla Socios del DataSet
            var tablaSocios = ds.Socios;

            // Obtener los socios desde la base de datos a través de EF
            //    Suponemos que tienes un método GetAll() en SociosRepository.
            List<Socios> socios = _socioRepo.GetAll();

            // Volcar los datos de EF al DataSet tipado
            foreach (var socio in socios)
            {
                // Creamos una nueva fila del DataTable Socios
                var fila = tablaSocios.NewSociosRow();

                // Asignamos los campos
                fila.Id = socio.Id;
                fila.Nombre = socio.Nombre;
                fila.Email = socio.Email;
                fila.Activo = socio.Activo;

                // Añadimos la fila a la tabla
                tablaSocios.AddSociosRow(fila);
            }

            // Crear el informe Crystal correspondiente
            var report = new InformeMaestroSocios();

            // Asignar el DataSet como origen de datos del informe
            report.SetDataSource(ds);

            // Devolver el ReportDocument listo para mostrar
            return report;
        }

        /// <summary>
        /// Libera los recursos del repositorio (DbContext).
        /// </summary>
        public void Dispose()
        {
            // Si tu repositorio implementa IDisposable, lo limpiamos
            _socioRepo?.Dispose();
        }
    }
}

