using GeneralLabSolutions.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VelzonModerna.ViewModels
{
    public class ContaViewModel
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Instituicao { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Documento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Date)]
        [DisplayName("Data de Vencimento")]
        public DateTime DataVencimento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DisplayName("Tipo de Conta")]
        public TipoDeConta TipoDeConta { get; set; }

        [DisplayName("Está Paga?")]
        public bool EstaPaga { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Data de Pagamento")]
        public DateTime? DataPagamento { get; set; }

        [StringLength(500, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string? Observacao { get; set; }

        public bool Inativa { get; set; }
    }
}