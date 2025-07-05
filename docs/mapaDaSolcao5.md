### Features da Solução GeneralLabSolutions - Parte 5

> Vamos aos problemas, agora?

- `AggregateListViewComponent` (ViewComponent responsável exibir as listas de agregados):


```csharp

using Microsoft.AspNetCore.Mvc;
using VelzonModerna.ViewModels;
using VelzonModerna.ViewModels.ViewComponents; // Não se esqueça de adicionar o using

namespace VelzonModerna.Configuration.ViewComponents.AggregateListViewComponent
{
    [ViewComponent(Name = "AggregateList")]
    public class AggregateListViewComponent : ViewComponent
    {
        // O construtor pode permanecer vazio por enquanto, ou receber dependências se necessário no futuro.
        public AggregateListViewComponent() { }

        // O método InvokeAsync agora recebe os parâmetros
        public IViewComponentResult Invoke(
            string parentEntityType,
            Guid parentEntityId,
            Guid parentPessoaId,
            string aggregateType,
            IEnumerable<object> items)
        {
            // Criamos um modelo com todos os dados necessários para a View do componente
            var model = new AggregateListViewModel
            {
                ParentEntityType = parentEntityType,
                ParentEntityId = parentEntityId,
                ParentPessoaId = parentPessoaId,
                AggregateType = aggregateType,
                Items = items
            };

            // Passamos o modelo para a View do componente
            return View(model);
        }
    }
}

```
---

- `Default.cshtml` do ViewComponent "AggregateListViewComponent", em sua devida pasta padrão:

```cshatml

@using VelzonModerna.ViewModels
@using GeneralLabSolutions.Domain.Enums
@model VelzonModerna.ViewModels.ViewComponents.AggregateListViewModel

@{
    // Define constantes para os nomes dos agregados para evitar "magic strings"
    const string DADOS_BANCARIOS = "DadosBancarios";
    const string TELEFONE = "Telefone";
    const string CONTATO = "Contato";
    const string ENDERECO = "Endereco";

    // Determina o título e o alvo do offcanvas com base no tipo de agregado
    var title = System.Text.RegularExpressions.Regex.Replace(Model.AggregateType, "([A-Z])", " $1").Trim();
    var offcanvasTarget = $"#offcanvas{Model.AggregateType}";
}

<div class="card mb-3">
    <div class="card-header align-items-center d-flex">
        <h4 class="card-title mb-0 flex-grow-1">@title</h4>
        <div class="flex-shrink-0">
            <button type="button" class="btn btn-primary btn-sm add-aggregate-btn"
                    data-bs-toggle="offcanvas"
                    data-bs-target="@offcanvasTarget"
                    data-aggregate-type="@Model.AggregateType"
                    data-pessoa-id="@Model.ParentPessoaId">
                <i class="ri-add-line align-bottom me-1"></i> Adicionar Novo
            </button>
        </div>
    </div>
    <div class="card-body">
        @if (Model.Items != null && Model.Items.Any())
        {
            <div class="table-responsive">
                <table class="table table-hover table-striped align-middle mb-0">
                    <thead>
                        <tr>
                            @* --- Lógica para renderizar os cabeçalhos corretos --- *@
                            @switch (Model.AggregateType)
                            {
                                case DADOS_BANCARIOS:
                                    <th>Banco</th>
                                    <th>Agência</th>
                                    <th>Conta</th>
                                    <th>Tipo</th>
                                    break;
                                case TELEFONE:
                                    <th>DDD</th>
                                    <th>Número</th>
                                    <th>Tipo</th>
                                    break;
                                case CONTATO:
                                    <th>Nome</th>
                                    <th>Email</th>
                                    <th>Telefone</th>
                                    <th>Tipo</th>
                                    break;
                                case ENDERECO:
                                    <th>Tipo</th>
                                    <th>Endereço</th>
                                    <th>Cidade</th>
                                    <th>País</th>
                                    break;
                            }
                        </tr>
                    </thead>
                    <tbody>
                        @foreach (var item in Model.Items)
                        {
                            @* --- Lógica para renderizar as linhas da tabela --- *@
                            <tr class="clickable-row aggregate-row"
                                data-id="@item.GetType().GetProperty("Id").GetValue(item)"
                                data-aggregate-type="@Model.AggregateType"
                                data-bs-toggle="offcanvas"
                                data-bs-target="@offcanvasTarget"
                                title="Clique para ver detalhes ou editar">

                                @switch (Model.AggregateType)
                                {
                                    case DADOS_BANCARIOS:
                                        var db = item as DadosBancariosViewModel;
                                        <td>@db.Banco</td>
                                        <td>@db.Agencia</td>
                                        <td>@db.Conta</td>
                                        <td>@db.TipoDeContaBancaria</td>
                                        break;
                                    case TELEFONE:
                                        var tel = item as TelefoneViewModel;
                                        <td>@tel.DDD</td>
                                        <td>@tel.Numero</td>
                                        <td>@tel.TipoDeTelefone</td>
                                        break;
                                    case CONTATO:
                                        var contato = item as ContatoViewModel;
                                        <td>@contato.Nome</td>
                                        <td>@contato.Email</td>
                                        <td>@contato.Telefone</td>
                                        <td>@contato.TipoDeContato</td>
                                        break;
                                    case ENDERECO:
                                        var end = item as EnderecoViewModel;
                                        <td>@end.TipoDeEndereco</td>
                                        <td>@end.LinhaEndereco1</td>
                                        <td>@end.Cidade</td>
                                        <td>@end.PaisCodigoIso</td>
                                        break;
                                }
                            </tr>
                        }
                    </tbody>
                </table>
            </div>
        } else
        {
            <div class="text-center py-3">
                <p class="text-muted mb-0">Nenhum item cadastrado.</p>
            </div>
        }
    </div>
</div>


```
---

