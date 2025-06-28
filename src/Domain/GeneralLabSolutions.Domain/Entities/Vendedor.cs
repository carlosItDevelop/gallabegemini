using GeneralLabSolutions.Domain.DomainObjects;
using GeneralLabSolutions.Domain.Entities.Audit;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Vendedor : EntityAudit, IAggregateRoot
    {
        // EF
        public Vendedor() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="documento"></param>
        /// <param name="tipoDePessoa"></param>
        /// <param name="email"></param>
        public Vendedor(string nome, string documento, TipoDePessoa tipoDePessoa, string email)
        {
            Pessoa = new Pessoa();
            PessoaId = Pessoa.Id;
            Email = email;
            Nome = nome;
            Documento = documento;
            TipoDePessoa = tipoDePessoa;

        }

        public Guid PessoaId { get; private set; }
         
        public Pessoa Pessoa { get; private set; }

        public string Nome { get; private set; }
        public string Documento { get; private set; }
        public TipoDePessoa TipoDePessoa { get; private set; }

        // Email do vendedor
        public string Email { get; set; }
        // Status do vendedor (e.g., Contratado, Freelancer)
        public StatusDoVendedor StatusDoVendedor { get; set; } = StatusDoVendedor.Contratado;
        // Coleção de pedidos realizados pelo vendedor
        public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();


        public void SetNome(string nome)
            => Nome = nome;

        // Define o tipo de pessoa
        public void SetTipoDePessoa(TipoDePessoa tipoDePessoa)
            => TipoDePessoa = tipoDePessoa;

        // Define o documento da pessoa
        public void SetDocumento(string documento)
            => Documento = documento;

    }
}
