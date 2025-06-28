using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLabSolutions.CoreShared.ViewModelsIdentidade
{
    public class UsuarioToken
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public IEnumerable<UsuarioClaim>? Claims { get; set; } = new List<UsuarioClaim>();
    }
}
