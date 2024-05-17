using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ContactManager.Data
{
    public class ContactManagerContextFactory : IDesignTimeDbContextFactory<ContactManagerContext>
    {
        public ContactManagerContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContactManagerContext>();

            // Adjust the path as necessary to locate the appsettings.json in the ContactManager.Web project
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ContactManager.Web"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new ContactManagerContext(optionsBuilder.Options);
        }
    }
}
