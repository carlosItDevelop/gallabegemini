namespace VelzonModerna.ViewModels.ViewComponents
{
    public class AggregateFormViewModel
    {
        public string ParentEntityType { get; set; } // Cliente, Fornecedor...
        public Guid ParentEntityId { get; set; }
        public Guid ParentPessoaId { get; set; }
        public string AggregateType { get; set; } // DadosBancarios, Telefone...
        public string FieldsPartialViewPath { get; set; }
    }
}
