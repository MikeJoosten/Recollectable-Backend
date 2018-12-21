using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Recollectable.API.Filters
{
    public class CustomXmlFormatter : OutputFormatter
    {
        public CustomXmlFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/xml"));
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/xml"));
        }

        protected override bool CanWriteType(Type type)
        {
            return true;
        }

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;

            var json = JsonConvert.SerializeObject(context.Object);
            var doc = context.Object.GetType().Name.Contains("List") ?
                JsonConvert.DeserializeXmlNode("{Element:" + json + "}", "Root") :
                JsonConvert.DeserializeXmlNode(json, "Element");

            doc.DocumentElement.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            doc.DocumentElement.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");

            var emptyElements = doc.SelectNodes(@"//*[not(node())]");

            for (int i = emptyElements.Count - 1; i >= 0; i--)
            {
                emptyElements[i].ParentNode.RemoveChild(emptyElements[i]);
            }

            return response.WriteAsync(doc.OuterXml);
        }
    }
}