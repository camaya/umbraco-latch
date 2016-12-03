using System.Web;

namespace UmbracoLatch.Core.Extensions
{
    public static class HttpContextExtensions
    {

        public static Umbraco.Core.Models.Membership.IUser GetCurrentBackofficeUser(this HttpContext context)
        {
            var identity = context.GetOwinContext().Request.User.Identity;
            var user = Umbraco.Core.ApplicationContext.Current.Services.UserService.GetByUsername(identity.Name);
            return user;
        }

    }
}