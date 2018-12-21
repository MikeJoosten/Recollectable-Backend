using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Recollectable.API.Filters
{
    public class HttpRequestsFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument document, DocumentFilterContext context)
        {
            foreach (PathItem path in document.Paths.Values)
            {
                path.Head = null;
                path.Options = null;
            }
        }
    }
}