//// Em VelzonModerna.Controllers.PedidoDeCompraController.cs

//using AutoMapper; // Adicionar
//using GeneralLabSolutions.Domain.Entities; // Para PedidoDeCompra, Fornecedor, etc.
//using GeneralLabSolutions.Domain.Enums; // Para StatusPedidoCompra
//using GeneralLabSolutions.Domain.Interfaces; // Para os repositórios e serviços de domínio
//using GeneralLabSolutions.Domain.Notifications;
//using GeneralLabSolutions.SharedKernel.Enums;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList
//using System.Collections.Generic; // Para List<>
//using System.Linq; // Para LINQ
//using System.Threading.Tasks; // Para Task<>
//using VelzonModerna.Controllers.Base;
//using VelzonModerna.ViewModels.PedidoDeCompra; // Nossas novas ViewModels

//namespace VelzonModerna.Controllers
//{
//    // [Authorize] // Descomentar quando a autenticação estiver configurada
//    public class PedidoDeCompraController : BaseMvcController
//    {
//        private readonly IMapper _mapper;
//        private readonly IPedidoDeCompraRepository _pedidoDeCompraRepository; // Repositório específico
//        private readonly IGenericRepository<Fornecedor, Guid> _fornecedorRepository; // Para dropdown
//        private readonly IGenericRepository<ApplicationUser, Guid> _userRepository; // Para dropdown de responsáveis (ajuste o tipo ApplicationUser conforme sua entidade de usuário)
//        private readonly IGenericRepository<Produto, Guid> _produtoRepository; // Para dropdown de produtos
//        private readonly IPedidoDeCompraDomainService _pedidoDeCompraService; // Serviço de domínio/aplicação

//        public PedidoDeCompraController(
//            INotificador notificador,
//            IMapper mapper,
//            IPedidoDeCompraRepository pedidoDeCompraRepository,
//            IGenericRepository<Fornecedor, Guid> fornecedorRepository,
//            IGenericRepository<ApplicationUser, Guid> userRepository, // Ajuste aqui
//            IGenericRepository<Produto, Guid> produtoRepository,
//            IPedidoDeCompraDomainService pedidoDeCompraService) : base(notificador)
//        {
//            _mapper = mapper;
//            _pedidoDeCompraRepository = pedidoDeCompraRepository;
//            _fornecedorRepository = fornecedorRepository;
//            _userRepository = userRepository;
//            _produtoRepository = produtoRepository;
//            _pedidoDeCompraService = pedidoDeCompraService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Index(
//            Guid? fornecedorIdFiltro,
//            StatusPedidoCompra? statusFiltro, // Usar o enum diretamente
//            Guid? responsavelIdFiltro,
//            DateTime? dataFiltro)
//        {
//            var viewModel = new PedidoCompraIndexViewModel();

//            // 1. Popular dados para os Filtros
//            await PopularFiltrosViewModel(viewModel.Filtros, fornecedorIdFiltro, statusFiltro, responsavelIdFiltro, dataFiltro);

//            // 2. Obter e Mapear a Lista de Pedidos de Compra
//            // O repositório deve ter um método que aceite os filtros
//            var pedidosDeCompraEntidades = await _pedidoDeCompraRepository.ObterTodosFiltradoAsync(
//                fornecedorIdFiltro,
//                statusFiltro,
//                responsavelIdFiltro,
//                dataFiltro /*, pagina, tamanhoPagina - para paginação no servidor */
//            );

//            viewModel.ListaDePedidos = _mapper.Map<IEnumerable<PedidoDeCompraListViewModel>>(pedidosDeCompraEntidades);
//            // No AutoMapperConfig, você precisará de um mapeamento de PedidoDeCompra para PedidoDeCompraListViewModel
//            // que popule NomeFornecedor, NomeResponsavel, StatusCssClass, e as flags PodeEditar, etc.

//            // 3. Popular dados para o Formulário de Novo Pedido (Offcanvas)
//            await PopularFormularioNovoPedidoViewModel(viewModel.FormularioNovoPedido);

//            return View(viewModel);
//        }

//        private async Task PopularFiltrosViewModel(
//            FiltrosPedidoCompraViewModel filtrosViewModel,
//            Guid? fornecedorIdSel,
//            StatusPedidoCompra? statusSel,
//            Guid? responsavelIdSel,
//            DateTime? dataSel)
//        {
//            // Atribuir valores selecionados para manter o estado dos filtros na view
//            filtrosViewModel.FornecedorIdSelecionado = fornecedorIdSel;
//            filtrosViewModel.StatusSelecionado = statusSel?.ToString(); // Convertendo enum para string para o select
//            filtrosViewModel.ResponsavelIdSelecionado = responsavelIdSel;
//            filtrosViewModel.PeriodoDataSelecionada = dataSel;

//            var fornecedores = await _fornecedorRepository.GetAllAsync();
//            filtrosViewModel.FornecedoresOptions = new SelectList(fornecedores, "Id", "Nome", fornecedorIdSel);

//            // Para Status, podemos construir a SelectList a partir do Enum
//            var statusValues = Enum.GetValues(typeof(StatusPedidoCompra))
//                                   .Cast<StatusPedidoCompra>()
//                                   .Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() }) // Ajustar Text para [Description] se tiver
//                                   .ToList();
//            filtrosViewModel.StatusOptions = new SelectList(statusValues, "Value", "Text", statusSel?.ToString());

//            var responsaveis = await _userRepository.GetAllAsync(); // Assumindo que usuários são os responsáveis
//            filtrosViewModel.ResponsaveisOptions = new SelectList(responsaveis, "Id", "UserName", responsavelIdSel); // Ajuste "UserName" para a propriedade correta
//        }

//        private async Task PopularFormularioNovoPedidoViewModel(CriarPedidoDeCompraViewModel formViewModel)
//        {
//            var fornecedores = await _fornecedorRepository.GetAllAsync();
//            formViewModel.FornecedoresOptions = new SelectList(fornecedores, "Id", "Nome");

//            var responsaveis = await _userRepository.GetAllAsync();
//            formViewModel.ResponsaveisOptions = new SelectList(responsaveis, "Id", "UserName");

//            var produtos = await _produtoRepository.GetAllAsync();
//            formViewModel.ProdutosOptions = new SelectList(produtos, "Id", "Descricao"); // Para o select de produto nos itens

//            // Popular CondicoesPagamentoOptions se você tiver uma entidade/enum para isso
//            // formViewModel.CondicoesPagamentoOptions = ...;
//        }

//        // TODO: Implementar as Actions para:
//        // - GET/POST para Create (recebendo CriarPedidoDeCompraViewModel)
//        // - GET para Details (retornando JSON para popular o modal _ModalDetalhesPedidoCompraPartial)
//        // - POST para Editar Item (recebendo ItemPedidoDeCompraInputViewModel e Id do PC)
//        // - POST para Registrar Recebimento
//        // - POST para Cancelar Pedido
//        // - GET para as partials de lista de itens dentro do modal, etc.
//    }
//}