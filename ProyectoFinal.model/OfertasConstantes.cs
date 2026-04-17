using System.Collections.Generic;

namespace ProyectoFinal.model
{
    public static class OfertasConstantes
    {
        public const string ESTADO_PENDIENTE = "PENDIENTE";
        public const string ESTADO_ACEPTADA = "ACEPTADA";
        public const string ESTADO_RECHAZADA = "RECHAZADA";

        public static readonly List<string> Estados = new List<string>
        {
            ESTADO_PENDIENTE,
            ESTADO_ACEPTADA,
            ESTADO_RECHAZADA
        };
    }
}