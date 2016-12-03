using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Core.Models.Identity;
using Umbraco.Core.Security;
using Umbraco.Core.Services;
using UmbracoLatch.Core.Services;

namespace UmbracoLatch.Core.Checkers
{
    public class LatchLoginChecker : LatchChecker, IBackOfficeUserPasswordChecker
    {

        private readonly IPasswordHasher passwordHasher;

        public LatchLoginChecker(LatchOperationService latchOperationSvc, ILocalizedTextService textService, IPasswordHasher passwordHasher) 
            : base(latchOperationSvc, textService)
        {
            this.passwordHasher = passwordHasher;
        }

        public async Task<BackOfficeUserPasswordCheckerResult> CheckPasswordAsync(BackOfficeIdentityUser user, string password)
        {
            var hash = passwordHasher.HashPassword(password);
            var passwordIsCorrect = user.PasswordHash.Equals(hash, StringComparison.InvariantCulture);
            if (!passwordIsCorrect)
            {
                return BackOfficeUserPasswordCheckerResult.InvalidCredentials;
            }

            if (LatchesShouldBeChecked)
            {
                var latches = latchOperationSvc.GetLatches(LatchOperationType.Login);
                if (!latches.Any())
                {
                    return BackOfficeUserPasswordCheckerResult.ValidCredentials;
                }

                var latchesToApply = GetLatchesApplyingToUser(latches, user.Id);
                var loginIsLocked = AnyLatchIsClosed(latchesToApply);
                if (loginIsLocked)
                {
                    return BackOfficeUserPasswordCheckerResult.InvalidCredentials;
                }
            }

            return BackOfficeUserPasswordCheckerResult.ValidCredentials;
        }

    }
}