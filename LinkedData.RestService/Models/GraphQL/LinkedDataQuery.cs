namespace LinkedData.RestService.Models.GraphQL
{
    using System.Collections.Generic;
    using System.Linq;
    using global::GraphQL.Types;
    using LinkedData.Data.Models;
    using LinkedData.Data.Repositories;

    public class LinkedDataQuery : ObjectGraphType
    {
        public LinkedDataQuery(GenesRepository genesRepository, 
            ProteinsRepository proteinsRepository)
        {
            Name = "LinkedDataQuery";

            Field<GeneType>(
                "gene",
                arguments: new QueryArguments(new QueryArgument[] 
                {
                    new QueryArgument<StringGraphType> { Name = "name" }
                }),
                resolve: context => 
                {
                    var geneName = context.GetArgument<string>("name");
                    return genesRepository.Get((Gene gene) => gene.Name == geneName);
                });

            Field<ListGraphType<GeneType>>(
                "genes",
                resolve: context => genesRepository.GetAll());
            
            Field<ListGraphType<GeneWithRelationsType>>(
                "genesRelations",
                arguments: new QueryArguments(new QueryArgument[] 
                {
                    new QueryArgument<StringGraphType> { Name = "geneName" }
                }),
                resolve: context => 
                {
                    var geneName = context.GetArgument<string>("geneName");
                    return !string.IsNullOrEmpty(geneName)
                        ? genesRepository.GetTree((Gene gene) => gene.Name == geneName)
                        : genesRepository.GetTree();
                });
            
            Field<ListGraphType<ProteinWithRelationsType>>(
                "proteins",
                arguments: new QueryArguments(new QueryArgument[] 
                {
                    new QueryArgument<StringGraphType> { Name = "geneName" }
                }),
                resolve: context =>
                {
                    var geneName = context.GetArgument<string>("geneName");
                    return !string.IsNullOrEmpty(geneName) 
                        ? proteinsRepository.GetTree((Gene gene) => gene.Name == geneName)
                        : proteinsRepository.GetTree();
                });
        }
    }
}