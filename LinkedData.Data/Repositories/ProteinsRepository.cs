namespace LinkedData.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using LinkedData.Data.External;
    using LinkedData.Data.Models;
    using Neo4jClient.Extension.Cypher;

    public class ProteinsRepository : BaseRepository<Protein>
    {
        public ProteinsRepository(Neo4jDataService neo4jService)
            : base("Protein", neo4jService)
        { }

        public IEnumerable<KeyValuePair<Protein, IEnumerable<Comment>>> GetTree()
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.MatchRelationship(new ProteinCommentRelationship())
                .Return((protein, comment) => new 
                { 
                    Protein = protein.As<Protein>(), 
                    Comments = comment.CollectAs<Comment>() 
                })
                .Results.Select(d => new KeyValuePair<Protein, IEnumerable<Comment>>(d.Protein, d.Comments));
                
            return searchedItems;
        }

        public IEnumerable<KeyValuePair<Protein, IEnumerable<Comment>>> GetTree(Expression<Func<Gene, bool>> filter)
        {
            var client = _neo4jService.Client;

            var searchedItems = client.Cypher.MatchRelationship(new ProteinCommentRelationship())
                .MatchRelationship(new GeneProteinRelationship())
                .Where(filter)
                .Return((protein, comment) => new 
                { 
                    Protein = protein.As<Protein>(), 
                    Comments = comment.CollectAs<Comment>() 
                })
                .Results.Select(d => new KeyValuePair<Protein, IEnumerable<Comment>>(d.Protein, d.Comments));
                
            return searchedItems;
        }

        public override void PutRelated(Protein item)
        {
            throw new NotImplementedException();
        }
    }
}