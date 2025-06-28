using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Services;
using Newtonsoft.Json;


namespace VelzonModerna.Helpers;

public static class OpenAIHelper
{

    public static List<ChatMessage> BuildPrompt(
    string pergunta,
    object dadosContextuais,
    IEnumerable<MensagemChat> historico,
    string systemPrompt // <- Passe aqui o texto do seu settings!
    )
    {
        var msgs = new List<ChatMessage>
        {
            new("system", systemPrompt),
            new("system", "Dados JSON: " + JsonConvert.SerializeObject(dadosContextuais, Formatting.None))
        };

        msgs.AddRange(historico
            .OrderBy(m => m.DataHora)
            .Select(m => new ChatMessage(
                m.EhRespostaIA ? "assistant" : "user",
                m.Conteudo)));

        msgs.Add(new ChatMessage("user", pergunta));
        return msgs;
    }


}
