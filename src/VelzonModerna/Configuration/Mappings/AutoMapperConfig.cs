using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Entities.CRM;
using VelzonModerna.ViewModels;
using VelzonModerna.ViewModels.CRM;

namespace VelzonModerna.Configuration.Mappings
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Cliente, ClienteViewModel>()
                .ForMember(dest => dest.PessoaId, opt => opt.MapFrom(src => src.PessoaId)) // Cliente -> ViewModel (OK)
                .ForMember(dest => dest.DadosBancarios, opt => opt.MapFrom(src => src.Pessoa.DadosBancarios))
                .ForMember(dest => dest.Telefones, opt => opt.MapFrom(src => src.Pessoa.Telefones))
                .ForMember(dest => dest.Contatos, opt => opt.MapFrom(src => src.Pessoa.Contatos))
                .ForMember(dest => dest.Enderecos, opt => opt.MapFrom(src => src.Pessoa.Enderecos))
                .ReverseMap() // ViewModel -> Cliente
                              // *** ADICIONAR ESTA LINHA PARA O MAPEAMENTO REVERSO ***
                    .ForMember(dest => dest.PessoaId, opt => opt.Ignore())
                    // Ignora PessoaId ao mapear ViewModel -> Cliente
                    // Ignorar Pessoa também, pois não queremos substituir a instância carregada
                    .ForMember(dest => dest.Pessoa, opt => opt.Ignore())
                    // Ignorar coleções também, elas são gerenciadas separadamente
                    .ForMember(dest => dest.Pedidos, opt => opt.Ignore());
            // As coleções em Pessoa (DadosBancarios, etc.) não existem no ClienteViewModel, então não precisam ser ignoradas explicitamente aqui.

            CreateMap<Contato, ContatoViewModel>().ReverseMap();
            CreateMap<Fornecedor, FornecedorViewModel>().ReverseMap();
            CreateMap<CategoriaProduto, CategoriaProdutoViewModel>().ReverseMap();
            CreateMap<Vendedor, VendedorViewModel>().ReverseMap();
            CreateMap<Produto, ProdutoViewModel>().ReverseMap();
            CreateMap<Pedido, PedidoViewModel>().ReverseMap();

            CreateMap<Telefone, TelefoneViewModel>().ReverseMap(); // PessoaId será mapeado por convenção

            CreateMap<Conta, ContaViewModel>().ReverseMap();

            CreateMap<Endereco, EnderecoViewModel>()
                            // Mapeia o enum interno para o enum do ViewModel (se os nomes/valores forem iguais, pode ser por convenção)
                            .ForMember(dest => dest.TipoDeEndereco, opt => opt.MapFrom(src => (Endereco.TipoDeEnderecoEnum)src.TipoDeEndereco))
                            .ReverseMap()
                            .ForMember(dest => dest.TipoDeEndereco, opt => opt.MapFrom(src => (Endereco.TipoDeEnderecoEnum)src.TipoDeEndereco)); // Para mapeamento reverso

            CreateMap<DadosBancarios, DadosBancariosViewModel>().ReverseMap();

            // Kanban Mapper
            CreateMap<KanbanTask, KanbanTaskViewModel>()
                .ReverseMap()
                .ForMember(dest => dest.Participantes, opt => opt.Ignore());
            CreateMap<Participante, ParticipanteViewModel>().ReverseMap();

            // Para CRM Leads

            // --- ✅ INÍCIO DOS MAPEAMENTOS DO MÓDULO CRM ---
            CreateMap<Lead, LeadViewModel>().ReverseMap();
            CreateMap<CrmTask, CrmTaskViewModel>().ReverseMap();
            CreateMap<Activity, ActivityViewModel>().ReverseMap();
            CreateMap<LeadNote, LeadNoteViewModel>().ReverseMap();
            CreateMap<Log, LogViewModel>().ReverseMap();
            CreateMap<CrmTaskComment, CrmTaskCommentViewModel>()
            // ✅ Mapeia a propriedade 'DataInclusao' da entidade para 'CreatedAt' na ViewModel
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.DataInclusao));
            // O mapeamento reverso pode continuar simples, pois não precisamos mapear a data de volta
            CreateMap<CrmTaskCommentViewModel, CrmTaskComment>();
            CreateMap<CrmTaskAttachment, CrmTaskAttachmentViewModel>().ReverseMap();
            // --- FIM DOS MAPEAMENTOS DO MÓDULO CRM ---


        }
    }
}