using System.Collections.Generic;
using GraphQL.Types;
using LinkedData.Data.Models;

namespace LinkedData.RestService.Models.GraphQL
{
    public class ProteinWithRelationsType : ObjectGraphType<(Protein Protein, IEnumerable<Comment> Comments)>
    {
        public ProteinWithRelationsType()
        {
            Field(g => g.Protein.Name);
            Field("comments", g => g.Comments, type: typeof(ListGraphType<CommentType>));
        }
        
    }
}