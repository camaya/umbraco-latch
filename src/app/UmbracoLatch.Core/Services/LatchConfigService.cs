using LatchSDK;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Data;
using UmbracoLatch.Core.Models;

namespace UmbracoLatch.Core.Services
{
    public class LatchConfigService : LatchService
    {

        public LatchConfigService(UmbracoDatabase database, ILocalizedTextService textService) 
            : base(database, textService) { }

        public UmbracoLatchResponse AddApplication(string applicationId, string secret)
        {
            var latchApplication = new LatchApplication
            {
                ApplicationId = applicationId,
                Secret = secret
            };

            latchRepo.AddApplication(latchApplication);

            var successMessage = textService.Localize("latch_application/addSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        public UmbracoLatchResponse Pair(string token, int userId)
        {
            var application = GetApplication();
            var latch = new Latch(application.ApplicationId, application.Secret);

            var response = latch.Pair(token);
            if (response.Error != null)
            {
                var errorMessage = GetPairingResponseMessage("error" + response.Error.Code);
                return new UmbracoLatchResponse(false, errorMessage);
            }

            if (response.Data == null || !response.Data.ContainsKey("accountId"))
            {
                var errorMessage = GetPairingResponseMessage("pairError");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var pairedAccount = new LatchPairedAccount
            {
                UserId = userId,
                AccountId = response.Data["accountId"] as string
            };
            latchRepo.AddPairedAccount(pairedAccount);

            var successMessage = GetPairingResponseMessage("pairSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        public UmbracoLatchResponse Unpair()
        {
            var application = GetApplication();
            var account = latchRepo.GetPairedAccount();

            if (account == null)
            {
                var errorMessage = GetPairingResponseMessage("alreadyUnpaired");
                return new UmbracoLatchResponse(false, errorMessage);
            }

            var latch = new Latch(application.ApplicationId, application.Secret);
            var response = latch.Unpair(account.AccountId);

            if (response.Error != null)
            {
                var errorMessage = GetPairingResponseMessage("error" + response.Error.Code);
                return new UmbracoLatchResponse(false, errorMessage);
            }

            latchRepo.DeletePairedAccount();

            var successMessage = GetPairingResponseMessage("unpairSuccess");
            return new UmbracoLatchResponse(true, successMessage);
        }

        private string GetPairingResponseMessage(string key)
        {
            var message = textService.Localize("latch_pairing/" + key);
            return message;
        }

    }
}