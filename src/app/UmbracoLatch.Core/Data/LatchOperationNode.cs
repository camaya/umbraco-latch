using Umbraco.Core.Persistence;

namespace UmbracoLatch.Core.Data
{
    [TableName("LatchOperationNode")]
    public class LatchOperationNode
    {
        public int NodeId { get; set; }
        public int LatchOperationId { get; set; }
    }
}