﻿using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Utilities;
using Microsoft.Extensions.Logging;

namespace Microsoft.DSX.ProjectTemplate.Test.Infrastructure
{
    public class TestDataSeeder
    {
        private readonly ProjectTemplateDbContext _dbContext;
        private readonly ILogger<TestDataSeeder> _logger;

        public TestDataSeeder(ProjectTemplateDbContext context, ILogger<TestDataSeeder> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public void SeedTestData()
        {
            _logger.LogInformation("Database seeding started.");

            SeedUsers(10);

            SeedTokens(20);

            _logger.LogInformation("Database seeding completed.");
        }

        private void SeedUsers(int entityCount)
        {
            for (int i = 0; i < entityCount; i++)
            {
                var newUser = SeedHelper.CreateValidNewUser();
                _dbContext.Users.Add(newUser);
            }

            _dbContext.SaveChanges();
        }

        private void SeedTokens(int entityCount)
        {
            for (int i = 0; i < entityCount; i++)
            {
                var newToken = SeedHelper.CreateValidNewToken(_dbContext);
                _dbContext.Tokens.Add(newToken);
            }

            _dbContext.SaveChanges();
        }
    }
}