- ViewModel responsavel por carregar os dados para as listas: `AggregateListViewModel`:

```csharp

namespace VelzonModerna.ViewModels.ViewComponents
{
    public class AggregateListViewModel
    {
        public string ParentEntityType { get; set; } // "Cliente", "Fornecedor", etc.
        public Guid ParentEntityId { get; set; }
        public Guid ParentPessoaId { get; set; }
        public string AggregateType { get; set; } // "DadosBancarios", "Telefone", etc.
        public IEnumerable<object> Items { get; set; }
    }

}


```
---

- `Views/Shared/PartialViews/_offcanvas_dados_bancarios_cliente.cshtml` (OffCanvas parao CRUD de Dadosbancarios em Detalhes e Edit de Cliente):

```cshtml

<!-- ========== Offcanvas Dados Bancários ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasDadosBancarios" aria-labelledby="offcanvasDadosBancariosLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasDadosBancariosLabel">Dados Bancários</h5> @* O título pode ser alterado via JS *@
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        @* Formulário para Adicionar/Editar Dados Bancários *@
        @* NOTA: Não usamos Tag Helpers aqui (asp-for) porque popularemos/submeteremos via JS/AJAX *@
        <form id="dadosBancariosOffcanvasForm" novalidate>
            @* Adicionar AntiForgeryToken se for validar no POST AJAX *@
            @Html.AntiForgeryToken()

            @* Campos ocultos essenciais para identificar o registro e o dono *@
            <input type="hidden" id="dadosBancarios_Id" name="Id" value="00000000-0000-0000-0000-000000000000" /> @* Guid.Empty por padrão *@
            <input type="hidden" id="dadosBancarios_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* Pega o PessoaId do ClienteViewModel principal *@

            @* Campos do Formulário *@
            <div class="mb-3">
                <label for="dadosBancarios_Banco" class="form-label">Banco</label>
                <input type="text" class="form-control" id="dadosBancarios_Banco" name="Banco" required placeholder="Nome do Banco">
                <div class="invalid-feedback">Por favor, informe o nome do banco.</div> @* Para validação JS/HTML5 *@
                <span id="validation_Banco" class="text-danger field-validation-error"></span> @* Para erros do servidor *@
            </div>

            <div class="mb-3">
                <label for="dadosBancarios_Agencia" class="form-label">Agência</label>
                <input type="text" class="form-control" id="dadosBancarios_Agencia" name="Agencia" required placeholder="Número da Agência">
                <div class="invalid-feedback">Por favor, informe a agência.</div>
                <span id="validation_Agencia" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="dadosBancarios_Conta" class="form-label">Conta</label>
                <input type="text" class="form-control" id="dadosBancarios_Conta" name="Conta" required placeholder="Número da Conta">
                <div class="invalid-feedback">Por favor, informe a conta.</div>
                <span id="validation_Conta" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="dadosBancarios_TipoDeContaBancaria" class="form-label">Tipo de Conta</label>
                <select class="form-select" id="dadosBancarios_TipoDeContaBancaria" name="TipoDeContaBancaria" required>
                    <option value="">-- Selecione --</option>
                    @* Pré-carrega as opções do Enum usando o Helper do C# *@
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Enums.TipoDeContaBancaria>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo da conta.</div>
                <span id="validation_TipoDeContaBancaria" class="text-danger field-validation-error"></span>
            </div>

            @* Área para exibir erros gerais do formulário retornados pelo servidor *@
            <div id="offcanvasFormErrors" class="text-danger mt-2 mb-3"></div>

            @* Botões de Ação dentro do Offcanvas *@
            <div class="mt-4 d-flex justify-content-end gap-2">
                @* Botão Salvar (aciona o submit do form) *@
                <button type="submit" class="btn btn-success" id="btnSalvarDadosBancarios">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>

                @* Botão Excluir (controlado via JS) *@
                <button type="button" class="btn btn-danger" id="btnExcluirDadosBancarios" style="display: none;">
                    @* Começa escondido *@
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>

                @* Botão Fechar (usa o atributo data-bs-dismiss do Bootstrap) *@
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div> <!-- end offcanvas-body -->
</div> <!-- end offcanvas -->
<!-- ========== End Offcanvas Dados Bancários ========== -->

```
---


