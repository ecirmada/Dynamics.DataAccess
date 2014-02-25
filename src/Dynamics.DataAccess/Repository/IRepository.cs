using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Dynamics.DataAccess.Repository
{
    public interface IRepository<T> : IReadonlyRepository<T>, IWriteonlyRepository<T> 
        where T : Entity
    {
    }

    public interface IReadonlyRepository<T> 
        where T : Entity
    {
        /// <summary>
        /// Get an entity by id. Eager load the extended properties specified in the include.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        T Get(Guid id, params Expression<Func<T, object>>[] include);

        /// <summary>
        /// Get an object by id limited to the columnset passed in. Eager load the extended properties specified in the include.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="columnSet"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        T Get(Guid id, ColumnSet columnSet, params Expression<Func<T, object>>[] include);

        /// <summary>
        /// Get an entity by entity reference. Eager load the extended properties specified in the include 
        /// </summary>
        /// <param name="er"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        T Get(EntityReference er, params Expression<Func<T, object>>[] include);

        /// <summary>
        /// Get an entity by entity reference limited to the columnset passed in. Eager load the extended properties specified in the include 
        /// </summary>
        /// <param name="er"></param>
        /// <param name="columnSet"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        T Get(EntityReference er, ColumnSet columnSet, Expression<Func<T, object>>[] include);

        /// <summary>
        /// Find all entities that match all the given fields in the passed in partially filled entity.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="include"></param>
        /// <returns></returns>
        IEnumerable<T> Find(T e, Expression<Func<T, object>>[] include);

        /// <summary>
        /// Return a context wrapped in a Queryable repository
        /// </summary>
        /// <returns></returns>
        IQueryableRepository AsQueryable();
    }

    public interface IWriteonlyRepository<in T>
        where T : Entity
    {
        /// <summary>
        /// Create the given entity
        /// </summary>
        /// <param name="e"></param>
        void Create(T e);

        /// <summary>
        /// Update the given entity
        /// </summary>
        /// <param name="e"></param>
        void Update(T e);

        /// <summary>
        /// Delete the given entity
        /// </summary>
        /// <param name="e"></param>
        void Delete(T e);
    }
}