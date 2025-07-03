# Gemini Project Configuration

## Indexação da Solução
- Mantenha a solução sempre indexada. Antes de iniciar a interação com o usuário, monte uma árvore com os nomes dos arquivos de cada pasta da solução, formando um "Mapa da Solução".

## About This Project
Esta é uma solução .NET/C# chamada `GeneralLabsSolutions`. Ela inclui múltiplos projetos, como a aplicação web ASP.NET Core `VelzonModerna` e várias bibliotecas de classe para domínio, infraestrutura e serviços.

## Common Commands
- **Construir a solução:** `dotnet build GeneralLabsSolutions.sln`
- **Executar testes:** `dotnet test` (Assumindo que os testes fazem parte da solução)

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

## CancellationTokens
Para métodos *async* públicos, verifique se existe parâmetro `CancellationToken`. Caso não, sugira incluí-lo, se o caso pedir, e/ou, permitir.

## Async All The Way
Não misture chamadas síncronas em pipelines assíncronos. Alerte quando vir `.Result` ou `.Wait()` em código de produção.

## Secrets Handling
Nunca inclua chaves ou strings de conexão no código sugerido. Use **UserSecrets ou variáveis de ambiente** e aponte onde configurá-las.

## Markdown in Docs
Quando gerar README ou documentação, use títulos H2, listas ordenadas e exemplos de código. Evite tabelas quando uma lista simples for suficiente.

## Culture-Invariant
Para conversão de números ou datas, peça para usar `CultureInfo.InvariantCulture` ou o `Culture` configurado na aplicação.

## SOLID Reminder
Valide se as classes respeitam **SRP** e **OCP**; aponte violações com exemplos concretos e confirme se quer proceguir a implementação assim mesmo.

## Avoid Unwanted Packages
Não adicione dependências externas sem permissão explícita. Prefira soluções com a **BCL ou bibliotecas já referenciadas** no projeto.

## Code Comment Style
Comentários XML devem conter `<summary>`, `<param>` e `<returns>` completos.  
Não crie comentários redundantes (“Gets or sets …”).

## Ask Before Large Operations
Se a mudança afetar mais de **5 arquivos** ou qualquer `.csproj`, pergunte se deve prosseguir ou dividir em etapas menores.

## Logging Guidelines
Se detectar blocos `try/catch` vazios ou sem log, sugira incluir **Serilog / ILogger** com mensagens claras. Nunca crie logs de nível *Fatal* em código de domínio.


