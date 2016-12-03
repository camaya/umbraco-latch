using System.Net;
using System.Web.Http;
using Umbraco.Core;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;
using UmbracoLatch.Core.Filters;
using UmbracoLatch.Core.Models;
using UmbracoLatch.Core.Security;
using UmbracoLatch.Core.Services;
using UmbracoLatch.Core.WebApi;

namespace UmbracoLatch.Core.Controllers
{

    [PluginController(LatchConstants.SectionName)]
    [AngularJsonOnlyConfigurationCamelCase]
    [LatchAuthorize]
    public class ApplicationController : UmbracoAuthorizedJsonController
    {

        private readonly LatchConfigService latchConfigSvc;
        private readonly LatchOperationService latchOperationSvc;

        public ApplicationController()
        {
            latchConfigSvc = new LatchConfigService(ApplicationContext.Current.DatabaseContext.Database, Services.TextService);
            latchOperationSvc = new LatchOperationService(
                ApplicationContext.Current.DatabaseContext.Database,
                Services.TextService,
                Services.UserService
            );
        }

        [HttpPost]
        [ValidateModelState]
        public UmbracoLatchResponse AddApplication(LatchApplicationRequestModel application)
        {
            var response = latchConfigSvc.AddApplication(application.ApplicationId, application.Secret);
            return response;
        }

        public LatchApplicationDto GetApplication()
        {
            var application = latchConfigSvc.GetApplication();
            return application;
        }

        [HttpPost]
        public UmbracoLatchResponse Pair([FromBody] string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var pairingResponse = latchConfigSvc.Pair(token, Security.CurrentUser.Id);
            latchOperationSvc.CreateDefaultOperation();
            return pairingResponse;
        }
        
        public LatchPairedAccountDto GetPairedAccount()
        {
            var account = latchConfigSvc.GetPairedAccount();
            return account;
        }

        [HttpPost]
        public UmbracoLatchResponse Unpair()
        {
            var response = latchConfigSvc.Unpair();
            return response;
        }

    }
}