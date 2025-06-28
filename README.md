# Estrutura (informal) do Projeto GeneralLabSolutions:

## Visão Geral

O **GeneralLabSolutions** é uma aplicação desenvolvida para informatizar e automatizar os principais processos de uma empresa de importação e exportação de maquinários e itens de laboratório, operando através de dropshipping. O sistema é projetado para permitir que o dono da empresa gerencie todas as' operações de maneira eficiente, automatizando tarefas manuais e gerando relatórios detalhados para a tomada de decisões.

## Objetivos do Projeto

1. **Automatizar processos comerciais** como vendas, compras, orçamentos e pedidos.
2. **Fornecer ferramentas analíticas** para ajudar na gestão da empresa.
3. **Facilitar a comunicação e o relacionamento** com clientes e fornecedores.
4. **Gerenciar tarefas e atividades** dos vendedores e outros funcionários.
5. **Implementar um controle financeiro** eficiente com geração automatizada de notas fiscais e relatórios financeiros.

## Estrutura do Sistema

### 1. Processo Principal

O fluxo principal do sistema envolve a gestão de propostas de venda, pedidos de venda e pedidos de compra. Cada etapa do processo gera dados que alimentam relatórios e análises.

1. **Proposta de Venda:**
   - O vendedor inicia o contato com o cliente, podendo registrar um novo cliente ou selecionar um já existente.
   - Cria uma "Proposta de Venda" (em formato PDF) que inclui uma lista de produtos, quantidades, preços, e informações adicionais.
   - A proposta tem um prazo de expiração e pode ser revisada, gerando novas versões que devem ser rastreadas.
   - As propostas servem como base para negociações e decisões futuras.

2. **Pedido de Venda:**
   - Quando a proposta é aceita, transforma-se em um "Pedido de Venda", contendo as mesmas informações, com datas de efetivação e prazos de entrega.
   - Representa a necessidade do cliente e aguarda o processo de compra para a entrega do produto.

3. **Pedido de Compra:**
   - Após a compra do produto, o pedido de venda é convertido em um "Pedido de Compra".
   - Inclui detalhes sobre o fornecedor, logística, formas de pagamento, e prazos.
   - Serve para monitorar o status dos pedidos que estão sendo atendidos.

### 2. Relatórios e Análises

O sistema deve gerar uma variedade de relatórios e análises para apoiar a gestão da empresa, como:

- Desempenho de vendedores.
- Estatísticas de clientes, fornecedores, e produtos.
- Relatórios regionais de vendas (quais locais vendem mais/menos).
- Análise de vendas por períodos.
- Monitoramento de propostas, pedidos de venda e compra.

### 3. Controle de Funcionários e Tarefas

Implementar um sistema de **Kanban** para organizar as tarefas dos vendedores, incluindo:

- Registro de atividades diárias.
- Notificações para lembrar os vendedores de interações pendentes com clientes.
- Integração com o sistema de propostas e pedidos para lembretes automáticos.

### 4. Sistema de Mensageria

O sistema deve facilitar a comunicação com clientes através de:

- Notificações para recompra de produtos.
- Promoções e ofertas especiais baseadas no histórico de compras.
- Alertas sobre a validade de produtos e necessidade de reposição.

### 5. Sistema de Agenda

Uma agenda compartilhada deve permitir:

- Marcação de eventos e compromissos.
- Geração de notificações automáticas.
- Integração com o sistema de mensageria e controle de tarefas.

### 6. Cálculo de Preços

Implementar um sistema de automação para cálculos de preços, configurado pelo dono da empresa e utilizado pelos vendedores. Isso substituirá o processo manual onde os vendedores consultam o dono.

### 7. Geração de Documentos

- **Orçamentos em PDF:** Geração de orçamentos padronizados para envio aos clientes.
- **Notas Fiscais Automáticas:** Inclusão de cálculos necessários e controle de faturamento.

