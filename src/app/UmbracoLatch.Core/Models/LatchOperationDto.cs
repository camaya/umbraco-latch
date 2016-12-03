using System.Collections.Generic;

namespace UmbracoLatch.Core.Models
{
    public class LatchOperationDto
    {

        public LatchOperationDto()
        {
            Users = new List<LatchOperationUserDto>();
        }

        public int Id { get; set; }
        public string OperationId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public bool ApplyToAllUsers { get; set; }
        public List<LatchOperationUserDto> Users { get; set; }
        public bool ApplyToAllNodes { get; set; }

        public IEnumerable<int> Nodes { get; set; }

        //public string Nodes { get; set; }
    }

    public class LatchOperationUserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}