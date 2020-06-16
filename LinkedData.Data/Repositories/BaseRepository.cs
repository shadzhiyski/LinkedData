namespace LinkedData.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using LinkedData.Data.External;
    using Neo4jClient.Extension.Cypher;

    public abstract class BaseRepository<T> : IRepository<T>
        where T : class
    {
        protected Neo4jDataService _neo4jService;
        
        protected BaseRepository(string matchEntity, Neo4jDataService neo4jService)
        {
            MatchEntity = matchEntity;
            MatchEntityQuery = string.Format("({0}:{1})", MatchEntity.ToLower(), matchEntity);
            _neo4jService = neo4jService;
        }

        public string MatchEntity { get; protected set; }

        public string MatchEntityQuery { get; protected set; }

        public virtual T Get(Expression<Func<T, bool>> filter)
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.Match(MatchEntityQuery)
                .Where(filter)
                .Return<T>(MatchEntity.ToLower())
                .Results.FirstOrDefault();
                
            return searchedItems;
        }

        public virtual IEnumerable<T> GetAll()
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.Match(MatchEntityQuery)
                .Return<T>(MatchEntity.ToLower())
                .Results;
                
            return searchedItems;
        }

        public virtual IEnumerable<T> GetAll(Expression<Func<T, bool>> filter)
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.Match(MatchEntityQuery)
                .Where(filter)
                .Return<T>(MatchEntity.ToLower())
                .Results;
                
            return searchedItems;
        }

        public IEnumerable<T> GetAll<TFilter>(Expression<Func<TFilter, bool>> filter, BaseRelationship relationship)
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.MatchRelationship(relationship)
                .Where(filter)
                .Return<T>(MatchEntity.ToLower())
                .Results;
                
            return searchedItems;
        }

        public void Put(T item)
        {
            _neo4jService.Client.Cypher.MergeEntity(item)
                .ExecuteWithoutResults();
        }

        public abstract void PutRelated(T item);
    }
}