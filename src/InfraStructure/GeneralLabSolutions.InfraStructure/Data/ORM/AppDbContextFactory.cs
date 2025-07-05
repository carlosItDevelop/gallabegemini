using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GeneralLabSolutions.InfraStructure.Data.ORM
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string [] args)
        {
            // 1. Construir a configuração
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Define o diretório base
                .AddJsonFile("appsettings.json", optional: true) // Lê appsettings.json se existir
                .AddJsonFile($"appsettings.Development.json", optional: true) // Lê appsettings.Development.json se existir e se for o ambiente de desenvolvimento
                .AddUserSecrets<AppDbContextFactory>() // Lê User Secrets (importante para desenvolvimento!)
                .Build();

            // 2. Obter a string de conexão da configuração
            var connectionString = configuration.GetConnectionString("DefaultConnection"); // Ou o nome que você estiver usando na configuração

            // 3. Usar a string de conexão lida
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // No design-time, não tentamos injetar IMediatorHandler – passamos null
            return new AppDbContext(optionsBuilder.Options, null);
        }
    }
}