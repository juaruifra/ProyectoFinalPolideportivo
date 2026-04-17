using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de operaciones
    /// Incluye validaciones y operaciones complejas
    /// </summary>
    public class OperacionesController
    {
        // API para comunicarse con los repositorios
        private OperacionesAPI _api;
        private InmueblesAPI _inmueblesApi;
        private OfertasAPI _ofertasApi;

        /// <summary>
        /// Constructor que inicializa las APIs
        /// </summary>
        public OperacionesController()
        {
            _api = new OperacionesAPI();
            _inmueblesApi = new InmueblesAPI();
            _ofertasApi = new OfertasAPI();
        }

        /// <summary>
        /// Obtener todas las operaciones ordenadas por fecha
        /// </summary>
        /// <returns>Lista de todas las operaciones</returns>
        public List<Operaciones> ObtenerTodas()
        {
            try
            {
                return _api.ObtenerTodas();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las operaciones", ex);
            }
        }

        /// <summary>
        /// Obtener una operacion por su identificador
        /// </summary>
        /// <param name="idOperacion">ID de la operacion</param>
        /// <returns>Operacion encontrada o null</returns>
        public Operaciones ObtenerPorId(int idOperacion)
        {
            try
            {
                return _api.ObtenerPorId(idOperacion);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la operacion con ID {idOperacion}", ex);
            }
        }

        /// <summary>
        /// Obtener el total de operaciones registradas
        /// </summary>
        /// <returns>Numero total de operaciones</returns>
        public int ObtenerTotal()
        {
            try
            {
                return _api.ObtenerTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de operaciones", ex);
            }
        }

        /// <summary>
        /// Guardar una operacion con validaciones completas
        /// </summary>
        /// <param name="operacion">Objeto operacion a guardar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se guardo correctamente, False si hubo error</returns>
        public bool Guardar(Operaciones operacion, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar datos de la operacion
                if (!ValidarOperacion(operacion, ref tituloError, ref mensajeError))
                {
                    return false;
                }

                bool esNueva = operacion.IdOperacion == 0;

                // Si es nueva, validar que el inmueble no tenga ya una operacion
                if (esNueva)
                {
                    if (_api.InmuebleYaTieneOperacion(operacion.IdInmueble))
                    {
                        tituloError = "Inmueble no disponible";
                        mensajeError = "El inmueble ya tiene una operacion asociada.";
                        return false;
                    }
                }
                else
                {
                    // Si es edición, verificar que no haya otra operación del mismo inmueble
                    if (_api.InmuebleYaTieneOperacion(operacion.IdInmueble, operacion.IdOperacion))
                    {
                        tituloError = "Inmueble no disponible";
                        mensajeError = "El inmueble ya tiene otra operacion asociada.";
                        return false;
                    }
                }

                // Guardar en la base de datos
                _api.Guardar(operacion);

                // Si es nueva operacion, actualizar estado del inmueble
                if (esNueva)
                {
                    ActualizarEstadoInmueble(operacion.IdInmueble, operacion.TipoOperacion);
                }

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar la operacion: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Eliminar una operacion con validaciones
        /// Al eliminar, el inmueble vuelve a estado DISPONIBLE
        /// </summary>
        /// <param name="operacion">Operacion a eliminar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se elimino correctamente, False si hubo error</returns>
        public bool Eliminar(Operaciones operacion, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar que la operacion exista
                if (operacion == null || operacion.IdOperacion < 1)
                {
                    tituloError = "Operacion no valida";
                    mensajeError = "No se puede eliminar una operacion que no existe.";
                    return false;
                }

                // Guardar el ID del inmueble antes de eliminar
                int idInmueble = operacion.IdInmueble;

                // Eliminar de la base de datos
                _api.Eliminar(operacion);

                // Revertir el estado del inmueble a DISPONIBLE
                RevertirEstadoInmueble(idInmueble);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al eliminar";
                mensajeError = $"No se pudo eliminar la operacion: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Verificar si una operacion puede ser editada
        /// Si viene de una oferta, solo se puede editar fecha y precio
        /// Si no viene de oferta, se puede editar todo
        /// </summary>
        /// <param name="operacion">Operacion a verificar</param>
        /// <returns>True si se puede editar</returns>
        public bool PuedeEditarse(Operaciones operacion)
        {
            if (operacion == null) return false;
            return true; // Todas las operaciones pueden editarse
        }

        /// <summary>
        /// Verificar si la operacion viene de una oferta
        /// </summary>
        /// <param name="operacion">Operacion a verificar</param>
        /// <returns>True si tiene oferta asociada</returns>
        public bool VieneDeOferta(Operaciones operacion)
        {
            if (operacion == null) return false;
            return operacion.IdOferta.HasValue && operacion.IdOferta.Value > 0;
        }

        /// <summary>
        /// Actualizar el estado del inmueble según el tipo de operacion
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <param name="tipoOperacion">Tipo de operacion (Venta/Alquiler)</param>
        private void ActualizarEstadoInmueble(int idInmueble, string tipoOperacion)
        {
            try
            {
                Inmuebles inmueble = _inmueblesApi.ObtenerPorId(idInmueble);
                if (inmueble != null)
                {
                    // Si es venta, marcar como vendido. Si es alquiler, marcar como alquilado
                    if (tipoOperacion == InmueblesConstantes.TIPO_VENTA)
                    {
                        inmueble.Estado = InmueblesConstantes.ESTADO_VENDIDO;
                    }
                    else if (tipoOperacion == InmueblesConstantes.TIPO_ALQUILER)
                    {
                        inmueble.Estado = InmueblesConstantes.ESTADO_ALQUILADO;
                    }

                    _inmueblesApi.Guardar(inmueble);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar el estado del inmueble", ex);
            }
        }

        /// <summary>
        /// Revertir el estado del inmueble a DISPONIBLE
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        private void RevertirEstadoInmueble(int idInmueble)
        {
            try
            {
                Inmuebles inmueble = _inmueblesApi.ObtenerPorId(idInmueble);
                if (inmueble != null)
                {
                    inmueble.Estado = InmueblesConstantes.ESTADO_DISPONIBLE;
                    _inmueblesApi.Guardar(inmueble);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al revertir el estado del inmueble", ex);
            }
        }

        /// <summary>
        /// Validar los datos de una operacion
        /// </summary>
        /// <param name="operacion">Operacion a validar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si es valida, False si hay errores</returns>
        private bool ValidarOperacion(Operaciones operacion, ref string tituloError, ref string mensajeError)
        {
            // Validar que el objeto no sea nulo
            if (operacion == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "La operacion no puede ser nula.";
                return false;
            }

            // Validar cliente
            if (operacion.IdCliente <= 0)
            {
                tituloError = "Cliente requerido";
                mensajeError = "Debe seleccionar un cliente para la operacion.";
                return false;
            }

            // Validar agente
            if (operacion.IdAgente <= 0)
            {
                tituloError = "Agente requerido";
                mensajeError = "Debe seleccionar un agente para la operacion.";
                return false;
            }

            // Validar inmueble
            if (operacion.IdInmueble <= 0)
            {
                tituloError = "Inmueble requerido";
                mensajeError = "Debe seleccionar un inmueble para la operacion.";
                return false;
            }

            // Validar tipo de operacion
            if (string.IsNullOrWhiteSpace(operacion.TipoOperacion))
            {
                tituloError = "Tipo de operacion requerido";
                mensajeError = "Debe seleccionar el tipo de operacion (Venta/Alquiler).";
                return false;
            }

            if (operacion.TipoOperacion != InmueblesConstantes.TIPO_VENTA &&
                operacion.TipoOperacion != InmueblesConstantes.TIPO_ALQUILER)
            {
                tituloError = "Tipo de operacion no valido";
                mensajeError = "El tipo de operacion debe ser Venta o Alquiler.";
                return false;
            }

            // Validar precio final
            if (operacion.PrecioFinal <= 0)
            {
                tituloError = "Precio no valido";
                mensajeError = "El precio final debe ser mayor que cero.";
                return false;
            }

            if (operacion.PrecioFinal > 999999999)
            {
                tituloError = "Precio muy alto";
                mensajeError = "El precio no puede superar los 999,999,999.";
                return false;
            }

            // Validar fecha de operacion
            if (operacion.FechaOperacion == default(DateTime))
            {
                tituloError = "Fecha requerida";
                mensajeError = "Debe ingresar la fecha de la operacion.";
                return false;
            }

            return true;
        }
    }
}
