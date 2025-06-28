using Microsoft.Extensions.Configuration;

namespace GeneralLabSolutions.WebApiCore.Extensions
{

    public static class ConfigurationExtensions
    {
        public static string? GetDefaultConnectionString(this IConfiguration configuration)
        {
            return configuration.GetConnectionString(nameof(ConnectionStrings.DefaultConnection));
        }
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = null!;
    }
}
