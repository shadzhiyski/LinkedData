namespace LinkedData.RestService.Models.GraphQL
{
    using global::GraphQL.Types;
    using LinkedData.Data.Models;
    using LinkedData.Data.Repositories;

    public class LinkedDataMutation : ObjectGraphType
    {
        public LinkedDataMutation(GenesRepository repository)
        {
            Name = "LinkedDataMutation";

            // put gene function
            Field<GeneType>(
                "putGene",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<GeneInputType>> { Name = "gene" }
                ),
                resolve: context =>
                {
                    var gene = context.GetArgument<Gene>("gene");
                    repository.Put(gene);
                    repository.PutRelated(gene);

                    return gene;
                }
            );
        }
    }
}