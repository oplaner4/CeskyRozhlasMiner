using AutoMapper;
using MediatR;
using Microsoft.DSX.ProjectTemplate.Data;
using Microsoft.DSX.ProjectTemplate.Data.Abstractions;
using Microsoft.DSX.ProjectTemplate.Data.Services;
using Microsoft.DSX.ProjectTemplate.Test.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;

namespace Microsoft.DSX.ProjectTemplate.Test.Tests.Unit
{
    [TestCategory("Unit")]
    public abstract class BaseUnitTest : BaseTest
    {
        protected ILoggerFactory LoggerFactory { get; set; }

        protected Mock<IMediator> MockMediator { get; set; } = new();

        protected static IMapper Mapper => new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

        protected Mock<IEmailService> MockEmailService { get; set; } = new();

        protected BaseUnitTest()
        {
            // redirect all logging to console
            var services = new ServiceCollection();
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
            });
            var serviceProvider = services.BuildServiceProvider();
            LoggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            // output sent emails to console
            MockEmailService
                .Setup(x => x.SendEmailAsync(It.IsNotNull<string>(),
                                            It.IsNotNull<string>(),
                                            It.IsNotNull<string>(),
                                            It.IsNotNull<string>()))
                .Returns((string from, string to, string subject, string body) =>
                {
                    var logger = LoggerFactory.CreateLogger<EmailService>();
                    logger.LogInformation($"From {from} | To: {to} | Subject: '{subject}' | Body: '{body}'");
                    return Task.CompletedTask;
                });
        }

        /// <summary>
		/// Execute a query against an in-memory database seeded with test data and then check assertions using the return value.
		/// </summary>
        /// <remarks>To test methods which we expect to throw an exception, don't specify <paramref name="assertions"/>.</remarks>
		/// <param name="func">Function to execute.</param>
		/// <param name="assertions">Assertions to execute after the function has executed.</param>
		protected async Task ExecuteWithDb<TResult>(
            Func<ProjectTemplateDbContext, Task<TResult>> func,
            Action<TResult, ProjectTemplateDbContext> assertions = null)
        {
            using (var db = new ProjectTemplateDbContext(CreateInMemoryContextOptions()))
            {
                // seed test data
                new TestDataSeeder(db, LoggerFactory.CreateLogger<TestDataSeeder>()).SeedTestData();

                // execute our test
                var result = await func(db);

                // check our test results
                assertions?.Invoke(result, db);
            }
        }

        private static DbContextOptions<ProjectTemplateDbContext> CreateInMemoryContextOptions()
        {
            var builder = new DbContextOptionsBuilder<ProjectTemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // we use a new GUID so that each unit test gets its own fresh database
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            return builder.Options;
        }
    }
}
