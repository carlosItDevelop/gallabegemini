using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GeneralLabSolutions.Identidade.Data
{
    /// <summary>
    /// Classe de Context do Identity, que herda de DbContext
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
