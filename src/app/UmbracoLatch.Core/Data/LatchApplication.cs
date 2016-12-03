using Umbraco.Core.Persistence;

namespace UmbracoLatch.Core.Data
{
    [TableName("LatchApplication")]
    public class LatchApplication
    {
        public string ApplicationId { get; set; }
        public string Secret { get; set; }
    }
}
