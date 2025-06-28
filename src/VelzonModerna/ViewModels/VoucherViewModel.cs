using System;
using System.ComponentModel.DataAnnotations;
using GeneralLabSolutions.Domain.Enums;

namespace VelzonModerna.ViewModels
{
    public class VoucherViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [StringLength(25, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres.")]
        public string? Codigo { get; set; }

        [Display(Name = "Percentual de Desconto")]
        [DataType(DataType.Currency, ErrorMessage = "O campo {0} está inválido.")]
        public decimal? Percentual { get; set; }

        [Display(Name = "Valor do Desconto")]
        [DataType(DataType.Currency, ErrorMessage = "O campo {0} está inválido.")]
        public decimal? ValorDesconto { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Tipo de Desconto")]
        public TipoDescontoVoucher TipoDescontoVoucher { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Data de Criação")]
        [DataType(DataType.Date, ErrorMessage = "O campo {0} está inválido.")]
        public DateTime DataCriacao { get; set; }

        [Display(Name = "Data de Utilização")]
        [DataType(DataType.Date, ErrorMessage = "O campo {0} está inválido.")]
        public DateTime? DataUtilizacao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Data de Validade")]
        [DataType(DataType.Date, ErrorMessage = "O campo {0} está inválido.")]
        public DateTime DataValidade { get; set; }

        [Display(Name = "Ativo")]
        public bool Ativo { get; set; }

        [Display(Name = "Utilizado")]
        public bool Utilizado { get; set; }
    }
}