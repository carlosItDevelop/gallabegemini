using FluentValidation;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Validations
{
    public class VendedorValidation : AbstractValidator<Vendedor>
    {
        public VendedorValidation()
        {
            RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser preenchido.")
            .NotNull().WithMessage("O campo {PrpertyName} não pode ser nulo.")
            .Length(2, 200).WithMessage("O campo '{PropertyName}' precisa ter entre {MinLength} e {MaxLength} caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser preenchido.")
                .NotNull().WithMessage("O campo {PrpertyName} não pode ser nulo.")
                .Length(6, 200).WithMessage("O campo '{PropertyName}' precisa ter entre {MinLength} e {MaxLength} caracteres.")
                .EmailAddress().WithMessage("O campo '{PropertyName}' está inválido!");

            RuleFor(x => x.StatusDoVendedor)
            .NotEmpty().WithMessage("O campo {PropertyName} precisa ser selecionado.")
            .NotNull().WithMessage("O campo {PrpertyName} não pode ser nulo.");

            // Todo Regx

            When(f => f.TipoDePessoa == TipoDePessoa.Fisica, () =>
            {
                RuleFor(f => f.Documento.Length).Equal(CpfValidacao.TamanhoCpf)
                    .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");
                RuleFor(f => CpfValidacao.Validar(f.Documento)).Equal(true)
                    .WithMessage("O documento fornecido é inválido.");
            });

            When(f => f.TipoDePessoa == TipoDePessoa.Juridica, () =>
            {
                RuleFor(f => f.Documento.Length).Equal(CnpjValidacao.TamanhoCnpj)
                    .WithMessage("O campo Documento precisa ter {ComparisonValue} caracteres e foi fornecido {PropertyValue}.");
                RuleFor(f => CnpjValidacao.Validar(f.Documento)).Equal(true)
                    .WithMessage("O documento fornecido é inválido.");
            });
        }
    }
}
