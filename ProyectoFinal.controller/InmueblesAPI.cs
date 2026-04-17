using ProyectoFinal.model;
using ProyectoFinal.model.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.controller
{
    /// <summary>
    /// API para gestionar las operaciones sobre inmuebles
    /// Intermediario entre los repositorios y los controladores
    /// Prepara los datos para que la vista los consuma facilmente
    /// </summary>
    public class InmueblesAPI
    {
        // Repositorio de inmuebles para acceder a la BBDD
        private InmueblesRepository _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio
        /// </summary>
        public InmueblesAPI()
        {
            _repo = new InmueblesRepository();
        }

        /// <summary>
        /// Obtener todos los inmuebles de la base de datos
        /// </summary>
        /// <returns>Lista de inmuebles ordenada por fecha de alta descendente</returns>
        public List<Inmuebles> ObtenerTodos()
        {
            try
            {
                return _repo.GetAll().OrderByDescending(i => i.FechaAlta).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los inmuebles", ex);
            }
        }

        /// <summary>
        /// Obtener un inmueble por su ID
        /// </summary>
        /// <param name="idInmueble">Identificador del inmueble</param>
        /// <returns>Inmueble encontrado o null</returns>
        public Inmuebles ObtenerPorId(int idInmueble)
        {
            try
            {
                return _repo.GetById(idInmueble);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener el inmueble con ID {idInmueble}", ex);
            }
        }

        /// <summary>
        /// Buscar inmuebles por ciudad
        /// </summary>
        /// <param name="ciudad">Nombre de la ciudad</param>
        /// <returns>Lista de inmuebles que coinciden</returns>
        public List<Inmuebles> BuscarPorCiudad(string ciudad)
        {
            try
            {
                // Si la ciudad esta vacia, devolvemos la lista vacia
                if (string.IsNullOrWhiteSpace(ciudad)) return new List<Inmuebles>();

                return _repo.BuscarPorCiudad(ciudad);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar inmuebles por ciudad", ex);
            }
        }

        /// <summary>
        /// Guardar un inmueble (insertar o actualizar)
        /// </summary>
        /// <param name="inmueble">Objeto inmueble</param>
        public void Guardar(Inmuebles inmueble)
        {
            try
            {
                _repo.Save(inmueble);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el inmueble", ex);
            }
        }

        /// <summary>
        /// Eliminar un inmueble
        /// </summary>
        /// <param name="inmueble">Objeto inmueble a eliminar</param>
        public void Eliminar(Inmuebles inmueble)
        {
            try
            {
                _repo.Delete(inmueble);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el inmueble", ex);
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
                return _repo.GetTotal();
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
                return _repo.GetTotalDisponibles();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener inmuebles disponibles", ex);
            }
        }
    }
}
