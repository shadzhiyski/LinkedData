using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using LinkedData.Data.Models;
using Newtonsoft.Json;

namespace LinkedData.RestService.Models.GraphQL
{
    public class GeneWithRelationsType : ObjectGraphType<(string Gene, IEnumerable<Variation> Variations, IEnumerable<Protein> Proteins)>
    {
        public GeneWithRelationsType()
        {
            Field("name", g => g.Gene);

            Field("variations", g => g.Variations, type: typeof(ListGraphType<VariationType>));

            Field("proteins", g => g.Proteins, type: typeof(ListGraphType<ProteinType>));
        }
    }
}