- `Views/Shared/PartialViews/_scriptjs_dados_bancarios_cliente.cshtml` (Partial com o JavaScript de manipulação do agregado "DadosBancarios" em Cliente):


```JavaScript

<script>
    // Este log serve para confirmar que o ficheiro está a ser carregado pelo navegador.
    // Ele deve aparecer no console independentemente do jQuery.
    console.log("Script _scriptjs_dados_bancarios_cliente.cshtml carregado.");

    // A forma mais padrão e segura de executar código após o carregamento do jQuery.
    // Todo o nosso código que usa o símbolo '$' deve estar dentro desta função.
    $(function() {

        console.log("jQuery está pronto! Inicializando os eventos para Dados Bancários do Cliente.");

        // --- Funções e Variáveis para Dados Bancários do Cliente ---
        const offcanvasElement = document.getElementById('offcanvasDadosBancarios');
        const offcanvasInstance = offcanvasElement ? new bootstrap.Offcanvas(offcanvasElement) : null;
        const form = $('#dadosBancariosOffcanvasForm');
        const listContainerId = '#dados-bancarios-list-container';
        const clienteId = '@Model.Id';

        function showToast(message, type = 'success') {
            const bg = type === 'success' ? 'linear-gradient(to right, #00b09b, #96c93d)' : 'linear-gradient(to right, #ff5f6d, #ffc371)';
            Toastify({ text: message, duration: 3000, gravity: 'top', position: 'right', close: true, style: { background: bg } }).showToast();
        }

        function resetOffcanvasForm() {
            if (!form.length) return;
            form[0].reset();
            form.removeClass('was-validated');
            $('#dadosBancarios_Id').val('00000000-0000-0000-0000-000000000000');
            $('#dadosBancarios_PessoaId').val('@Model.PessoaId');
            $('#offcanvasFormErrors').text('');
            $('#btnExcluirDadosBancarios').hide();
            $('#offcanvasDadosBancariosLabel').text('Dados Bancários');
            $('#btnSalvarDadosBancarios').prop('disabled', false).html('<i class="ri-save-line align-bottom me-1"></i> Salvar');
        }

        // --- Event Listeners ---

        // Abrir Offcanvas para ADICIONAR
        $(document).on('click', '.add-dados-bancarios-btn', function () {
            if (!offcanvasInstance) return;
            resetOffcanvasForm();
            const pessoaId = $(this).data('pessoa-id');
            $('#dadosBancarios_PessoaId').val(pessoaId);
            $('#offcanvasDadosBancariosLabel').text('Adicionar Dados Bancários');
            offcanvasInstance.show();
        });

        // Abrir Offcanvas para EDITAR
        $(document).on('click', '.dados-bancarios-row', function () {
            console.clear();
            console.log("===================================");
            console.log("DIAGNÓSTICO - CLIQUE NA LINHA");

            const dadosBancariosId = $(this).data('id');
            const pessoaIdNaLinha = $(this).data('pessoa-id');

            console.log("ID do Agregado (data-id):", dadosBancariosId);
            console.log("ID da Pessoa (data-pessoa-id):", pessoaIdNaLinha);
            console.log("ID do Cliente (da página):", clienteId);
            console.log("===================================");

            if (!offcanvasInstance) return;

            resetOffcanvasForm();
            $('#offcanvasDadosBancariosLabel').text('Editar Dados Bancários');
            form.find('input, select').prop('disabled', true);

            $.ajax({
                url: '@Url.Action("GetDadosBancariosFormData", "Cliente")',
                type: 'GET',
                data: { dadosBancariosId: dadosBancariosId, clienteId: clienteId },
                success: function (data) {
                    if (data) {
                        $('#dadosBancarios_Id').val(data.id);
                        $('#dadosBancarios_PessoaId').val(data.pessoaId);
                        $('#dadosBancarios_Banco').val(data.banco);
                        $('#dadosBancarios_Agencia').val(data.agencia);
                        $('#dadosBancarios_Conta').val(data.conta);
                        $('#dadosBancarios_TipoDeContaBancaria').val(data.tipoDeContaBancaria);
                        $('#btnExcluirDadosBancarios').show();
                        offcanvasInstance.show();
                    } else {
                        showToast('Dados bancários não encontrados!', 'error');
                    }
                },
                error: function () {
                    showToast('Erro ao buscar dados bancários. Tente novamente!', 'error');
                },
                complete: function () {
                    form.find('input, select').prop('disabled', false);
                }
            });
        });

        // Submeter Formulário (SALVAR)
        form.on('submit', function (event) {
            event.preventDefault();
            if (this.checkValidity() === false) {
                event.stopPropagation();
                $(this).addClass('was-validated');
                return;
            }
            $(this).removeClass('was-validated');

            const formDataArray = $(this).serializeArray();
            formDataArray.push({ name: "clienteId", value: clienteId });
            const dataToSend = $.param(formDataArray);

            const url = '@Url.Action("SalvarDadosBancarios", "Cliente")';
            const btnSalvar = $('#btnSalvarDadosBancarios');
            btnSalvar.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Salvando...');

            $.ajax({
                url: url,
                type: 'POST',
                data: dataToSend,
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    if (response.success) {
                        offcanvasInstance.hide();
                        refreshDadosBancariosList();
                        showToast('Dados bancários salvos com sucesso!', 'success');
                    } else {
                        displayServerErrors(response.errors);
                        showToast('Ocorreu um erro ao salvar.', 'error');
                    }
                },
                error: function () {
                    displayServerErrors("Erro de comunicação com o servidor.");
                    showToast('Ocorreu um erro ao salvar.', 'error');
                },
                complete: function () {
                    btnSalvar.prop('disabled', false).html('<i class="ri-save-line align-bottom me-1"></i> Salvar');
                }
            });
        });

        // EXCLUIR
        $(document).on('click', '#btnExcluirDadosBancarios', function () {
            const dadosBancariosId = $('#dadosBancarios_Id').val();
            if (!dadosBancariosId || dadosBancariosId === '00000000-0000-0000-0000-000000000000') {
                Swal.fire('Ops!', 'ID inválido para exclusão.', 'error');
                return;
            }
            Swal.fire({
                title: 'Tem certeza?',
                text: "Esta operação removerá permanentemente estes dados bancários.",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Sim, excluir!',
                cancelButtonText: 'Cancelar',
                customClass: { confirmButton: 'btn btn-danger', cancelButton: 'btn btn-secondary ms-2' },
                buttonsStyling: false
            }).then((result) => {
                if (!result.isConfirmed) return;

                const url = '@Url.Action("ExcluirDadosBancarios", "Cliente")';
                const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
                const btnExcluir = $('#btnExcluirDadosBancarios');
                btnExcluir.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Excluindo...');

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: { clienteId: clienteId, dadosBancariosId: dadosBancariosId, __RequestVerificationToken: antiForgeryToken },
                    success: function (response) {
                        if (response.success) {
                            offcanvasInstance.hide();
                            refreshDadosBancariosList();
                            Swal.fire('Excluído!', 'Dados bancários removidos.', 'success');
                        } else {
                            Swal.fire('Erro!', (response.errors ?? ['Erro desconhecido']).join('<br>'), 'error');
                        }
                    },
                    error: function () { Swal.fire('Erro de Comunicação!', 'Não foi possível excluir os dados.', 'error'); },
                    complete: function () { btnExcluir.prop('disabled', false).html('<i class="ri-delete-bin-line align-bottom me-1"></i> Excluir'); }
                });
            });
        });

        // Limpar form ao fechar o offcanvas
        if(offcanvasElement) {
            offcanvasElement.addEventListener('hidden.bs.offcanvas', resetOffcanvasForm);
        }

        // --- Carregamento Inicial da Lista ---
        const tabLink = $('a[data-bs-toggle="tab"][href="#dados-bancarios"]');
        if (tabLink.hasClass('active')) {
            // A função refreshDadosBancariosList() precisa estar definida aqui ou antes
            //refreshDadosBancariosList(); // Descomente quando a função estiver no escopo
        }
        tabLink.on('shown.bs.tab', function (e) {
            //refreshDadosBancariosList(); // Descomente quando a função estiver no escopo
        });
    });
</script>

```
---


