using GraphQL;
using GraphQL.Types;

namespace LinkedData.RestService.Models.GraphQL
{
    public class LinkedDataSchema : Schema
    {
        public LinkedDataSchema(IDependencyResolver resolver) 
            : base(resolver)
        {
            Query = resolver.Resolve<LinkedDataQuery>();
            Mutation = resolver.Resolve<LinkedDataMutation>();
        }
    }
}