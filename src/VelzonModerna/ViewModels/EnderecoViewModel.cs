using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using GeneralLabSolutions.Domain.Entities; // Para referenciar Endereco.TipoDeEnderecoEnum

namespace VelzonModerna.ViewModels
{
    public class EnderecoViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "A associação com a Pessoa é obrigatória.")]
        public Guid PessoaId { get; set; }

        [Required(ErrorMessage = "O País é obrigatório.")]
        [Display(Name = "País (Código ISO)")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "O código do país deve ter 2 caracteres (ex: BR, US).")]
        public string PaisCodigoIso { get; set; }

        [Required(ErrorMessage = "A Linha de Endereço 1 é obrigatória.")]
        [Display(Name = "Endereço Linha 1")]
        [MaxLength(200, ErrorMessage = "Máximo de {1} caracteres.")]
        public string LinhaEndereco1 { get; set; }

        [Display(Name = "Endereço Linha 2 (Opcional)")]
        [MaxLength(200, ErrorMessage = "Máximo de {1} caracteres.")]
        public string LinhaEndereco2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Cidade é obrigatória.")]
        [Display(Name = "Cidade / Localidade")]
        [MaxLength(100, ErrorMessage = "Máximo de {1} caracteres.")]
        public string Cidade { get; set; }

        [Display(Name = "Estado / Província / Região (Opcional)")]
        [MaxLength(100, ErrorMessage = "Máximo de {1} caracteres.")]
        public string EstadoOuProvincia { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Código Postal é obrigatório.")]
        [Display(Name = "CEP / Código Postal / ZIP Code")]
        [MaxLength(20, ErrorMessage = "Máximo de {1} caracteres.")]
        public string CodigoPostal { get; set; }

        [Display(Name = "Informações Adicionais (Opcional)")]
        [MaxLength(500, ErrorMessage = "Máximo de {1} caracteres.")]
        public string InformacoesAdicionais { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Tipo de Endereço é obrigatório.")]
        [Display(Name = "Tipo de Endereço")]
        public Endereco.TipoDeEnderecoEnum TipoDeEndereco { get; set; } = Endereco.TipoDeEnderecoEnum.Principal;

        [DisplayName(displayName: "Data Inclusão")]
        public DateTime? DataInclusao { get; set; }

        [DisplayName(displayName: "Data Última Modificação")]
        public DateTime? DataUltimaModificacao { get; set; }

        [DisplayName(displayName: "Usuário Inclusão")]
        public string? UsuarioInclusao { get; set; }

        [DisplayName(displayName: "Usuário Última Modificação")]
        public string? UsuarioUltimaModificacao { get; set; }
    }
}