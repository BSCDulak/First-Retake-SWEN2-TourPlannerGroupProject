using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SWEN2_TourPlannerGroupProject.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ Lee appsettings.json desde el directorio actual
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2️⃣ Obtiene la cadena de conexión
            var connectionString = config.GetConnectionString("DefaultConnection");

            // 3️⃣ Configura las opciones de EF Core
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // 4️⃣ Retorna el contexto listo para usar
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
