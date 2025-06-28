### Changelog 2 - Melhorias e NewFeatures

- `ThenInclude`:
- [ ] Por query com `ThenInclude` nos Repositórios;
	- [ ] Produto, mostrar Categoria;
	- [ ] Categoria, mostrar Lista de Produtos (Details);
	- [ ] Cliente, mostrar Pedidos (Details);
	- [ ] Pedido, mostrar Cliente, Vendedor, Fornecedor (em linhas com botão de edição) e ItemPedido (Details);
	- [ ] Etc.
	
# Checklist da Feature: Kanban Dinâmico ✔

---

**Equipe Cooperchip, vamos revisar os seguintes arquivos:**

**Backend:**

- **Model:**
    - `GerenciadorDeFluxoKanban.cs` (com a estrutura e os métodos corretos)
    - `MembroEquipe.cs` (se você optou por usar uma tabela separada para os membros)
- **Interface do Repositório:**
    - `IGerenciadorDeFluxoKanbanRepository.cs`
- **Implementação do Repositório:**
    - `GerenciadorDeFluxoKanbanRepository.cs`
- **Controller:**
    - `TarefasController.cs` (com a action `KanbanGlSolutions` e outras actions para as funcionalidades AJAX, como `AtualizarStatusTarefa`, `CriarTarefa` e `AdicionarMembrosTarefa`)
- **Seed Data:**
    - `SeedDataGerenciadorDeFluxoKanban.cs` (com a lógica de geração de dados)

**Frontend:**

- **View:**
    - `KanbanGlSolutions.cshtml` (adaptada para exibir dados dinâmicos e com os IDs corretos)
- **JavaScript:**
    - `kanban-gl-solutions.init.js` (refatorado para buscar os dados via AJAX, renderizar o Kanban, implementar o Drag and Drop e as funcionalidades AJAX de criar, atualizar e adicionar membros às tarefas)

**Configurações:**

- **Injeção de Dependência:**
    - `Startup.cs` ou `Program.cs` (verificar se o `GerenciadorDeFluxoKanbanRepository` está sendo injetado corretamente no container de DI)

**Testes:**

- **Testes Unitários:**
    - Criar testes unitários para o Repositório (`GerenciadorDeFluxoKanbanRepository`) e para a Controller (`TarefasController`) para garantir que a lógica está funcionando corretamente.
- **Testes de Integração:**
    - Criar testes de integração para validar a comunicação entre o frontend e o backend.

**Outros Pontos de Atenção:**

- **Segurança:** Verificar se os mecanismos de autenticação e autorização estão implementados para proteger o Kanban de acessos não autorizados.
- **Performance:** Avaliar a performance do Kanban, principalmente em cenários com um grande volume de dados. Otimizar consultas ao banco de dados e o código do frontend, se necessário.

**Líder Técnico:**  Equipe,  revisem cada item do checklist com atenção! Qualquer dúvida ou problema,  é só falar! 😊

	