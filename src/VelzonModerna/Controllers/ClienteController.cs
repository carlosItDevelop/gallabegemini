﻿using AutoMapper;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VelzonModerna.Controllers.Base;
using VelzonModerna.ViewModels;


namespace VelzonModerna.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ClienteController : BaseMvcController
    {
        private readonly IMapper _mapper;
        private readonly IClienteRepository _clienteRepository;
        private readonly IClienteDomainService _clienteDomainService;

        #region: Construtor (Existente)
        public ClienteController(
            INotificador notificador,
            IMapper mapper,
            IClienteRepository clienteRepository,
            IClienteDomainService clienteDomainService)
            : base(notificador)
        {
            _mapper = mapper;
            _clienteRepository = clienteRepository;
            _clienteDomainService = clienteDomainService;
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

            // *** ATUALIZAR: Usar ObterClienteCompleto ou carregar Telefones também ***
            // var cliente = await _clienteRepository.ObterClienteComDadosBancarios(id); // Linha antiga
            var cliente = await _clienteRepository.ObterClienteCompleto(id); // Carrega DadosBancarios e Telefones (e futuros)

            if (cliente is null)
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
            if (cliente is null)
                return NotFound();
            var clienteViewModel = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var cliente = await _clienteRepository.GetByIdAsync(id);
            if (cliente is null)
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
        {
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var dadosBancariosEntities = cliente?.Pessoa?.DadosBancarios ?? new List<DadosBancarios>();
                var dadosBancariosViewModels = _mapper.Map<List<DadosBancariosViewModel>>(dadosBancariosEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "DadosBancarios",
                    items = dadosBancariosViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetDadosBancariosFormData(Guid? dadosBancariosId, Guid clienteId)
        { 
            if (dadosBancariosId is null || dadosBancariosId == Guid.Empty)
            {
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound();
                var newViewModel = new DadosBancariosViewModel { PessoaId = clienteId }; // Aqui era clienteId -> PessoaId do Cliente
                return Json(newViewModel);
            } else
            {
                var dadosBancarios = await _clienteRepository.ObterClienteCompleto(dadosBancariosId.Value);
                if (dadosBancarios is null)
                    return NotFound();
                var viewModel = _mapper.Map<DadosBancariosViewModel>(dadosBancarios);
                return Json(viewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarDadosBancarios(
            Guid clienteId,
            DadosBancariosViewModel viewModel)
        {
            // 1. Validação do modelo recebido
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            // 2. Validação do parâmetro de rota
            if (clienteId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "O ID do Cliente não foi fornecido." }
                });
            }

            // 3. Inclusão ou atualização dos dados bancários
            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarDadosBancariosAsync(
                    clienteId,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            } else
            {
                await _clienteDomainService.AtualizarDadosBancariosAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.Banco,
                    viewModel.Agencia,
                    viewModel.Conta,
                    viewModel.TipoDeContaBancaria);
            }

            // 4. Checagem de notificações de domínio
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            // 5. Persistência e tratamento de exceções
            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new
                    {
                        success = false,
                        errors = new List<string> { "Commit retornou falso." }
                    });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine(
                    $"DbUpdateException: {dbEx.Message} " +
                    $"Inner: {dbEx.InnerException?.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro no banco." }
                });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro inesperado." }
                });
            }

            // 6. Resposta final de sucesso
            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirDadosBancarios(
            Guid clienteId,
            Guid dadosBancariosId)
        {
            // 1. Validação de parâmetros
            if (clienteId == Guid.Empty || dadosBancariosId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "IDs inválidos." }
                });
            }

            // 2. Remoção dos dados bancários
            await _clienteDomainService.RemoverDadosBancariosAsync(
                clienteId,
                dadosBancariosId);

            // 3. Verificação de notificações de domínio
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            // 4. Persistência da exclusão
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string> { "Erro ao excluir." }
                });
            }

            // 5. Resposta final de sucesso
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
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var telefoneEntities = cliente?.Pessoa?.Telefones ?? new List<Telefone>();
                var telefoneViewModels = _mapper.Map<List<TelefoneViewModel>>(telefoneEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "Telefone",
                    items = telefoneViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }
        }


        /// <summary>
        /// Retorna os dados de um telefone específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTelefoneFormData(Guid? telefoneId, Guid clienteId) // clienteId para caso de criação
        {
            if (telefoneId is null || telefoneId == Guid.Empty)
            {
                // ...
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar telefone.");

                // Para Telefone, precisamos do PessoaId do Cliente para preencher o formulário
                // *** CORREÇÃO APLICADA AQUI ***
                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);
                // ******************************

                if (clienteParaPessoaId is null)
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

                if (telefone is null)
                    return NotFound("Telefone não encontrado ou não pertence a este cliente.");

                var viewModel = _mapper.Map<TelefoneViewModel>(telefone);
                return Json(viewModel);
            }
        }

        /// <summary>
        /// Salva (adiciona ou atualiza) o telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvarTelefone(
            Guid clienteId,
            TelefoneViewModel viewModel)
        {
            /* 1. Validação do modelo --------------------------------------------- */
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            /* 2. Validação do parâmetro clienteId -------------------------------- */
            if (clienteId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "O ID do Cliente não foi fornecido."
                    }
                });
            }

            /* 3. Inclusão ou atualização do telefone ----------------------------- */
            bool isNew = viewModel.Id == Guid.Empty;

            if (isNew)
            {
                await _clienteDomainService.AdicionarTelefoneAsync(
                    clienteId,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            } else
            {
                await _clienteDomainService.AtualizarTelefoneAsync(
                    clienteId,
                    viewModel.Id,
                    viewModel.DDD,
                    viewModel.Numero,
                    viewModel.TipoDeTelefone);
            }

            /* 4. Verificação de notificações de domínio -------------------------- */
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            /* 5. Persistência e tratamento de exceções --------------------------- */
            try
            {
                if (!await _clienteRepository.UnitOfWork.CommitAsync())
                {
                    return Json(new
                    {
                        success = false,
                        errors = new List<string>
                        {
                            "Erro ao salvar o telefone no banco de dados (Commit retornou falso)."
                        }
                    });
                }
            } catch (DbUpdateException dbEx)
            {
                Console.WriteLine(
                    $"DbUpdateException ao salvar telefone: {dbEx.Message} " +
                    $"Inner: {dbEx.InnerException?.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "Erro no banco de dados ao salvar o telefone."
                    }
                });
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception ao salvar telefone: {ex.Message}");

                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "Ocorreu um erro inesperado ao salvar o telefone."
                    }
                });
            }

            /* 6. Resposta final de sucesso --------------------------------------- */
            return Json(new { success = true });
        }


        /// <summary>
        /// Exclui um telefone de um cliente. Chamado via AJAX.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirTelefone(
            Guid clienteId,
            Guid telefoneId)
        {
            /* 1. Validação de parâmetros ----------------------------------------- */
            if (clienteId == Guid.Empty || telefoneId == Guid.Empty)
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
                    {
                        "IDs inválidos fornecidos para exclusão."
                    }
                });
            }

            /* 2. Remoção do telefone --------------------------------------------- */
            await _clienteDomainService.RemoverTelefoneAsync(
                clienteId,
                telefoneId);

            /* 3. Verificação de notificações de domínio -------------------------- */
            if (!OperacaoValida())
            {
                return Json(new
                {
                    success = false,
                    errors = ObterNotificacoes()
                        .Select(n => n.Mensagem)
                        .ToList()
                });
            }

            /* 4. Persistência da exclusão ---------------------------------------- */
            if (!await _clienteRepository.UnitOfWork.CommitAsync())
            {
                return Json(new
                {
                    success = false,
                    errors = new List<string>
            {
                "Erro ao excluir o telefone no banco de dados."
            }
                });
            }

            /* 5. Resposta final de sucesso --------------------------------------- */
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
            try
            {
                var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
                var contatoEntities = cliente?.Pessoa?.Contatos ?? new List<Contato>();
                var contatoViewModels = _mapper.Map<List<ContatoViewModel>>(contatoEntities);

                return ViewComponent("AggregateList", new
                {
                    parentEntityType = "Cliente",
                    parentEntityId = clienteId,
                    parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                    aggregateType = "Contato",
                    items = contatoViewModels
                });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        /// <summary>
        /// Retorna os dados de um contato específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetContatoFormData(Guid? contatoId, Guid clienteId)
        {
            if (contatoId is null || contatoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar contato.");
                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);
                if (clienteParaPessoaId is null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o contato.");

                var newViewModel = new ContatoViewModel { PessoaId = clienteParaPessoaId.PessoaId };
                return Json(newViewModel);
            } else
            {
                // Modo Edição: Buscar contato pelo seu ID.
                var cliente = await _clienteRepository.ObterClienteComContatos(clienteId);
                var contato = cliente?.Pessoa?.Contatos.FirstOrDefault(c => c.Id == contatoId.Value);

                if (contato is null)
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

            try
            { 
            var cliente = await _clienteRepository.ObterClienteCompleto(clienteId);
            var enderecoEntities = cliente?.Pessoa?.Enderecos ?? new List<Endereco>();
            var enderecoViewModels = _mapper.Map<List<EnderecoViewModel>>(enderecoEntities);

            return ViewComponent("AggregateList", new
            {
                parentEntityType = "Cliente",
                parentEntityId = clienteId,
                parentPessoaId = cliente?.PessoaId ?? Guid.Empty,
                aggregateType = "Endereco",
                items = enderecoViewModels
            });

            } catch (Exception ex)
            {
                // Se ocorrer qualquer erro inesperado (ex: no AutoMapper),
                // ele será capturado e não quebrará a aplicação.
                // O ideal é logar o erro 'ex' aqui.
                // Retorna um erro 500 com uma mensagem clara para o AJAX.
                return StatusCode(500, $"Erro interno ao processar os endereços: {ex.Message}");
            }

        }

        /// <summary>
        /// Retorna os dados de um endereço específico (para edição) ou um ViewModel vazio (para adição).
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetEnderecoFormData(Guid? enderecoId, Guid clienteId)
        {
            if (enderecoId is null || enderecoId == Guid.Empty)
            {
                // Modo Criação
                if (!await _clienteRepository.TemCliente(clienteId))
                    return NotFound("Cliente não encontrado para adicionar endereço.");

                var clienteParaPessoaId = await _clienteRepository.ObterClienteCompleto(clienteId);

                if (clienteParaPessoaId is null)
                    return NotFound("Cliente não encontrado ao buscar PessoaId para o endereço.");

                var newViewModel = new EnderecoViewModel { PessoaId = clienteParaPessoaId.PessoaId };

                return Json(newViewModel);
            } else
            {
                // Modo Edição
                var cliente = await _clienteRepository.ObterClienteComEnderecos(clienteId);
                var endereco = cliente?.Pessoa?.Enderecos.FirstOrDefault(e => e.Id == enderecoId.Value);

                if (endereco is null)
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