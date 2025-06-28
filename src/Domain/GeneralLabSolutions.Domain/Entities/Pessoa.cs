using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Pessoa : EntityBase
    {
        /// <summary>
        /// Construtor vazio para uso pelo EF
        /// </summary>
        public Pessoa() { }


        // Já refatorado para Pessoa 1:N Telefone
        public virtual ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();

        // Já refatorado para Pessoa 1:N Contatos
        public virtual ICollection<Contato> Contatos { get; set; } = new List<Contato>();

        // Já refatorado para Pessoa 1:N DadosBancarios
        public virtual ICollection<DadosBancarios> DadosBancarios { get; set; } = new List<DadosBancarios>();

        // Já refatorado para Pessoa 1:N Enderecos
        public virtual ICollection<Endereco> Enderecos { get; set; } = new List<Endereco>();

    }
}
