using ProyectoFinal.model;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// Controlador para gestionar la logica de negocio de inmuebles
    /// Incluye validaciones y operaciones complejas
    /// </summary>
    public class InmueblesController
    {
        // API para comunicarse con los repositorios
        private InmueblesAPI _api;
        private OperacionesAPI _operacionesApi;
        private OfertasAPI _ofertasApi;

        /// <summary>
        /// Constructor que inicializa la API
        /// </summary>
        public InmueblesController()
        {
            _api = new InmueblesAPI();
            _operacionesApi = new OperacionesAPI();
            _ofertasApi = new OfertasAPI();
        }

        /// <summary>
        /// Obtener todos los inmuebles ordenados por fecha
        /// </summary>
        /// <returns>Lista de todos los inmuebles</returns>
        public List<Inmuebles> ObtenerTodos()
        {
            try
            {
                // Obtener todos los inmuebles a traves de la API
                return _api.ObtenerTodos();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los inmuebles", ex);
            }
        }

        /// <summary>
        /// Obtener un inmueble por su identificador
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>Inmueble encontrado o null</returns>
        public Inmuebles ObtenerPorId(int idInmueble)
        {
            try
            {
                // Obtener el inmueble por ID a traves de la API
                return _api.ObtenerPorId(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el inmueble con ID {idInmueble}", ex);
            }
        }

        /// <summary>
        /// Buscar inmuebles por ciudad
        /// </summary>
        /// <param name="ciudad">Ciudad a buscar</param>
        /// <returns>Lista de inmuebles que coinciden</returns>
        public List<Inmuebles> BuscarPorCiudad(string ciudad)
        {
            try
            {
                // Buscar inmuebles por ciudad a traves de la API
                return _api.BuscarPorCiudad(ciudad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar inmuebles por ciudad", ex);
            }
        }

        /// <summary>
        /// Obtener el total de inmuebles registrados
        /// </summary>
        /// <returns>Numero total de inmuebles</returns>
        public int ObtenerTotal()
        {
            try
            {
                // Obtener el total de inmuebles a traves de la API
                return _api.ObtenerTotal();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de inmuebles", ex);
            }
        }

        /// <summary>
        /// Obtener el total de inmuebles disponibles
        /// </summary>
        /// <returns>Numero de inmuebles disponibles</returns>
        public int ObtenerTotalDisponibles()
        {
            try
            {
                // Obtener el total de inmuebles disponibles a traves de la API
                return _api.ObtenerTotalDisponibles();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener inmuebles disponibles", ex);
            }
        }

        /// <summary>
        /// Guardar un inmueble con validaciones completas
        /// Si el inmueble tiene ofertas u operaciones, solo se pueden editar: precio, ciudad y direccion
        /// </summary>
        /// <param name="inmueble">Objeto inmueble a guardar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se guardo correctamente, False si hubo error</returns>
        public bool Guardar(Inmuebles inmueble, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar datos del inmueble
                if (!ValidarInmueble(inmueble, ref tituloError, ref mensajeError))
                {
                    return false;
                }

                // Si es una edicion (no es nuevo)
                if (inmueble.IdInmueble > 0)
                {
                    // Verificar si tiene ofertas u operaciones
                    bool tieneOfertas = _ofertasApi.ExistenPorInmueble(inmueble.IdInmueble);
                    bool tieneOperaciones = _operacionesApi.ExistenPorInmueble(inmueble.IdInmueble);

                    if (tieneOfertas || tieneOperaciones)
                    {
                        // Obtener el inmueble original de la base de datos
                        Inmuebles inmuebleOriginal = _api.ObtenerPorId(inmueble.IdInmueble);

                        if (inmuebleOriginal != null)
                        {
                            // Validar que solo se hayan modificado los campos permitidos: precio, ciudad y direccion
                            if (inmueble.TipoOperacion != inmuebleOriginal.TipoOperacion)
                            {
                                tituloError = "Campo no editable";
                                mensajeError = "No se puede cambiar el tipo de operacion de un inmueble con ofertas u operaciones asociadas.";
                                return false;
                            }

                            if (inmueble.Estado != inmuebleOriginal.Estado)
                            {
                                tituloError = "Campo no editable";
                                mensajeError = "No se puede cambiar el estado de un inmueble con ofertas u operaciones asociadas.";
                                return false;
                            }
                        }
                    }
                }

                // Guardar en la base de datos
                _api.Guardar(inmueble);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al guardar";
                mensajeError = $"No se pudo guardar el inmueble: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Verificar si un inmueble tiene ofertas u operaciones asociadas
        /// </summary>
        /// <param name="idInmueble">ID del inmueble</param>
        /// <returns>True si tiene ofertas u operaciones</returns>
        public bool TieneOfertasUOperaciones(int idInmueble)
        {
            try
            {
                bool tieneOfertas = _ofertasApi.ExistenPorInmueble(idInmueble);
                bool tieneOperaciones = _operacionesApi.ExistenPorInmueble(idInmueble);
                return tieneOfertas || tieneOperaciones;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Eliminar un inmueble con validaciones
        /// </summary>
        /// <param name="inmueble">Inmueble a eliminar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si se elimino correctamente, False si hubo error</returns>
        public bool Eliminar(Inmuebles inmueble, ref string tituloError, ref string mensajeError)
        {
            try
            {
                // Validar que el inmueble exista
                if (inmueble == null || inmueble.IdInmueble < 1)
                {
                    tituloError = "Inmueble no valido";
                    mensajeError = "No se puede eliminar un inmueble que no existe.";
                    return false;
                }

                // Verificar si tiene operaciones asociadas
                if (_operacionesApi.ExistenPorInmueble(inmueble.IdInmueble))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El inmueble tiene operaciones asociadas y no se puede eliminar.";
                    return false;
                }

                // Verificar si tiene ofertas asociadas
                if (_ofertasApi.ExistenPorInmueble(inmueble.IdInmueble))
                {
                    tituloError = "No se puede eliminar";
                    mensajeError = "El inmueble tiene ofertas asociadas y no se puede eliminar.";
                    return false;
                }

                // Eliminar de la base de datos
                _api.Eliminar(inmueble);

                return true;
            }
            catch (Exception ex)
            {
                tituloError = "Error al eliminar";
                mensajeError = $"No se pudo eliminar el inmueble: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Validar los datos de un inmueble
        /// </summary>
        /// <param name="inmueble">Inmueble a validar</param>
        /// <param name="tituloError">Titulo del error (por referencia)</param>
        /// <param name="mensajeError">Mensaje de error (por referencia)</param>
        /// <returns>True si es valido, False si hay errores</returns>
        private bool ValidarInmueble(Inmuebles inmueble, ref string tituloError, ref string mensajeError)
        {
            // Validar que el objeto no sea nulo
            if (inmueble == null)
            {
                tituloError = "Datos incompletos";
                mensajeError = "El inmueble no puede ser nulo.";
                return false;
            }

            // Validar direccion
            if (string.IsNullOrWhiteSpace(inmueble.Direccion))
            {
                tituloError = "Direccion requerida";
                mensajeError = "Debe ingresar la direccion del inmueble.";
                return false;
            }

            if (inmueble.Direccion.Length < 5)
            {
                tituloError = "Direccion muy corta";
                mensajeError = "La direccion debe tener al menos 5 caracteres.";
                return false;
            }

            if (inmueble.Direccion.Length > 200)
            {
                tituloError = "Direccion muy larga";
                mensajeError = "La direccion no puede superar los 200 caracteres.";
                return false;
            }

            // Validar ciudad
            if (string.IsNullOrWhiteSpace(inmueble.Ciudad))
            {
                tituloError = "Ciudad requerida";
                mensajeError = "Debe ingresar la ciudad del inmueble.";
                return false;
            }

            if (inmueble.Ciudad.Length < 3)
            {
                tituloError = "Ciudad muy corta";
                mensajeError = "La ciudad debe tener al menos 3 caracteres.";
                return false;
            }

            if (inmueble.Ciudad.Length > 100)
            {
                tituloError = "Ciudad muy larga";
                mensajeError = "La ciudad no puede superar los 100 caracteres.";
                return false;
            }

            // Validar precio
            if (inmueble.Precio <= 0)
            {
                tituloError = "Precio no valido";
                mensajeError = "El precio debe ser mayor que cero.";
                return false;
            }

            if (inmueble.Precio > 999999999)
            {
                tituloError = "Precio muy alto";
                mensajeError = "El precio no puede superar los 999,999,999.";
                return false;
            }

            // Validar tipo de operacion
            if (string.IsNullOrWhiteSpace(inmueble.TipoOperacion))
            {
                tituloError = "Tipo de operacion requerido";
                mensajeError = "Debe seleccionar el tipo de operacion (Venta o Alquiler).";
                return false;
            }

            if (!InmueblesConstantes.TiposOperacion.Contains(inmueble.TipoOperacion))
            {
                tituloError = "Tipo de operacion no valido";
                mensajeError = "El tipo de operacion debe ser VENTA o ALQUILER.";
                return false;
            }

            // Validar estado
            if (string.IsNullOrWhiteSpace(inmueble.Estado))
            {
                tituloError = "Estado requerido";
                mensajeError = "Debe seleccionar el estado del inmueble.";
                return false;
            }

            if (!InmueblesConstantes.Estados.Contains(inmueble.Estado))
            {
                tituloError = "Estado no valido";
                mensajeError = "El estado debe ser DISPONIBLE, VENDIDO o ALQUILADO.";
                return false;
            }

            return true;
        }
    }
}
