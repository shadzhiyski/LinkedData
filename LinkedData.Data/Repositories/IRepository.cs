namespace LinkedData.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Neo4jClient.Extension.Cypher;

    public interface IRepository<T>
    {
        void Put(T item);

        void PutRelated(T item);

        T Get(Expression<Func<T, bool>> filter);

        IEnumerable<T> GetAll();

        IEnumerable<T> GetAll(Expression<Func<T, bool>> filter);

        IEnumerable<T> GetAll<TFilter>(Expression<Func<TFilter, bool>> filter, 
            BaseRelationship relationship);
    }
}