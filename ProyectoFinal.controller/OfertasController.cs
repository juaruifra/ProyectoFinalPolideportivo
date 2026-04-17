using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de ofertas
    /// Incluye validaciones y operaciones complejas
    /// </summary>
    public class OfertasController
    {
        // API para comunicarse con los repositorios
        private OfertasAPI _api;
        private OperacionesAPI _operacionesApi;
        private InmueblesAPI _inmueblesApi;

        /// <summary>
        /// Constructor que inicializa las APIs
        /// </summary>
        public OfertasController()
        {
            _api = new OfertasAPI();
            _operacionesApi = new OperacionesAPI();
            _inmueblesApi = new InmueblesAPI();
        }

        /// <summary>
        /// Obtener todas las ofertas ordenadas por fecha
        /// </summary>
        /// <returns>Lista de todas las ofertas</returns>
        public List<Ofertas> ObtenerTodas()
        {
            try
            {
                // Obtener todas las ofertas a traves de la API
                return _api.ObtenerTodas();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener las ofertas", ex);
            }
        }

        /// <summary>
        /// Obtener una oferta por su identificador
        /// </summary>
        /// <param name="idOferta">ID de la oferta</param>
        /// <returns>Oferta encontrada o null</returns>
        public Ofertas ObtenerPorId(int idOferta)
        {
            try
            {
                // Obtener la oferta por ID a traves de la API
                return _api.ObtenerPorId(idOferta);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener la oferta con ID {idOferta}", ex);
            }
        }

        /// <summary>
        /// Obtener ofertas de un inmueble especifico
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>Lista de ofertas del inmueble</returns>
        public List<Ofertas> ObtenerPorInmueble(int idInmueble)
        {
            try
            {
                return _api.ObtenerPorInmueble(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener ofertas del inmueble {idInmueble}", ex);
            }
        }

        /// <summary>
        /// Obtener el total de ofertas registradas
        /// </summary>
        /// <returns>Numero total de ofertas</returns>
        public int ObtenerTotal()
        {
            try
            {
                // Obtener el total de ofertas a traves de la API
                return _api.ObtenerTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de ofertas", ex);
            }
        }

        /// <summary>
        /// Guardar una oferta con validaciones completas
        /// Si el estado es ACEPTADA, rechaza automaticamente las demas ofertas del mismo inmueble
        /// </summary>
        /// <param name="oferta">Objeto oferta a guardar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se guardo correctamente, False si hubo error</returns>
        public bool Guardar(Ofertas oferta, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar datos de la oferta
                if (!ValidarOferta(oferta, ref tituloError, ref mensajeError))
                {
                    return false;
                }

                // Validar que el inmueble no tenga ya una operacion
                if (_operacionesApi.ExistenPorInmueble(oferta.IdInmueble))
                {
                    tituloError = "Inmueble no disponible";
                    mensajeError = "El inmueble ya tiene una operacion asociada y no se pueden hacer mas ofertas.";
                    return false;
                }

                // Guardar el estado actual antes de modificar
                bool esNueva = oferta.IdOferta == 0;
                string estadoAnterior = null;

                if (!esNueva)
                {
                    var ofertaOriginal = _api.ObtenerPorId(oferta.IdOferta);
                    if (ofertaOriginal != null)
                    {
                        estadoAnterior = ofertaOriginal.Estado;
                    }
                }

                // Si se intenta aceptar una oferta, validar que no haya otra aceptada
                if (oferta.Estado == OfertasConstantes.ESTADO_ACEPTADA &&
                    estadoAnterior != OfertasConstantes.ESTADO_ACEPTADA)
                {
                    var ofertasInmueble = _api.ObtenerPorInmueble(oferta.IdInmueble);
                    var yaHayAceptada = ofertasInmueble.Any(o =>
                        o.IdOferta != oferta.IdOferta &&
                        o.Estado == OfertasConstantes.ESTADO_ACEPTADA);

                    if (yaHayAceptada)
                    {
                        tituloError = "Ya existe oferta aceptada";
                        mensajeError = "Ya hay una oferta aceptada para este inmueble. Debe rechazarla primero.";
                        return false;
                    }
                }

                // Guardar en la base de datos
                _api.Guardar(oferta);

                // Si la oferta cambio a ACEPTADA, rechazar las demas del mismo inmueble
                if (oferta.Estado == OfertasConstantes.ESTADO_ACEPTADA &&
                    estadoAnterior != OfertasConstantes.ESTADO_ACEPTADA)
                {
                    RechazarDemasOfertas(oferta.IdInmueble, oferta.IdOferta);
                }

                // Si la oferta cambio de ACEPTADA a PENDIENTE, reactivar las demas
                if (oferta.Estado == OfertasConstantes.ESTADO_PENDIENTE &&
                    estadoAnterior == OfertasConstantes.ESTADO_ACEPTADA)
                {
                    ReactivarOfertasDelInmueble(oferta.IdInmueble);
                }

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar la oferta: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Eliminar una oferta con validaciones
        /// No se puede eliminar si el inmueble ya tiene una operacion asociada
        /// </summary>
        /// <param name="oferta">Oferta a eliminar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se elimino correctamente, False si hubo error</returns>
        public bool Eliminar(Ofertas oferta, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar que la oferta exista
                if (oferta == null || oferta.IdOferta < 1)
                {
                    tituloError = "Oferta no valida";
                    mensajeError = "No se puede eliminar una oferta que no existe.";
                    return false;
                }

                // Verificar si el inmueble de la oferta ya tiene una operacion
                if (_operacionesApi.ExistenPorInmueble(oferta.IdInmueble))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El inmueble de esta oferta ya tiene una operacion asociada y no se puede eliminar.";
                    return false;
                }

                // Eliminar de la base de datos
                _api.Eliminar(oferta);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al eliminar";
                mensajeError = $"No se pudo eliminar la oferta: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Verificar si una oferta puede ser editada
        /// No se puede editar si el inmueble ya tiene una operacion asociada
        /// </summary>
        /// <param name="idOferta">ID de la oferta</param>
        /// <returns>True si se puede editar, False si esta bloqueada</returns>
        public bool PuedeEditarse(int idOferta)
        {
            try
            {
                // Obtener la oferta
                var oferta = _api.ObtenerPorId(idOferta);
                if (oferta == null) return false;

                // Verificar si el inmueble tiene operacion
                return !_operacionesApi.ExistenPorInmueble(oferta.IdInmueble);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verificar si una oferta puede ser eliminada
        /// No se puede eliminar si el inmueble ya tiene una operacion asociada
        /// </summary>
        /// <param name="idOferta">ID de la oferta</param>
        /// <returns>True si se puede eliminar, False si esta bloqueada</returns>
        public bool PuedeEliminarse(int idOferta)
        {
            try
            {
                // Obtener la oferta
                var oferta = _api.ObtenerPorId(idOferta);
                if (oferta == null) return false;

                // Verificar si el inmueble tiene operacion
                return !_operacionesApi.ExistenPorInmueble(oferta.IdInmueble);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Verificar si se puede generar una operacion a partir de una oferta
        /// Solo si la oferta esta ACEPTADA y el inmueble no tiene operacion
        /// </summary>
        /// <param name="oferta">Oferta a verificar</param>
        /// <returns>True si se puede generar operacion</returns>
        public bool PuedeGenerarOperacion(Ofertas oferta)
        {
            if (oferta == null) return false;

            // Debe estar aceptada y el inmueble no debe tener operacion
            return oferta.Estado == OfertasConstantes.ESTADO_ACEPTADA &&
                   !_operacionesApi.ExistenPorInmueble(oferta.IdInmueble);
        }

        /// <summary>
        /// Rechazar automaticamente todas las ofertas del mismo inmueble
        /// excepto la que se acaba de aceptar
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <param name="idOfertaAceptada">ID de la oferta aceptada (para excluirla)</param>
        private void RechazarDemasOfertas(int idInmueble, int idOfertaAceptada)
        {
            try
            {
                // Obtener todas las ofertas del inmueble
                var ofertas = _api.ObtenerPorInmueble(idInmueble);

                // Cambiar estado a RECHAZADA en todas menos la aceptada
                foreach (var oferta in ofertas)
                {
                    if (oferta.IdOferta != idOfertaAceptada &&
                        oferta.Estado != OfertasConstantes.ESTADO_RECHAZADA)
                    {
                        oferta.Estado = OfertasConstantes.ESTADO_RECHAZADA;
                        _api.Guardar(oferta);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al rechazar las demas ofertas", ex);
            }
        }

        /// <summary>
        /// Reactivar todas las ofertas de un inmueble a PENDIENTE
        /// cuando una oferta ACEPTADA vuelve a PENDIENTE
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        private void ReactivarOfertasDelInmueble(int idInmueble)
        {
            try
            {
                // Obtener todas las ofertas rechazadas del inmueble
                var ofertas = _api.ObtenerPorInmueble(idInmueble)
                    .Where(o => o.Estado == OfertasConstantes.ESTADO_RECHAZADA)
                    .ToList();

                // Cambiar todas a PENDIENTE
                foreach (var oferta in ofertas)
                {
                    oferta.Estado = OfertasConstantes.ESTADO_PENDIENTE;
                    _api.Guardar(oferta);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al reactivar las ofertas del inmueble", ex);
            }
        }

        /// <summary>
        /// Validar los datos de una oferta
        /// </summary>
        /// <param name="oferta">Oferta a validar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si es valida, False si hay errores</returns>
        private bool ValidarOferta(Ofertas oferta, ref string tituloError, ref string mensajeError)
        {
            // Validar que el objeto no sea nulo
            if (oferta == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "La oferta no puede ser nula.";
                return false;
            }

            // Validar cliente
            if (oferta.IdCliente <= 0)
            {
                tituloError = "Cliente requerido";
                mensajeError = "Debe seleccionar un cliente para la oferta.";
                return false;
            }

            // Validar inmueble
            if (oferta.IdInmueble <= 0)
            {
                tituloError = "Inmueble requerido";
                mensajeError = "Debe seleccionar un inmueble para la oferta.";
                return false;
            }

            // Validar monto ofertado
            if (oferta.PrecioOfertado <= 0)
            {
                tituloError = "Monto no valido";
                mensajeError = "El monto de la oferta debe ser mayor que cero.";
                return false;
            }

            if (oferta.PrecioOfertado > 999999999)
            {
                tituloError = "Monto muy alto";
                mensajeError = "El monto no puede superar los 999,999,999.";
                return false;
            }

            // Validar estado
            if (string.IsNullOrWhiteSpace(oferta.Estado))
            {
                tituloError = "Estado requerido";
                mensajeError = "Debe seleccionar el estado de la oferta.";
                return false;
            }

            if (!OfertasConstantes.Estados.Contains(oferta.Estado))
            {
                tituloError = "Estado no valido";
                mensajeError = "El estado debe ser PENDIENTE, ACEPTADA o RECHAZADA.";
                return false;
            }

            // Validar fecha de oferta
            if (oferta.FechaOferta == default(DateTime))
            {
                tituloError = "Fecha requerida";
                mensajeError = "Debe ingresar la fecha de la oferta.";
                return false;
            }

            if (oferta.FechaOferta > DateTime.Now)
            {
                tituloError = "Fecha no valida";
                mensajeError = "La fecha de la oferta no puede ser futura.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Generar una operacion a partir de una oferta aceptada
        /// Crea la operacion y la vincula con la oferta
        /// </summary>
        /// <param name="oferta">Oferta aceptada</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>La operacion creada o null si hubo error</returns>
        public Operaciones GenerarOperacion(Ofertas oferta, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar que la oferta exista
                if (oferta == null || oferta.IdOferta < 1)
                {
                    tituloError = "Oferta no valida";
                    mensajeError = "No se puede generar una operacion desde una oferta que no existe.";
                    return null;
                }

                // Validar que la oferta este aceptada
                if (oferta.Estado != OfertasConstantes.ESTADO_ACEPTADA)
                {
                    tituloError = "Estado no valido";
                    mensajeError = "Solo se pueden generar operaciones desde ofertas ACEPTADAS.";
                    return null;
                }

                // Validar que el inmueble no tenga ya una operacion
                if (_operacionesApi.ExistenPorInmueble(oferta.IdInmueble))
                {
                    tituloError = "Inmueble ya operado";
                    mensajeError = "El inmueble ya tiene una operacion asociada.";
                    return null;
                }

                // Obtener el inmueble para el tipo de operacion
                var inmueble = oferta.Inmuebles;
                if (inmueble == null)
                {
                    tituloError = "Inmueble no encontrado";
                    mensajeError = "No se pudo obtener la informacion del inmueble.";
                    return null;
                }

                // Crear la operacion
                Operaciones operacion = new Operaciones
                {
                    IdCliente = oferta.IdCliente,
                    IdAgente = oferta.IdAgente,
                    IdInmueble = oferta.IdInmueble,
                    IdOferta = oferta.IdOferta,
                    TipoOperacion = inmueble.TipoOperacion,
                    PrecioFinal = oferta.PrecioOfertado,
                    FechaOperacion = DateTime.Now,
                    Observaciones = $"Generada desde oferta #{oferta.IdOferta}"
                };

                // Guardar la operacion
                _operacionesApi.Guardar(operacion);

                Inmuebles inm = _inmueblesApi.ObtenerPorId(oferta.IdInmueble);
                if (inm != null)
                {
                    // Si es venta, marcar como vendido. Si es alquiler, marcar como alquilado
                    if (inm.TipoOperacion == InmueblesConstantes.TIPO_VENTA)
                    {
                        inm.Estado = InmueblesConstantes.ESTADO_VENDIDO;
                    }
                    else if (inm.TipoOperacion == InmueblesConstantes.TIPO_ALQUILER)
                    {
                        inm.Estado = InmueblesConstantes.ESTADO_ALQUILADO;
                    }

                    // Guardar el inmueble con el nuevo estado
                    _inmueblesApi.Guardar(inm);
                }

                return operacion;
            }
            catch (Exception ex)
            {
                tituloError = "Error al generar operacion";
                mensajeError = $"No se pudo generar la operacion: {ex.Message}";
                return null;
            }
        }



    }
}
