using Newtonsoft.Json.Serialization;
using Umbraco.Web.WebApi;

namespace UmbracoLatch.Core.WebApi
{
    public class AngularJsonMediaFormatterCamelCase : AngularJsonMediaTypeFormatter
    {
        public AngularJsonMediaFormatterCamelCase()
        {
            SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}