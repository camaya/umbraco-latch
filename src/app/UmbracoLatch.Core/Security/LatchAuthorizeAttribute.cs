using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using Umbraco.Web;

namespace UmbracoLatch.Core.Security
{
    public class LatchAuthorizeAttribute : AuthorizeAttribute
    {

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            try
            {
                var currentUser = UmbracoContext.Current.Security.CurrentUser;
                var hasLatchAccess = currentUser.AllowedSections.Any(section => section.Equals(LatchConstants.SectionAlias, StringComparison.InvariantCultureIgnoreCase));
                return hasLatchAccess;
            }
            catch (Exception)
            {
                // By default, we assume the user has no access to the latch section.
                return false;
            }
        }

    }
}