using System.Collections.Generic;
using GraphQL.Types;
using LinkedData.Data.Models;

namespace LinkedData.RestService.Models.GraphQL
{
    public class ProteinWithRelationsType : ObjectGraphType<KeyValuePair<Protein, IEnumerable<Comment>>>
    {
        public ProteinWithRelationsType()
        {
            Field(g => g.Key.Name);
            Field("descriptions", g => g.Value, type: typeof(ListGraphType<CommentType>));
        }
        
    }
}