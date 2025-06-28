
### Interação:

- Uma interação é o contato feito por um vendedor com um cliente. Ele se dá através de um determinado departamento do cliente, e através de certos contatods do cliente. esta interação servirá como registro de trabalho do vededor e como ele se relaciona com os clientes e seus departamentos, e/ou, 
filiais. 

- Uma interação será criada pelo vendedor, quando ele fizer ou planejar fazer contato com um departamento qualquer de um cliente.

- Esta estrutura poderá ser exibida em forma de kanban, agenda e lista.

- A interação será criada inserindo os dados de quando será a interação, informações relacionadas(observações, intenção, etc)

- Quem tem acesso a quais interações?
	- Glauco e Camila(perfil gestor) tem acesso a todas as interações de todos os vendedores;
	- Cada vendedor só  tera acesso às suas próprias interações (Usuário estará logado, e a informação do criador do evento será preenchida automaticamente.);
		- No caso da Camila e Glauco, eles escolhem de qual vendedor querem enxergar e criar as interações;
	- Decidir como controlar exceções de adiamento, cancelamento etc.(eu acho que o melhor a se fazer é apenas colocar um status como adiada, e criar outra, e talvez colocar campos adicionais ou escrever na observação.
	

- As interações possuem informações como:
	- Data planejada
	- DatadeCriaçãodainteração
	- Qual vendedor realizou
    - Qual cliente e departamento estavam envolvidos
    - A data da interação
    - Objetivo programado
    - Observações antes, durante e após
    - Contatos participantes
    - Tipo da interação (visita técnica, apresentação, follow-up etc.)
    - Status (adiada, concluída, reagendada etc.)
	

###Abaico parte que o gpt fez pra explicar melhor



# 🧭 Fluxo de Processos: Contatos, Departamentos, Interações e Clientes

## 📌 Visão Geral

O sistema foi projetado para mapear e acompanhar **todo o ciclo de relacionamento comercial** com empresas clientes, desde o **cadastro da estrutura organizacional** (empresa → unidades → departamentos → contatos) até as **interações realizadas pelos vendedores**. 

Esse fluxo permite ao gestor compreender **com quem foi feito contato, sobre o que se tratava, qual o contexto interno do cliente e qual o histórico daquela relação**.

---

## 🏢 1. Cadastro da Empresa e suas Unidades (Clientes)

- A **Empresa** representa o grupo econômico, marca ou organização como um todo.
- Cada **Cliente** representa uma **unidade física ou filial da empresa**, onde ocorrem as operações.
- A relação entre **Empresa → Clientes** é de **um-para-muitos**.

🔹 **Exemplo prático**:
> Empresa: *Laboratórios Alpha S/A*  
> Cliente 1: *Alpha Unidade RJ*  
> Cliente 2: *Alpha Unidade SP*

---

## 🧩 2. Departamentos dentro do Cliente

Cada cliente (unidade da empresa) possui **Departamentos**, que representam setores como:

- Compras
- Laboratório
- Manutenção
- Financeiro
- Engenharia

Esses departamentos são os **principais pontos de contato para interação comercial**, pois concentram os responsáveis pela solicitação de orçamentos, dúvidas técnicas e tomadas de decisão.

---

## 👥 3. Contatos (Pessoas)

Os **Contatos** são as **pessoas físicas** vinculadas a um **Departamento** específico. Cada contato possui:

- Nome, cargo, e-mail, telefone
- Tipo de contato (comercial, técnico, etc.)

Eles são os **indivíduos com quem os vendedores realmente conversam**. Um mesmo contato pode estar associado a múltiplas **Pessoas** do sistema por meio de relações de apoio, parceria ou referência cruzada.

---

## 🗓️ 4. Interações

As **Interações** representam o **registro de uma visita, reunião, ligação ou conversa** realizada entre um **vendedor** e um **departamento específico de um cliente**.

Cada interação registra:

- Qual vendedor realizou
- Qual cliente e departamento estavam envolvidos
- A data da interação
- Objetivo programado
- Observações antes, durante e após
- Contatos participantes
- Tipo da interação (visita técnica, apresentação, follow-up etc.)
- Status (adiada, concluída, reagendada etc.)

🔄 **Objetivo**: rastrear e documentar **todo o histórico de relacionamento comercial** com aquele cliente.

---

## 👨‍💼 5. Vendedor e Relacionamento

O **Vendedor** é o agente responsável por:

- Cadastrar interações
- Associar contatos
- Definir objetivos da visita
- Acompanhar departamentos com pendências ou oportunidades
- Alimentar o CRM interno do sistema

Todas as interações feitas por um vendedor ficam vinculadas a ele, permitindo **relatórios e metas personalizadas**.

---

## 📈 Como o sistema se conecta

| Elemento        | Relacionado a...                                                |
|-----------------|-----------------------------------------------------------------|
| Empresa         | Coleção de Clientes (Unidades)                                  |
| Cliente         | Possui vários Departamentos                                     |
| Departamento    | Contém vários Contatos e registra Interações                    |
| Contato         | Participa de múltiplas Interações                               |
| Interação       | Une Vendedor + Departamento + Cliente + Contatos + Histórico    |
| Vendedor        | Realiza e registra Interações                                   |

---

## ✅ Resultado

Essa estrutura permite que o gestor:

- Visualize **quais contatos estão mais ativos**
- Entenda o **histórico completo de relacionamento com cada unidade**
- Analise o desempenho de vendedores
- Detecte gargalos (ex: muitos contatos sem interação recente)
- Crie **relatórios de acompanhamento inteligente por cliente ou vendedor**


###Sobre orçamentos:

# 🧾 Fluxo de Orçamentos, Versões e Geração de Pedidos

## 📌 Visão Geral

O sistema implementa uma arquitetura robusta para controlar **todo o processo de negociação**, **histórico de propostas** e **geração de pedidos** a partir de orçamentos. Essa estrutura permite que cada orçamento tenha sua versão, possibilitando **comparações, rastreamento de alterações** e uma **decisão clara de conversão em pedido**.

---

## 📝 1. Criação de Orçamentos (OrcamentoVenda)

- Cada **OrçamentoVenda** representa uma **proposta comercial feita para o cliente** em um dado momento.
- O campo `Versao` indica o número da versão (1, 2, 3...) daquela negociação.
- O `PedidoBaseId` é compartilhado entre as versões, unificando o histórico da negociação.
- Cada orçamento pode conter:
  - Dados do cliente
  - Observações gerais da proposta
  - Itens propostos com preços calculados ou arbitrários
  - Vínculo com um `Pedido` (se o orçamento for aceito)

🎯 **Objetivo**: manter o histórico das propostas feitas para o cliente antes do fechamento.

---

## 📦 2. Itens de Orçamento (ItemDeOrcamento)

Cada orçamento possui múltiplos **ItemDeOrcamento**, que registram:

- Nome do produto
- Quantidade ofertada
- Preço proposto
- Justificativa de alteração (em relação à versão anterior)
- Se aquele item foi modificado em relação à versão anterior (`FoiAlteradoNaVersaoSeguinte`)
- Observações sobre a negociação

✅ O preço pode ser:
- Calculado com base no preço base do produto + fator da categoria
- Definido de forma manual para casos especiais

---

## 🔁 3. Controle de Versões

Sempre que uma nova negociação ocorre (com alterações ou contrapropostas), o sistema permite criar uma nova versão:

- Copia os itens da versão anterior
- Permite edição livre nos itens
- Registra o campo `JustificativaModificacao` em cada item alterado

🔍 Isso permite o rastreamento completo de **como e por que a proposta evoluiu até o pedido final**.

---

## 📑 4. Geração de Pedido

Quando um **OrcamentoVenda é aceito**, ele é convertido em um `Pedido` com os `ItensPedido` correspondentes.

- O pedido gerado mantém vínculo com o orçamento aceito
- A versão final do orçamento torna-se **referência** para geração do pedido real
- O fluxo de status do item passa a valer a partir do pedido gerado (ex: PedidoDeVenda → PedidoDeCompra...)

---

## 🧠 Benefícios da Arquitetura

- Histórico completo de todas as propostas já feitas
- Rastreabilidade de versões e alterações
- Conversão fluida e auditável entre orçamento e pedido
- Flexibilidade para aplicar regras de negócio e precificação dinâmica
- Clareza para o gestor sobre decisões de compra e negociação

---

## 🔗 Como as entidades se conectam

| Entidade            | Descrição                                                                 |
|---------------------|---------------------------------------------------------------------------|
| OrcamentoVenda      | Proposta de venda para o cliente com versão e histórico                   |
| ItemDeOrcamento     | Produto ofertado no orçamento com controle de preço e justificativas      |
| Pedido              | Pedido real criado a partir de um orçamento aceito                        |
| ItemPedido          | Resultado da conversão de ItemDeOrcamento em linha de pedido              |

---

## 📊 Exemplo de fluxo

1. OrcamentoVenda v1 criado com 3 itens
2. Cliente pede alteração na quantidade e no preço de 1 item
3. Criado OrcamentoVenda v2
4. Cliente aprova a proposta
5. Sistema gera Pedido com base na v2
6. Histórico de v1 e v2 é mantido para rastreamento

---
- Pode-se ter também uma relação com produtos, igual a item do pedido.
Essa abordagem garante **rastreabilidade, consistência e profissionalismo na gestão de propostas comerciais**.