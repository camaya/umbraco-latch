using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using Umbraco.Web.WebApi;

namespace UmbracoLatch.Core.WebApi
{
    public class AngularJsonOnlyConfigurationCamelCaseAttribute : AngularJsonOnlyConfigurationAttribute
    {

        public override void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var toRemove = controllerSettings.Formatters.Where(t => (t is JsonMediaTypeFormatter) || (t is XmlMediaTypeFormatter)).ToList();
            foreach (var r in toRemove)
            {
                controllerSettings.Formatters.Remove(r);
            }
            controllerSettings.Formatters.Add(new AngularJsonMediaFormatterCamelCase());
        }

    }
}