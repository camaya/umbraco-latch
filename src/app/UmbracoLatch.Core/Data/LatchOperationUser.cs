using Umbraco.Core.Persistence;

namespace UmbracoLatch.Core.Data
{
    [TableName("LatchOperationUser")]
    public class LatchOperationUser
    {
        public int UserId { get; set; }
        public int LatchOperationId { get; set; }
    }
}