using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace TaskManager.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // Build the configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path to the current directory
                .AddJsonFile("appsettings.json")  // Add the appsettings.json to the configuration
                .Build();

            // Create the options builder for AppDbContext
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Use MySQL connection string from configuration (ensure MySQL version compatibility)
            optionsBuilder.UseMySql(
                configuration.GetConnectionString("DefaultConnection"), // Connection string name from appsettings.json
                new MySqlServerVersion(new Version(8, 0, 29))  // Specify the MySQL server version (adjust version if necessary)
            );

            // Return a new instance of AppDbContext with the configured options
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
