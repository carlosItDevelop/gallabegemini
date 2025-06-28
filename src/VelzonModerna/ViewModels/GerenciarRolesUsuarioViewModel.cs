using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering; // Para SelectList

namespace VelzonModerna.ViewModels
{
    public class GerenciarRolesUsuarioViewModel
    {
        public string UserId { get; set; }
        public string NomeUsuario { get; set; } // Para exibir o nome do usuário na página
        public string ApelidoUsuario { get; set; }

        // Roles que o usuário já possui
        public IList<string> RolesAtuaisDoUsuario { get; set; } = new List<string>();

        // Todas as roles disponíveis no sistema, para popular um dropdown
        public IEnumerable<RoleViewModel> TodasAsRolesDoSistema { get; set; } = new List<RoleViewModel>();

        // Propriedade para o binding do formulário de adição de role
        [Required(ErrorMessage = "Por favor, selecione uma permissão para adicionar.")]
        [Display(Name = "Adicionar à Permissão")]
        public string RoleSelecionadaParaAdicionar { get; set; }

        // Para popular o dropdown de forma mais fácil na view
        public SelectList DropdownTodasAsRoles { get; set; }

        public GerenciarRolesUsuarioViewModel() { }

        public GerenciarRolesUsuarioViewModel(string userId, string nomeUsuario, string apelidoUsuario, IList<string> rolesAtuais, IEnumerable<RoleViewModel> todasRoles)
        {
            UserId = userId;
            NomeUsuario = nomeUsuario;
            ApelidoUsuario = apelidoUsuario;
            RolesAtuaisDoUsuario = rolesAtuais ?? new List<string>();
            TodasAsRolesDoSistema = todasRoles ?? new List<RoleViewModel>();

            // Prepara o SelectList para o dropdown, excluindo as roles que o usuário já possui
            var rolesDisponiveisParaAdicionar = TodasAsRolesDoSistema
                .Where(r => !RolesAtuaisDoUsuario.Contains(r.Nome, StringComparer.OrdinalIgnoreCase))
                .OrderBy(r => r.Nome);

            DropdownTodasAsRoles = new SelectList(rolesDisponiveisParaAdicionar, nameof(RoleViewModel.Nome), nameof(RoleViewModel.Nome));
        }
    }
}