using Microsoft.Owin;
using Owin;
using Umbraco.Core;
using Umbraco.Core.Security;
using Umbraco.Web.Security.Identity;
using UmbracoLatch.Core;
using Umbraco.Core.Models.Identity;
using UmbracoLatch.Core.Checkers;
using UmbracoLatch.Core.Services;

[assembly: OwinStartup("UmbracoLatchOwinStartup", typeof(UmbracoLatchOwinStartup))]

namespace UmbracoLatch.Core
{
    public class UmbracoLatchOwinStartup
    {

        public void Configuration(IAppBuilder app)
        {
            var applicationContext = ApplicationContext.Current;
            app.ConfigureUserManagerForUmbracoBackOffice<BackOfficeUserManager, BackOfficeIdentityUser>(
                applicationContext,
                (options, context) =>
                {
                    var membershipProvider = MembershipProviderExtensions.GetUsersMembershipProvider().AsUmbracoMembershipProvider();
                    var userManager = BackOfficeUserManager.Create(options,
                        applicationContext.Services.UserService,
                        applicationContext.Services.ExternalLoginService,
                        membershipProvider);
 
                    // Overrides Umbraco's password checker
                    var latchOperationSvc = new LatchOperationService(applicationContext.DatabaseContext.Database, applicationContext.Services.TextService, applicationContext.Services.UserService);
                    userManager.BackOfficeUserPasswordChecker = new LatchLoginChecker(latchOperationSvc, applicationContext.Services.TextService, userManager.PasswordHasher);
                    return userManager;
                });

            //Ensure owin is configured for Umbraco back office authentication
            app
                .UseUmbracoBackOfficeCookieAuthentication(ApplicationContext.Current)
                .UseUmbracoBackOfficeExternalCookieAuthentication(ApplicationContext.Current);
        }

    }
}
