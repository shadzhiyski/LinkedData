namespace LinkedData.RestService.Models.GraphQL
{
    using System.ComponentModel.DataAnnotations;
    using global::GraphQL.Types;
    using LinkedData.Data.Models;

    public class GeneInputType : InputObjectGraphType<Gene>
    {
        public GeneInputType()
        {
            Name = "GeneInputType";
            Field("name", g => g.Name);
            //Field<NonNullGraphType<StringGraphType>>("name");
        }
    }
}