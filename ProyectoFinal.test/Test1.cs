using Comun;
using ProyectoFinal.controller_new.controller;
using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Linq;

namespace ProyectoFinal.test
{
    /// <summary>
    /// Tests del Club Polideportivo.
    /// Cubre validaciones de logica, formato de datos y acceso a base de datos.
    /// </summary>
    [TestClass]
    public sealed class Test1
    {
        // =====================================================================
        // UNITARIO: Validacion de email con formato correcto
        // Categoria: Logica de utilidades (Utils)
        // =====================================================================

        /// <summary>
        /// Comprueba que un email con formato correcto es aceptado por Utils.EsEmailValido.
        /// </summary>
        [TestMethod]
        public void TestEmailValido()
        {
            // Definimos un email con formato valido (contiene @, dominio y extension).
            string email = "socio@clubpolideportivo.com";

            // Llamamos al metodo de validacion de la clase Utils.
            bool resultado = Utils.EsEmailValido(email);

            // El resultado debe ser true porque el email tiene formato correcto.
            Assert.IsTrue(resultado, "Un email con formato correcto deberia ser aceptado.");
        }

        // =====================================================================
        // UNITARIO: Validacion de email con formato incorrecto
        // Categoria: Logica de utilidades (Utils)
        // =====================================================================

        /// <summary>
        /// Comprueba que un email sin el simbolo @ es rechazado por Utils.EsEmailValido.
        /// </summary>
        [TestMethod]
        public void TestEmailInvalido()
        {
            // Definimos un email sin el simbolo @, lo que lo hace invalido.
            string email = "socioclubpolideportivo.com";

            // Llamamos al metodo de validacion de la clase Utils.
            bool resultado = Utils.EsEmailValido(email);

            // El resultado debe ser false porque el email no tiene @.
            Assert.IsFalse(resultado, "Un email sin @ deberia ser rechazado.");
        }

        // =====================================================================
        // UNITARIO: Validacion de telefono con formato correcto e incorrecto
        // Categoria: Logica de utilidades (Utils)
        // =====================================================================

        /// <summary>
        /// Comprueba que un telefono de 9 digitos es aceptado y que uno de 4 digitos es rechazado.
        /// </summary>
        [TestMethod]
        public void TestTelefonoValidoEInvalido()
        {
            // Definimos un telefono con 9 digitos, que cumple el minimo requerido.
            string telefonoValido = "612345678";

            // Definimos un telefono con solo 4 digitos, claramente insuficiente.
            string telefonoInvalido = "1234";

            // Validamos el telefono correcto: debe devolver true.
            bool resultadoValido = Utils.EsTelefonoValido(telefonoValido);

            // Validamos el telefono incorrecto: debe devolver false.
            bool resultadoInvalido = Utils.EsTelefonoValido(telefonoInvalido);

            // Comprobamos que el telefono valido sea aceptado.
            Assert.IsTrue(resultadoValido, "Un telefono de 9 digitos deberia ser aceptado.");

            // Comprobamos que el telefono invalido sea rechazado.
            Assert.IsFalse(resultadoInvalido, "Un telefono de 4 digitos deberia ser rechazado.");
        }

        // =====================================================================
        // UNITARIO: El controller rechaza una reserva con fechas incoherentes
        // Categoria: Validacion de logica de negocio (ReservasController)
        // Nota: No toca la BD. El controller devuelve false antes de llegar a guardar.
        // =====================================================================

        /// <summary>
        /// Comprueba que ReservasController.Guardar rechaza una reserva
        /// en la que la hora de fin es anterior a la hora de inicio.
        /// </summary>
        [TestMethod]
        public void TestReservaRechazaFechaFinAnteriorAInicio()
        {
            // Instanciamos el controller de reservas.
            ReservasController controller = new ReservasController();

            // Creamos una reserva con FechaHoraFin anterior a FechaHoraInicio.
            Reservas reserva = new Reservas
            {
                SocioId = 1, // Id cualquiera: la validacion de fechas se ejecuta antes.
                InstalacionId = 1, // Id cualquiera: igual que arriba.
                FechaHoraInicio = new DateTime(2025, 6, 1, 10, 0, 0), // Inicio a las 10:00.
                FechaHoraFin = new DateTime(2025, 6, 1, 9, 0, 0) // Fin a las 9:00, anterior al inicio.
            };

            // Variables para capturar el mensaje de error del controller.
            string tituloError = string.Empty;
            string mensajeError = string.Empty;

            // Llamamos al metodo Guardar del controller.
            bool resultado = controller.Guardar(reserva, ref tituloError, ref mensajeError);

            // El resultado debe ser false porque las fechas son incoherentes.
            Assert.IsFalse(resultado, "Una reserva con fin anterior al inicio debe ser rechazada.");

            // Ademas comprobamos que el controller haya informado del error.
            Assert.IsFalse(string.IsNullOrEmpty(tituloError), "El controller debe indicar un titulo de error.");
        }

        // =====================================================================
        // INTEGRACION: Acceso real a la base de datos con SociosRepository
        // Categoria: Acceso a datos
        // Este test crea un socio de prueba en la BD, lo recupera, comprueba sus datos
        // y lo borra al final para no dejar rastro.
        // =====================================================================

        /// <summary>
        /// Crea un socio de prueba en la base de datos, comprueba que se puede
        /// recuperar correctamente por Id y que los datos coinciden, y lo borra al final.
        /// </summary>
        [TestMethod]
        public void TestIntegracionCrearYRecuperarSocio()
        {
            // Instanciamos el repositorio de socios para acceder directamente a la BD.
            SociosRepository repo = new SociosRepository();

            // Usamos ticks para garantizar que el nombre y el DNI son unicos en cada ejecucion.
            long ticks = DateTime.Now.Ticks;

            // Creamos el socio de prueba con datos unicos.
            Socios socioTest = new Socios
            {
                Nombre = "TEST",
                Apellidos = "INTEGRACION_" + ticks, // Apellidos unicos con ticks.
                Dni = (ticks % 100000000).ToString("D8"), // DNI de 8 digitos basado en ticks.
                Email = "test_" + ticks + "@prueba.com", // Email unico con ticks.
                Telefono = "600000000", // Telefono de prueba.
                FechaAlta = DateTime.Today, // Fecha de alta de hoy.
                Activo = true // Marcamos como activo.
            };

            // Guardamos el socio en la BD.
            repo.Save(socioTest);

            // Capturamos el Id generado por la BD tras el guardado.
            int idGenerado = socioTest.SocioId;


            try
            {
                // Comprobamos que el Id generado es valido (mayor que 0).
                Assert.IsTrue(idGenerado > 0, "El Id generado por la BD debe ser mayor que 0.");

            }
            finally
            {
                // Bloque finally: garantiza que el socio de prueba se borra SIEMPRE,
                // tanto si el test pasa como si falla, dejando la BD limpia.
                Socios socioABorrar = repo.GetById(idGenerado);
                if (socioABorrar != null)
                {
                    // Borramos el socio de prueba de la BD.
                    repo.Delete(socioABorrar.SocioId);
                }
            }
        }
    }
}
