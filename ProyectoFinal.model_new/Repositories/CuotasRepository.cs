using ProyectoFinal.model_new;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoFinal.model_new.Repositories
{
    /// <summary>
    /// Repositorio para gestionar las operaciones CRUD de Cuotas en la base de datos ClubPolideportivoDB.
    /// </summary>
    public class CuotasRepository : RepositoryBase
    {
        /// <summary>
        /// Devuelve todas las cuotas, con filtro opcional por socio y/o solo pendientes.
        /// </summary>
        /// <param name="socioId">Si se indica, filtra por ese socio. Null devuelve todas.</param>
        /// <param name="soloPendientes">Si true, devuelve solo las no pagadas.</param>
        /// <returns>Lista de cuotas.</returns>
        public List<Cuotas> GetAll(int? socioId = null, bool soloPendientes = false)
        {
            // Comenzamos con todos los registros, incluyendo el socio relacionado.
            var q = Context.Cuotas.Include("Socios").AsQueryable();

            // Filtramos por socio si se indica.
            if (socioId.HasValue)
                q = q.Where(c => c.SocioId == socioId.Value);

            // Filtramos solo pendientes si se pide.
            if (soloPendientes)
                q = q.Where(c => !c.Pagada);

            // Ordenamos por año desc y mes desc para mostrar las más recientes primero.
            return q.OrderByDescending(c => c.Anio)
                    .ThenByDescending(c => c.Mes)
                    .ToList();
        }

        /// <summary>
        /// Indica si ya existe una cuota para el mismo socio, anio y mes.
        /// Excluye el propio registro al editar.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <param name="anio">Anio de la cuota.</param>
        /// <param name="mes">Mes de la cuota.</param>
        /// <param name="excludeCuotaId">CuotaId a excluir (0 en alta).</param>
        /// <returns>True si ya existe un duplicado.</returns>
        public bool ExisteDuplicado(int socioId, int anio, int mes, int excludeCuotaId = 0)
        {
            // Buscamos cuota con mismo socio, anio y mes excluyendo el registro actual.
            return Context.Cuotas.Any(c =>
                c.SocioId == socioId &&
                c.Anio == anio &&
                c.Mes == mes &&
                c.CuotaId != excludeCuotaId);
        }

        /// <summary>
        /// Devuelve una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        /// <returns>Cuota encontrada o null.</returns>
        public Cuotas GetById(int cuotaId)
        {
            // Buscamos por clave primaria.
            return Context.Cuotas.Include("Socios").FirstOrDefault(c => c.CuotaId == cuotaId);
        }

        /// <summary>
        /// Indica si un socio tiene cuotas pendientes con fecha de vencimiento pasada.
        /// Se usa para validar si puede hacer reservas.
        /// </summary>
        /// <param name="socioId">Id del socio.</param>
        /// <returns>True si tiene cuotas vencidas sin pagar.</returns>
        public bool TieneCuotasPendientesVencidas(int socioId)
        {
            // Fecha actual.
            var hoy = DateTime.Today;

            // Buscamos cuotas no pagadas con vencimiento anterior a hoy.
            return Context.Cuotas.Any(c =>
                c.SocioId == socioId &&
                !c.Pagada &&
                c.FechaVencimiento < hoy);
        }

        /// <summary>
        /// Guarda una cuota (alta si CuotaId menor que 1, modificación si existe).
        /// </summary>
        /// <param name="cuota">Entidad Cuotas.</param>
        public void Save(Cuotas cuota)
        {
            try
            {
                if (cuota.CuotaId < 1)
                {
                    // Alta: añadimos nueva cuota.
                    Context.Cuotas.Add(new Cuotas
                    {
                        SocioId = cuota.SocioId,
                        Anio = cuota.Anio,
                        Mes = cuota.Mes,
                        Importe = cuota.Importe,
                        FechaVencimiento = cuota.FechaVencimiento,
                        FechaPago = cuota.FechaPago,
                        Pagada = cuota.Pagada
                    });
                }
                else
                {
                    // Modificacion: buscamos y actualizamos.
                    var c = Context.Cuotas.FirstOrDefault(x => x.CuotaId == cuota.CuotaId);
                    if (c != null)
                    {
                        c.SocioId = cuota.SocioId; // Socio.
                        c.Anio = cuota.Anio; // Anio.
                        c.Mes = cuota.Mes; // Mes.
                        c.Importe = cuota.Importe; // Importe.
                        c.FechaVencimiento = cuota.FechaVencimiento; // Vencimiento.
                        c.FechaPago = cuota.FechaPago; // Fecha pago.
                        c.Pagada = cuota.Pagada; // Estado.
                    }
                }

                // Persistimos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos el error para que la capa superior lo gestione.
                throw new Exception("Error al guardar la cuota en la base de datos.", ex);
            }
        }

        /// <summary>
        /// Marca una cuota como pagada con la fecha indicada.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        /// <param name="fechaPago">Fecha en que se realiza el pago.</param>
        public void MarcarPagada(int cuotaId, DateTime fechaPago)
        {
            try
            {
                // Buscamos la cuota.
                var c = Context.Cuotas.FirstOrDefault(x => x.CuotaId == cuotaId);

                // Si no existe, no hacemos nada.
                if (c == null) return;

                // Actualizamos estado de pago.
                c.Pagada = true; // Marcamos pagada.
                c.FechaPago = fechaPago; // Asignamos fecha.

                // Guardamos.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos error.
                throw new Exception("Error al marcar la cuota como pagada.", ex);
            }
        }

        /// <summary>
        /// Elimina fisicamente una cuota por su ID.
        /// </summary>
        /// <param name="cuotaId">Id de la cuota.</param>
        public void Delete(int cuotaId)
        {
            try
            {
                // Buscamos la cuota.
                var c = Context.Cuotas.FirstOrDefault(x => x.CuotaId == cuotaId);

                // Si no existe, no hacemos nada.
                if (c == null) return;

                // Eliminamos.
                Context.Cuotas.Remove(c);

                // Guardamos cambios.
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Envolvemos error.
                throw new Exception("Error al borrar la cuota en la base de datos.", ex);
            }
        }
    }
}
