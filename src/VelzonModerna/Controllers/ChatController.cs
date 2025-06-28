using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Services;
using GeneralLabSolutions.WebApiCore.Usuario;
using GeneralLabSolutions.InfraStructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Markdig;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using GeneralLabSolutions.Domain.Settings;
using Microsoft.Extensions.Options;
using VelzonModerna.Helpers;
using GeneralLabSolutions.CoreShared.Interfaces;


namespace VelzonModerna.Controllers
{
    public class ChatController : Controller
    {
        const int quantidadeHistoricoIA = 5;
        const int quantidadeHistoricoInicial = 15;

        private readonly IOpenAIService _openAI;
        private readonly AppDbContext _db;
        private readonly IAspNetUser _user;
        private readonly ILogger<ChatController> _log;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAgenteDeIARepository _agenteRepo;
        private readonly OpenAISettings _openAiSettings;

        public ChatController(
            IOpenAIService openAI,
            AppDbContext db,
            IAspNetUser user,
            ILogger<ChatController> log,
            ICompositeViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IAgenteDeIARepository agenteRepo,
            IOptions<OpenAISettings> openAiSettings)
        {
            _openAI = openAI;
            _db = db;
            _user = user;
            _log = log;
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _agenteRepo = agenteRepo;
            _openAiSettings = openAiSettings.Value;
        }

        private static readonly Dictionary<string, List<string>> ContextKeywords = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            { "Fornecedores", new List<string> { "fornecedor", "fornecedores", "supplier", "suppliers", "distribuidor", "distribuidores" } },
            { "Vendedores", new List<string> { "vendedor", "vendedores", "representante", "representantes", "representante comercial" } },
            { "Clientes", new List<string> { "cliente", "clientes", "comprador", "compradores" } }
        };

        private enum TipoContexto
        {
            Clientes,
            Vendedores,
            Fornecedores
        }

        public async Task<IActionResult> Index()
        {
            var historico = await _db.MensagensChat
                                     .AsNoTracking()
                                     .OrderByDescending(m => m.DataHora)
                                     .Take(quantidadeHistoricoInicial)
                                     .OrderBy(m => m.DataHora)
                                     .ToListAsync();

            ViewBag.HistoricoChat = historico;
            return View("Chat", new MensagemChat());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage([FromForm] string pergunta)
        {
            if (string.IsNullOrWhiteSpace(pergunta))
            {
                TempData ["ChatError"] = "A pergunta não pode estar vazia.";
                return RedirectToAction(nameof(Index));
            }

            await ProcessarPerguntaAsync(pergunta);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessageAjax([FromBody] ChatAjaxInput model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Pergunta))
                return BadRequest(new { success = false, error = "A pergunta não pode estar vazia." });

            await ProcessarPerguntaAsync(model.Pergunta);

            var historico = await _db.MensagensChat
                                     .AsNoTracking()
                                     .OrderByDescending(m => m.DataHora)
                                     .Take(quantidadeHistoricoInicial)
                                     .OrderBy(m => m.DataHora)
                                     .ToListAsync();

            var rendered = await RenderPartialViewToStringAsync("_ChatMessagesPartial", historico);

            return Json(new { success = true, html = rendered });
        }

        /// <summary>
        /// Processa a pergunta e salva resposta da IA
        /// </summary>
        private async Task ProcessarPerguntaAsync(string pergunta)
        {
            var chatHistory = await _db.MensagensChat
                                       .OrderByDescending(m => m.DataHora)
                                       .Take(quantidadeHistoricoIA)
                                       .ToListAsync();

            // 2. Obtém o contexto dinâmico fortemente tipado
            var (tipoContexto, contextoObj) = await ObterContextoDinamicoAsync(pergunta);


            // No método ProcessarPerguntaAsync
            var debugJson = JsonConvert.SerializeObject(contextoObj, Formatting.Indented);
            _log.LogInformation($"[DEBUG IA] Contexto enviado para IA ({tipoContexto}): {debugJson}");


            // 3. Monta prompt para IA (o helper pode receber um objeto tipado, pois JSON será igual)
            var mensagens = OpenAIHelper.BuildPrompt(
                pergunta,
                contextoObj,
                chatHistory,
                _openAiSettings.SystemPrompt
            );

            // 4. Chama OpenAI
            var respostaIa = await _openAI.ChatAsync(mensagens);

            // 5. Converte markdown para HTML
            var pipeline = new MarkdownPipelineBuilder()
                               .UseAdvancedExtensions()
                               .UseSoftlineBreakAsHardlineBreak()
                               .Build();
            var htmlIa = Markdown.ToHtml(respostaIa, pipeline);

            // 6. Persiste pergunta + resposta
            var timestampAtual = DateTime.Now;
            _db.MensagensChat.AddRange(
                new MensagemChat
                {
                    Usuario = _user.ObterApelido(),
                    Conteudo = pergunta,
                    EhRespostaIA = false,
                    DataHora = timestampAtual
                },
                new MensagemChat
                {
                    Usuario = "Assistente IA",
                    Conteudo = htmlIa,
                    EhRespostaIA = true,
                    DataHora = timestampAtual
                });

            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Determina o tipo de contexto e retorna o contexto fortemente tipado já pronto
        /// </summary>
        private async Task<(TipoContexto, object)> ObterContextoDinamicoAsync(string pergunta)
        {
            TipoContexto tipoContextoDeterminado = TipoContexto.Clientes; // Padrão

            foreach (var kvp in ContextKeywords)
            {
                if (kvp.Value.Any(keyword => pergunta.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                {
                    _log.LogInformation("Palavra-chave '{KeywordTrigger}' encontrada para o contexto '{ContextName}'.",
                                        kvp.Value.First(kw => pergunta.Contains(kw, StringComparison.OrdinalIgnoreCase)),
                                        kvp.Key);

                    if (kvp.Key == "Fornecedores")
                    {
                        tipoContextoDeterminado = TipoContexto.Fornecedores;
                        break;
                    }
                    if (kvp.Key == "Vendedores")
                    {
                        tipoContextoDeterminado = TipoContexto.Vendedores;
                        break;
                    }
                    if (kvp.Key == "Clientes")
                    {
                        tipoContextoDeterminado = TipoContexto.Clientes;
                        break;
                    }
                }
            }

            _log.LogInformation("Carregando dados para o contexto: {Contexto}", tipoContextoDeterminado);

            switch (tipoContextoDeterminado)
            {
                case TipoContexto.Fornecedores:
                    var fornecedores = await _agenteRepo.ObterContextoFornecedoresParaIAAsync();
                    return (TipoContexto.Fornecedores, fornecedores);
                case TipoContexto.Vendedores:
                    var vendedores = await _agenteRepo.ObterContextoVendedoresParaIAAsync();
                    return (TipoContexto.Vendedores, vendedores);
                case TipoContexto.Clientes:
                    var clientes = await _agenteRepo.ObterContextoClientesParaIAAsync();
                    return (TipoContexto.Clientes, clientes);
                default:
                    clientes = await _agenteRepo.ObterContextoClientesParaIAAsync();
                    return (TipoContexto.Clientes, clientes);
            }

        }

        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException(nameof(viewName));

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (!viewResult.Success)
                    throw new InvalidOperationException($"A view '{viewName}' não foi encontrada.");

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString();
            }
        }

        public class ChatAjaxInput
        {
            public string Pergunta { get; set; }
        }
    }
}
