using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Data.Extensions
{
    public static class LatchPairedAccountExtensions
    {

        public static LatchPairedAccountDto ToDto(this LatchPairedAccount pairedAccount)
        {
            var dto = new LatchPairedAccountDto();

            if (pairedAccount != null)
            {
                dto.AccountId = pairedAccount.AccountId;
            }

            return dto;
        }

    }
}