using ProyectoFinal.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Clase base para los repositorios. Todos dependeran de estas
/// Donde se define el contexto que da acceso a la BBDD
/// </summary>
public abstract class RepositoryBase
{
    protected InmobiliariaEntities Context;

    /// <summary>
    /// Repositorio base para que todos los repositorios compartan el mismo contexto
    /// y no haya que definirlo en cada uno de los repositiorios
    /// </summary>
    protected RepositoryBase()
    {
        Context = new InmobiliariaEntities();
    }
}
