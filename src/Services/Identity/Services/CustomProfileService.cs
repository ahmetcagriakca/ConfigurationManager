using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer.UserServices
{
    public class CustomProfileService : IProfileService
    {

        public CustomProfileService()
        {
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            //logger.LogInformation(string.Format("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
            //    context.Subject.GetSubjectId(),
            //    context.Client.ClientName ?? context.Client.ClientId,
            //    context.RequestedClaimTypes,
            //    context.Caller));
            var claims = new List<Claim>
            {
                new Claim("userid", 1.ToString()),
                new Claim("username", "TEST")
            };
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            //var sub = context.Subject.GetSubjectId();
            //var user = _accountService.GetUser(long.Parse(context.Subject.GetSubjectId()));
            context.IsActive = true; // (user != null && user.IsActive);
        }
    }
}
