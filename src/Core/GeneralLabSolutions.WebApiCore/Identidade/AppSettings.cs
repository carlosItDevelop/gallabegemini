namespace GeneralLabSolutions.WebApiCore.Identidade
{
    /// <summary>
    /// Classe de configuração para o JWT.
    /// </summary>
    public class AppSettings
    {
        /// <summary>
        /// Chave secreta utilizada para a assinatura do token JWT.
        /// </summary>
        public string? Secret { get; set; } = string.Empty;

        /// <summary>
        /// Tempo de expiração do token em horas.
        /// </summary>
        public int ExpiracaoHoras { get; set; }

        /// <summary>
        /// URL do emissor do token.
        /// </summary>
        public string? Emissor { get; set; } = string.Empty;

        /// <summary>
        /// URL do destinatário do token.
        /// </summary>
        public string? ValidoEm { get; set; } = string.Empty;
    }
}
