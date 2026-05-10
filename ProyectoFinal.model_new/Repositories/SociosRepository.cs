using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Socios en la base de datos ClubPolideportivoDB.
    /// </summary>
    public class SociosRepository : RepositoryBase
    {
        /// <summary>
        /// Obtiene todos los socios, opcionalmente solo los activos.
        /// </summary>
        /// <param name="soloActivos">Si true, devuelve solo socios activos.</param>
        /// <returns>Lista de socios.</returns>
        public List<Socios> GetAll(bool soloActivos = false)
        {
            // Empezamos con todos los socios.
            var q = Context.Socios.AsQueryable();

            // Si se pide, filtramos solo los que estan activos.
            if (soloActivos)
                q = q.Where(s => s.Activo);

            // Devolvemos el resultado.
            return q.ToList();
        }

        /// <summary>
        /// Obtiene un socio por su identificador.
        /// </summary>
        /// <param name="idSocio">Id del socio.</param>
        /// <returns>Socio encontrado o null.</returns>
        public Socios GetById(int idSocio)
        {
            // Buscamos el primer socio que coincida con el id indicado.
            return Context.Socios.FirstOrDefault(s => s.SocioId == idSocio);
        }

        /// <summary>
        /// Busca socios cuyo nombre, apellidos o DNI contengan el texto indicado.
        /// </summary>
        /// <param name="texto">Texto a buscar.</param>
        /// <returns>Lista de socios que coinciden.</returns>
        public List<Socios> BuscarPorNombreODni(string texto)
        {
            // Si el texto esta vacio devolvemos lista vacia sin consultar.
            if (string.IsNullOrWhiteSpace(texto)) return new List<Socios>();

            // Quitamos espacios del texto de busqueda.
            texto = texto.Trim();

            // Buscamos socios que coincidan en nombre, apellidos o DNI.
            return Context.Socios
                .Where(s => s.Nombre.Contains(texto)
                         || s.Apellidos.Contains(texto)
                         || s.Dni.Contains(texto))
                .ToList();
        }

        /// <summary>
        /// Comprueba si un DNI ya esta registrado en otro socio distinto.
        /// </summary>
        /// <param name="dni">DNI a comprobar.</param>
        /// <param name="idSocioExcluir">Id del socio a excluir en la comprobacion (para edicion).</param>
        /// <returns>True si el DNI ya existe.</returns>
        public bool DniYaExiste(string dni, int? idSocioExcluir = null)
        {
            // Si el DNI esta vacio no puede existir duplicado.
            if (string.IsNullOrWhiteSpace(dni)) return false;

            // Buscamos socios con ese DNI.
            var q = Context.Socios.Where(s => s.Dni == dni);

            // Si estamos editando, excluimos al propio socio de la busqueda.
            if (idSocioExcluir.HasValue)
                q = q.Where(s => s.SocioId != idSocioExcluir.Value);

            // Devolvemos true si existe alguno.
            return q.Any();
        }

        /// <summary>
        /// Guarda un socio nuevo o actualiza uno existente.
        /// </summary>
        /// <param name="socio">Socio a guardar.</param>
        public void Save(Socios socio)
        {
            try
            {
                // Si el id es 0 o menor, es un socio nuevo que hay que insertar.
                if (socio.SocioId < 1)
                {
                    // Asignamos la fecha de alta automatica antes de insertar.
                    socio.FechaAlta = DateTime.Now;

                    // Añadimos el objeto original al contexto para que EF
                    // actualice su SocioId con el valor generado por la BD tras SaveChanges.
                    Context.Socios.Add(socio);
                }
                else
                {
                    // Buscamos el socio existente para actualizarlo.
                    var s = Context.Socios.FirstOrDefault(x => x.SocioId == socio.SocioId);

                    if (s != null)
                    {
                        s.Nombre = socio.Nombre; // Actualizamos nombre.
                        s.Apellidos = socio.Apellidos; // Actualizamos apellidos.
                        s.Dni = socio.Dni; // Actualizamos DNI.
                        s.Telefono = socio.Telefono; // Actualizamos telefono.
                        s.Email = socio.Email; // Actualizamos email.
                        s.Activo = socio.Activo; // Actualizamos estado activo.
                    }
                }

                // Persistimos los cambios en la base de datos.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos el error con un mensaje descriptivo.
                throw new Exception("Error al guardar socio en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Marca un socio como inactivo sin borrarlo fisicamente.
        /// </summary>
        /// <param name="socio">Socio a desactivar.</param>
        public void Desactivar(Socios socio)
        {
            try
            {
                // Buscamos el socio por id.
                var s = Context.Socios.FirstOrDefault(x => x.SocioId == socio.SocioId);

                if (s != null)
                {
                    s.Activo = false; // Marcamos como inactivo.
                    Context.SaveChanges(); // Guardamos el cambio.
                }
            }
            catch (Exception ex)
            {
                // Envolvemos el error con un mensaje descriptivo.
                throw new Exception("Error al desactivar socio en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Devuelve el numero total de socios, opcionalmente solo los activos.
        /// </summary>
        /// <param name="soloActivos">Si true, cuenta solo los activos.</param>
        /// <returns>Total de socios.</returns>
        public int GetTotal(bool soloActivos = false)
        {
            // Si se pide solo activos contamos con filtro, si no contamos todos.
            return soloActivos ? Context.Socios.Count(s => s.Activo) : Context.Socios.Count();
        }

        /// <summary>
        /// Elimina fisicamente un socio de la base de datos.
        /// </summary>
        /// <param name="socioId">Id del socio a borrar.</param>
        public void Delete(int socioId)
        {
            try
            {
                // Buscamos el socio por su id.
                var s = Context.Socios.FirstOrDefault(x => x.SocioId == socioId);

                // Si no existe no hacemos nada.
                if (s == null) return;

                // Marcamos el registro para eliminar.
                Context.Socios.Remove(s);

                // Persistimos el borrado en la base de datos.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos el error con un mensaje descriptivo.
                throw new Exception("Error al borrar socio en la base de datos.", ex);
            }
        }
    }
}
