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
        public List<Socios> GetAll(bool soloActivos = false)
        {
            var q = Context.Socios.AsQueryable();

            if (soloActivos)
                q = q.Where(s => s.Activo);

            return q.ToList();
        }

        public Socios GetById(int idSocio)
        {
            return Context.Socios.FirstOrDefault(s => s.SocioId == idSocio);
        }

        public List<Socios> BuscarPorNombreODni(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return new List<Socios>();

            texto = texto.Trim();

            return Context.Socios
                .Where(s => s.Nombre.Contains(texto)
                         || s.Apellidos.Contains(texto)
                         || s.Dni.Contains(texto))
                .ToList();
        }

        public bool DniYaExiste(string dni, int? idSocioExcluir = null)
        {
            if (string.IsNullOrWhiteSpace(dni)) return false;

            var q = Context.Socios.Where(s => s.Dni == dni);

            if (idSocioExcluir.HasValue)
                q = q.Where(s => s.SocioId != idSocioExcluir.Value);

            return q.Any();
        }

        public void Save(Socios socio)
        {
            try
            {
                if (socio.SocioId < 1)
                {
                    Context.Socios.Add(new Socios
                    {
                        Nombre = socio.Nombre,
                        Apellidos = socio.Apellidos,
                        Dni = socio.Dni,
                        Telefono = socio.Telefono,
                        Email = socio.Email,
                        FechaAlta = DateTime.Now,
                        Activo = socio.Activo
                    });
                }
                else
                {
                    var s = Context.Socios.FirstOrDefault(x => x.SocioId == socio.SocioId);
                    if (s != null)
                    {
                        s.Nombre = socio.Nombre;
                        s.Apellidos = socio.Apellidos;
                        s.Dni = socio.Dni;
                        s.Telefono = socio.Telefono;
                        s.Email = socio.Email;
                        s.Activo = socio.Activo;
                    }
                }

                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar socio en la base de datos.", ex);
            }
        }

        public void Desactivar(Socios socio)
        {
            try
            {
                var s = Context.Socios.FirstOrDefault(x => x.SocioId == socio.SocioId);
                if (s != null)
                {
                    s.Activo = false;
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al desactivar socio en la base de datos.", ex);
            }
        }

        public int GetTotal(bool soloActivos = false)
        {
            return soloActivos ? Context.Socios.Count(s => s.Activo) : Context.Socios.Count();
        }

        // NOTA: La validacion de "en uso" se centraliza en ReservasRepository.

        /// <summary>
        /// Elimina físicamente un socio.
        /// IMPORTANTE: solo debe llamarse si no está en uso.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        public void Delete(int socioId)
        {
            try
            {
                // Buscamos el socio.
                var s = Context.Socios.FirstOrDefault(x => x.SocioId == socioId);

                // Si no existe, no hacemos nada.
                if (s == null) return;

                // Eliminamos.
                Context.Socios.Remove(s);

                // Guardamos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos error.
                throw new Exception("Error al borrar socio en la base de datos.", ex);
            }
        }
    }
}
