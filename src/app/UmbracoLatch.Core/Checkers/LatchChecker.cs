using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Models;
using UmbracoLatch.Core.Services;

namespace UmbracoLatch.Core.Checkers
{
    public abstract class LatchChecker
    {

        private bool? latchesShouldBeChecked;

        protected readonly LatchOperationService latchOperationSvc;
        protected readonly ILocalizedTextService textService;

        protected LatchChecker(LatchOperationService latchOperationSvc, ILocalizedTextService textService)
        {
            this.latchOperationSvc = latchOperationSvc;
            this.textService = textService;
        }

        protected bool LatchesShouldBeChecked
        {
            get
            {
                if (latchesShouldBeChecked == null)
                {
                    latchesShouldBeChecked = latchOperationSvc.AccountIsPaired;
                }
                return latchesShouldBeChecked.Value;
            }
        }

        protected IEnumerable<LatchOperationDto> GetLatchesApplyingToUser(IEnumerable<LatchOperationDto> latches, int userId)
        {
            var latchesApplyingToUser = latches.Where(latch => LatchApplyToUser(latch, userId));
            return latchesApplyingToUser;
        }

        protected bool AnyLatchIsClosed(IEnumerable<LatchOperationDto> latchesToApply)
        {
            foreach (var latch in latchesToApply)
            {
                var latchIsOpen = latchOperationSvc.LatchIsOpen(latch.OperationId);
                if (!latchIsOpen)
                {
                    return true;
                }
            }
            return false;
        }

        protected string GetErrorMessage(string key)
        {
            var message = textService.Localize("latch_checkers/" + key);
            return message;
        }

        private bool LatchApplyToUser(LatchOperationDto latch, int userId)
        {
            if (latch.ApplyToAllUsers)
            {
                return true;
            }

            var latchApplyToUser = latch.Users.Any(x => x.Id.Equals(userId));
            return latchApplyToUser;
        }

    }
}