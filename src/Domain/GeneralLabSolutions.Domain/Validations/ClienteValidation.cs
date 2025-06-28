using FluentValidation;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.Base; // Para CpfValidacao, CnpjValidacao
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Extensions.Extending; // Para .ApenasNumeros()
using System.Text.RegularExpressions;

namespace GeneralLabSolutions.Domain.Validations;

public class ClienteValidation : AbstractValidator<Cliente>
{
    private const int NomeMin = 2;
    private const int NomeMax = 200;
    private const int EmailMin = 6;
    private const int EmailMax = 254;
    private static readonly Regex ApenasNumerosRegex = new(@"^\d+$", RegexOptions.Compiled); // Renomeei para evitar conflito com o método
    // private static readonly Regex MascaraTelefone = new(@"^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$",
    //                                                 RegexOptions.Compiled);

    public ClienteValidation()
    {
        CascadeMode = CascadeMode.Stop;   // aborta na 1ª falha

        // ───── Nome ────────────────────────────────────────────────
        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Length(NomeMin, NomeMax);

        // ───── Email ───────────────────────────────────────────────
        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Length(EmailMin, EmailMax)
            .EmailAddress().WithMessage("O {PropertyName} informado é inválido.");

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

        // ───── Telefone Principal ─────────────────────────────────
        RuleFor(c => c.TelefonePrincipal)
            .NotEmpty().WithMessage("O campo {PropertyName} é obrigatório.")
            .Must(t => ApenasNumerosRegex.IsMatch(t.ApenasNumeros()) && // MODIFICADO (e usei ApenasNumerosRegex)
                       t.ApenasNumeros().Length is >= 10 and <= 11)     // MODIFICADO
            .WithMessage("O {PropertyName} deve ter 10 ou 11 dígitos (DDD + número).");

        // ou, se preferir aceitar máscara: .Matches(MascaraTelefone)…

        // ───── Campos opcionais com tamanho máximo ────────────────
        RuleFor(c => c.ContatoRepresentante).MaximumLength(50);
        RuleFor(c => c.InscricaoEstadual).MaximumLength(80);
        RuleFor(c => c.Observacao).MaximumLength(4000);

        // ───── Enums obrigatórios ─────────────────────────────────
        RuleFor(c => c.TipoDePessoa).IsInEnum();
        RuleFor(c => c.StatusDoCliente).IsInEnum();
        RuleFor(c => c.TipoDeCliente).IsInEnum();

        // Inclua se realmente existir na entidade Cliente
        // RuleFor(c => c.RegimeTributario).IsInEnum();

        // ───── RuleSets (Create x Update) ─────────────────────────
        RuleSet("create", () =>
        {
            RuleFor(c => c.StatusDoCliente)
                .NotEqual(StatusDoCliente.Inativo)
                .WithMessage("Cliente não pode ser criado como Inativo.");

            RuleFor(c => c.TipoDeCliente)
                .NotEqual(TipoDeCliente.Inadimplente)
                .WithMessage("Cliente não pode ser criado como Inadimplente.");
        });

        RuleSet("update", () =>
        {
            // nada específico por enquanto; mas serve para flexibilizar depois
        });
    }
}