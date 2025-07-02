namespace VelzonModerna.ViewModels.ViewComponents
{
    public class AggregateListViewModel
    {
        public string ParentEntityType { get; set; } // "Cliente", "Fornecedor", etc.
        public Guid ParentEntityId { get; set; }
        public Guid ParentPessoaId { get; set; }
        public string AggregateType { get; set; } // "DadosBancarios", "Telefone", etc.
        public IEnumerable<object> Items { get; set; }
    }

}