- Vou postar o JavaScript do Agregado Endereco de Cliente também, pois está diferente do de DadosBancarios:

- `Views/Shared/PartialViews/_scriptjs_endereco_bancarios_cliente.cshtml`

```JavaScript

<script>

    // ========================================================
    // NOVO Bloco JS para Endereços
    // ========================================================
    $(document).ready(function () {

        // --- Referências e Variáveis para Endereço ---
        const offcanvasEnderecoElement = document.getElementById('offcanvasEndereco');
        const offcanvasEnderecoInstance = offcanvasEnderecoElement ? new bootstrap.Offcanvas(offcanvasEnderecoElement) : null;
        const formEndereco = $('#enderecoOffcanvasForm');
        const listEnderecoContainerId = '#enderecos-list-container'; // ID do container da lista de endereços
        const clienteIdEndereco = '@Model.Id'; // ID do Cliente principal
        // Reutiliza showToast (assumindo que está definida globalmente ou no primeiro bloco de script)
        function showToast(message, type = 'success') {
            const bg = type === 'success' ? 'linear-gradient(#198754, #157347)' : type === 'error' ? 'linear-gradient(#dc3545, #bb2d3b)' : type === 'warning' ? 'linear-gradient(#f39c12, #e58e09)' : 'linear-gradient(#0d6efd, #0a58ca)';
            Toastify({ text: message, duration: 3000, gravity: 'top', position: 'right', close: true, style: { background: bg } }).showToast();
        }


        // --- Funções Auxiliares para Endereço ---
        function resetEnderecoOffcanvasForm() {
            if (formEndereco.length === 0) return;
            formEndereco[0].reset();
            formEndereco.removeClass('was-validated');
            $('#endereco_Id').val('00000000-0000-0000-0000-000000000000');
            $('#endereco_PessoaId').val('@Model.PessoaId'); // PessoaId do Cliente atual
            // Limpar spans de erro específicos do formulário de endereço
            $('#offcanvasEnderecoFormErrors').text(''); // Limpa erros gerais
            $('#btnExcluirEndereco').hide();
            $('#offcanvasEnderecoLabel').text('Endereço');
            $('#btnSalvarEndereco').prop('disabled', false).html('<i class="ri-save-line align-bottom me-1"></i> Salvar');
        }

        function displayEnderecoServerErrors(errors) {
            $('#offcanvasEnderecoFormErrors').text('');
            if (Array.isArray(errors)) {
                if (errors.length > 0) $('#offcanvasEnderecoFormErrors').html(errors.join('<br>'));
            } else if (typeof errors === 'string') {
                $('#offcanvasEnderecoFormErrors').text(errors);
            } else {
                $('#offcanvasEnderecoFormErrors').text('Ocorreu um erro inesperado.');
            }
        }

        function refreshEnderecosList() {
            if (!clienteIdEndereco || clienteIdEndereco === '00000000-0000-0000-0000-000000000000') {
                console.error("ClienteId inválido para recarregar a lista de endereços.");
                return;
            }
            const url = '@Url.Action("GetEnderecosListPartial", "Cliente")?clienteId=' + clienteIdEndereco;

            $(listEnderecoContainerId).html('<div class="text-center p-3"><div class="spinner-border text-primary"><span>Carregando...</span></div></div>');

            $.ajax({
                url: url, type: 'GET',
                success: function (result) { $(listEnderecoContainerId).html(result); },
                error: function () {
                    showToast('Erro ao carregar a lista de endereços!', 'error');
                    $(listEnderecoContainerId).html('<div class="alert alert-danger">Erro ao carregar os endereços.</div>');
                }
            });
        }

        // --- Event Listeners para Endereço ---

        // 1. Abrir Offcanvas para ADICIONAR Endereço
        $(listEnderecoContainerId).on('click', '.add-endereco-btn', function () {
            if (!offcanvasEnderecoInstance) return;
            resetEnderecoOffcanvasForm();
            const pessoaId = $(this).data('pessoa-id');
            $('#endereco_PessoaId').val(pessoaId); // PessoaId do Cliente
            $('#offcanvasEnderecoLabel').text('Adicionar Endereço');
            $('#btnExcluirEndereco').hide();
        });


        // Listener para o evento 'shown.bs.tab' da aba de Endereços
        // O seletor 'a[data-bs-toggle="tab"][href="#border-navs-settings"]'
        // corresponde ao link da aba de Endereços na sua Details.cshtml
        $('a[data-bs-toggle="tab"][href="#border-navs-settings"]').on('shown.bs.tab', function (e) {
            console.log('Aba de Endereços (#border-navs-settings) foi mostrada.');
            refreshEnderecosList(); // Chama a função para carregar/atualizar a lista
        });

        // Verifica se a aba de Endereços já está ativa quando a página carrega
        // Isso é importante se a aba de Endereços puder ser a aba padrão.
        if ($('#border-navs-settings').hasClass('active')) {
            console.log('Aba de Endereços (#border-navs-settings) já está ativa no carregamento da página.');
            refreshEnderecosList();
        }

        // 2. Abrir Offcanvas para EDITAR Endereço
        $(listEnderecoContainerId).on('click', '.endereco-row', function () {
            if (!offcanvasEnderecoInstance) return;
            resetEnderecoOffcanvasForm();
            const enderecoId = $(this).data('id');
            $('#offcanvasEnderecoLabel').text('Editar Endereço');
            formEndereco.find('input, select, textarea').prop('disabled', true);

            $.ajax({
                url: '@Url.Action("GetEnderecoFormData", "Cliente")', type: 'GET',
                data: { enderecoId: enderecoId, clienteId: clienteIdEndereco },
                success: function (data) {
                    if (data) {
                        $('#endereco_Id').val(data.id);
                        $('#endereco_PessoaId').val(data.pessoaId);
                        $('#endereco_TipoDeEndereco').val(data.tipoDeEndereco);
                        $('#endereco_PaisCodigoIso').val(data.paisCodigoIso);
                        $('#endereco_LinhaEndereco1').val(data.linhaEndereco1);
                        $('#endereco_LinhaEndereco2').val(data.linhaEndereco2);
                        $('#endereco_Cidade').val(data.cidade);
                        $('#endereco_EstadoOuProvincia').val(data.estadoOuProvincia);
                        $('#endereco_CodigoPostal').val(data.codigoPostal);
                        $('#endereco_InformacoesAdicionais').val(data.informacoesAdicionais);
                        $('#btnExcluirEndereco').show();
                    } else {
                        showToast('Endereço não encontrado.', 'error');
                        offcanvasEnderecoInstance.hide();
                    }
                },
                error: function () { showToast('Erro ao buscar dados do endereço.', 'error'); offcanvasEnderecoInstance.hide(); },
                complete: function() { formEndereco.find('input, select, textarea').prop('disabled', false); }
            });
        });

        // 3. Submeter Formulário (SALVAR Endereço - Add/Edit)
        formEndereco.on('submit', function (event) {
            event.preventDefault();
            if (!offcanvasEnderecoInstance) return;
            if (this.checkValidity() === false) { event.stopPropagation(); $(this).addClass('was-validated'); return; }
            $(this).removeClass('was-validated');

            const formDataArray = $(this).serializeArray();
            formDataArray.push({ name: "ClienteId", value: clienteIdEndereco });
            const dataToSend = $.param(formDataArray);
            const url = '@Url.Action("SalvarEndereco", "Cliente")';
            const btnSalvar = $('#btnSalvarEndereco');
            btnSalvar.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Salvando...');

            $.ajax({
                url: url, type: 'POST', data: dataToSend,
                headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() },
                success: function (response) {
                    if (response.success) {
                        offcanvasEnderecoInstance.hide();
                        refreshEnderecosList();
                        showToast('Endereço salvo com sucesso!', 'success');
                    } else {
                        displayEnderecoServerErrors(response.errors);
                        showToast('Erro ao salvar endereço.', 'error');
                    }
                },
                error: function () { displayEnderecoServerErrors("Erro de comunicação."); showToast('Erro ao salvar endereço.', 'error'); },
                complete: function() { btnSalvar.prop('disabled', false).html('<i class="ri-save-line align-bottom me-1"></i> Salvar'); }
            });
        });

        // 4. EXCLUIR Endereço
        $('#btnExcluirEndereco').on('click', function () {
            const enderecoId = $('#endereco_Id').val();
            if (!offcanvasEnderecoInstance) return;
            if (!enderecoId || enderecoId === '00000000-0000-0000-0000-000000000000') {
                Swal.fire('Ops!', 'ID inválido para exclusão.', 'error'); return;
            }

            Swal.fire({
                title: 'Tem certeza?', text: "Você realmente deseja excluir este endereço?", icon: 'warning',
                showCancelButton: true, confirmButtonText: 'Sim, excluir!', cancelButtonText: 'Cancelar',
                customClass: { confirmButton: 'btn btn-danger', cancelButton: 'btn btn-secondary ms-2' },
                buttonsStyling: false
            }).then((result) => {
                if (!result.isConfirmed) return;

                const url = '@Url.Action("ExcluirEndereco", "Cliente")';
                const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
                const btnExcluir = $('#btnExcluirEndereco');
                btnExcluir.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span> Excluindo...');

                $.ajax({
                    url: url, type: 'POST',
                    data: { clienteId: clienteIdEndereco, enderecoId: enderecoId, __RequestVerificationToken: antiForgeryToken },
                    success: function (response) {
                        if (response.success) {
                            offcanvasEnderecoInstance.hide();
                            refreshEnderecosList();
                            Swal.fire('Excluído!', 'O endereço foi removido.', 'success');
                        } else {
                            Swal.fire('Erro!', (response.errors ?? ['Erro desconhecido']).join('<br>'), 'error');
                        }
                    },
                    error: function () { Swal.fire('Erro de Comunicação!', 'Não foi possível excluir o endereço.', 'error'); },
                    complete: function() { btnExcluir.prop('disabled', false).html('<i class="ri-delete-bin-line align-bottom me-1"></i> Excluir'); }
                });
            });
        });

        // Limpar formulário quando Offcanvas de Endereço for fechado
        if(offcanvasEnderecoElement) {
            offcanvasEnderecoElement.addEventListener('hidden.bs.offcanvas', event => {
               resetEnderecoOffcanvasForm();
            });
        }
    });
    // Fim do bloco JS para Endereços
</script>

```

