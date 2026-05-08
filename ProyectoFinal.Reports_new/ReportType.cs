namespace ProyectoFinal.Reports_new
{
    /// <summary>
    /// Enumeracion con los tipos de informes disponibles en el club polideportivo.
    /// Cada valor corresponde a un informe Crystal Reports (.rpt) diferente.
    /// </summary>
    public enum ReportType
    {
        // Informe 1: listado simple de socios activos del club.
        SociosActivos,

        // Informe 2: reservas agrupadas por instalacion.
        ReservasPorInstalacion,

        // Informe 3: ingresos y estadisticas agrupados por tipo de instalacion.
        IngresosPorTipoInstalacion,

        // Informe 4: reservas filtradas por rango de fechas.
        ReservasPorFecha
    }
}
