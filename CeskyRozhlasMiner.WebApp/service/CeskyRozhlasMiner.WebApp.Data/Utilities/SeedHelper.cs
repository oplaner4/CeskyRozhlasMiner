using CeskyRozhlasMiner.WebApp.Data.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.DSX.ProjectTemplate.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Microsoft.DSX.ProjectTemplate.Data.Utilities
{
    public static class SeedHelper
    {
        public static Group GetRandomGroup(ProjectTemplateDbContext database)
        {
            return database.Groups.OrderBy(_ => Guid.NewGuid()).First();
        }

        public static User GetRandomUser(ProjectTemplateDbContext database)
        {
            return database.Users.OrderBy(_ => Guid.NewGuid()).First();
        }

        public static Token GetRandomToken(ProjectTemplateDbContext database)
        {
            return database.Tokens.Include(t => t.Owner).OrderBy(_ => Guid.NewGuid()).First();
        }

        public static User CreateValidNewUser()
        {
            var name = RandomFactory.GetAlphanumericString(10);
            var result = new User()
            {
                DisplayName = name,
                Email = $"{name}@email.com",
                Verified = false,
            };

            result.PasswordHash = new PasswordHasher<User>().HashPassword(result, result.Email);
            return result;
        }

        public static Token CreateValidNewToken(ProjectTemplateDbContext database, User owner = null)
        {
            return new Token()
            {
                Value = TokenValueGenerator.GetNewValue(),
                Owner = owner ?? GetRandomUser(database),
                UsedCount = 0,
            };
        }
    }
}
