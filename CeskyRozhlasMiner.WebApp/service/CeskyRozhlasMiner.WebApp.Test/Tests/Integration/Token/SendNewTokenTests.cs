using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.DSX.ProjectTemplate.Test.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Integration.Token
{
    [TestClass]
    [TestCategory("Tokens - Send new token")]
    public class SendNewTokenTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task SendNew_Success()
        {
            // Arrange
            Data.Models.User randomUser = null;
            ServiceProvider.ExecuteWithDbScope(db => randomUser = SeedHelper.GetRandomUser(db));
            SetupAuthenticatedUser(randomUser);

            // Act
            using var response = await Client.PostAsJsonAsync<string>("/api/tokens", null);

            // Assert
            var result = await EnsureObject<bool>(response);
            result.Should().Be(true);

            GetSentCount().Should().Be(1);
        }

        [TestMethod]
        public async Task SendNew_Unauthorized()
        {
            // Arrange
            SetupAuthorization(false);

            // Act
            using var response = await Client.PostAsJsonAsync<string>("/api/tokens", null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