- Agora, vamos para as Views Edit e Details de Cliente:


- `Edit.cshtml`:


```cshtml

@using GeneralLabSolutions.Domain.Enums
@model VelzonModerna.ViewModels.ClienteViewModel

@{
    ViewBag.Title = "Edição de Cliente";
    ViewBag.pTitle = "Edição de Cliente";
    ViewBag.pageTitle = "Cadastro";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@section Styles {
    <link href="~/css/site.css" rel="stylesheet" />
    <link href="~/assets/libs/toastify-js/src/toastify.css" rel="stylesheet" />
    <link href="~/assets/libs/sweetalert2/sweetalert2.min.css" rel="stylesheet" />
}




@section Breadcrumb {
    <ol class="breadcrumb m-0">
        <li class="breadcrumb-item"><a asp-controller="GalLabs" asp-action="GlDashboard">Dashboard</a></li>
        <li class="breadcrumb-item"><a asp-action="Index">Lista de Cliente</a></li>
        <li class="breadcrumb-item active">Detalhes</li>
    </ol>
}


<!-- ViewComponent Consolidado de Cliente... -->
<vc:cliente-consolidado cliente-id="Model.Id" />
<!-- --------------------------------------- -->

<div class="row">
    <div class="col-lg-12">
        <div class="card">
            <div class="card-header align-items-center d-flex">
                <h4 class="card-title mb-0 flex-grow-1"> <strong>Cliente >> @Model.Nome.ToUpper() </strong></h4>
            </div>
            <div class="card-body">

                <p class="text-muted">Para <code>Editar um Cliente</code>, observe os campos obrigatórios e atente-se para o Tipo de Pessoa/Documento.</p>

                <div class="live-preview">
                    <form asp-action="Edit" asp-controller="Cliente">

                        <vc:summary></vc:summary>

                        <partial name="_edit_e_create" model="Model" />

                        <input type="hidden" asp-for="Id" />

                        <hr />
                        <div>
                            <input type="submit" value="Atualizar" class="btn btn-outline-success" />
                            <a asp-action="Index" class="btn btn-outline-warning">Voltar à Lista</a>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

<div class="col-lg-12">
    <h5 class="mb-3">Dados Complementares de Cliente</h5>
    <div class="card">
        <div class="card-body">
            <!-- Nav tabs -->
            <ul class="nav nav-pills nav-customs nav-danger mb-3" role="tablist">
                <li class="nav-item">
                    <a class="nav-link active" data-bs-toggle="tab" href="#dados-bancarios" role="tab">Dados bancários</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="tab" href="#telefones" role="tab">Telefones do Cliente</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="tab" href="#contatos" role="tab">Contatos do Cliente</a>
                </li>
                <li class="nav-item">
                    <a class="nav-link" data-bs-toggle="tab" href="#enderecos" role="tab">Endereços deste Cliente</a>
                </li>
            </ul><!-- Tab panes -->
            <div class="tab-content text-muted">

                <partial name="_aggregateListCliente" model="@Model" />

            </div>
        </div><!-- end card-body -->
    </div>
</div><!--end col-->




<partial name="PartialViews/_offcanvas_dados_bancarios_cliente" />
<partial name="PartialViews/_offcanvas_telefone_cliente" />
<partial name="PartialViews/_offcanvas_contato_cliente" />
<partial name="PartialViews/_offcanvas_endereco_cliente" />




@section scripts {

    <script src="~/assets/libs/prismjs/prism.js"></script>
    <script src="~/assets/libs/toastify-js/src/toastify.js"></script>
    <script src="~/assets/libs/sweetalert2/sweetalert2.min.js"></script>
    <script src="~/assets/js/app.js"></script>

    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");

        await Html.RenderPartialAsync("PartialViews/_animacao_consolidado_cliente");

        await Html.RenderPartialAsync("PartialViews/_scriptjs_dados_bancarios_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_telefone_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_contato_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_endereco_cliente");
    }

}

```
---


