using System.Linq;
using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Data.Extensions
{
    public static class LatchOperationExtensions
    {

        public static LatchOperationDto ToDto(this LatchOperation operation)
        {
            var dto = new LatchOperationDto
            {
                Id = operation.Id,
                OperationId = operation.OperationId,
                Name = operation.Name,
                Type = operation.Type,
                Action = operation.Action,
                ApplyToAllUsers = operation.ApplyToAllUsers,
                ApplyToAllNodes = operation.ApplyToAllNodes
            };

            if (operation.UserIds != null && operation.UserIds.Any())
            {
                dto.Users.AddRange(operation.UserIds.Select(x => new LatchOperationUserDto { Id = x }));
            }

            if (operation.NodeIds != null && operation.NodeIds.Any())
            {
                dto.Nodes = operation.NodeIds;
            }

            return dto;
        }

    }
}