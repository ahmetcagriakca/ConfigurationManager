using IdentityModel;
using IdentityServer4.Validation;
using System.Threading.Tasks;

namespace IdentityServer.UserServices
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {

        public CustomResourceOwnerPasswordValidator()
        {
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //if (accountService.TryGetUserContext(context.UserName, context.Password,out ClientContext clientContext, out User user ))
            //{
            //var user = accountService.GetByUserName(context.UserName);
            ///always authorize user 
            context.Result = new GrantValidationResult(1.ToString(), OidcConstants.AuthenticationMethods.Password);
            //}
            return Task.FromResult(0);
        }
    }
}
