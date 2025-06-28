# ChangeLog

## Versão 1.1.0 (Em Progresso)

### Imagens em Produto
- [ ] Imagem no Produto (ViewModel terá Imagem e ImagemUpload, além de uma Imagem padrão).
- [ ] Imagem para Thumbnail (para a lista de Produtos, diferente das Imagens de Detalhes de Produtos, que são maiores).

---

## Versão 1.0.0 (MVP)

### Pacotes Instalados
- [x] Install AutoMapper.
- [x] Install FluentValidation no Domain Project.
- [x] Install MediatoR no Domain Project.
- [x] Install EntityFrameworkCore.
- [x] Install EntityFrameworkCore.SqlServer.
- [x] Install EntityFrameworkCore.Design.
- [x] Install EntityFrameworkCore.Tools.

### Modelos
- [x] Criar Produto.
- [x] Criar CategoriaProduto.
- [x] Criar Vendedor.
- [x] Criar Cliente.
- [x] Criar Pedido.
- [x] Criar ItemPedido.
- [x] Criar Fornecedor.

### Mapeamentos AutoMapper
- [x] Criar AutoMapperConfig com ReverseMap.
- [x] Cliente => ClienteViewModel.
- [ ] Produto => ProdutoViewModel.
- [ ] CategoriaProduto => CategoriaProdutoViewModel.
- [ ] Fornecedor => FornecedorViewModel.
- [ ] Pedido => PedidoViewModel.
- [ ] ItemPedido => ItemPedidoViewModel.
- [ ] Vendedor => VendedorViewModel.
- [x] Registrar AutoMapperConfig.

### Mapeamentos Fluent API (EF Core)
- [x] ProdutoMap.
- [x] CategoriaProdutoMap.
- [x] VendedorMap.
- [x] ClienteMap.
- [x] PedidoMap.
- [x] ItemPedidoMap.
- [x] FornecedorMap.

### Repositórios
- [x] Criar GenericRepository e IGenericRepository.
- [x] Criar IQueryRepository e QueryRepository.
- [ ] Registrar IQueryRepository e QueryRepository.

### AppDbContext (EF Core)
- [x] Configuração padrão de string/varchar para 100 caracteres (quando omisso).
- [x] Configuração de Cascade Restrict em deleções.
- [x] Configuração de transações em Add e Update corrigidas.
- [x] Método de extensão `AddGeneralLabsDbConfig` para configurar a conexão com o banco de dados SQL Server.

### SeedData
- [x] SeedDataProduto.
- [x] SeedDataCategoriaProduto.
- [x] SeedDataCliente.
- [x] SeedDataPedido.
- [x] SeedDataItemPedido.
- [x] SeedDataVendedor.
- [x] SeedDataFornecedor.

### ViewModels (Aplicação)
- [x] FornecedorViewModel.
- [x] VendedorViewModel.
- [x] ClienteViewModel.
- [x] PedidoViewModel.
- [x] ProdutoViewModel.
- [x] CategoriaProdutoViewModel.
- [x] ItemPedidoViewModel.

### Controllers
- [x] Criar FornecedorController.
- [x] Criar VendedorController.
- [x] Criar ClienteController.
- [x] Criar PedidoController.
- [x] Criar ProdutoController.

#### Domain Services
- [x] Criar ClienteDomainService + Interface.
- [ ] Criar FornecedorDomainService + Interface.
- [ ] Criar VendedorDomainService + Interface.
- [ ] Criar ProdutoDomainService + Interface.
- [ ] Criar CategoriaDomainService + Interface.
- [ ] Criar PedidoDomainService + Interface.

---

