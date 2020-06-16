using GraphQL.Types;
using LinkedData.Data.Models;

namespace LinkedData.RestService.Models.GraphQL
{
    public class VariationType : ObjectGraphType<Variation>
    {
        public VariationType()
        {
            Field(g => g.Location);

            Field(g => g.Description);
        }
    }
}