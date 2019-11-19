using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace Recollectable.API.Filters
{
    //TODO Fix Swagger Header Filter
    /*public class FromHeaderAttributeFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var fromHeaders = operation.Parameters.Where(p => p.In == "header").ToList();

            foreach (var attribute in fromHeaders)
            {
                operation.Parameters.Remove(attribute);
            }
        }
    }*/
}