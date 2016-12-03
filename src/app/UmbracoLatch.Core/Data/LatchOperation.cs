using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UmbracoLatch.Core.Data
{
    [TableName("LatchOperation")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class LatchOperation
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string OperationId { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public bool ApplyToAllUsers { get; set; }
        public bool ApplyToAllNodes { get; set; }

        [ResultColumn]
        public IEnumerable<int> UserIds { get; set; }

        [ResultColumn]
        public IEnumerable<int> NodeIds { get; set; }
    }
}