using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UmbracoLatch.Core.Data
{
    [TableName("LatchPairedAccount")]
    [PrimaryKey("UserId", autoIncrement = false)]
    public class LatchPairedAccount
    {
        [PrimaryKeyColumn(AutoIncrement = false)]
        public int UserId { get; set; }
        public string AccountId { get; set; }
    }
}