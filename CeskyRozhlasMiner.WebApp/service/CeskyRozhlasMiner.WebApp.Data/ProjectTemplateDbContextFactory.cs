using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Microsoft.DSX.ProjectTemplate.Data
{
    public class ProjectTemplateDbContextFactory : IDesignTimeDbContextFactory<ProjectTemplateDbContext>
    {
        public ProjectTemplateDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectTemplateDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=master;Trusted_Connection=True;");
            return new ProjectTemplateDbContext(optionsBuilder.Options);
        }
    }
}
