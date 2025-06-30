using FluentValidation;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Extensions.Extending;

namespace GeneralLabSolutions.Domain.Validations
{
    public class FornecedorValidation : AbstractValidator<Fornecedor>
    {
        public FornecedorValidation()
        {
            RuleFor(f => f.Nome)
                .NotEmpty().WithMessage("O campo {PropertyName} precisa ser fornecido")
                .Length(2, 200).WithMessage("O campo {PropertyName} precisa ter entre {MinLength} e {MaxLength} caracteres");

            RuleFor(f => f.Email)
                .NotEmpty().WithMessage("O campo E-mail é obrigatório.")
                .EmailAddress().WithMessage("O E-mail informado não é válido.");

            When(f => f.TipoDePessoa == TipoDePessoa.Fisica, () =>
            {
                RuleFor(f => f.Documento.ApenasNumeros().Length).Equal(CpfValidacao.TamanhoCpf)
                    .WithMessage("O CPF precisa ter 11 dígitos.");
                RuleFor(f => CpfValidacao.Validar(f.Documento)).Equal(true)
                    .WithMessage("O CPF fornecido é inválido.");
            });

            When(f => f.TipoDePessoa == TipoDePessoa.Juridica, () =>
            {
                RuleFor(f => f.Documento.ApenasNumeros().Length).Equal(CnpjValidacao.TamanhoCnpj)
                    .WithMessage("O CNPJ precisa ter 14 dígitos.");
                RuleFor(f => CnpjValidacao.Validar(f.Documento)).Equal(true)
                    .WithMessage("O CNPJ fornecido é inválido.");
            });
        }
    }
}
