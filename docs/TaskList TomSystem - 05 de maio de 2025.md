### TaskList TomSystem - 05 de maio de 2025

- [x] Lista de `DadosBancariosViewModel` em ClienteViewModel;
- [ ] Configurar o AutoMapper para mapear Cliente.Pessoa.DadosBancarios para ClienteViewModel.DadosBancarios.
- [x] Criar DadosBancariosAtualizadosEvent;
- [x] Criar DadosBancariosRemovidosEvent;
- [x] Criar DadosBancariosAdicionadosEvent;
- [ ] Criar os Handlers dos eventos de DadosBancarios;
- [ ] Carregamento da Pessoa: Os métodos assumem que a propriedade Cliente.Pessoa (e, por consequência, Pessoa.DadosBancarios se for lazy loading ou já incluída) está carregada quando eles são chamados. Isso geralmente é responsabilidade do Repositório ao buscar o agregado Cliente. Adicionei uma verificação básica (if (Pessoa == null)).
- [ ] Exemplo de como receber o `AggregateId`, abaixo:
---

```csharp
 // AggregateId aqui será o Cliente.Id
 public DadosBancariosAdicionadosEvent(Guid clienteId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoDeContaBancaria)
     : base(clienteId)
```
---

- [x] Adição de dados bancários na Model Cliente;
- [x] Atualização de dados bancários na Model Cliente;
- [x] Remoção de dados bancários na Model Cliente;
- [x] Métodos de manipulação de DadosBancarios na Interface IClienteDomainService;

> Nota: Usei parâmetros primitivos (string, Guid, enum) para os dados bancários. Uma alternativa seria criar um DTO (Data Transfer Object) específico, como DadosBancariosInputDto, para passar esses dados, o que pode ser mais organizado se houver muitos parâmetros. Por enquanto, vamos com tipos primitivos.

- [x] Adicionar Método ao IClienteRepository (Necessário): Como o serviço precisa carregar o Cliente com seus DadosBancarios, precisamos de um método no IClienteRepository (e sua implementação em ClienteRepository) para fazer isso eficientemente.

- [x] Task<Cliente?> ObterClienteComDadosBancarios(Guid clienteId);
- [x] Task<Cliente?> ObterClienteCompleto(Guid clienteId);

---

> **Observações:**
	- Injeção de Dependência: Certifique-se de que IClienteRepository está sendo injetado corretamente onde IQueryGenericRepository<Cliente, Guid> estava sendo usado no construtor do ClienteDomainService, se você decidiu substituir.
	
	- Carregamento Eager Loading: O método ObterClienteComDadosBancarios usa Include/ThenInclude para garantir que os dados necessários sejam carregados junto com o Cliente (Eager Loading). Isso evita problemas de referência nula ou lazy loading indesejado nos métodos de domínio.
	
	- Tratamento de Erros: O serviço agora usa try-catch para capturar exceções lançadas pelos métodos de domínio (como InvalidOperationException se a conta não for encontrada) e as notifica usando o INotificador.
	
	- Commit: Reitero que o CommitAsync não é chamado dentro desses métodos do serviço. A responsabilidade é da camada que chama o serviço (a ClienteController).
---

- [x] Remover IRepository da solution e verificar se há reflexo negativo no código (acho que não, pois está com referência ZERO;
- [ ] Remover `IQueryGenericRepository` e `QueryGenericRepository`. Seus métodos já estão dentro de `IGenericRepository` e `GenericRepository`;
- [x] Resolver a questão do Notificar(string msg) dentro de ClienteController;
- [x] Adicionar `Task<List<DadosBancarios>> ObterDadosBancariosPorClienteId(Guid pessoaId)` em ClienteRepository e sua Interface;
- [x] Adicionar `Task<DadosBancarios?> ObterDadosBancariosPorId(Guid dadosBancariosId)` em ClienteRepository e sua Interface;
- [x] AutoMapper reconfigurado para adaptar FormMember para Cliente/ClienteViewModel e ajustar Pessoa e PessoId entre Cliente e DadosBancarios;

