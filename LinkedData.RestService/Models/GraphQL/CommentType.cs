using GraphQL.Types;
using LinkedData.Data.Models;

namespace LinkedData.RestService.Models.GraphQL
{
    public class CommentType : ObjectGraphType<Comment>
    {
        public CommentType()
        {
            Field(c => c.Type);
            Field(c => c.Text);
        }
    }
}