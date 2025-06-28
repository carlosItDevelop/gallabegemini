using GeneralLabSolutions.CoreShared.ViewModelsIdentidade;
using Microsoft.AspNetCore.Mvc;
using velzon.Models;

namespace VelzonModerna.Controllers.Base
{
    public class BaseMvcToApiController : Controller
    {
        // VelzonModerna.Controllers.Base.BaseMvcToApiController.cs
        protected bool ResponsePossuiErros(ResponseResult resposta)
        {
            if (resposta != null && resposta.Errors != null && resposta.Errors.Mensagens != null && resposta.Errors.Mensagens.Any())
            {
                foreach (var mensagem in resposta.Errors.Mensagens)
                {
                    ModelState.AddModelError(string.Empty, mensagem);
                }
                return true;
            }
            return false;
        }

        protected void AdicionarErroValidacao(string mensagem)
        {
            ModelState.AddModelError(string.Empty, mensagem);
        }

        protected bool OperacaoValida()
        {
            return ModelState.ErrorCount == 0;
        }
    }
}
