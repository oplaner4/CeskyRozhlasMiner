using AutoMapper;
using CeskyRozhlasMiner.WebApp.Data.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.DSX.ProjectTemplate.Data.DTOs;
using Microsoft.DSX.ProjectTemplate.Data.Models;
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
            return database.Tokens.OrderBy(_ => Guid.NewGuid()).First();
        }

        public static Group CreateValidNewGroup(ProjectTemplateDbContext database, string name = "")
        {
            return new Group()
            {
                Name = name.Length == 0 ? RandomFactory.GetAlphanumericString(8) : name,
                IsActive = RandomFactory.GetBoolean()
            };
        }

        public static Project CreateValidNewProject(ProjectTemplateDbContext database, Group group = null)
        {
            return new Project()
            {
                Name = RandomFactory.GetCodeName(),
                Group = group ?? GetRandomGroup(database)
            };
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
