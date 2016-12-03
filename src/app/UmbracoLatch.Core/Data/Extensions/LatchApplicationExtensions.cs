using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Data.Extensions
{
    public static class LatchApplicationExtensions
    {

        public static LatchApplicationDto ToDto(this LatchApplication application)
        {
            var dto = new LatchApplicationDto();

            if (application != null)
            {
                dto.ApplicationId = application.ApplicationId;
                dto.Secret = application.Secret;
            }

            return dto;
        }

    }
}