### 8. Controle de Estoque Interno

Embora a empresa opere por dropshipping, será necessário um sistema para:

- Monitorar produtos e insumos que sobram ou estão temporariamente disponíveis na empresa.
- Gerar relatórios sobre esses itens para otimização do uso.

### 9. Gestão de Dados e Snapshots

O sistema deve ser capaz de:

- Armazenar snapshots (instantâneos) das transações, permitindo o rastreamento histórico mesmo após alterações.
- Manter dados antigos acessíveis para relatórios e análises históricas.

## Perguntas para Discussão com o Contratante

1. **Necessidade de Estoque Interno:** Qual o volume e a frequência de itens que precisam ser gerenciados internamente? Vale a pena investir em um módulo de estoque interno?
   
2. **Relatórios e Dashboards:** Quais são as métricas mais críticas que o contratante deseja monitorar? Algum relatório específico que ainda não foi mencionado?
   
3. **Cálculos de Preço Automatizados:** Como os cálculos de preços são atualmente definidos? Existem fórmulas específicas ou regras que precisam ser configuradas no sistema?

4. **Integração com Sistemas Existentes:** Existe algum sistema atual que precisa ser integrado, como ERPs ou sistemas financeiros?

5. **Segurança e Backup:** Quais são as expectativas de segurança de dados e como os backups serão gerenciados? Qual é a política de retenção de dados?

6. **Escalabilidade:** Existe alguma previsão de crescimento da empresa que possa impactar a necessidade de escalabilidade do sistema?

## Próximos Passos

1. **Revisão e Aprovação:** Apresentar este documento ao contratante para revisão e aprovação.
2. **Desenvolvimento do MVP:** Iniciar o desenvolvimento do Produto Mínimo Viável (MVP) com foco no fluxo principal de propostas, pedidos de venda e compra.
3. **Integração e Testes:** Implementar os módulos de integração e realizar testes exaustivos.
4. **Feedback e Ajustes:** Recolher feedback do contratante e fazer os ajustes necessários antes do lançamento.

## Links Importantes

