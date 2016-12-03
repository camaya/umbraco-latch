using System.Linq;
using System.Web;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Extensions;
using UmbracoLatch.Core.Services;

namespace UmbracoLatch.Core.Checkers
{
    public class LatchLocalizationChecker : LatchChecker
    {

        public LatchLocalizationChecker(LatchOperationService latchSvc, ILocalizedTextService textService) 
            : base(latchSvc, textService) { }

        public void LatchDictionaryItemDeleting(ILocalizationService sender, DeleteEventArgs<IDictionaryItem> e)
        {
            if (!LatchesShouldBeChecked) return;

            var user = HttpContext.Current.GetCurrentBackofficeUser();
            var latches = latchOperationSvc.GetLatches(LatchOperationType.Dictionary, LatchOperationAction.Delete);
            if (latches.Any())
            {
                var latchesToApply = GetLatchesApplyingToUser(latches, user.Id);
                var operationIsLocked = AnyLatchIsClosed(latchesToApply);
                if (operationIsLocked)
                {
                    var errorMessage = GetErrorMessage("dictionaryDelete");
                    var eventMessage = new EventMessage(LatchConstants.SectionName, errorMessage, EventMessageType.Error);
                    e.CancelOperation(eventMessage);
                }
            }
        }

    }
}