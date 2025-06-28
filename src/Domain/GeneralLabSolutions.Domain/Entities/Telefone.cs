using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Telefone : EntityBase
    {
        // Construtor atualizado (exige PessoaId)
        public Telefone(string ddd, string numero, TipoDeTelefone tipoDeTelefone, Guid pessoaId)
        {
            DDD = ddd;
            Numero = numero;
            TipoDeTelefone = tipoDeTelefone;
            PessoaId = pessoaId; // Define FK
        }
        // EF Construtor vazio permanece
        public Telefone() { }

        public Guid PessoaId { get; private set; }
        public virtual Pessoa Pessoa { get; private set; }

        // Código de área do telefone
        public string DDD { get; set; }
        // Número do telefone
        public string Numero { get; set; }
        // Tipo de telefone (e.g., Celular, Residencial)
        public TipoDeTelefone TipoDeTelefone { get; set; }

        public void SetDDD(string ddd) => DDD = ddd;
        public void SetNumero(string numero) => Numero = numero;
        public void SetTipoDeTelefone(TipoDeTelefone tipo) => TipoDeTelefone = tipo;
        public void DefinePessoa(Guid pessoaId) => PessoaId = pessoaId; // Pode ser útil
    }

}
