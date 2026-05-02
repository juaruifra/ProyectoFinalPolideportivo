using ProyectoFinal.model_new;
using ProyectoFinal.model_new.Repositories;
using System;
using System.Collections.Generic;

namespace ProyectoFinal.controller_new.api
{
    /// <summary>
    /// API para gestionar las operaciones sobre cuotas.
    /// Intermediario entre los repositorios y los controladores.
    /// </summary>
    public class CuotasAPI
    {
        // Repositorio de cuotas.
        private readonly CuotasRepository _repo;

        /// <summary>
        /// Constructor: inicializa el repositorio.
        /// </summary>
        public CuotasAPI()
        {
            // Instanciamos el repositorio.
            _repo = new CuotasRepository();
        }

        /// <summary>
        /// Obtiene todas las cuotas con filtros opcionales.
        /// </summary>
        /// <param name="socioId">Filtro por socio. Null devuelve todas.</param>
        /// <param name="soloPendientes">Si true, solo devuelve las no pagadas.</param>
        /// <returns>Lista de cuotas.</returns>
        public List<Cuotas> ObtenerTodos(int? socioId = null, bool soloPendientes = false)
        {
            try
            {
                // Delegamos en el repositorio.
                return _repo.GetAll(socioId, soloPendientes);
            }
            catch (Exception ex)
            {
                // Envolvemos el error.
                throw new Exception("Error al obtener las cuotas.", ex);
            }
        }

        /// <summary>
        /// Obtiene una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        /// <returns>Cuota encontrada o null.</returns>
        public Cuotas ObtenerPorId(int cuotaId)
        {
            try
            {
                // Buscamos por id.
                return _repo.GetById(cuotaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception($"Error al obtener la cuota con ID {cuotaId}.", ex);
            }
        }

        /// <summary>
        /// Indica si un socio tiene cuotas pendientes vencidas.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si tiene deuda vencida.</returns>
        public bool TieneCuotasPendientesVencidas(int socioId)
        {
            try
            {
                // Delegamos en el repositorio.
                return _repo.TieneCuotasPendientesVencidas(socioId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al verificar cuotas pendientes del socio.", ex);
            }
        }

        /// <summary>
        /// Indica si ya existe una cuota para el mismo socio, anio y mes.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <param name="anio">Anio de la cuota.</param>
        /// <param name="mes">Mes de la cuota.</param>
        /// <param name="excludeCuotaId">CuotaId a excluir en edicion (0 en alta).</param>
        /// <returns>True si ya existe un duplicado.</returns>
        public bool ExisteDuplicado(int socioId, int anio, int mes, int excludeCuotaId = 0)
        {
            try
            {
                // Delegamos en el repositorio.
                return _repo.ExisteDuplicado(socioId, anio, mes, excludeCuotaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al verificar duplicado de cuota.", ex);
            }
        }

        /// <summary>
        /// Guarda una cuota (alta o modificación).
        /// </summary>
        /// <param name="cuota">Entidad Cuotas.</param>
        public void Guardar(Cuotas cuota)
        {
            try
            {
                // Delegamos.
                _repo.Save(cuota);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al guardar la cuota.", ex);
            }
        }

        /// <summary>
        /// Marca una cuota como pagada.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        /// <param name="fechaPago">Fecha del pago.</param>
        public void MarcarPagada(int cuotaId, DateTime fechaPago)
        {
            try
            {
                // Delegamos en el repositorio.
                _repo.MarcarPagada(cuotaId, fechaPago);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al marcar la cuota como pagada.", ex);
            }
        }

        /// <summary>
        /// Borra fisicamente una cuota.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        public void Borrar(int cuotaId)
        {
            try
            {
                // Delegamos en el repositorio.
                _repo.Delete(cuotaId);
            }
            catch (Exception ex)
            {
                // Envolvemos.
                throw new Exception("Error al borrar la cuota.", ex);
            }
        }
    }
}
