namespace GeneralLabSolutions.Identidade.Configuration
{

    /// <summary>
    /// Configurações para o seeding de usuários e roles iniciais.
    /// </summary>
    public class SeedUserSettings
    {
        /// <summary>
        /// Email do super administrador.
        /// </summary>
        public string? SuperAdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha do super administrador.
        /// </summary>
        public string? SuperAdminPassword { get; set; } = string.Empty;

        /// <summary>
        /// Email do administrador.
        /// </summary>
        public string? AdminEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha do administrador.
        /// </summary>
        public string? AdminPassword { get; set; } = string.Empty;

        /// <summary>
        /// Email do usuário padrão.
        /// </summary>
        public string? DefaultUserEmail { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário padrão.
        /// </summary>
        public string? DefaultUserPassword { get; set; } = string.Empty;
    }
}