## Validações
- [x] Criar FornecedorValidation.
- [ ] Criar VendedorValidation.
- [ ] Criar ProdutoValidation.
- [ ] Criar PedidoValidation.
- [ ] Criar CategoriaValidation.
- [x] Criar ClienteValidation com validação de CPF/CNPJ.
- [x] Criar DeleteClienteValidation, sem a validação de Documentos e outras desnecessárias na exclusão;
- [x] Configurar validação com base no `TipoDePessoa` (física ou jurídica).
- [x] Validar regra de inadimplência para impedir atualizações de clientes inadimplentes.
- [x] Exibição de erros de regra de negócio no Edit e Create corrigida.

---

## Notificações
- [x] Criar INotificador + Interface.
- [x] Criar Notificacao.
- [x] Ajustar o sistema de notificações no BaseService, centralizando validações e notificações.
- [x] Garantir que todas as Controllers utilizem o INotificador no MainController.

---

## Scripts
- [x] Correção na ordem de carregamento do jQuery, garantindo que ele seja carregado antes do jquery.validate.min.js e jquery.validate.unobtrusive.min.js.
- [x] Garantir que a validação funciona corretamente com arquivos locais de jquery.validate.min.js e jquery.validate.unobtrusive.min.js.

---

## Views
- [x] Correção no uso de _ValidationScriptsPartial.
- [x] Alteração de asp-validation-summary para "All" no Default.cshtml do componente de sumário.
- [x] Simplificação na SummaryViewComponents, removendo o Task.FromResult(model) desnecessário no InvokeAsync.

---

## GS Dashboard
### Cards e ViewComponents
- [x] Criar link para GS Dashboard.
- [x] Criar Cards para GS Dashboard.
- [x] Reutilizar ViewComponents de Consolidação de Pedidos no GL Dashboard.
- [x] Criar primeiro ViewComponent funcional do GS Dashboard.
- [x] Criar PartialView para `Atividades Recentes` no GL Dashboard.
- [ ] Criar dados reais para exibir em `Atividades Recentes`.

### Gráficos para GS Dashboard
- [x] Criar componente HTML para gráficos.
- [x] Criar jQuery/Ajax para alimentar o gráfico.
- [ ] Avaliar criação de um ViewComponent para alimentar o gráfico.

---

## Outros
- [x] Adicionar DataValidade em Produto.
- [x] Adicionar `Imagem Padrão` em Produto.
- [x] Criar SeedData para ItemPedido junto com Pedido.
- [x] Refatorar MainController para compatibilidade com MVC.
- [x] Criar Rotas Personalizadas em ProdutoController.
- [x] Criar Rotas Personalizadas em PedidoController, FornecedorController, ClienteController, e ContatoController.
- [x] Implementar global.json.
- [x] Criar e configurar User Secrets.
- [ ] Usar Select2 em alguns Select (exemplo em Forms => FormsLayouts => Auto Sizing).
- [x] Mover Interfaces de Domínio para o diretório Interfaces.
- [x] Validação de TipoDePessoa em FornecedorValidation e ClienteValidation.
- [x] Ajuste da Controller `MainController` com o método `OperacaoValida` que verifica se tem "Notificações";
- [x] Nas controllers, passar INotificador para a classe base `MainController`;

---

## Kanban e Todo
- [x] Add models  `GerenciadorDeFluxoKanban.cs` and `MembroEquipe.cs`, `KanbanGlSolutions.cshtml` and `kanban-gl-solutions.init.js`;
- [x] Criar `GerenciadorDeFluxoKanbanMap`;
- [x] Criar `MembroEquipeMap`;
- [x] Add `GerenciadorDeFluxoKanban` and `MembroEquipe` in `AppDbContext`;
- [x] Roda a Migration para adicionar as Models acima na Base de dados;

