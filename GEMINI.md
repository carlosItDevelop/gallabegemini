# Gemini Project Configuration

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

## File Searching
- Ao procurar por arquivos, utilize a estrutura de namespaces como a principal referência para a localização. Se, mesmo assim, não encontrar o arquivo, você deve me perguntar pela localização.

## Indexação da Solução
- Mantenha a solução sempre indexada. Antes de iniciar a interação com o usuário, monte uma árvore com os nomes dos arquivos de cada pasta da solução, formando um "Mapa da Solução".

## Important Notes
- Respeite a separação de responsabilidades entre os diferentes projetos (Domain, InfraStructure, Services).
- Antes de adicionar novas funcionalidades, verifique se os componentes existentes podem ser reutilizados.
