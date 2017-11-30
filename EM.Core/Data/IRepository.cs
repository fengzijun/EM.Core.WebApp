using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace EM.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        T GetById(object id);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        T Insert(T entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Insert(IEnumerable<T> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(T entity);

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Update(IEnumerable<T> entities);

        /// <summary>
        ///
        /// </summary>
        /// <param name="id"></param>
        void Delete(long id);

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Delete(T entity);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <param name="proNames"></param>
        /// <param name="isProUpdate"></param>
        void Update(T model, List<string> proNames, bool isProUpdate = true);

        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <param name="whereLambda"></param>
        /// <param name="modifiedProNames"></param>
        void Update(T model, Expression<Func<T, bool>> whereLambda, params string[] modifiedProNames);

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }
    }

    //public partial interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    //{
    //}
}