## Bugs e Novas Features
- [x] **Bug**: Na View de Edit de Cliente, o campo Nome não está sendo validado. (fixed - jquery-unobstrutive adjuted)
- [x] Ao tentar gravar um Cliente com `TipoDePessoa` inválido, não mostra a notificação. (fixed)
- [x] Não conseguimos atualizar `StatusDoCliente` para Especial. (fixed - Retirei NotEmpty de ClienteNotification)
- [ ] Implementar Rotas Personalizadas para várias Controllers (Fornecedor, Cliente, Contato).
- [ ] Usar Select2 para melhorar o UX de alguns `Selects`.
- [ ] Implementar ImagemUpload;
- [ ] Criar Model Endereço, ViewModelEndereco, EnderecoMap, EnderecoValidation;
- [ ] Criar EnderecoApplicationService + Interface, EnderecoRepository + Interface, EnderecoDomainService + Interface, Registrar as duas dependências;
- [ ] ConsolidadoClienteRepository - Responsável por toda a Query de Relatórios e Consolidados de um modo geral;

---

### Commits e Boas Práticas
- [x] Realizar commits menores e mais frequentes para manter um histórico claro.
- [x] Usar mensagens de commit descritivas, refletindo exatamente as mudanças feitas (Exemplo: "Correção de scripts de validação" ou "Adição de validação de CPF/CNPJ no ClienteValidation").


---

- Modelo de Código para Html.GetEnumSelectList e PartialView:

```cshtml
	<div class="form-group">
		<label class="control-label">Status do Cliente</label>
		<select asp-for="StatusDoCliente" class="form-control" asp-items="ViewBag.StatusDoCliente">
			<option value="">.:: Selecione um Status ::.</option>
		</select>
		<span asp-validation-for="StatusDoCliente" class="text-danger"></span>
	</div>

	<div class="form-group">
		<label class="control-label">Tipo de Cliente</label>
		<select asp-for="TipoDeCliente" class="form-control" asp-items="ViewBag.TipoDeCliente">
			<option value="">.:: Selecione um Tipo ::.</option>
		</select>
		<span asp-validation-for="TipoDeCliente" class="text-danger"></span>
	</div>
	<div class="form-group">
		<label class="control-label">Tipo de Pessoa</label>
		<select asp-for="TipoDePessoa" class="form-control" asp-items="Html.GetEnumSelectList<TipoDePessoa>()">
			<option value="">.:: Selecione um Tipo ::.</option>
		</select>
		<span asp-validation-for="TipoDePessoa" class="text-danger"></span>
	</div>
```

### Processos de Criação da Controller e das Views de Pedido

- Criar uma Lista de Pedidos em PedidoController / Index:
- Ao clicar em "Detalhes" de um "Pedido", exibir os detalhes do Pedido, depois duas linhas: A primeira com informações básicas do Vendedor e um Botão de Editar. A segunda com alguns dados do Cliente e um botão de Editar. Logo abaixo destas duas linhas mostrar uma tabela com os Itens do Pedido, cada linha com as opções de "Ecluir" o Item do Pedido e "Editar", para editarmos a quantidade de Item no Pedido;
- Avaliar a possibilidade de uso de `Modal` ou `OffCanvas` para edição de Quantidade de Itens no Pedido e Detalhes de Vendedor e Cliente;
- Avaliar as Possibilidades de utilização de Partial Views;
- Avaliar a possibilidade de Edição do Pedido, em si. "DataPedido" e "StatusDoPedido";

---

Essas ideias preliminares para a criação da controller e das views de Pedido parecem bem estruturadas e cobrem aspectos importantes da funcionalidade esperada. Vou destacar algumas sugestões, críticas construtivas, e possíveis dificuldades para ajudar a planejar a implementação de forma eficaz.

1. Lista de Pedidos (PedidoController / Index)
Sugestões:

A lista de pedidos deve permitir filtragem e paginação, principalmente se houver muitos registros. Filtros por StatusDoPedido, DataPedido, Vendedor, e Cliente podem ser úteis.
Adicionar uma funcionalidade de pesquisa rápida por ID do Pedido, Nome do Cliente, ou Nome do Vendedor.
Mostrar o StatusDoPedido com um badge colorido para facilitar a visualização do estado atual de cada pedido.
Dificuldades:

