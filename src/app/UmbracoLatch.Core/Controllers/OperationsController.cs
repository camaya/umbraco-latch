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
    public class OperationsController : UmbracoAuthorizedJsonController
    {

        private readonly LatchOperationService latchOperationSvc;

        public OperationsController()
        {
            latchOperationSvc = new LatchOperationService(
                ApplicationContext.Current.DatabaseContext.Database,
                Services.TextService, 
                Services.UserService
            );
        }

        [HttpPost]
        [ValidateModelState]
        public UmbracoLatchResponse Create(LatchOperationRequestModel operation)
        {
            var response = latchOperationSvc.CreateOperation(operation);
            return response;
        }

        public LatchOperationDto GetOperation(int operationId)
        {
            var operation = latchOperationSvc.GetOperationById(operationId);
            return operation;
        }

        [HttpPost]
        [ValidateModelState]
        public UmbracoLatchResponse Edit([FromUri] int operationId, LatchOperationRequestModel operation)
        {
            if (!latchOperationSvc.OperationExists(operationId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var response = latchOperationSvc.EditOperation(operationId, operation);
            return response;
        }

        [HttpDelete]
        public UmbracoLatchResponse Delete(int operationId)
        {
            if (!latchOperationSvc.OperationExists(operationId))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var response = latchOperationSvc.DeleteOperation(operationId);
            return response;
        }

    }
}