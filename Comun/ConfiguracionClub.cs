using System;

namespace Comun
{
    /// <summary>
    /// Configuracion general del club polideportivo.
    /// Los valores estan definidos como variables estaticas para facilitar
    /// su futura carga desde base de datos sin cambiar la logica que los consume.
    /// </summary>
    public static class ConfiguracionClub
    {
        /// <summary>
        /// Hora de apertura de las instalaciones.
        /// Futura fuente: tabla Configuracion en base de datos.
        /// </summary>
        public static TimeSpan HoraApertura = new TimeSpan(8, 0, 0); // 08:00

        /// <summary>
        /// Hora de cierre de las instalaciones.
        /// Futura fuente: tabla Configuracion en base de datos.
        /// </summary>
        public static TimeSpan HoraCierre = new TimeSpan(22, 0, 0); // 22:00

        /// <summary>
        /// Duracion minima en minutos para que un hueco se considere reservable.
        /// Futura fuente: tabla Configuracion en base de datos.
        /// </summary>
        public static int DuracionMinimaMinutos = 30;
    }
}