Manter a performance em uma lista extensa pode ser um desafio. Implementar paginação e carga assíncrona (com AJAX) para evitar sobrecarga de dados pode ser necessário.
2. Detalhes do Pedido
Sugestões:
Exibir informações detalhadas do pedido em um layout claro e organizado. Usar uma combinação de tabs ou accordion para dividir seções como Dados do Pedido, Vendedor, Cliente, e Itens do Pedido.
Adicionar a possibilidade de exportar detalhes do pedido em PDF ou outros formatos, o que pode ser útil para documentação ou compartilhamento.
Dificuldades:
Gerenciar estados de edição pode ser complicado se muitas operações de edição forem permitidas simultaneamente. Por exemplo, se o vendedor, cliente, e itens podem ser editados ao mesmo tempo, é importante gerenciar os estados com precisão.
Sincronizar atualizações em tempo real se múltiplos usuários estiverem trabalhando no mesmo pedido pode ser um desafio.
3. Edição de Vendedor e Cliente
Sugestões:

Usar Modals ou OffCanvas para editar os detalhes do vendedor e do cliente pode melhorar a UX, mantendo o usuário na mesma página sem recarregar o conteúdo principal.
Validar as mudanças antes de salvar, especialmente se essas entidades forem usadas em outros pedidos. Talvez adicionar um aviso ou confirmação para o usuário ao modificar informações críticas.
Dificuldades:

Garantir que as mudanças não impactem negativamente outros pedidos que possam estar vinculados ao mesmo vendedor ou cliente. Implementar validações robustas e, se necessário, criar históricos de alterações.
4. Edição dos Itens do Pedido
Sugestões:

Um Modal para editar a quantidade de itens seria uma boa abordagem, evitando a recarga da página. Alternativamente, usar Partial Views com AJAX para carregar e enviar dados sem afetar a experiência do usuário.
Implementar a possibilidade de adicionar novos itens ao pedido, caso isso seja permitido no processo de negócios.
Dificuldades:

Sincronizar a atualização dos valores totais do pedido ao editar os itens pode ser complexo, especialmente se múltiplas mudanças ocorrerem simultaneamente. A precisão no cálculo e na apresentação dos valores é crítica.
5. Possibilidades de Edição do Pedido
Sugestões:

Permitir a edição de DataPedido e StatusDoPedido diretamente na view de detalhes do pedido, usando dropdowns ou pickers para facilitar a seleção.
Adicionar logs ou auditoria para acompanhar quem alterou o status e quando, o que pode ser útil para rastrear o histórico do pedido.
Dificuldades:

Implementar restrições para editar certas informações, dependendo do status do pedido (por exemplo, não permitir que o status seja alterado para "Enviado" se os itens ainda não foram processados).
6. Partial Views e AJAX
Sugestões:

Utilizar Partial Views para os componentes de edição, carregando-os dinamicamente via AJAX, pode melhorar a performance e a experiência do usuário.
Partial Views para a lista de itens dentro do pedido, que possam ser atualizadas independentemente, são recomendadas.
Dificuldades:

Gerenciar a consistência de dados entre os componentes carregados via AJAX e o servidor pode ser complicado, exigindo uma implementação cuidadosa para garantir que as alterações sejam persistentes e refletidas corretamente.
Conclusão:
As ideias apresentadas são sólidas e seguem boas práticas de desenvolvimento, especialmente com a consideração de modais, partial views, e controle granular de edição. A principal dificuldade será garantir que o sistema seja responsivo e escalável, especialmente à medida que o volume de dados cresce. Usar AJAX para operações dinâmicas, aplicar validações robustas, e garantir a sincronização de estados são pontos chave a serem considerados na implementação.



---

### Transcrição revisada da Conversa entre Lucas Ferreira e o CEO da Gal. Labs Solutions

