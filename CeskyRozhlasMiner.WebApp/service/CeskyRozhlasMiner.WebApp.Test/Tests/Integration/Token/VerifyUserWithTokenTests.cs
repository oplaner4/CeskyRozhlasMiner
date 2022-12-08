using CeskyRozhlasMiner.Time;
using CeskyRozhlasMiner.WebApp.Data.Utilities;
using FluentAssertions;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.DSX.ProjectTemplate.Test.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Integration.Token
{
    [TestClass]
    [TestCategory("Tokens - Verify user with token")]
    public class VerifyUserWithTokenTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task VerifyUser_ValidToken_Success()
        {
            // Arrange
            Data.Models.Token randomToken = null;
            ServiceProvider.ExecuteWithDbScope(db => randomToken = SeedHelper.GetRandomToken(db));

            FakeTimeProvider.UtcNow = DateTime.UtcNow.AddHours(23);

            // Act
            using var response = await Client.PutAsJsonAsync("/api/tokens", Mapper.Map<TokenDto>(randomToken));

            // Assert
            var result = await EnsureObject<UserDto>(response);
            randomToken.OwnerId.Should().Be(result.Id);

            Data.Models.User user = null;
            ServiceProvider.ExecuteWithDbScope(db => user = db.Users.FirstOrDefault(u => u.Id == result.Id));
            user.Should().NotBeNull();
            user.Verified.Should().Be(true);

            Data.Models.Token token = null;
            ServiceProvider.ExecuteWithDbScope(db => token = db.Tokens.FirstOrDefault(t => t.Id == randomToken.Id));
            token.Should().NotBeNull();
            token.UsedCount.Should().BeGreaterThan(0);
        }

        [TestMethod]
        public async Task VerifyUser_TokenExpired_NotAcceptable()
        {
            // Arrange
            Data.Models.Token randomToken = null;
            ServiceProvider.ExecuteWithDbScope(db => randomToken = SeedHelper.GetRandomToken(db));

            FakeTimeProvider.UtcNow = DateTime.UtcNow.AddHours(70);

            // Act
            using var response = await Client.PutAsJsonAsync("/api/tokens", Mapper.Map<TokenDto>(randomToken));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotAcceptable);
        }

        [TestMethod]
        public async Task VerifyUser_NotSavedToken_NotFound()
        {
            // Arrange
            TokenDto dto = new()
            {
                Value = TokenValueGenerator.GetNewValue(),
            };

            // Act
            using var response = await Client.PutAsJsonAsync("/api/tokens", dto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
