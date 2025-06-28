using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralLabSolutions.CoreShared.ViewModelsIdentidade
{
    public class UsuarioRespostaLogin
    {
        public string? AccessToken { get; set; }
        public double? ExpiresIn { get; set; }
        public UsuarioToken? UsuarioToken { get; set; }
        public ResponseResult? ResponseResult { get; set; }

        public Guid RefreshToken { get; set; }
    }
}
