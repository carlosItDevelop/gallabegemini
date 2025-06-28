using System.Security.Claims;
using GeneralLabSolutions.CoreShared.DTOs.DtosIdentidade;
using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using GeneralLabSolutions.Identidade.Data;
using GeneralLabSolutions.WebApiCore.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp; // Adicionar using para ImageSharp
using SixLabors.ImageSharp.PixelFormats; // Para carregar a imagem como Rgba32

namespace GeneralLabSolutions.Identidade.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAspNetUser _aspNetUser;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IAspNetUser aspNetUser,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _aspNetUser = aspNetUser;
            _webHostEnvironment = webHostEnvironment;
        }


        #region: Obter todos os Usuários

        public async Task<IEnumerable<UserResponseDto>> GetUsersDto()
        {

            var usuarios = new List<UserResponseDto>();
            foreach (var user in _userManager.Users)
            {
                var userDto = new UserResponseDto
                {
                    NomeCompleto = user.NomeCompleto,
                    Apelido = user.Apelido,
                    DataNascimento = user.DataNascimento,
                    ImgProfilePath = user.ImgProfilePath,
                    UserId = user.Id,
                    Email = user.Email!,
                    EmailConfirmado = user.EmailConfirmed
                };
                usuarios.Add(userDto);
            }
            return await Task.FromResult(usuarios);
        }
        #endregion


        #region: Obter um Usuário por Id

        public async Task<ApplicationUser?> GetUserById(string userId)
        {                       
            return await Task.FromResult(_userManager.Users.FirstOrDefault(u => u.Id == userId));
        }
        #endregion


        #region: Criar Usuários

        public async Task<IdentityResult> CriarUsuarioAsync(CriarUsuarioDto criarUsuarioDto)
        {
            var user = new ApplicationUser
            {
                NomeCompleto = criarUsuarioDto.NomeCompleto,
                Apelido = criarUsuarioDto.Apelido,
                DataNascimento = criarUsuarioDto.DataNascimento,
                ImgProfilePath = criarUsuarioDto.ImgProfilePath,
                UserName = criarUsuarioDto.Email,
                Email = criarUsuarioDto.Email,
                EmailConfirmed = true // Email confirmado automaticamente
            };

            var resultado = await _userManager.CreateAsync(user, criarUsuarioDto.Senha);

            if (resultado.Succeeded)
            {
                // Atribui a role "Default" ao novo usuário
                // Substitua "Default" pelo nome da role desejada
                await _userManager.AddToRoleAsync(user, "Default"); 
            }

            return resultado;
        }
        #endregion


        #region: Adicionar um Usuário a uma Role
        public async Task<IdentityResult> AdicionarUsuarioARoleAsync(string userId, string roleName)
        {
            // Validações de userId e roleName (implementar)
            // ...

            // Busca o usuário pelo ID
            var usuario = await _userManager.FindByIdAsync(userId);

            // Busca a role pelo nome
            var role = await _roleManager.FindByNameAsync(roleName);

            if (usuario == null || role == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário ou Role não encontrados." });

            // Adiciona o usuário à role
            var resultado = await _userManager.AddToRoleAsync(usuario, roleName);
            return resultado;
        }
        #endregion


        #region: Obter todas as Claims do Sistema
        public async Task<IEnumerable<Claim>> ObterTodasClaimsAsync(string? tipo = null, string? valor = null)
        {
            var claims = await _userManager.GetClaimsAsync(new ApplicationUser()); // Obtém todas as claims do sistema 

            // Aplica filtragem se tipo ou valor forem fornecidos
            if (!string.IsNullOrEmpty(tipo))
            {
                claims = claims.Where(c => c.Type == tipo).ToList();
            }

            if (!string.IsNullOrEmpty(valor))
            {
                claims = claims.Where(c => c.Value == valor).ToList();
            }

            return claims;
        }
        #endregion


        #region: Obter Claim do usuário

        public async Task<IEnumerable<Claim>> ObterClaimsDoUsuarioAsync(string userId, string? tipo = null, string? valor = null)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return new List<Claim>(); // Retorna lista vazia se usuário não for encontrado

            var claims = await _userManager.GetClaimsAsync(usuario);

            if (!string.IsNullOrEmpty(tipo))
                claims = claims.Where(c => c.Type == tipo).ToList();

            if (!string.IsNullOrEmpty(valor))
                claims = claims.Where(c => c.Value == valor).ToList();

            return claims;
        }

        #endregion


        #region: Obter Usuários por Cliam

        public async Task<IEnumerable<ApplicationUser>> ObterUsuariosPorClaimAsync(string tipo, string valor)
        {
            var usuarios = await _userManager.Users.ToListAsync();

            return usuarios.Where(u => _userManager.GetClaimsAsync(u).Result
                .Any(c => c.Type == tipo && c.Value == valor));
        }

        #endregion


        #region: Excluir Claim de um Usuário

        public async Task<IdentityResult> ExcluirClaimDoUsuarioAsync(string userId, string claimType, string claimValue)
        {
            // 1. Verificar se o usuário existe
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            // 2. Buscar a claim
            var claim = (await _userManager.GetClaimsAsync(usuario)).FirstOrDefault(c => c.Type == claimType && c.Value == claimValue);
            if (claim == null)
                return IdentityResult.Failed(new IdentityError { Description = "Claim não encontrada para este usuário." });

            // 3. Excluir a claim
            var resultado = await _userManager.RemoveClaimAsync(usuario, claim);
            return resultado;
        }

        #endregion


        #region: Excluir um Usuário (desde que não seja um usuário "Admin")

        public async Task<IdentityResult> ExcluirUsuarioAsync(string userId)
        {
            // 1. Verificar se o usuário existe
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            // 2. Impedir a exclusão de usuários "Admin" 
            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
                return IdentityResult.Failed(new IdentityError { Description = "Não é possível excluir um usuário 'Admin'." });

            // 3. Excluir as roles do usuário
            var rolesDoUsuario = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesDoUsuario);

            // 4. Excluir as claims do usuário
            var claimsDoUsuario = await _userManager.GetClaimsAsync(usuario);
            await _userManager.RemoveClaimsAsync(usuario, claimsDoUsuario);

            // 5. Excluir o usuário
            var resultado = await _userManager.DeleteAsync(usuario);
            return resultado;
        }

        #endregion


        #region: Atualizar Usuário

        public async Task<IdentityResult> AtualizarUsuarioPorAdminAsync(AtualizarUsuarioDto dto)
        {
            var usuario = await _userManager.FindByIdAsync(dto.UserId);

            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            usuario.NomeCompleto = dto.NomeCompleto;
            usuario.DataNascimento = dto.DataNascimento;
            usuario.Email = dto.Email;
            usuario.IsAtivo = dto.IsAtivo;

            var resultado = await _userManager.UpdateAsync(usuario);
            return resultado;
        }


        #endregion


        #region: Alterar Senha

        public async Task<IdentityResult> AtualizarSenhaAsync(AtualizarSenhaDto dto)
        {

            // Obter o ID do usuário autenticado
            var userIdAutenticado = _aspNetUser.ObterUserId(); // Utilize sua implementação de IAspNetUser 

            if (dto.UserId != userIdAutenticado.ToString())
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "Você só pode atualizar sua própria senha."
                });
            }

            var usuario = await _userManager.FindByIdAsync(dto.UserId);

            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            var resultado = await _userManager.ChangePasswordAsync(usuario, dto.SenhaAtual, dto.NovaSenha);
            return resultado;
        }

        #endregion


        #region: Ativar ou Inativar um Usuário (por Admin)
        public async Task<IdentityResult> AtivarDesativarUsuarioAsync(string userId, bool ativar)
        {
            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            // Verificar se o usuário é um Admin e se está tentando inativar
            if (await _userManager.IsInRoleAsync(usuario, "Admin") && !ativar)
                return IdentityResult.Failed(new IdentityError { Description = "Não é possível inativar um usuário 'Admin'." });

            if (ativar)
            {
                usuario.AtivarUsuario();
            } else
            {
                usuario.InativarUsuario();
            }

            var resultado = await _userManager.UpdateAsync(usuario);
            return resultado;
        }
        #endregion


        #region: Bloquear um Usuário (NÃO Admin) por 'X' minutos

        public async Task<IdentityResult> BloquearDesbloquearUsuarioAsync(string userId, TimeSpan? tempoBloqueio = null)
        {
            var usuario = await _userManager.FindByIdAsync(userId);

            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            // Verificar se o usuário é um Admin
            if (await _userManager.IsInRoleAsync(usuario, "Admin"))
                return IdentityResult.Failed(new IdentityError { Description = "Não é possível bloquear um usuário 'Admin'." });

            // Se tempoBloqueio for nulo, desbloqueia o usuário (definindo LockoutEnd para null)
            if (tempoBloqueio == null)
            {
                await _userManager.SetLockoutEndDateAsync(usuario, null);
                return IdentityResult.Success;
            }

            // Bloqueia o usuário definindo LockoutEnd para a data atual + tempoBloqueio
            await _userManager.SetLockoutEndDateAsync(usuario, DateTimeOffset.UtcNow.Add(tempoBloqueio.Value));
            return IdentityResult.Success;
        }

        #endregion


        #region: Fazer Upload da ImgProfilePath
        public async Task<IdentityResult> UploadImagemAsync(UploadImagemDto dto)
        {
            // 1. Validações
            var usuario = await _userManager.FindByIdAsync(dto.UserId);
            if (usuario == null)
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });

            if (dto.Imagem == null || dto.Imagem.Length == 0)
                return IdentityResult.Failed(new IdentityError { Description = "Imagem inválida." });

            if (!dto.Imagem.ContentType.Equals("image/png", StringComparison.OrdinalIgnoreCase))
                return IdentityResult.Failed(new IdentityError { Description = "A imagem deve ser do tipo PNG." });

            // Carregar a imagem com ImageSharp
            using var img = await Image.LoadAsync<Rgba32>(dto.Imagem.OpenReadStream());
            if (img.Width > 300 || img.Height > 300)
                return IdentityResult.Failed(new IdentityError { Description = "As dimensões da imagem devem ser no máximo 300x300 pixels." });

            // 2. Obter o diretório onde as imagens serão salvas
            var caminhoDiretorioImagens = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");

            // 3. Criar o diretório se ele não existir
            if (!Directory.Exists(caminhoDiretorioImagens))
            {
                Directory.CreateDirectory(caminhoDiretorioImagens);
            }

            // 4. Gerar um nome único para a imagem
            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(dto.Imagem.FileName)}";

            // 5. Caminho completo para salvar a imagem
            var caminhoArquivo = Path.Combine(caminhoDiretorioImagens, nomeArquivo);

            // 6. Salvar a imagem no sistema de arquivos
            using (var stream = new FileStream(caminhoArquivo, FileMode.Create))
            {
                await dto.Imagem.CopyToAsync(stream);
            }

            // 7. Atualizar o campo ImgProfilePath do usuário
            if (usuario != null)
            {
                usuario.ImgProfilePath = $"images/profiles/{nomeArquivo}";
                await _userManager.UpdateAsync(usuario);
            }

            // 8. Retornar Success
            return IdentityResult.Success;
        }
        #endregion


        #region: Remover Usuário de uma Role
        public async Task<IdentityResult> RemoverUsuarioDeRoleAsync(string userId, string roleName)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Usuário não encontrado." });
            }

            // Verificar se a role existe (opcional, mas bom para consistência)
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError { Description = $"Role '{roleName}' não encontrada." });
            }

            if (!await _userManager.IsInRoleAsync(usuario, roleName))
            {
                // Usuário já não está na role, pode considerar sucesso ou um erro específico
                // Para simplificar, vamos considerar que a operação "remover" teve sucesso se ele não está lá.
                // Ou pode retornar um erro:
                // return IdentityResult.Failed(new IdentityError { Description = $"Usuário não está na role '{roleName}'." });
                return IdentityResult.Success; // Ou um IdentityResult.Failed se preferir indicar que não houve mudança
            }

            return await _userManager.RemoveFromRoleAsync(usuario, roleName);
        }
        #endregion

        // Deixar este método por último

        #region: mapeamento de IdentityUser para UserDto

        /// <summary>
        /// Mapea um IdentityUser para UserDto para evitar 
        /// expor os dados sensíveis da tabela IdentityUser
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserDto MapearParaUserDto(ApplicationUser user)
        {
            return new UserDto
            {
                UserId = user.Id,
                NomeCompleto = user.NomeCompleto,
                Apelido = user.Apelido,
                DataNascimento = user.DataNascimento,
                ImgProfilePath = user.ImgProfilePath,
                UserName = user.UserName!,
                Email = user.Email!,
                EmailConfirmado = user.EmailConfirmed ? "Sim" : "Não",
                UsuarioBloqueado = user.LockoutEnd != null ? "Sim" : "Não",
                NumeroDeErroDeLogin = user.AccessFailedCount
            };
        }

        #endregion

    }
}
