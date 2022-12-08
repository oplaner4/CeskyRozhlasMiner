using Microsoft.AspNetCore.Authentication;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace CeskyRozhlasMiner.WebApp.Test.Infrastructure
{
    public class FakeAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Used when it is enough to make an action authorized and do not care about user.
        /// </summary>
        public static readonly User FakeDefaultUser = new()
        {
            Email = "fake.user@gmail.com",
            Id = -1,
        };

        /// <summary>
        /// Claims are given to this user. If null, performed action is not authorized.
        /// </summary>
        internal static User FakeUser = FakeDefaultUser;

        /// <summary>
        /// The name of the authorizaton scheme that this handler will respond to.
        /// </summary>
        public const string AuthScheme = "FakeAuth";

        public FakeAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// Marks all authentication requests as successful, and injects the
        /// default company id into the user claims.
        /// </summary>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (FakeUser == null)
            {
                return Task.FromResult(AuthenticateResult.Fail("Fake user have not been set."));
            }

            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                    new(ClaimTypes.Name, FakeUser.Email),
                    new(ClaimTypes.NameIdentifier, FakeUser.Id.ToString())
                }, AuthScheme)),
                new AuthenticationProperties(),
                AuthScheme);
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }
    }
}
