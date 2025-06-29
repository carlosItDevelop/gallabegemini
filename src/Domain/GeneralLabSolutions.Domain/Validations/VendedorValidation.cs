using FluentValidation;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Extensions.Extending;

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

            // ───── Documento: CPF / CNPJ ───────────────────────────────
            RuleFor(c => c.Documento)
                .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.");

            When(c => c.TipoDePessoa == TipoDePessoa.Fisica, () =>
            {
                RuleFor(c => c.Documento.ApenasNumeros().Length) // MODIFICADO
                    .Equal(CpfValidacao.TamanhoCpf)
                    .WithMessage($"O CPF deve conter {CpfValidacao.TamanhoCpf} dígitos.");

                RuleFor(c => CpfValidacao.Validar(c.Documento)) // CpfValidacao.Validar internamente usa .ApenasNumeros()
                    .Equal(true).WithMessage("O CPF fornecido é inválido.");
            });

            When(c => c.TipoDePessoa == TipoDePessoa.Juridica, () =>
            {
                RuleFor(c => c.Documento.ApenasNumeros().Length) // MODIFICADO
                    .Equal(CnpjValidacao.TamanhoCnpj)
                    .WithMessage($"O CNPJ deve conter {CnpjValidacao.TamanhoCnpj} dígitos.");

                RuleFor(c => CnpjValidacao.Validar(c.Documento)) // CnpjValidacao.Validar internamente usa .ApenasNumeros()
                    .Equal(true).WithMessage("O CNPJ fornecido é inválido.");
            });

        }
    }
}