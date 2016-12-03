using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Data;
using UmbracoLatch.Core.Data.Extensions;
using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Services
{
    public abstract class LatchService
    {

        private bool? accountIsPaired;

        protected readonly LatchRepository latchRepo;
        protected readonly ILocalizedTextService textService;

        protected LatchService(UmbracoDatabase database, ILocalizedTextService textService)
        {
            latchRepo = new LatchRepository(database);
            this.textService = textService;
        }

        public bool AccountIsPaired
        {
            get
            {
                if (accountIsPaired == null)
                {
                    var account = latchRepo.GetPairedAccount();
                    accountIsPaired = account != null;
                }
                return accountIsPaired.Value;
            }
        }

        public LatchApplicationDto GetApplication()
        {
            var application = latchRepo.GetApplication();
            var dto = application.ToDto();
            return dto;
        }

        public LatchPairedAccountDto GetPairedAccount()
        {
            var account = latchRepo.GetPairedAccount();
            var dto = account.ToDto();
            return dto;
        }

    }

}