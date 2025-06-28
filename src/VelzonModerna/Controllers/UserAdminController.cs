using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http; // Necessário para IFormFile
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VelzonModerna.Controllers.Base;
using VelzonModerna.Helpers;
using VelzonModerna.Services;
using VelzonModerna.ViewModels; // Para UploadImagemViewModel

namespace VelzonModerna.Controllers
{
    [Authorize]
    [Route("admin/usuarios")]
    public class UserAdminController : BaseMvcToApiController
    {
        private readonly IUserAdminMvcService _userAdminMvcService;
        private readonly IAutenticacaoService _autenticacaoService;

        public UserAdminController(IUserAdminMvcService userAdminMvcService,
                                   IAutenticacaoService autenticacaoService)
        {
            _userAdminMvcService = userAdminMvcService;
            _autenticacaoService = autenticacaoService;
        }

        // GET: /admin/usuarios/upload-imagem/{userId}
        [HttpGet("upload-imagem/{userId}")]
        // [ClaimsAuthorize("Usuario", "UploadImagem", "Admin")] // Adicione sua autorização
        public async Task<IActionResult> UploadImagem(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var usuario = await _userAdminMvcService.ObterUsuarioPorIdAsync(userId);
            if (usuario == null)
            {
                TempData ["Error"] = "Usuário não encontrado.";
                // Pode ser melhor redirecionar para uma página de erro ou para o Index com a mensagem.
                return RedirectToAction(nameof(Index));
            }

            var viewModel = new UploadImagemViewModel
            {
                UserId = userId,
                NomeUsuario = usuario.NomeCompleto
            };

            // Lógica para construir a URL da imagem atual
            if (!string.IsNullOrEmpty(usuario.ImgProfilePath) && usuario.ImgProfilePath != "imagemPadrao.png")
            {
                var nomeImagem = usuario.ImgProfilePath.Split('/').LastOrDefault();
                if (!string.IsNullOrEmpty(nomeImagem))
                {
                    // Obter a URL base da API de Identidade (ex: "https://localhost:5013")
                    var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl(); // Ex: https://localhost:5013

                    // O endpoint `obter-imagem` está em `UserAdminController` da API, que tem o prefixo de rota "api/admin"
                    // Portanto, a URL completa será: {baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}
                    viewModel.ImagemAtualUrl = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                } else
                {
                    // Se ImgProfilePath for algo inesperado, use a padrão
                    viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; // Caminho para imagem padrão no wwwroot do MVC
                }
            } else
            {
                viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; // Caminho para imagem padrão no wwwroot do MVC
            }
            // Certifique-se que a imagem "/images/user-dummy-img.jpg" existe no seu wwwroot/images do projeto MVC

            return View(viewModel);
        }