- [Consultar e Estudar a Estratégia de Snapshot] (https://backupgarantido.com.br/blog/o-que-e-snapshot/)

### CodeBase

- Nossa classe de Produto:

```csharp
using GeneralLabSolutions.Domain.Entities.Base;
using GeneralLabSolutions.Domain.Enums;

namespace GeneralLabSolutions.Domain.Entities
{
    public class Produto : EntityBase
	{
		public Produto(string? codigo, 
					   string? descricao, 
					   string ncm, 
					   decimal valorUnitario, 
					   Guid categoriaId, 
					   Guid fornecedorId)
		{
			Codigo = codigo;
			Descricao = descricao;
			Ncm = ncm;
			ValorUnitario = valorUnitario;
			CategoriaId = categoriaId;
			FornecedorId = fornecedorId;
		}

		// EF
		public Produto() { }

		public string? Codigo { get; init; }
		public string? Descricao { get; private set; }
		public string? Ncm { get; private set; }
		public decimal ValorUnitario { get; private set; }
		public StatusDoProtudo Status { get; set; } 
			= StatusDoProtudo.Ativo;

		public DateTime DataDeValidade { get; set; } = DateTime.UtcNow.AddMonths(10);
		public string Imagem { get; set; } = "/cooperchip/imagens/img-padrao.png";

        // Todo: Alterar Status para StatusDoProduto



        // Relacionamentos
        public Guid CategoriaId { get; private set; }
		public virtual CategoriaProduto? CategoriaProduto { get; set; }

		public Guid FornecedorId { get; private set; }
		public virtual Fornecedor? Fornecedor { get; set; }

		// Métodos de atualização
		public void AlterarValorUnitario(decimal novoValor) => this.ValorUnitario = novoValor;
		public void AlterarDescricao(string descricao) => this.Descricao = descricao;

		public void AlterarStatus(StatusDoProtudo status) => this.Status = status;
	}
}


```
---

### Informação importante para Regras de Negócios:

> A sigla "NCM" refere-se à Nomenclatura Comum do Mercosul, que é um código utilizado para classificar mercadorias de acordo com a sua natureza, com o objetivo de padronizar e facilitar as operações de comércio exterior dentro dos países que compõem o Mercosul. Este código é essencial para determinar a tributação, tanto de importação quanto de exportação, e para o controle aduaneiro.

> No seu exemplo específico, o código "9027.90.99" é um código NCM que se refere a Instrumentos e aparelhos para análise física ou química, que não estão especificados em outros lugares. Este tipo de código é usado para classificar mercadorias de alta precisão, geralmente utilizadas em laboratórios ou processos industriais específicos.

---

## Desenhando o Módulo de Gestão de Leads e Clientes Potenciais

---

Vamos mergulhar fundo no acompanhamento de `Leads` para a General Lab Solutions, focando em livrá-los da burocracia e da perda de informações.

A ideia é criar um ecossistema coeso onde o `Lead` é o centro, e todas as interações, agendamentos e informações fluem de maneira inteligente e automatizada.

---

## Desenhando o Módulo de Gestão de Leads e Clientes Potenciais

**Objetivo Central:** Transformar a prospecção e o acompanhamento de leads em um processo fluido, rastreável, eficiente e menos dependente de controles manuais, culminando na conversão e fidelização do cliente.

**Princípios Chave:**

1. **Centralização da Informação:** Tudo sobre o lead em um só lugar.
2. **Automação Inteligente:** Reduzir tarefas repetitivas e proativamente guiar o usuário.
3. **Visibilidade e Transparência:** Fácil entendimento do status e histórico de cada lead.
4. **Colaboração:** Facilitar o trabalho em equipe, se aplicável.
5. **Experiência do Usuário (UX) Intuitiva:** Minimizar a curva de aprendizado e o atrito.

---

### **I. Telas / Features Principais e Interligações:**

### **1. Dashboard de Leads (Tela Inicial do Módulo)**

- **Visão Geral:**
    - KPIs chave: Novos Leads (hoje/semana/mês), Leads em Contato, Leads Qualificados, Taxa de Conversão (Lead -> Oportunidade), Atividades Agendadas para Hoje, Atividades Atrasadas.
    - Gráficos: Funil de Leads (Novos, Contatados, Qualificados, Proposta, Negociação), Leads por Origem, Leads por Vendedor.
    - Lista de "Próximas Ações Importantes": Follow-ups vencendo hoje/amanhã, leads sem contato há X dias.
- **Interligações:**
    - Cliques nos KPIs/gráficos levam a listas filtradas no "Quadro Kanban de Leads" ou "Lista de Leads".
    - Links diretos para atividades no "Calendário de Atividades".
- **Inteligência/Service Workers:**
    - Atualização em tempo real (ou quase-real) dos KPIs.
    - Notificações push (via Service Worker) para alertas críticos (ex: Lead importante estagnado).

### **2. Quadro Kanban de Leads (Pipeline Visual)**

- **Visão Geral:**
    - Colunas representando os estágios do lead (ex: Novo, Primeiro Contato, Qualificação, Demonstração Agendada, Proposta Enviada, Negociação, Follow-up Agendado, Ganho, Perdido, Nutrição).
    - Cards representando cada lead, exibindo informações essenciais (Nome do Lead, Empresa, Valor Potencial, Próxima Atividade, Responsável).
    - Drag-and-Drop para mover leads entre os estágios.
- **Ações no Card:**
    - Abrir "Detalhes do Lead".
    - Adicionar Nota Rápida.
    - Agendar Próxima Atividade ( ligação, email, reunião, follow-up).
    - Marcar como Ganho/Perdido.
- **Interligações:**
    - Mover um card pode disparar automações (ex: mover para "Proposta Enviada" pode criar uma tarefa para o financeiro preparar algo ou para o vendedor agendar um follow-up em X dias).
    - Filtros por vendedor, origem do lead, data de criação, tags.
- **Inteligência/Service Workers:**
    - Atualização visual instantânea para todos os usuários conectados.
    - Notificação ao responsável se um lead for movido para seu estágio.
    - Cards podem mudar de cor com base em regras (ex: lead "quente" ou lead "esquecido").

### **3. Lista de Leads (Visão Tabular)**

- **Visão Geral:**
    - Alternativa ao Kanban para quem prefere listas.
    - Colunas customizáveis (Nome, Empresa, Email, Telefone, Status, Responsável, Último Contato, Próxima Atividade, Origem, etc.).
    - Filtros avançados e ordenação.
    - Ações em lote (ex: atribuir vários leads a um vendedor, adicionar tag a vários leads).
- **Interligações:**
    - Seleção de um lead abre "Detalhes do Lead".
    - Exportação para CSV/Excel (embora o objetivo seja reduzir a necessidade disso).

### **4. Detalhes do Lead (Tela Centralizadora de Informações)**

- **Layout:**
    - **Cabeçalho:** Nome do Lead, Empresa, Cargo, Foto (se disponível), Status atual, "Temperatura" (Quente, Morno, Frio), Valor Potencial, Responsável. Botões de Ação Rápida (Ligar, Enviar Email, Agendar Atividade, Converter em Cliente/Oportunidade).
    - **Abas/Seções:**
        - **Informações:** Todos os campos de contato (telefones, emails, endereços), origem do lead, tags, campos customizáveis.
        - **Linha do Tempo de Atividades (Crucial!):**
            - Um feed cronológico de TODAS as interações e eventos:
                - Criação do Lead.
                - Emails enviados e recebidos (com status de abertura/clique se integrado).
                - Registros de chamadas telefônicas (data, hora, duração, resumo).
                - Notas de reuniões/visitas (com anexos, se houver).
                - Tarefas criadas e concluídas relacionadas ao lead.
                - Mudanças de status no Kanban.
                - Agendamentos e re-agendamentos.
                - Comentários internos da equipe.
            - Filtros na linha do tempo (ex: mostrar apenas emails, apenas notas).
        - **Emails:** Listagem de todos os emails trocados, com visualização do conteúdo. Opção de "Responder" ou "Encaminhar" diretamente da tela (integração com o cliente de email do usuário ou sistema de envio próprio).
        - **Tarefas:** Lista de tarefas pendentes e concluídas para este lead.
        - **Agendamentos:** Próximos e passados (ligações, reuniões, follow-ups) listados.
        - **Notas:** Todas as notas registradas.
        - **Documentos/Anexos:** Propostas, apresentações, etc.
        - **(Opcional) Produtos de Interesse:** Se aplicável, listar produtos/serviços que o lead demonstrou interesse.
- **Interligações:**
    - Tudo o que acontece no sistema referente a este lead é logado aqui.
    - A partir daqui, pode-se iniciar qualquer ação (email, ligação, agendamento).
- **Inteligência/Service Workers:**
    - **Sugestão de Próxima Ação:** Com base no histórico e status, o sistema pode sugerir o que fazer a seguir.
    - **Notificação de Inatividade:** Se o vendedor não interagir com um lead "quente" por X dias, o sistema (ou o gerente) é alertado.
    - **Sincronização de Email:** Service Worker pode verificar a caixa de entrada do vendedor (com permissão) por emails de/para o lead e associá-los automaticamente à Linha do Tempo.

### **5. Calendário de Atividades (Integração com FullCalendar)**

- **Visão Geral:**
    - Visão diária, semanal, mensal de todas as atividades agendadas (ligações, emails para enviar, reuniões, follow-ups, tarefas com prazo).
    - Cores diferentes para tipos de atividade ou status (ex: vermelho para atrasado).
    - Drag-and-drop para reagendar.
- **Ações no Evento:**
    - Marcar como concluído.
    - Reagendar.
    - Abrir o "Detalhes do Lead" associado.
    - Adicionar notas sobre a atividade.
- **Interligações:**
    - Sincronização bidirecional com o Google Calendar/Outlook Calendar do vendedor (desejável).
    - Criação de atividades a partir do Kanban ou "Detalhes do Lead" reflete aqui.
- **Inteligência/Service Workers:**
    - **Lembretes de Atividades:** Notificações push (desktop/mobile) via Service Worker antes do horário da atividade.
    - **Notificação de Atividades Atrasadas.**

### **6. Funcionalidade de Envio de Email Integrada**

- **Visão Geral:**
    - Editor de email dentro do sistema.
    - Possibilidade de usar templates de email pré-definidos (com placeholders como `{{lead.nome}}`).
    - Anexar arquivos.
    - Agendar envio de email.
- **Rastreamento (se possível e com consentimento):**
    - Abertura de email.
    - Cliques em links.
- **Interligações:**
    - Emails enviados são logados automaticamente na "Linha do Tempo de Atividades" do lead.
    - Respostas podem ser puxadas para a Linha do Tempo (via Service Worker e integração com IMAP/API do provedor de email).
- **Inteligência:**
    - Sugestão de templates com base no estágio do lead.
    - Análise de quais templates têm melhor taxa de abertura/resposta.

### **7. Registro de Contatos (Ligações, Reuniões, Visitas)**

- **Visão Geral:**
    - Formulário simples para registrar detalhes de uma ligação feita/recebida, uma reunião ou visita.
    - Campos: Data/Hora, Duração (para ligações), Tipo (Ligação, Reunião Presencial, Videoconferência, Visita), Resumo da Conversa, Próximos Passos Acordados, Sentimento do Lead.
    - Opção de agendar a "Próxima Atividade" diretamente deste formulário.
- **Interligações:**
    - Logado automaticamente na "Linha do Tempo de Atividades".
    - Atualiza o campo "Último Contato" no lead.
- **Inteligência:**
    - Se o vendedor usa um softphone ou app de ligação no celular, explorar integrações futuras para auto-log de chamadas.

### **8. Gestão de Tarefas**

- **Visão Geral:**
    - Criar tarefas associadas a um lead ou gerais.
    - Atribuir a si mesmo ou a outro membro da equipe.
    - Definir prazo.
    - Marcar prioridade.
- **Interligações:**
    - Visível na "Linha do Tempo de Atividades" do lead e no "Calendário de Atividades".
- **Inteligência/Service Workers:**
    - Lembretes de tarefas via notificações push.

### **9. Automações de Fluxo de Trabalho (Workflows)**

- **Visão Geral (Back-end, configurável pelo Admin):**
    - Permitir criar regras do tipo "Se [gatilho], Então [ação]".
    - **Exemplos de Gatilhos:**
        - Lead entra em novo estágio no Kanban.
        - Email é aberto/clicado.
        - Lead não é contatado por X dias.
        - Um campo específico é preenchido (ex: "Interesse em Produto X").
    - **Exemplos de Ações:**
        - Criar uma tarefa para o vendedor.
        - Enviar um email automático (template).
        - Mudar a "temperatura" do lead.
        - Adicionar uma tag.
        - Notificar o gerente.
        - Agendar um follow-up padrão.
- **Interligações:**
    - Conecta todas as partes do sistema, tornando-o proativo.
- **Inteligência:**
    - O coração da "redução de burocracia".

### **10. Lead Scoring (Opcional, mas poderoso)**

- **Visão Geral (Back-end, configurável):**
    - Atribuir pontos a leads com base em:
        - **Dados Demográficos:** Cargo, Indústria, Tamanho da Empresa.
        - **Engajamento:** Abertura de emails, cliques, visitas ao site (se integrado com tracking), download de material.
        - **Comportamento:** Solicitação de demonstração, preenchimento de formulário específico.
    - Leads com pontuação alta são priorizados.
- **Interligações:**
    - Pode ser um critério para mover leads no Kanban ou para disparar workflows.
    - Exibido no "Detalhes do Lead" e no Kanban/Lista.
- **Inteligência/Service Workers:**
    - Recalculo automático da pontuação quando novas interações ocorrem.

### **11. Conversão de Lead**

- **Processo:**
    - Quando um lead é "Ganho" ou está pronto para uma proposta formal/venda.
    - Botão "Converter Lead".
    - Opções:
        - Criar uma **Oportunidade/Negócio** (associado ao Contato e Empresa).
        - Criar/Atualizar **Contato**.
        - Criar/Atualizar **Empresa**.
    - O histórico do Lead é transferido/vinculado à nova Oportunidade/Contato/Empresa.
- **Interligações:**
    - Liga o módulo de Leads ao módulo de Vendas/Oportunidades.

---

### **II. Como os Service Workers e outras tecnologias se encaixam para "liberar da burocracia":**

- **Notificações Proativas (Service Workers):**
    - Lembretes de follow-ups, reuniões, tarefas.
    - Alertas de leads "esquecidos" ou que precisam de atenção.
    - Notificações de novas interações importantes (ex: resposta de email de um lead quente).
    - Avisos sobre leads atingindo um score alto.
    - Isso mantém o vendedor engajado e no controle sem precisar ficar checando tudo manualmente.
- **Sincronização em Background (Service Workers):**
    - Sincronizar emails enviados/recebidos com a Linha do Tempo do Lead.
    - Atualizar dados de engajamento (aberturas, cliques) se houver tracking.
    - Verificar status de atividades e atualizar o sistema.
- **Atualizações em Tempo Real (WebSockets ou polling eficiente):**
    - Kanban board se atualiza para todos os usuários quando um card é movido.
    - Novos leads ou atividades aparecem instantaneamente.
- **FullCalendar:**
    - Centraliza todos os compromissos, tornando o planejamento visual e fácil.
    - Re-agendamentos por Drag-and-drop são intuitivos.
- **Templates e Automações:**
    - Reduzem drasticamente o tempo gasto em digitação repetitiva (emails, notas padrão).
    - Garantem que os processos sejam seguidos (ex: sempre agendar follow-up após uma ligação).

---

### **III. Próximos Passos para o Use Case:**

Com base neste "desenho", podemos começar a detalhar Use Cases. Por exemplo:

1. **Use Case: Registrar um Novo Lead Manualmente**
    - Ator: Vendedor
    - Descrição: O vendedor recebe um cartão de visitas em um evento e precisa inserir o lead no sistema.
    - Fluxo Principal, Campos, Validações, Pós-condições.
2. **Use Case: Gerenciar Leads no Kanban**
    - Ator: Vendedor
    - Descrição: O vendedor visualiza seus leads no Kanban, move um lead de "Primeiro Contato" para "Qualificação" e agenda uma ligação.
    - Fluxo Principal, Interações, Disparos de Automação.
3. **Use Case: Acompanhar um Lead na Tela de Detalhes**
    - Ator: Vendedor
    - Descrição: O vendedor abre um lead para revisar todo o histórico de interações, envia um email usando um template e registra uma nota sobre uma conversa telefônica.
    - Fluxo Principal, Acesso à Informação, Ações.
4. **Use Case: Receber Notificação de Follow-up (via Service Worker)**
    - Ator: Vendedor
    - Descrição: O vendedor está trabalhando em outra tarefa e recebe uma notificação no desktop/navegador lembrando-o de um follow-up importante.
    - Fluxo Principal, Interação com a Notificação.

E assim por diante, detalhando cada interação chave.