- `Details.cshtml` (de Cliente):


```cshtml

@using VelzonModerna.ViewModels
@model ClienteViewModel


@{
    ViewBag.Title = "Detalhes de Cliente";
    ViewBag.pTitle = "Detalhes de Cliente";
    ViewBag.pageTitle = "Cadastro";
    Layout = "~/Views/Shared/_Layout.cshtml";
}


@section Styles {
    <link href="~/css/site.css" rel="stylesheet" />
    <link href="~/assets/libs/toastify-js/src/toastify.css" rel="stylesheet" />
    <link href="~/assets/libs/sweetalert2/sweetalert2.min.css" rel="stylesheet" />
}

@section Breadcrumb {
    <ol class="breadcrumb m-0">
        <li class="breadcrumb-item"><a asp-controller="GalLabs" asp-action="GlDashboard">Dashboard</a></li>
        <li class="breadcrumb-item"><a asp-action="Index">Lista de Cliente</a></li>
        <li class="breadcrumb-item active">Detalhes</li>
    </ol>
}

<!-- ViewComponent Consolidado de Cliente... -->
<vc:cliente-consolidado cliente-id="Model.Id" />
<!-- --------------------------------------- -->

<div class="row">
    <div class="col-lg-12">
        <div class="card">
            <div class="card-header align-items-center d-flex">
                <h4 class="card-title mb-0 flex-grow-1"> <strong>Cliente >> @Model.Nome.ToUpper() </strong></h4>
            </div><!-- end card header -->

            <div style="padding: 20px;">
                <div>

                    <partial name="_details_e_delete" model="Model" />

                </div>
                <hr />
                <div>
                    <a asp-action="Edit" class="btn btn-outline-info" asp-route-id="@Model?.Id">Editar</a>
                    <a asp-action="Index" class="btn btn-outline-success">Voltar à Lista</a>
                </div>

            </div>

        </div>



        <div class="col-lg-12">
            <h5 class="mb-3">Dados Complementares de Cliente</h5>
            <div class="card">
                <div class="card-body">
                    <!-- Nav tabs -->
                    <ul class="nav nav-pills nav-customs nav-danger mb-3" role="tablist">
                        <li class="nav-item">
                            <a class="nav-link active" data-bs-toggle="tab" href="#dados-bancarios" role="tab">Dados bancários</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-bs-toggle="tab" href="#telefones" role="tab">Telefones do Cliente</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-bs-toggle="tab" href="#contatos" role="tab">Contatos do Cliente</a>
                        </li>
                        <li class="nav-item">
                            <a class="nav-link" data-bs-toggle="tab" href="#enderecos" role="tab">Endereços deste Cliente</a>
                        </li>
                    </ul><!-- Tab panes -->
                    <div class="tab-content text-muted">

                        <partial name="_aggregateListCliente" model="@Model" />

                    </div>
                </div><!-- end card-body -->
            </div>
        </div><!--end col-->


    </div>
</div>


<partial name="PartialViews/_offcanvas_dados_bancarios_cliente" />
<partial name="PartialViews/_offcanvas_telefone_cliente" />
<partial name="PartialViews/_offcanvas_contato_cliente" />
<partial name="PartialViews/_offcanvas_endereco_cliente" />


@section scripts {

    <script src="~/assets/libs/prismjs/prism.js"></script>
    <script src="~/assets/libs/toastify-js/src/toastify.js"></script>
    <script src="~/assets/libs/sweetalert2/sweetalert2.min.js"></script>

    <script src="~/assets/js/app.js"></script>

    @{
        await Html.RenderPartialAsync("_ValidationScriptsPartial");

        await Html.RenderPartialAsync("PartialViews/_animacao_consolidado_cliente");

        await Html.RenderPartialAsync("PartialViews/_scriptjs_dados_bancarios_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_telefone_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_contato_cliente");
        await Html.RenderPartialAsync("PartialViews/_scriptjs_endereco_cliente");
    }

}



```
---


