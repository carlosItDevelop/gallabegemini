/*

Refatoração Completa do SeedDataUsersAndRoles
Visão geral:

1. "Substituímos os dois arrays paralelos (nomesCompleto e apelidos) por uma coleção tipada (List<UsuarioSeedInfo>), que agrupa todas as informações de criação em um único objeto."

2. "Mantemos a política de retry com Polly, o scope de serviços DI, os logs e a lógica de idempotência."

3. "Incluímos XML comments para facilitar a geração de documentação e a navegação no código."

4. "Mantivemos as assinaturas públicas dos métodos auxiliares para não quebrar chamadas externas."
 
 
*/



using GeneralLabSolutions.Identidade.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;

namespace GeneralLabSolutions.Identidade.Configuration
{
    /// <summary>
    /// Popula tabelas do Identity com roles e usuários iniciais.
    /// </summary>
    public static class SeedDataUsersAndRoles
    {
        /// <summary>
        /// Executa a rotina de seeding de forma idempotente e resiliente (Polly).
        /// </summary>
        public static async Task InitializeAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Estratégia de retry exponencial para falhas temporárias (ex. banco iniciando)
            var retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (ex, ts, attempt, ctx) =>
                        logger.LogWarning(ex,
                            "Tentativa {Attempt}/3 para popular o BD após {Delay:n1}s. Erro: {Message}",
                            attempt, ts.TotalSeconds, ex.Message));

            await retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = serviceProvider.CreateScope();
                var sp = scope.ServiceProvider;
                var contextOptions = sp.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
                await using var ctx = new ApplicationDbContext(contextOptions);

                var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
                var seedUserSettings = configuration
                                        .GetSection(nameof(SeedUserSettings))
                                        .Get<SeedUserSettings>()!;

                /* 1. Verificações prévias --------------------------------------------- */
                if (!await ctx.Database.CanConnectAsync() || !ctx.Database.GetAppliedMigrations().Any())
                {
                    logger.LogInformation("Tabelas do Identity não encontradas. Execute as migrations primeiro.");
                    return;
                }

                if (await roleManager.RoleExistsAsync("Admin"))
                {
                    logger.LogInformation("BD já populado anteriormente — encerrando.");
                    return;
                }

                /* 2. Definições de seed ----------------------------------------------- */
                // a) Roles
                var roles = new [] { "Admin", "Default", "SuperAdmin" };

                // b) Usuários (ordem não importa, idempotente)
                var usuarios = new List<UsuarioSeedInfo>
                {
                    new("Carlos A Santos", "Carlos Alberto",
                        seedUserSettings.SuperAdminEmail!, seedUserSettings.SuperAdminPassword!, "SuperAdmin"),

                    new("Camila Oliveira", "Camila Oliveira",
                        seedUserSettings.AdminEmail!,      seedUserSettings.AdminPassword!,      "Admin"),

                    new("Lucas Ferreira", "Lucas Ferreira",
                        seedUserSettings.DefaultUserEmail!, seedUserSettings.DefaultUserPassword!, "Default")
                };

                /* 3. Criação de roles -------------------------------------------------- */
                logger.LogInformation("Criando roles…");
                foreach (var roleName in roles)
                    await CriarRoleAsync(roleManager, roleName, logger);

                /* 4. Criação de usuários --------------------------------------------- */
                logger.LogInformation("Criando usuários…");
                foreach (var u in usuarios)
                {
                    await CriarUsuarioAsync(
                        userManager,
                        u.NomeCompleto,
                        u.Apelido,
                        DateTime.Now.AddYears(-20), // >> Data de nascimento fictícia
                        "imagemPadrao.png",
                        u.Email,
                        u.Senha,
                        u.Role,
                        logger);
                }

                await ctx.SaveChangesAsync();
                logger.LogInformation("População concluída com sucesso.");
            });
        }

        #region Métodos auxiliares

        private static async Task CriarRoleAsync(
            RoleManager<IdentityRole> roleManager, string roleName, ILogger logger)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                logger.LogInformation("Role '{Role}' já existe.", roleName);
                return;
            }

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
                logger.LogInformation("Role '{Role}' criada com sucesso.", roleName);
            else
                logger.LogError("Falha ao criar role '{Role}': {Errors}",
                    roleName, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private static async Task CriarUsuarioAsync(
            UserManager<ApplicationUser> userManager,
            string nomeCompleto,
            string apelido,
            DateTime dataNascimento,
            string imgProfilePath,
            string email,
            string password,
            string role,
            ILogger logger)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
            {
                logger.LogInformation("Usuário '{Email}' já existe.", email);
                return;
            }

            var user = new ApplicationUser
            {
                NomeCompleto = nomeCompleto,
                Apelido = apelido,
                DataNascimento = dataNascimento,
                ImgProfilePath = imgProfilePath,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                logger.LogError("Erro ao criar usuário '{Email}': {Errors}",
                    email, string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }

            await userManager.AddToRoleAsync(user, role);
            logger.LogInformation("Usuário '{Email}' criado e adicionado à role '{Role}'.", email, role);
        }

        #endregion

        #region Tipos internos

        /// <summary>
        /// DTO interno que agrupa dados necessários para criar um usuário seed.
        /// </summary>
        private sealed record UsuarioSeedInfo(
            string NomeCompleto,
            string Apelido,
            string Email,
            string Senha,
            string Role);

        #endregion
    }
}

