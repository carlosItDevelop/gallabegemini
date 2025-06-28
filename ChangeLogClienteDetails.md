## ChangeLog / Task List: Refatorações e UI Cliente - 05/05/2025

Esta lista detalha as tarefas concluídas e pendentes relacionadas à refatoração das entidades filhas (relação 1:N com Pessoa), adoção de Domínio Rico, e implementação da interface de gerenciamento via OffCanvas nas views de Cliente.

**Status:**
*   `[x]` - Concluído
*   `[✓]` - Concluído com Refinamentos (SweetAlert/Toastr)
*   `[!]` - Atenção/Pendente (Validação de Negócio)
*   `[ ]` - Pendente

---

### I. Mudanças no Modelo de Domínio (Entidades)

*   **DadosBancarios:** `[x]` (Completo conforme lista anterior)
*   **Telefone:**
    *   `[x]` Adicionar propriedade `PessoaId` (FK para Pessoa).
    *   `[x]` Adicionar propriedade de navegação `Pessoa`.
    *   `[x]` Remover coleção `ICollection<PessoaTelefone>`.
    *   `[x]` Adicionar métodos `SetDDD`, `SetNumero`, `SetTipoDeTelefone`, `DefinePessoa`.
    *   `[x]` Atualizar construtor para incluir `PessoaId`.
*   **Pessoa:**
    *   `[x]` Remover coleção `ICollection<PessoaTelefone>`.
    *   `[x]` Adicionar coleção inversa `ICollection<Telefone> Telefones`.
*   **PessoaTelefone:**
    *   `[x]` Excluir entidade do projeto.
*   **Cliente (Agregado Raiz):**
    *   `[x]` Adicionar método `AdicionarTelefone(...)`.
    *   `[x]` Adicionar método `AtualizarTelefone(...)`.
    *   `[x]` Adicionar método `RemoverTelefone(...)`.
    *   `[!]` **Pendente:** Adicionar validação de negócio para evitar telefones duplicados (mesmo DDD, Número, Tipo) para o mesmo `PessoaId` dentro dos métodos `AdicionarTelefone` e `AtualizarTelefone`.

### II. Mudanças na Camada de Infraestrutura

*   **DbContext (`AppDbContext`):**
    *   `[x]` Remover `DbSet<PessoaTelefone>`.
*   **Mapeamento (Fluent API):**
    *   `[x]` Excluir `PessoaTelefoneMap.cs`.
    *   `[x]` Atualizar `TelefoneMap.cs`:
        *   `[x]` Configurar relação `HasOne(Pessoa).WithMany(Telefones).HasForeignKey(PessoaId)`.
        *   `[x]` Definir `IsRequired()` e `OnDelete(DeleteBehavior.Cascade)`.
        *   `[x]` Adicionar índice para `PessoaId`.
*   **AutoMapper (`AutoMapperConfig.cs`):**
    *   `[x]` Adicionar mapeamento `Cliente.Pessoa.Telefones` -> `ClienteViewModel.Telefones`.
    *   `[x]` Garantir mapeamento `Telefone` <=> `TelefoneViewModel` (incluindo `PessoaId`).
*   **Seed Data:**
    *   `[x]` Reescrever `SeedDataTelefone.cs` para modelo 1:N, usando `DbContext` e associando `PessoaId`.
*   **Banco de Dados & Migrations (para Telefone):**
    *   `[x]` Dropar/Recriar banco (dev).
    *   `[x]` Gerar e aplicar novas migrations.
    *   `[x]` Validar Seed Data.
*   **Repositórios (`IClienteRepository` / `ClienteRepository`):**
    *   `[x]` Adicionar `ObterClienteComTelefones(Guid clienteId)`.
    *   `[x]` Adicionar `AdicionarTelefoneAsync(Cliente cliente, Telefone novo)`.
    *   `[x]` Adicionar `RemoverTelefoneAsync(Cliente cliente, Telefone telefone)`.
    *   `[x]` Atualizar `ObterClienteCompleto` para incluir `Telefones`.
    *   `[ ]` Criar `ITelefoneRepository` e `TelefoneRepository` dedicados (adiado).

### III. Mudanças na Camada de Domínio (Serviços e Eventos)

*   **Serviços de Domínio (`ClienteDomainService`):**
    *   `[x]` Adicionar/Implementar `Adicionar/Atualizar/RemoverTelefoneAsync` em `IClienteDomainService` e `ClienteDomainService`.
*   **Eventos de Domínio:**
    *   `[x]` Criar `TelefoneAdicionadoEvent`.
    *   `[x]` Criar `TelefoneAtualizadoEvent`.
    *   `[x]` Criar `TelefoneRemovidoEvent`.
    *   `[x]` Integrar disparo desses eventos nos métodos da entidade `Cliente`.
    *   `[ ]` Criar Handlers para os novos eventos de `Telefone` (adiado).

### IV. Mudanças na Camada de Aplicação (Controller & ViewModel)

*   **ViewModel (`TelefoneViewModel.cs`):**
    *   `[x]` Adicionar propriedade `PessoaId`.
*   **ViewModel (`ClienteViewModel.cs`):**
    *   `[x]` Garantir coleção `List<TelefoneViewModel> Telefones`.
*   **Controller (`ClienteController.cs`):**
    *   `[x]` Atualizar `Details`/`Edit` (GET) para carregar `Telefones` (via `ObterClienteCompleto`).
    *   `[x]` Adicionar Actions AJAX: `GetTelefonesListPartial`, `GetTelefoneFormData`, `SalvarTelefone`, `ExcluirTelefone`.

### V. Mudanças na Camada de Apresentação (Views & JS)

*   **Views (`Details.cshtml`/`Edit.cshtml`):**
    *   `[x]` Incluir container (`#telefones-list-container`) na tab apropriada.
    *   `[x]` Chamar `Html.PartialAsync("_TelefonesListClientePartial", ...)`.
    *   `[x]` Incluir estrutura HTML do OffCanvas (`#offcanvasTelefone`) com formulário e `@Html.AntiForgeryToken()`.
*   **Partial Views (`_TelefonesListClientePartial.cshtml`):**
    *   `[x]` Implementar dinamicamente: receber `IEnumerable<>`, exibir tabela, botão "Adicionar", linhas clicáveis.
*   **JavaScript (para Telefone):**
    *   `[✓]` Implementar lógica AJAX completa (Adicionar, Editar, Excluir com SweetAlert).
    *   `[✓]` Implementar atualização da lista.
    *   `[✓]` Implementar feedback ao usuário (Toastify/SweetAlert).
    *   `[ ]` Refinar `showToast` para usar consistentemente Toastr.js do Velzon (adiado).

---
