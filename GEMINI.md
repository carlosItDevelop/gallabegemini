# Gemini Project Configuration



## About This Project
Esta é uma solução .NET/C# chamada `GeneralLabsSolutions`. Ela inclui múltiplos projetos, como a aplicação web ASP.NET Core `VelzonModerna` e várias bibliotecas de classe para domínio, infraestrutura e serviços.

## Coding Style
- Siga as convenções de codificação C# e ASP.NET Core existentes.
- Use os padrões de injeção de dependência estabelecidos.
- Mantenha a estrutura arquitetônica existente (Controllers, Services, Repositories, etc.).

## Commit Messages
- As mensagens de commit devem ser escritas em inglês.

## Portuguese Output Default
Todas as respostas devem ser em **português (pt-BR)**, exceto trechos de código, nomes de classe ou mensagens de commit, que permanecem em inglês.

## File Searching
- Ao procurar por arquivos, utilize a estrutura de namespaces como a principal referência para a localização. Se, mesmo assim, não encontrar o arquivo, você deve me perguntar pela localização.

## File Resolution
Ao referenciar arquivos, pesquise primeiro pelo **namespace completo** em toda a solução.  
Se não encontrar, pergunte-me onde o arquivo está antes de concluir que ele não existe.

## Important Notes
- Respeite a separação de responsabilidades entre os diferentes projetos (Domain, InfraStructure, Services).
- Antes de adicionar novas funcionalidades, verifique se os componentes existentes podem ser reutilizados.

## LINQ Performance
Identifique consultas que chamam `.ToList()` antes de filtrar ou paginar; proponha mover o ToList para o final ou usar `AsQueryable()`.

## Null-Safety Checks
Alerta se encontrar acesso direto a membros sem checar `null`. Sugira `ArgumentNullException.ThrowIfNull()` nos construtores.

## Async All The Way
Não misture chamadas síncronas em pipelines assíncronos. Alerte quando vir `.Result` ou `.Wait()` em código de produção.

## Markdown in Docs
Quando gerar README ou documentação, use títulos H2, listas ordenadas e exemplos de código. Evite tabelas quando uma lista simples for suficiente.

## Culture-Invariant
Para conversão de números ou datas, peça para usar `CultureInfo.InvariantCulture` ou o `Culture` configurado na aplicação.

## Code Comment Style
Comentários XML devem conter `<summary>`, `<param>` e `<returns>` completos.  
Não crie comentários redundantes (“Gets or sets …”).

## Logging Guidelines
Se detectar blocos `try/catch` vazios ou sem log, sugira incluir **Serilog / ILogger** com mensagens claras. Nunca crie logs de nível *Fatal* em código de domínio.