        // POST: /admin/usuarios/upload-imagem
        [HttpPost("upload-imagem")]
        [ValidateAntiForgeryToken]
        // [ClaimsAuthorize("Usuario", "UploadImagem", "Admin")] // Adicione sua autorização
        public async Task<IActionResult> UploadImagem(UploadImagemViewModel viewModel)
        {
            // Repopular NomeUsuario e ImagemAtualUrl em caso de falha de validação,
            // pois eles não são enviados de volta pelo formulário e seriam perdidos.
            async Task RepopulateViewModelForError()
            {
                if (!string.IsNullOrEmpty(viewModel.UserId))
                {
                    var usuario = await _userAdminMvcService.ObterUsuarioPorIdAsync(viewModel.UserId);
                    if (usuario != null)
                    {
                        viewModel.NomeUsuario = usuario.NomeCompleto;
                        if (!string.IsNullOrEmpty(usuario.ImgProfilePath) && usuario.ImgProfilePath != "imagemPadrao.png")
                        {
                            var nomeImagem = usuario.ImgProfilePath.Split('/').LastOrDefault();
                            if (!string.IsNullOrEmpty(nomeImagem))
                            {
                                var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                                viewModel.ImagemAtualUrl = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                            } else
                            { viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                        } else
                        { viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                    } else // Se não encontrar o usuário, use valores padrão
                    {
                        viewModel.NomeUsuario = "Usuário Desconhecido";
                        viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg";
                    }
                } else
                { viewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
            }


            if (!ModelState.IsValid)
            {
                await RepopulateViewModelForError();
                return View(viewModel);
            }

            var (resultado, caminhoImagemRetornadoDaApi) = await _userAdminMvcService.UploadImagemPerfilAsync(viewModel.UserId, viewModel.Imagem);

            if (ResponsePossuiErros(resultado))
            {
                // Os erros já foram adicionados ao ModelState pelo ResponsePossuiErros
                // dentro do _userAdminMvcService ou pelo BaseMvcToApiController
                await RepopulateViewModelForError();
                return View(viewModel);
            }

            // Mensagem de sucesso principal
            TempData [TempDataKeys.SuccessMessage] = "Imagem de perfil alterada com sucesso!";
            // Mensagem informativa adicional
            TempData [TempDataKeys.InfoMessage] = "A nova imagem no menu superior poderá levar alguns instantes para atualizar ou será visível no seu próximo login.";
            // Idealmente, redirecionar para a página de detalhes do usuário ou para onde for mais apropriado.
            return RedirectToAction(nameof(Detalhes), new { id = viewModel.UserId });
        }


        // Inclua aqui as outras actions que já tínhamos (Index, Detalhes, Criar, Editar, Excluir, AtivarDesativar...)
        // ...
        // GET: /admin/usuarios
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userAdminMvcService.ObterTodosUsuariosAsync();
            if (usuarios == null)
            {
                AdicionarErroValidacao("Não foi possível obter a lista de usuários da API.");
                return View(new List<UserResponseDto>());
            }
            // Mapear ImgProfilePath para URLs completas se necessário para exibição na lista
            var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
            var usuariosViewModel = usuarios.Select(u => new UserResponseDto // Poderia ser um UserListViewModel dedicado
            {
                UserId = u.UserId,
                NomeCompleto = u.NomeCompleto,
                Apelido = u.Apelido,
                Email = u.Email,
                DataNascimento = u.DataNascimento,
                EmailConfirmado = u.EmailConfirmado,
                ImgProfilePath = (!string.IsNullOrEmpty(u.ImgProfilePath) && u.ImgProfilePath != "imagemPadrao.png")
                                 ? $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{u.ImgProfilePath.Split('/').LastOrDefault()}"
                                 : "/cooperchip/images/img-padrao.jpg" // Imagem padrão do MVC
            }).ToList();


            return View(usuariosViewModel); // Passa a lista de usuários para a view
        }

        // GET: /admin/usuarios/detalhes/{id}
        [HttpGet("detalhes/{id}")]
        public async Task<IActionResult> Detalhes(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var usuario = await _userAdminMvcService.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                TempData ["Error"] = "Usuário não encontrado ou falha ao obter dados da API.";
                return RedirectToAction(nameof(Index));
            }

            // Ajustar a URL da imagem para a view de detalhes
            if (!string.IsNullOrEmpty(usuario.ImgProfilePath) && usuario.ImgProfilePath != "imagemPadrao.png")
            {
                var nomeImagem = usuario.ImgProfilePath.Split('/').LastOrDefault();
                if (!string.IsNullOrEmpty(nomeImagem))
                {
                    var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                    usuario.ImgProfilePath = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                } else
                {
                    usuario.ImgProfilePath = "/cooperchip/images/img-padrao.jpg";
                }
            } else
            {
                usuario.ImgProfilePath = "/cooperchip/images/img-padrao.jpg";
            }

            return View(usuario);
        }


        // GET: /admin/usuarios/criar
        [HttpGet("adicionar-usuario")]
        public IActionResult Create()
        {
            return View(new CriarUsuarioDto());
        }

        // POST: /admin/usuarios/criar
        [HttpPost("adicionar-usuario")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CriarUsuarioDto criarUsuarioDto)
        {
            if (!ModelState.IsValid)
            {
                return View(criarUsuarioDto);
            }

            var resultado = await _userAdminMvcService.CriarUsuarioAsync(criarUsuarioDto);

            if (ResponsePossuiErros(resultado))
            {
                return View(criarUsuarioDto);
            }

            TempData ["Success"] = "Usuário criado com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /admin/usuarios/editar/{id}
        [HttpGet("editar/{id}")]
        public async Task<IActionResult> Editar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var usuarioApiDto = await _userAdminMvcService.ObterUsuarioPorIdAsync(id); // Este já terá o ImgProfilePath ajustado pela action Detalhes se fosse chamado de lá. Mas aqui é uma chamada direta.
            if (usuarioApiDto == null)
            {
                TempData ["Error"] = "Usuário não encontrado ou falha ao obter dados da API.";
                return RedirectToAction(nameof(Index));
            }

            // O UserDto da API (usuarioApiDto.ImgProfilePath) contém o nome do arquivo relativo, ex: "guid.png" ou "images/profiles/guid.png"
            // O AtualizarUsuarioDto não tem campo de imagem para *mostrar* a imagem atual, mas sim para *atualizar* os dados do usuário.
            // Se a view de edição precisar mostrar a imagem atual, ela precisará da URL completa.
            // Vamos popular um ViewModel para a edição que contenha essa URL.

            var editViewModel = new AtualizarUsuarioViewModel // CRIE ESTE VIEWMODEL
            {
                UserId = usuarioApiDto.UserId,
                NomeCompleto = usuarioApiDto.NomeCompleto,
                Email = usuarioApiDto.Email,
                DataNascimento = usuarioApiDto.DataNascimento,
                IsAtivo = !string.Equals(usuarioApiDto.UsuarioBloqueado, "sim", System.StringComparison.OrdinalIgnoreCase) // Inferindo IsAtivo
            };

            if (!string.IsNullOrEmpty(usuarioApiDto.ImgProfilePath) && usuarioApiDto.ImgProfilePath != "imagemPadrao.png")
            {
                var nomeImagem = usuarioApiDto.ImgProfilePath.Split('/').LastOrDefault();
                if (!string.IsNullOrEmpty(nomeImagem))
                {
                    var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                    editViewModel.ImagemAtualUrl = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                } else
                {
                    editViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg";
                }
            } else
            {
                editViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg";
            }

            return View(editViewModel);
        }

        // POST: /admin/usuarios/editar/{id}
        [HttpPost("editar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(string id, AtualizarUsuarioViewModel atualizarUsuarioViewModel) // Usando o novo ViewModel
        {
            if (string.IsNullOrEmpty(id) || id != atualizarUsuarioViewModel.UserId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Repopular ImagemAtualUrl se houver erro de validação
                if (!string.IsNullOrEmpty(atualizarUsuarioViewModel.UserId))
                {
                    var usuarioOriginal = await _userAdminMvcService.ObterUsuarioPorIdAsync(atualizarUsuarioViewModel.UserId);
                    if (usuarioOriginal != null && !string.IsNullOrEmpty(usuarioOriginal.ImgProfilePath) && usuarioOriginal.ImgProfilePath != "imagemPadrao.png")
                    {
                        var nomeImagem = usuarioOriginal.ImgProfilePath.Split('/').LastOrDefault();
                        if (!string.IsNullOrEmpty(nomeImagem))
                        {
                            var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                            atualizarUsuarioViewModel.ImagemAtualUrl = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                        } else
                        { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                    } else
                    { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                } else
                { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                return View(atualizarUsuarioViewModel);
            }

            // Mapear do ViewModel de edição para o DTO da API
            var dtoParaApi = new AtualizarUsuarioDto
            {
                UserId = atualizarUsuarioViewModel.UserId,
                NomeCompleto = atualizarUsuarioViewModel.NomeCompleto,
                DataNascimento = atualizarUsuarioViewModel.DataNascimento,
                Email = atualizarUsuarioViewModel.Email,
                IsAtivo = atualizarUsuarioViewModel.IsAtivo
            };

            var resultado = await _userAdminMvcService.AtualizarUsuarioAsync(id, dtoParaApi);

            if (ResponsePossuiErros(resultado))
            {
                // Repopular ImagemAtualUrl
                if (!string.IsNullOrEmpty(atualizarUsuarioViewModel.UserId))
                {
                    var usuarioOriginal = await _userAdminMvcService.ObterUsuarioPorIdAsync(atualizarUsuarioViewModel.UserId);
                    if (usuarioOriginal != null && !string.IsNullOrEmpty(usuarioOriginal.ImgProfilePath) && usuarioOriginal.ImgProfilePath != "imagemPadrao.png")
                    {
                        var nomeImagem = usuarioOriginal.ImgProfilePath.Split('/').LastOrDefault();
                        if (!string.IsNullOrEmpty(nomeImagem))
                        {
                            var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                            atualizarUsuarioViewModel.ImagemAtualUrl = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                        } else
                        { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                    } else
                    { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                } else
                { atualizarUsuarioViewModel.ImagemAtualUrl = "/cooperchip/images/img-padrao.jpg"; }
                return View(atualizarUsuarioViewModel);
            }

            TempData ["Success"] = "Usuário atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }


        // GET: /admin/usuarios/excluir/{id}
        [HttpGet("excluir/{id}")]
        public async Task<IActionResult> Excluir(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var usuario = await _userAdminMvcService.ObterUsuarioPorIdAsync(id);
            if (usuario == null)
            {
                TempData ["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }
            // Ajustar imagem para view de exclusão
            if (!string.IsNullOrEmpty(usuario.ImgProfilePath) && usuario.ImgProfilePath != "imagemPadrao.png")
            {
                var nomeImagem = usuario.ImgProfilePath.Split('/').LastOrDefault();
                if (!string.IsNullOrEmpty(nomeImagem))
                {
                    var baseUrlApiIdentidade = _autenticacaoService.GetApiBaseUrl();
                    usuario.ImgProfilePath = $"{baseUrlApiIdentidade}/api/admin/obter-imagem/{nomeImagem}";
                } else
                { usuario.ImgProfilePath = "/cooperchip/images/img-padrao.jpg"; }
            } else
            { usuario.ImgProfilePath = "/cooperchip/images/img-padrao.jpg"; }

            return View(usuario);
        }

        // POST: /admin/usuarios/excluir-confirmado/{id}
        [HttpPost("excluir-confirmado/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirConfirmado(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var resultado = await _userAdminMvcService.ExcluirUsuarioAsync(id);

            if (ResponsePossuiErros(resultado))
            {
                TempData ["Error"] = string.Join("; ", resultado.Errors.Mensagens);
                return RedirectToAction(nameof(Detalhes), new { id = id });
            }

            TempData ["Success"] = "Usuário excluído com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // POST: /admin/usuarios/ativar-desativar/{id}
        [HttpPost("ativar-desativar/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtivarDesativar(string id, [FromForm] bool ativar)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var resultado = await _userAdminMvcService.AtivarDesativarUsuarioAsync(id, ativar);

            if (ResponsePossuiErros(resultado))
            {
                TempData ["Error"] = string.Join("; ", resultado.Errors.Mensagens);
            } else
            {
                TempData ["Success"] = $"Usuário {(ativar ? "ativado" : "desativado")} com sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }


        // Action para REMOVER usuário de uma role
        // Esta action será chamada por um formulário na view GerenciarRolesDoUsuario.cshtml,
        // provavelmente um pequeno formulário para cada role que o usuário possui, com um botão "Remover".
        // O formulário enviará o 'userId' e o 'roleName' a ser removido.
        [HttpPost("remover-role-usuario")]
        [ValidateAntiForgeryToken]
        // [ClaimsAuthorize("Role", "Remover", "Admin")] // Use a claim de autorização apropriada, igual à da API
        public async Task<IActionResult> RemoverRoleDoUsuario(string userId, string roleName) // Parâmetros vêm do formulário
        {
            // Validação básica dos parâmetros recebidos
            if (string.IsNullOrEmpty(userId))
            {
                TempData ["Error"] = "ID do Usuário não fornecido para remoção de role.";
                return RedirectToAction(nameof(Index)); // Redireciona para a lista de usuários se o ID for inválido
            }
            if (string.IsNullOrEmpty(roleName))
            {
                TempData ["Error"] = "Nome da Role não fornecido para remoção.";
                // Redireciona de volta para a página de gerenciamento de roles do usuário específico
                return RedirectToAction(nameof(GerenciarRolesDoUsuario), new { userId = userId });
            }

            // Chama o serviço para remover a role do usuário
            var resultado = await _userAdminMvcService.RemoverUsuarioDeRoleAsync(userId, roleName);

            // Verifica se houve erros na resposta do serviço
            if (ResponsePossuiErros(resultado)) // Seu método base para tratar ResponseResult
            {
                // Adiciona as mensagens de erro ao TempData para exibição na próxima página
                TempData ["Error"] = string.Join("; ", resultado.Errors.Mensagens);
            } else
            {
                // Mensagem de sucesso
                TempData ["Success"] = $"Role '{roleName}' removida do usuário com sucesso!";
            }

            // Redireciona de volta para a página de gerenciamento de roles do usuário
            return RedirectToAction(nameof(GerenciarRolesDoUsuario), new { userId = userId });
        }



        // GET: /admin/usuarios/gerenciar-roles/{userId}
        [HttpGet("gerenciar-roles/{userId}")]
        // [ClaimsAuthorize("Usuario", "GerenciarRoles", "Admin")] // Proteja conforme necessário
        public async Task<IActionResult> GerenciarRolesDoUsuario(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("ID do usuário não fornecido.");
            }

            // 1. Buscar detalhes do usuário (para obter o nome/apelido)
            var usuarioDto = await _userAdminMvcService.ObterUsuarioPorIdAsync(userId);
            if (usuarioDto == null)
            {
                TempData ["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Index));
            }

            // 2. Buscar todas as roles do sistema
            var todasRolesSistema = await _userAdminMvcService.ObterTodasRolesAsync();
            if (todasRolesSistema == null) // O serviço pode retornar null ou vazio em caso de erro
            {
                TempData ["Error"] = "Não foi possível carregar as roles do sistema.";
                todasRolesSistema = Enumerable.Empty<RoleViewModel>(); // Garante que não seja nulo para o ViewModel
            }

            // 3. Buscar as roles atuais do usuário
            var rolesAtuaisUsuario = await _userAdminMvcService.ObterRolesDoUsuarioAsync(userId);
            if (rolesAtuaisUsuario == null)
            {
                TempData ["Error"] = "Não foi possível carregar as roles atuais do usuário.";
                rolesAtuaisUsuario = new List<string>(); // Garante que não seja nulo para o ViewModel
            }

            // 4. Montar o ViewModel
            var viewModel = new GerenciarRolesUsuarioViewModel(
                userId,
                usuarioDto.NomeCompleto,
                usuarioDto.Apelido,
                rolesAtuaisUsuario,
                todasRolesSistema
            );

            return View(viewModel);
        }

        [HttpPost("adicionar-role-usuario")] // Ou "adicionar-permissao-usuario" se mudar a rota
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdicionarRoleAoUsuario(GerenciarRolesUsuarioViewModel viewModel)
        {
            // O ViewModel agora tem [Required] em RoleSelecionadaParaAdicionar.
            // O UserId vem do input hidden e deve ser sempre presente se o GET funcionou.
            // Vamos verificar explicitamente o UserId também para segurança.
            if (string.IsNullOrEmpty(viewModel.UserId))
            {
                TempData ["Error"] = "ID do Usuário não foi fornecido. Operação cancelada.";
                return RedirectToAction(nameof(Index)); // Ou uma página de erro mais apropriada
            }

            if (!ModelState.IsValid) // Captura a falha do [Required] em RoleSelecionadaParaAdicionar
            {
                // Se ModelState não for válido, significa que "RoleSelecionadaParaAdicionar" não foi preenchida.
                // Precisamos repopular os dados necessários para renderizar a view GerenciarRolesDoUsuario novamente.
                TempData ["Error"] = "Por favor, selecione uma permissão da lista para adicionar."; // Mensagem mais específica

                // Repopular dados para a view (igual à action GET)
                var usuarioDto = await _userAdminMvcService.ObterUsuarioPorIdAsync(viewModel.UserId);
                var todasRolesSistema = await _userAdminMvcService.ObterTodasRolesAsync();
                var rolesAtuaisUsuario = await _userAdminMvcService.ObterRolesDoUsuarioAsync(viewModel.UserId);

                // Recria o ViewModel com os dados atuais para a view não quebrar
                var modelParaView = new GerenciarRolesUsuarioViewModel(
                    viewModel.UserId,
                    usuarioDto?.NomeCompleto ?? "Usuário",
                    usuarioDto?.Apelido ?? "",
                    rolesAtuaisUsuario ?? new List<string>(),
                    todasRolesSistema ?? Enumerable.Empty<RoleViewModel>()
                );
                // viewModel original pode não ter todos os dados para a view,
                // por isso recriamos com base no que a action GET faria.
                // Ou, alternativamente, passar o viewModel original de volta se ele contiver
                // as listas necessárias (RolesAtuais e DropdownTodasAsRoles).
                // No seu caso, o viewModel do POST só tem UserId e RoleSelecionadaParaAdicionar.

                // Se você quer que a view de GerenciarRolesDoUsuario mostre os erros do ModelState:
                // Você teria que retornar View(modelParaView) em vez de RedirectToAction.
                // O problema do RedirectToAction é que o ModelState se perde.
                // Usar TempData é a forma de passar a mensagem para a próxima requisição.
                // Se quiser manter o redirect:
                return RedirectToAction(nameof(GerenciarRolesDoUsuario), new { userId = viewModel.UserId });
            }

            var dtoParaApi = new AdicionarUsuarioRoleDto
            {
                UserId = viewModel.UserId,
                RoleName = viewModel.RoleSelecionadaParaAdicionar // Nome da role selecionada
            };

            var resultado = await _userAdminMvcService.AdicionarUsuarioRoleAsync(dtoParaApi);

            if (ResponsePossuiErros(resultado))
            {
                TempData ["Error"] = string.Join("; ", resultado.Errors.Mensagens);
            } else
            {
                TempData ["Success"] = $"Permissão '{viewModel.RoleSelecionadaParaAdicionar}' adicionada ao usuário com sucesso!";
            }

            return RedirectToAction(nameof(GerenciarRolesDoUsuario), new { userId = viewModel.UserId });
        }


    }
}
