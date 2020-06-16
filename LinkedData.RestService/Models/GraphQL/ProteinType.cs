using GraphQL.Types;
using LinkedData.Data.Models;

namespace LinkedData.RestService.Models.GraphQL
{
    public class ProteinType : ObjectGraphType<Protein>
    {
        public ProteinType()
        {
            Field(g => g.Name);
        }
    }
}