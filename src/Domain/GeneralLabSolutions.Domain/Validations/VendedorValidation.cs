using FluentValidation;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Validations
{
    public class VendedorValidation : AbstractValidator<Vendedor>
    {
        public VendedorValidation()
        {
            RuleFor(v => v.Nome)
                .NotEmpty().WithMessage("O campo Nome precisa ser fornecido")
                .Length(2, 150).WithMessage("O campo Nome precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(v => v.Documento)
                .NotEmpty().WithMessage("O campo Documento precisa ser fornecido")
                .Length(11, 14).WithMessage("O campo Documento precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("O campo Email precisa ser fornecido")
                .EmailAddress().WithMessage("O campo Email está em formato inválido");

            RuleFor(v => v.TipoDePessoa)
                .IsInEnum().WithMessage("O campo Tipo de Pessoa precisa ser válido");
        }
    }
}