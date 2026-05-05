using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API para gestionar las operaciones sobre socios.
    /// Intermediario entre los repositorios y los controladores.
    /// </summary>
    public class SociosAPI
    {
        private SociosRepository _repo; // Repositorio de socios.
        private readonly ReservasRepository _reservasRepo; // Repositorio de reservas para comprobar uso.

        /// <summary>
        /// Constructor.
        /// </summary>
        public SociosAPI()
        {
            // Inicializamos los repositorios.
            _repo = new SociosRepository();
            _reservasRepo = new ReservasRepository();
        }

        /// <summary>
        /// Obtiene todos los socios, opcionalmente solo los activos.
        /// </summary>
        /// <param name="soloActivos">Si true, devuelve solo socios activos.</param>
        /// <returns>Lista de socios ordenada por apellidos y nombre.</returns>
        public List<Socios> ObtenerTodos(bool soloActivos = false)
        {
            try
            {
                // Obtenemos del repositorio y ordenamos por apellidos y luego por nombre.
                return _repo.GetAll(soloActivos)
                            .OrderBy(s => s.Apellidos)
                            .ThenBy(s => s.Nombre)
                            .ToList();
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion con mensaje descriptivo.
                throw new Exception("Error al obtener los socios", ex);
            }
        }

        /// <summary>
        /// Obtiene un socio por su identificador.
        /// </summary>
        /// <param name="idSocio">Id del socio.</param>
        /// <returns>Socio encontrado o null.</returns>
        public Socios ObtenerPorId(int idSocio)
        {
            try
            {
                // Delegamos la busqueda al repositorio.
                return _repo.GetById(idSocio);
            }
            catch (Exception ex)
            {
                // Envolvemos con el id para facilitar el diagnostico.
                throw new Exception($"Error al obtener el socio con ID {idSocio}", ex);
            }
        }

        /// <summary>
        /// Busca socios por nombre o DNI.
        /// </summary>
        /// <param name="texto">Texto a buscar.</param>
        /// <returns>Lista de socios que coinciden.</returns>
        public List<Socios> BuscarPorNombreODni(string texto)
        {
            try
            {
                // Si el texto esta vacio devolvemos lista vacia sin consultar.
                if (string.IsNullOrWhiteSpace(texto)) return new List<Socios>();

                // Delegamos la busqueda al repositorio.
                return _repo.BuscarPorNombreODni(texto);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al buscar socios", ex);
            }
        }

        /// <summary>
        /// Comprueba si un DNI ya esta registrado en otro socio.
        /// </summary>
        /// <param name="dni">DNI a comprobar.</param>
        /// <param name="idSocioExcluir">Id del socio a excluir en la comprobacion (para edicion).</param>
        /// <returns>True si el DNI ya existe.</returns>
        public bool DniYaExiste(string dni, int? idSocioExcluir = null)
        {
            try
            {
                // Delegamos la comprobacion al repositorio.
                return _repo.DniYaExiste(dni, idSocioExcluir);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al verificar DNI", ex);
            }
        }

        /// <summary>
        /// Guarda o actualiza un socio.
        /// </summary>
        /// <param name="socio">Socio a guardar.</param>
        public void Guardar(Socios socio)
        {
            try
            {
                // Delegamos el guardado al repositorio.
                _repo.Save(socio);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al guardar el socio", ex);
            }
        }

        /// <summary>
        /// Desactiva un socio sin borrarlo fisicamente.
        /// </summary>
        /// <param name="socio">Socio a desactivar.</param>
        public void Desactivar(Socios socio)
        {
            try
            {
                // Delegamos la desactivacion al repositorio.
                _repo.Desactivar(socio);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al desactivar el socio", ex);
            }
        }

        /// <summary>
        /// Devuelve el numero total de socios.
        /// </summary>
        /// <param name="soloActivos">Si true, cuenta solo los activos.</param>
        /// <returns>Total de socios.</returns>
        public int ObtenerTotal(bool soloActivos = false)
        {
            try
            {
                // Delegamos el conteo al repositorio.
                return _repo.GetTotal(soloActivos);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al obtener el total de socios", ex);
            }
        }

        /// <summary>
        /// Indica si el socio tiene reservas asociadas y por tanto no puede borrarse.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si esta en uso.</returns>
        public bool EstaEnUso(int socioId)
        {
            try
            {
                // Comprobamos si existe alguna reserva para este socio.
                return _reservasRepo.ExisteReservaParaSocio(socioId);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al verificar uso del socio", ex);
            }
        }

        /// <summary>
        /// Borra fisicamente un socio de la base de datos.
        /// </summary>
        /// <param name="socioId">Id del socio a borrar.</param>
        public void Borrar(int socioId)
        {
            try
            {
                // Delegamos el borrado al repositorio.
                _repo.Delete(socioId);
            }
            catch (Exception ex)
            {
                // Envolvemos la excepcion.
                throw new Exception("Error al borrar el socio", ex);
            }
        }
    }
}
