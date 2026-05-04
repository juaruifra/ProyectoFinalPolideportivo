using ProyectoFinal.model_new;
using System;

namespace ProyectoFinal.view_new
{
    /// <summary>
    /// DTO que representa un hueco libre reservable en una instalacion para un dia concreto.
    /// Solo se usa para mostrar en el grid del modal de nueva reserva.
    /// </summary>
    public class HuecoDiario
    {
        /// <summary>
        /// Instalacion a la que pertenece el hueco.
        /// </summary>
        public Instalaciones Instalacion { get; set; }

        /// <summary>
        /// Hora de inicio del hueco libre.
        /// </summary>
        public TimeSpan HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin del hueco libre.
        /// </summary>
        public TimeSpan HoraFin { get; set; }

        /// <summary>
        /// Duracion del hueco en minutos (calculado a partir de HoraInicio y HoraFin).
        /// </summary>
        public int DuracionMinutos
        {
            // Calculamos la diferencia en minutos entre inicio y fin.
            get { return (int)(HoraFin - HoraInicio).TotalMinutes; }
        }

        /// <summary>
        /// Hora de inicio formateada como HH:mm para mostrar en el grid.
        /// </summary>
        public string HoraInicioTexto
        {
            // Formateamos el TimeSpan como horas y minutos con dos digitos.
            get { return HoraInicio.ToString("hh\\:mm"); }
        }

        /// <summary>
        /// Hora de fin formateada como HH:mm para mostrar en el grid.
        /// </summary>
        public string HoraFinTexto
        {
            // Formateamos el TimeSpan como horas y minutos con dos digitos.
            get { return HoraFin.ToString("hh\\:mm"); }
        }

        /// <summary>
        /// Nombre de la instalacion obtenido de la entidad Instalacion.
        /// </summary>
        public string NombreInstalacion
        {
            // Devolvemos el nombre o cadena vacia si no hay instalacion.
            get { return Instalacion != null ? Instalacion.Nombre : string.Empty; }
        }

        /// <summary>
        /// Nombre del tipo de instalacion obtenido de la navegacion TiposInstalacion.
        /// </summary>
        public string TipoInstalacion
        {
            // Devolvemos el tipo si existe la relacion de navegacion.
            get
            {
                if (Instalacion == null) return string.Empty;
                if (Instalacion.TiposInstalacion == null) return string.Empty;
                return Instalacion.TiposInstalacion.Nombre;
            }
        }

        /// <summary>
        /// Precio por hora de la instalacion.
        /// </summary>
        public decimal PrecioHora
        {
            // Devolvemos el precio/hora de la instalacion o cero si no hay.
            get { return Instalacion != null ? Instalacion.PrecioHora : 0m; }
        }

        /// <summary>
        /// Duracion del hueco formateada como "Xh Ymin" para mostrar en el grid.
        /// </summary>
        public string DuracionTexto
        {
            get
            {
                // Calculamos horas y minutos restantes.
                int horas = DuracionMinutos / 60;
                int minutos = DuracionMinutos % 60;

                // Formateamos segun si hay horas completas o no.
                if (horas > 0 && minutos > 0)
                    return $"{horas}h {minutos}min";
                if (horas > 0)
                    return $"{horas}h";
                return $"{minutos}min";
            }
        }

        /// <summary>
        /// Precio por hora de la instalacion formateado con 2 decimales y simbolo EUR.
        /// </summary>
        public string PrecioHoraTexto
        {
            // Formateamos el precio con dos decimales.
            get { return $"{PrecioHora:F2} EUR"; }
        }
    }
}
