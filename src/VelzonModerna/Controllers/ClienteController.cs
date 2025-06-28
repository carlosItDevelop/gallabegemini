using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneralLabSolutions.Domain.Enums;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using GeneralLabSolutions.WebApiCore.Identidade;
using Microsoft.AspNetCore.Identity;
using GeneralLabSolutions.WebApiCore.Usuario;


namespace VelzonModerna.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClienteController : BaseMvcController
    {
        private readonly IAspNetUser _aspnetUser;

        private readonly IMapper _mapper;
        private readonly IClienteRepository _clienteRepository;
        private readonly IClienteDomainService _clienteDomainService;

        #region: Construtor (Existente)
        public ClienteController(
            INotificador notificador,
            IMapper mapper,
            IClienteRepository clienteRepository,
            IClienteDomainService clienteDomainService,
            IAspNetUser aspNetUser)
            : base(notificador)
        {
            _mapper = mapper;
            _clienteRepository = clienteRepository;
            _clienteDomainService = clienteDomainService;
            _aspnetUser = aspNetUser;
        }
        #endregion

        #region: Métodos Cliente (Index, Create, Edit, Delete - Ajustar Details/Edit GET)

        //[Authorize(Roles = "Admin")]
        //[ClaimsAuthorize("Cliente", "Visualizar", "Admin")] // Role alternativa não especificada
        public async Task<IActionResult> Index()
        {
            var clientes = await _clienteRepository.GetAllAsync();
            var listClienteViewModel = _mapper.Map<IEnumerable<ClienteViewModel>>(clientes);
            return View(listClienteViewModel);
        }

        [HttpGet]
        //[ClaimsAuthorize("role", "Admin")]
        //[Authorize(Roles = "Admin")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> Details(Guid id)
        {

            var usuarioLogado = _aspnetUser.EstaAutenticado();
            if (usuarioLogado)
            {
                var isAdmin = _aspnetUser.PossuiRole("Admin");
                var isSuperAdmin = _aspnetUser.PossuiRole("SuperAdmin");
                var userId = _aspnetUser.ObterUserId();
                var userEmail = _aspnetUser.ObterUserEmail();
                var userToken = _aspnetUser.ObterUserToken();
                var userRefreshToken = _aspnetUser.ObterUserRefreshToken();

                var nomeCompleto = _aspnetUser.ObterNomeCompleto();
                var apelido = _aspnetUser.ObterApelido();
                var dataNascimentoUtc = _aspnetUser.ObterDataNascimento();


                Console.WriteLine($"\n\t==============================================");


                if (dataNascimentoUtc.HasValue)
                {
                    var dataNascimentoLocal = dataNascimentoUtc.Value.ToLocalTime();
                    Console.WriteLine($"\n\tData Nascimento(UTC): {dataNascimentoUtc.Value}\n\tData Nascimento (Local): {dataNascimentoLocal}");
                    // Para a ViewModel, passar a dataNascimentoLocal
                } else
                {
                    Console.WriteLine("Data Nasc: Não disponível");
                }

                var imgPath = _aspnetUser.ObterImgProfilePath();

                Console.WriteLine($"\n\n\tUserId: {userId}\n\tEhAdmin: {isAdmin}\n\tEhSuperAdmin: {isSuperAdmin}\n\tAccessToken: {userToken}\n\tRefressToken: {userRefreshToken}\n\tEmail: {userEmail}");

                Console.WriteLine($"\n\t==============================================");

                Console.WriteLine($"\n\tNome Completo: {nomeCompleto}\n\tApelido: {apelido}\n\tImg: {imgPath}");

            }


            // *** ATUALIZAR: Usar ObterClienteCompleto ou carregar Telefones também ***
            // var cliente = await _clienteRepository.ObterClienteComDadosBancarios(id); // Linha antiga
            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega DadosBancarios e Telefones (e futuros)

            if (cliente == null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteViewModel);
        }

        public IActionResult Create() => View(new ClienteViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel clienteViewModel)
        {
            if (!ModelState.IsValid)
                return View(clienteViewModel);
            var cliente = _mapper.Map<Cliente>(clienteViewModel);
            await _clienteDomainService.AddClienteAsync(cliente);
            if (!OperacaoValida())
                return View(clienteViewModel);
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao salvar os dados do cliente.");
                return View(clienteViewModel);
            }
            TempData ["Success"] = "Cliente Adicionado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            // *** ATUALIZAR: Usar ObterClienteCompleto ou carregar Telefones também ***
            // var cliente = await _clienteRepository.ObterClienteComDadosBancarios(id); // Linha antiga
            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega DadosBancarios e Telefones (e futuros)

            if (cliente == null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);

            return View(clienteViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ClienteViewModel clienteViewModel)
        {
            if (id != clienteViewModel.Id)
                return NotFound();
            if (!ModelState.IsValid)
                return View(clienteViewModel);

            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega com relações
            if (cliente is null)
                return NotFound();

            _mapper.Map(clienteViewModel, cliente); // Mapeia ClienteViewModel para a entidade Cliente rastreada

            await _clienteDomainService.UpdateClienteAsync(cliente);

            if (!OperacaoValida())
                return View(clienteViewModel);
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao atualizar os dados do cliente.");
                var clienteAtualizado = await _clienteRepository.ObterClienteCompleto(id);
                var viewModelAtualizado = _mapper.Map<ClienteViewModel>(clienteAtualizado);
                return View(viewModelAtualizado);
            }
            TempData ["Success"] = "Cliente Atualizado com Sucesso!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente == null)
                return NotFound();
            await _clienteDomainService.DeleteClienteAsync(cliente);
            if (!OperacaoValida())
            {
                var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
                return View(clienteViewModel); // Retorna para a view Delete com erros
            }
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                NotificarErro("Ocorreu um erro ao excluir o cliente. Verifique dependências.");
                var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
                return View("Delete", clienteViewModel);
            }
            TempData ["Success"] = "Cliente excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region: Actions AJAX para Dados Bancários (Existente)
        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosListPartial(Guid clienteId)
        { /* ... código existente ... */
            var cliente = await _clienteRepository.ObterClienteComDadosBancarios(clienteId);
            if (cliente == null || cliente.Pessoa == null)
                return PartialView("PartialViews/_DadosBancariosListClientePartial", new List<DadosBancariosViewModel>());
            var viewModels = _mapper.Map<List<DadosBancariosViewModel>>(cliente.Pessoa.DadosBancarios);
            return PartialView("PartialViews/_DadosBancariosListClientePartial", viewModels);
        }

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosFormData(Guid? dadosBancariosId, Guid clienteId)
        { /* ... código existente ... */
            if (dadosBancariosId == null || dadosBancariosId == Guid.Empty)
            {
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound();
                var newViewModel = new DadosBancariosViewModel { PessoaId = clienteId }; // Aqui era clienteId -> PessoaId do Cliente
                return Json(newViewModel);
            } else
            {
                var dadosBancarios = await _clienteRepository.ObterDadosBancariosPorId(dadosBancariosId.Value);
                if (dadosBancarios == null)
                    return NotFound();
                var viewModel = _mapper.Map<DadosBancariosViewModel>(dadosBancarios);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarDadosBancarios(Guid clienteId, DadosBancariosViewModel viewModel)
        { /* ... código existente ... */
            if (!ModelState.IsValid)
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            if (clienteId == Guid.Empty)
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            bool isNew = viewModel.Id == Guid.Empty;
            if (isNew)
                await _clienteDomainService.AdicionarDadosBancariosAsync(clienteId, viewModel.Banco, viewModel.Agencia, viewModel.Conta, viewModel.TipoDeContaBancaria);
            else
                await _clienteDomainService.AtualizarDadosBancariosAsync(clienteId, viewModel.Id, viewModel.Banco, viewModel.Agencia, viewModel.Conta, viewModel.TipoDeContaBancaria);
            if (!OperacaoValida())
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            try
            { if (!await _clienteRepository.UnitOfWork.CommitAsync()) return Json(new { success = false, errors = new List<string> { "Commit retornou falso." } }); } catch (DbUpdateException dbEx) { Console.WriteLine($"DbUpdateException: {dbEx.Message} Inner: {dbEx.InnerException?.Message}"); return Json(new { success = false, errors = new List<string> { "Erro no banco." } }); } catch (Exception ex) { Console.WriteLine($"Exception: {ex.Message}"); return Json(new { success = false, errors = new List<string> { "Erro inesperado." } }); }
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirDadosBancarios(Guid clienteId, Guid dadosBancariosId)
        { /* ... código existente ... */
            if (clienteId == Guid.Empty || dadosBancariosId == Guid.Empty)
                return Json(new { success = false, errors = new List<string> { "IDs inválidos." } });
            await _clienteDomainService.RemoverDadosBancariosAsync(clienteId, dadosBancariosId);
            if (!OperacaoValida())
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir." } });
            return Json(new { success = true });
        }
        #endregion

        #region Actions AJAX para Telefones

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de telefones para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTelefonesListPartial(Guid clienteId)
        {
            var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId); // Ou ObterClienteCompleto
            if (cliente == null || cliente.Pessoa == null)
            {
                return PartialView("PartialViews/_TelefonesListClientePartial", new List<TelefoneViewModel>());
            }
            var viewModels = _mapper.Map<List<TelefoneViewModel>>(cliente.Pessoa.Telefones);
            return PartialView("PartialViews/_TelefonesListClientePartial", viewModels);
        }

        /// <summary>
        /// Retorna os dados de um telefone específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTelefoneFormData(Guid? telefoneId, Guid clienteId) // clienteId para caso de criação
        {
            if (telefoneId == null || telefoneId == Guid.Empty)
            {
                // ...
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar telefone.");

                // Para Telefone, precisamos do PessoaId do Cliente para preencher o formulário
                // *** CORREÇÃO APLICADA AQUI ***
                var clienteParaPessoaId = await _clienteRepository.GetByIdAsync(clienteId);
                // ******************************

                if (clienteParaPessoaId == null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId.");

                var newViewModel = new TelefoneViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
                // ...
            } else
            {
                // Modo Edição: Buscar telefone pelo seu ID.
                // Idealmente, ter um _telefoneRepository.GetByIdAsync(telefoneId.Value)
                // Por enquanto, vamos buscar através do cliente para garantir que pertence a ele.
                var cliente = await _clienteRepository.ObterClienteComTelefones(clienteId);
                var telefone = cliente?.Pessoa?.Telefones.FirstOrDefault(t => t.Id == telefoneId.Value);

                if (telefone == null)
                    return NotFound("Telefone não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<TelefoneViewModel>(telefone);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (Adiciona ou Atualiza) o telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTelefone(Guid clienteId, TelefoneViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (clienteId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarTelefoneAsync(
                    clienteId, // ID do Cliente
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone
                );
            } else
            {
                await _clienteDomainService.AtualizarTelefoneAsync(
                    clienteId,    // ID do Cliente
                    viewModel.Id, // ID do Telefone a ser atualizado
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone
                );
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            try // Bloco try-catch para CommitAsync
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new { success = false, errors = new List<string> { "Erro ao salvar o telefone no banco de dados (Commit retornou falso)." } });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException ao salvar telefone: {dbEx.Message} Inner: {dbEx.InnerException?.Message}");
                return Json(new { success = false, errors = new List<string> { "Erro no banco de dados ao salvar o telefone." } });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar telefone: {ex.Message}");
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro inesperado ao salvar o telefone." } });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Exclui um telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTelefone(Guid clienteId, Guid telefoneId)
        {
            if (clienteId == Guid.Empty || telefoneId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _clienteDomainService.RemoverTelefoneAsync(clienteId, telefoneId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o telefone no banco de dados." } });
            }

            return Json(new { success = true });
        }
        #endregion

        #region Actions AJAX para Contatos

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de contatos para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContatosListPartial(Guid clienteId)
        {
            var cliente = await _clienteRepository.ObterClienteComContatos(clienteId); // Ou ObterClienteCompleto
            if (cliente == null || cliente.Pessoa == null)
            {
                // Retorna uma partial vazia ou com mensagem de erro
                return PartialView("PartialViews/_ContatosListClientePartial", new List<ContatoViewModel>());
            }
            var viewModels = _mapper.Map<List<ContatoViewModel>>(cliente.Pessoa.Contatos);
            return PartialView("PartialViews/_ContatosListClientePartial", viewModels);
        }

        /// <summary>
        /// Retorna os dados de um contato específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContatoFormData(Guid? contatoId, Guid clienteId)
        {
            if (contatoId == null || contatoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar contato.");
                var clienteParaPessoaId = await _clienteRepository.GetByIdAsync(clienteId);
                if (clienteParaPessoaId == null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o contato.");

                var newViewModel = new ContatoViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
            } else
            {
                // Modo Edição: Buscar contato pelo seu ID.
                var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);
                var contato = cliente?.Pessoa?.Contatos.FirstOrDefault(c => c.Id == contatoId.Value);

                if (contato == null)
                    return NotFound("Contato não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<ContatoViewModel>(contato);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (Adiciona ou Atualiza) o contato de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarContato(Guid clienteId, ContatoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (clienteId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            }

            // Poderia validar se viewModel.PessoaId corresponde ao PessoaId do Cliente com clienteId,
            // mas o serviço de domínio fará a associação correta com base no clienteId buscado.
            // A entidade Cliente.AdicionarContato usará o Cliente.PessoaId correto.

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarContatoAsync(
                    clienteId, // ID do Cliente
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao
                );
            } else
            {
                await _clienteDomainService.AtualizarContatoAsync(
                    clienteId,    // ID do Cliente
                    viewModel.Id, // ID do Contato a ser atualizado
                    viewModel.Nome,
                    viewModel.Email,
                    viewModel.Telefone,
                    viewModel.TipoDeContato,
                    viewModel.EmailAlternativo,
                    viewModel.TelefoneAlternativo,
                    viewModel.Observacao
                );
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new { success = false, errors = new List<string> { "Erro ao salvar o contato no banco de dados (Commit retornou falso)." } });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException ao salvar contato: {dbEx.ToString()}"); // Log completo
                return Json(new { success = false, errors = new List<string> { "Erro no banco de dados ao salvar o contato." } });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar contato: {ex.ToString()}"); // Log completo
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro inesperado ao salvar o contato." } });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Exclui um contato de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirContato(Guid clienteId, Guid contatoId)
        {
            if (clienteId == Guid.Empty || contatoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _clienteDomainService.RemoverContatoAsync(clienteId, contatoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o contato no banco de dados." } });
            }

            return Json(new { success = true });
        }
        #endregion

        #region Actions AJAX para Endereços

        /// <summary>
        /// Retorna a Partial View com a lista atualizada de endereços para um cliente.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEnderecosListPartial(Guid clienteId)
        {
            var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId); // Ou ObterClienteCompleto
            if (cliente == null || cliente.Pessoa == null)
            {
                return PartialView("PartialViews/_EnderecosListClientePartial", new List<EnderecoViewModel>());
            }
            var viewModels = _mapper.Map<List<EnderecoViewModel>>(cliente.Pessoa.Enderecos);
            return PartialView("PartialViews/_EnderecosListClientePartial", viewModels);
        }

        /// <summary>
        /// Retorna os dados de um endereço específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEnderecoFormData(Guid? enderecoId, Guid clienteId)
        {
            if (enderecoId == null || enderecoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar endereço.");
                var clienteParaPessoaId = await _clienteRepository.GetByIdAsync(clienteId);
                if (clienteParaPessoaId == null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o endereço.");

                var newViewModel = new EnderecoViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
            } else
            {
                // Modo Edição
                var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);
                var endereco = cliente?.Pessoa?.Enderecos.FirstOrDefault(e => e.Id == enderecoId.Value);

                if (endereco == null)
                    return NotFound("Endereço não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<EnderecoViewModel>(endereco);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (Adiciona ou Atualiza) o endereço de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarEndereco(Guid clienteId, EnderecoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (clienteId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "O ID do Cliente não foi fornecido." } });
            }

            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarEnderecoAsync(
                    clienteId,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais
                );
            } else
            {
                await _clienteDomainService.AtualizarEnderecoAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.PaisCodigoIso,
                    viewModel.LinhaEndereco1,
                    viewModel.Cidade,
                    viewModel.CodigoPostal,
                    viewModel.TipoDeEndereco,
                    viewModel.LinhaEndereco2,
                    viewModel.EstadoOuProvincia,
                    viewModel.InformacoesAdicionais
                );
            }

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new { success = false, errors = new List<string> { "Erro ao salvar o endereço (Commit retornou falso)." } });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"DbUpdateException ao salvar endereço: {dbEx.ToString()}");
                return Json(new { success = false, errors = new List<string> { "Erro no banco de dados ao salvar o endereço." } });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar endereço: {ex.ToString()}");
                return Json(new { success = false, errors = new List<string> { "Ocorreu um erro inesperado ao salvar o endereço." } });
            }

            return Json(new { success = true });
        }

        /// <summary>
        /// Exclui um endereço de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEndereco(Guid clienteId, Guid enderecoId)
        {
            if (clienteId == Guid.Empty || enderecoId == Guid.Empty)
            {
                return Json(new { success = false, errors = new List<string> { "IDs inválidos fornecidos para exclusão." } });
            }

            await _clienteDomainService.RemoverEnderecoAsync(clienteId, enderecoId);

            if (!OperacaoValida())
            {
                return Json(new { success = false, errors = ObterNotificacoes().Select(n => n.Mensagem).ToList() });
            }

            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new { success = false, errors = new List<string> { "Erro ao excluir o endereço no banco de dados." } });
            }

            return Json(new { success = true });
        }
        #endregion

        #region Métodos Privados Auxiliares (Existente)
        private async Task<bool> ClienteExists(Guid id) => await _clienteRepository.TemCliente(id);
        private List<Notificacao> ObterNotificacoes() => HttpContext.RequestServices.GetRequiredService<INotificador>().ObterNotificacoes();
        #endregion
    }
}