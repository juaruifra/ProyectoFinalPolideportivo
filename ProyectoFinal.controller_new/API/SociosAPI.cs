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
        private SociosRepository _repo;
        private readonly ReservasRepository _reservasRepo;

        public SociosAPI()
        {
            _repo = new SociosRepository();
            _reservasRepo = new ReservasRepository();
        }

        public List<Socios> ObtenerTodos(bool soloActivos = false)
        {
            try
            {
                return _repo.GetAll(soloActivos)
                            .OrderBy(s => s.Apellidos)
                            .ThenBy(s => s.Nombre)
                            .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los socios", ex);
            }
        }

        public Socios ObtenerPorId(int idSocio)
        {
            try
            {
                return _repo.GetById(idSocio);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el socio con ID {idSocio}", ex);
            }
        }

        public List<Socios> BuscarPorNombreODni(string texto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(texto)) return new List<Socios>();
                return _repo.BuscarPorNombreODni(texto);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar socios", ex);
            }
        }

        public bool DniYaExiste(string dni, int? idSocioExcluir = null)
        {
            try
            {
                return _repo.DniYaExiste(dni, idSocioExcluir);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar DNI", ex);
            }
        }

        public void Guardar(Socios socio)
        {
            try
            {
                _repo.Save(socio);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el socio", ex);
            }
        }

        public void Desactivar(Socios socio)
        {
            try
            {
                _repo.Desactivar(socio);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al desactivar el socio", ex);
            }
        }

        public int ObtenerTotal(bool soloActivos = false)
        {
            try
            {
                return _repo.GetTotal(soloActivos);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el total de socios", ex);
            }
        }

        /// <summary>
        /// Indica si el socio está en uso (tiene reservas asociadas).
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si está en uso.</returns>
        public bool EstaEnUso(int socioId)
        {
            try
            {
                // Delegamos en reservas.
                return _reservasRepo.ExisteReservaParaSocio(socioId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar uso del socio", ex);
            }
        }

        /// <summary>
        /// Borra físicamente un socio.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        public void Borrar(int socioId)
        {
            try
            {
                // Delegamos.
                _repo.Delete(socioId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al borrar el socio", ex);
            }
        }
    }
}