- Agora a PartialView que chama meu ViewComponent que carrega a lista de Cliente `_aggregateListCliente.cshtml`:


```cshtml

@using VelzonModerna.ViewModels
@model ClienteViewModel


<div class="tab-pane active" id="dados-bancarios" role="tabpanel">
    <div id="dados-bancarios-list-container">
        @await Component.InvokeAsync("AggregateList", 
        new {
            parentEntityType = "Cliente",
            parentEntityId = Model.Id,
            parentPessoaId = Model.PessoaId,
            aggregateType = "DadosBancarios",
            items = Model.DadosBancarios
            })
    </div>
</div>


    <div class="tab-pane" id="telefones" role="tabpanel">
    <div id="telefones-list-container">
        @await Component.InvokeAsync("AggregateList", 
        new {
            parentEntityType = "Cliente",
            parentEntityId = Model.Id,
            parentPessoaId = Model.PessoaId,
            aggregateType = "Telefone",
            items = Model.Telefones
            })
    </div>
</div>


    <div class="tab-pane" id="contatos" role="tabpanel">
    <div id="contatos-list-container">

        <vc:aggregate-list parent-entity-type="Cliente", parent-entity-id="@Model.Id" 
                parent-pessoa-id="@Model.PessoaId" aggregate-type="Contato" 
                        items="@Model.Contatos" ></vc:aggregate-list>

@*         @await Component.InvokeAsync("AggregateList", 
        new {
            parentEntityType = "Cliente",
            parentEntityId = Model.Id,
            parentPessoaId = Model.PessoaId,
            aggregateType = "Contato",
            items = Model.Contatos
            }) *@

    </div>
</div>


    <div class="tab-pane" id="enderecos" role="tabpanel">
    <div id="enderecos-list-container">
        @await Component.InvokeAsync("AggregateList", 
        new {
            parentEntityType = "Cliente",
            parentEntityId = Model.Id,
            parentPessoaId = Model.PessoaId,
            aggregateType = "Endereco",
            items = Model.Enderecos
            })
    </div>
</div>

```
---


- Bom, tem as outras Views, PartialViews, offCanvas e Scripts dos outros agregados (Telefone, Contato e Endereco), mas vamos focar em um e, se corrigirmos, aplicamos nas outras features de agregados.

- Só mandei o JavaScript de Endereco, porque havia mexido muito no de DadosBancarios e acho que algo que fiz deixou o comportamento de carregamento diferente dos outros agragados!

- Que tal o desafio?
- Vamos criar um plano de ação para resolver?