Aqui está a versão revisada e mais profissional da conversa entre Lucas Ferreira e o CEO da Gal. Labs:
---

**Lucas Ferreira:** Bom dia! Podemos conversar sobre o sistema? Tenho algumas perguntas que gostaria de fazer, e seria ótimo se você pudesse me explicar tudo com o máximo de detalhes possível. Entendo que você deseja utilizar o sistema para administrar sua empresa. Durante o desenvolvimento, notei que o fluxo das vendas começa como uma proposta e depois se transforma em um pedido de compra. Quando o cliente faz a compra, precisamos gerenciar como vamos adquirir os produtos, determinar os fornecedores, prazos, etc. Além disso, é necessário controlar o fluxo financeiro, como o dinheiro recebido, pagamentos à vista ou a prazo. É isso mesmo?

**CEO:** Sim, exatamente. Há uma distinção importante entre compra e venda. Temos um processo licitatório ou uma consulta de preço que ocorre por e-mail. O cliente entra em contato conosco, formulamos uma proposta e enviamos por e-mail. Todo o processo de compras se inicia através dessas interações por e-mail.

**Lucas Ferreira:** Então, todas as compras são realizadas nesse formato, correto?

**CEO:** Sim, todas. O processo segue o fluxo de proposta, pedido de compra do cliente, e posteriormente o nosso pedido de compra aos fornecedores.

**Lucas Ferreira:** Entendi. Pensando nisso, você gostaria de ter um controle mais detalhado, por exemplo, sobre clientes que compraram à vista ou a prazo, e de que forma isso impacta o fluxo de caixa e a compra de materiais?

**CEO:** Exato. Há uma diferença entre a venda realizada e o faturamento. Vamos supor que você esteja em uma concessionária, onde um cliente deseja comprar um carro que será entregue em 30 dias. Nesse caso, há várias nuances no processo de faturamento e pagamento, que precisamos gerenciar.

**Lucas Ferreira:** Então, você gostaria de informatizar todo esse processo e ter relatórios detalhados sobre as vendas e pagamentos?

**CEO:** Isso mesmo. Para mim, é crucial ter controle sobre todo o processo – desde as vendas até os pagamentos. Eu gostaria de informatizar tudo para ter relatórios que me mostrem o desempenho das vendas, pagamentos realizados, prazos de pagamento, etc.

**Lucas Ferreira:** Entendi. Atualmente, você já tem algum tipo de controle sobre isso, mesmo que manual?

**CEO:** Sim, mas é tudo feito manualmente, e isso é um problema. A Bruna, minha filha, por exemplo, envia um relatório diário com o total de vendas, mas não temos um sistema para automatizar isso. O ideal seria ter um relatório que não só mostrasse as vendas realizadas, mas também o status dos pagamentos e entregas.

**Lucas Ferreira:** Sim, compreendo. Vamos incluir isso no sistema para otimizar o departamento de contas a receber, fornecendo relatórios precisos sobre os pagamentos realizados e os que estão pendentes.

**CEO:** Exatamente. Hoje, dependemos muito da memória e da organização manual para fazer esse controle, o que não é ideal. Seria ótimo se o sistema pudesse centralizar essas informações.

**Lucas Ferreira:** Perfeito. Vamos garantir que o sistema seja capaz de fornecer todos esses dados de forma automática e confiável. Agora, sobre os relatórios, você mencionou que gostaria de ver informações como clientes que não compram há algum tempo, ou clientes que compram regularmente. Podemos trabalhar nisso também?

**CEO:** Sim, gostaria de ter essa visão detalhada sobre o comportamento dos clientes, separando por área geográfica e frequência de compra. Além disso, seria interessante ver o desempenho dos vendedores, já que cada um tem sua própria carteira de clientes.

**Lucas Ferreira:** Entendido. Podemos criar relatórios específicos para acompanhar o desempenho de cada vendedor, bem como a evolução das vendas por cliente e por região. Algo mais que você gostaria de incluir?

