using System.Collections.Generic;

namespace ProyectoFinal.model
{
    /// <summary>
    /// Constantes para gestionar los valores de Inmuebles
    /// Permite reutilizar valores en validaciones y vistas
    /// </summary>
    public static class InmueblesConstantes
    {
        // ================= TIPOS DE OPERACION =================

        /// <summary>
        /// Tipo de operacion: Venta
        /// </summary>
        public const string TIPO_VENTA = "VENTA";

        /// <summary>
        /// Tipo de operacion: Alquiler
        /// </summary>
        public const string TIPO_ALQUILER = "ALQUILER";

        /// <summary>
        /// Lista de todos los tipos de operacion disponibles
        /// </summary>
        public static List<string> TiposOperacion => new List<string>
        {
            TIPO_VENTA,
            TIPO_ALQUILER
        };

        // ================= ESTADOS =================

        /// <summary>
        /// Estado: Disponible
        /// </summary>
        public const string ESTADO_DISPONIBLE = "DISPONIBLE";

        /// <summary>
        /// Estado: Vendido
        /// </summary>
        public const string ESTADO_VENDIDO = "VENDIDO";

        /// <summary>
        /// Estado: Alquilado
        /// </summary>
        public const string ESTADO_ALQUILADO = "ALQUILADO";

        /// <summary>
        /// Lista de todos los estados disponibles
        /// </summary>
        public static List<string> Estados => new List<string>
        {
            ESTADO_DISPONIBLE,
            ESTADO_VENDIDO,
            ESTADO_ALQUILADO
        };
    }
}
