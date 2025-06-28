using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Contato : EntityBase
    {
        // EF Core
        public Contato() { }

        public Contato(string nome, 
            string email, 
            string telefone, 
            TipoDeContato tipoDeContato, 
            Guid pessoaId)
        {
            Nome = nome;
            Email = email;
            Telefone = telefone;
            TipoDeContato = tipoDeContato;
            PessoaId = pessoaId;
        }

        public Guid PessoaId { get; private set; }
        public virtual Pessoa Pessoa { get; private set; }

        public string Nome { get; private set; }
        public string Email { get; private set; }
        public string Telefone { get; private set; }
        public string EmailAlternativo { get; set; } = string.Empty;
        public string TelefoneAlternativo { get; set; } = string.Empty;
        public string Observacao { get; set; } = string.Empty;



        public TipoDeContato TipoDeContato { get; set; } = TipoDeContato.Comercial;

        #region: Métodos Ad Hoc
        public void DefineTipoDeContato(TipoDeContato tipoDeContato) => TipoDeContato = tipoDeContato;

        public void DefinePessoa(Guid pessoaId) => PessoaId = pessoaId; // Pode ser útil


        public void AtualizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome não pode ser vazio ou nulo.", nameof(nome));

            Nome = nome;
        }

        public void AtualizarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("O email não pode ser vazio ou nulo.", nameof(email));

            Email = email;
        }

        public void AtualizarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                throw new ArgumentException("O telefone não pode ser vazio ou nulo.", nameof(telefone));

            Telefone = telefone;
        }

        public void AtualizarEmailAlternativo(string emailAlternativo)
        {
            EmailAlternativo = emailAlternativo ?? string.Empty;
        }

        public void AtualizarTelefoneAlternativo(string telefoneAlternativo)
        {
            TelefoneAlternativo = telefoneAlternativo ?? string.Empty;
        }

        public void AtualizarObservacao(string observacao)
        {
            Observacao = observacao ?? string.Empty;
        }

        #endregion

    }
}