**CEO:** Sim, seria interessante também vincular os produtos aos fornecedores, garantindo que a distribuição seja feita de forma eficiente e exclusiva quando necessário.

**Lucas Ferreira:** Compreendido. Vou incluir isso no sistema, garantindo que a vinculação de produtos aos fornecedores seja clara e que possamos rastrear tudo de maneira eficiente.

**CEO:** Perfeito. Também vamos precisar automatizar a geração de notas fiscais. Existem algumas nuances, como diferentes alíquotas de impostos dependendo da região de venda, que precisamos considerar.

**Lucas Ferreira:** Entendo. Vou buscar mais informações detalhadas sobre essas regras fiscais, e poderemos automatizar esse processo também. Podemos tratar disso em uma próxima reunião.

**CEO:** Combinado. Vamos agendar para discutir os detalhes posteriormente. Obrigado, Lucas.

---


### Anotações extras do projeto (Para Revisão)

Os produtos devem ter, no mínimo, para um produto viável:

Descrição em inglês e português, informações detalhadas, Código que segue o padrão (ABC1234-MERKEL200ml, onde "ABC1234" representa o código internacional do produto, "MERKEL" indica o fornecedor do produto, e "200ml" especifica uma característica do produto, como tamanhos diferentes: 200ml, 500ml, 1 litro, ou então P, M, G. Seriam três produtos diferentes nesse caso). O cliente mencionou que o custo em dólar poderia ficar no banco de dados, pois ele não se altera com frequência. No entanto, acredito que podemos encontrar outra forma de registrar o custo, como em um log ou histórico.

Como eu havia estruturado o banco de dados:
Eu criei as tabelas produtos, clientes e vendedores.
Criei também as tabelas que os relacionavam, que são:
- Propostas (armazenavam todas as propostas e tinham um campo `p`, `v`, `c` enum que definia se eram propostas de compra, venda, etc. Ela relacionava o vendedor e o cliente).

Os produtos devem ter fotos que os representem (imagens).

Vendedores e clientes devem ter um local onde são armazenados documentos ou arquivos relacionados a eles.

Produtos:
Possuem todos os campos pensados anteriormente e também:
- Imagens do produto.
- Arquivos relacionados ao produto.
- Campo "possui dados sensíveis" (sim ou não). Se for sim, ao cadastrá-lo em um pedido, deve ocorrer o seguinte:
  (o produto pode ser um consumível ou algo que possua validade, ou tempo de utilização, e este controle pode ser feito na hora de construir o pedido, pois o pedido irá solicitar as datas e dados essenciais dos produtos que possuam essa característica). Devem ser monitorados e gerar mensagens e notificações.

Vendedores:
- Nome.
- Emails.
- CPF como chave.
- Situação na empresa (campos ou texto que contenha seu cargo, responsabilidades e informações sobre sua relação com a empresa).
- Informações pessoais (pode ser um texto que contenha o que o dono acha dele, anotações, características, etc).
- Observações (campo ou log que contenha observações sobre o vendedor, como se ele é ou não um bom funcionário, o que já fez, conquistas, etc).
- Documentos (arquivos, imagens ou documentos que são vinculados ao vendedor; isso também pode se aplicar a cliente, fornecedor, pedido, etc).
- Status de ativo ou inativo.
- Categoria (vendedor premium, bom, novato; pode ser interessante essa filtragem).
- Fotos do vendedor.

Pedidos:

- Devem possuir versionamento, em forma de históricos, para controle dos orçamentos de forma sequencial. Inclusive quando se transformarem em pedidos de venda e de compra.

Sistema de fiscalização de vendedores:
- Onde os vendedores criam eventos em agenda, registrando o que fizeram em cada dia. O uso de Kanban nesse cenário seria benéfico.

Falar sobre custos dos produtos.

