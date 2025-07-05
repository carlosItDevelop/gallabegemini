### Features da Solução GeneralLabSolutions - Parte 6

Para ficar completo, que tal unificarmos os OffCanvas também?

- Talvez, criando outro ViewComponent unificado e genérico, talvez de uma outra forma?!
- Vou postar, abaixo, pelo menos dois offcanvas (de Cliente e Fornecedor) para você teruma base, e depois usando analogias, criamos a generalização do offCanva unificado. Eis 8 offCanvas para você analisar (Cliente e Fornecedor):


- `_offcanvas_contato_cliente.cshtml`:

```cshtml`

@* Cole este bloco no final de Views/Cliente/Details.cshtml e Views/Cliente/Edit.cshtml, ABAIXO do Offcanvas de Telefone e ANTES de @section Scripts *@

<!-- ========== Offcanvas Contato ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasContato" aria-labelledby="offcanvasContatoLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasContatoLabel">Contato</h5> @* Título pode ser alterado via JS *@
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="contatoOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken()

            <input type="hidden" id="contato_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="contato_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* PessoaId do Cliente principal *@

            <div class="mb-3">
                <label for="contato_Nome" class="form-label">Nome do Contato</label>
                <input type="text" class="form-control" id="contato_Nome" name="Nome" required placeholder="Nome completo do contato">
                <div class="invalid-feedback">Por favor, informe o nome do contato.</div>
                <span id="validation_Contato_Nome" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_Email" class="form-label">Email Principal</label>
                <input type="email" class="form-control" id="contato_Email" name="Email" required placeholder="email@dominio.com">
                <div class="invalid-feedback">Por favor, informe um email válido.</div>
                <span id="validation_Contato_Email" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_Telefone" class="form-label">Telefone Principal</label>
                <input type="text" class="form-control" id="contato_Telefone" name="Telefone" required placeholder="(XX) XXXXX-XXXX">
                <div class="invalid-feedback">Por favor, informe o telefone principal.</div>
                <span id="validation_Contato_Telefone" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_TipoDeContato" class="form-label">Tipo de Contato</label>
                <select class="form-select" id="contato_TipoDeContato" name="TipoDeContato" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Enums.TipoDeContato>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do contato.</div>
                <span id="validation_Contato_TipoDeContato" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_EmailAlternativo" class="form-label">Email Alternativo (Opcional)</label>
                <input type="email" class="form-control" id="contato_EmailAlternativo" name="EmailAlternativo" placeholder="email.alt@dominio.com">
            </div>

            <div class="mb-3">
                <label for="contato_TelefoneAlternativo" class="form-label">Telefone Alternativo (Opcional)</label>
                <input type="text" class="form-control" id="contato_TelefoneAlternativo" name="TelefoneAlternativo" placeholder="(XX) XXXXX-XXXX">
            </div>

            <div class="mb-3">
                <label for="contato_Observacao" class="form-label">Observação (Opcional)</label>
                <textarea class="form-control" id="contato_Observacao" name="Observacao" rows="3" placeholder="Detalhes adicionais sobre o contato"></textarea>
            </div>

            <div id="offcanvasContatoFormErrors" class="text-danger mt-2 mb-3"></div>

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarContato">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirContato" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Contato ========== -->

```
---


- `_offcanvas_contato_fornecedor.cshtml`:

```cshtml

@* Cole este bloco no final de Views/Fornecedor/Details.cshtml e Views/Fornecedor/Edit.cshtml, ABAIXO do Offcanvas de Telefone e ANTES de @section Scripts *@

<!-- ========== Offcanvas Contato ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasContato" aria-labelledby="offcanvasContatoLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasContatoLabel">Contato</h5> @* Título pode ser alterado via JS *@
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="contatoOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken()

            <input type="hidden" id="contato_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="contato_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* PessoaId do Fornecedor principal *@

            <div class="mb-3">
                <label for="contato_Nome" class="form-label">Nome do Contato</label>
                <input type="text" class="form-control" id="contato_Nome" name="Nome" required placeholder="Nome completo do contato">
                <div class="invalid-feedback">Por favor, informe o nome do contato.</div>
                <span id="validation_Contato_Nome" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_Email" class="form-label">Email Principal</label>
                <input type="email" class="form-control" id="contato_Email" name="Email" required placeholder="email@dominio.com">
                <div class="invalid-feedback">Por favor, informe um email válido.</div>
                <span id="validation_Contato_Email" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_Telefone" class="form-label">Telefone Principal</label>
                <input type="text" class="form-control" id="contato_Telefone" name="Telefone" required placeholder="(XX) XXXXX-XXXX">
                <div class="invalid-feedback">Por favor, informe o telefone principal.</div>
                <span id="validation_Contato_Telefone" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_TipoDeContato" class="form-label">Tipo de Contato</label>
                <select class="form-select" id="contato_TipoDeContato" name="TipoDeContato" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Enums.TipoDeContato>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do contato.</div>
                <span id="validation_Contato_TipoDeContato" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="contato_EmailAlternativo" class="form-label">Email Alternativo (Opcional)</label>
                <input type="email" class="form-control" id="contato_EmailAlternativo" name="EmailAlternativo" placeholder="email.alt@dominio.com">
            </div>

            <div class="mb-3">
                <label for="contato_TelefoneAlternativo" class="form-label">Telefone Alternativo (Opcional)</label>
                <input type="text" class="form-control" id="contato_TelefoneAlternativo" name="TelefoneAlternativo" placeholder="(XX) XXXXX-XXXX">
            </div>

            <div class="mb-3">
                <label for="contato_Observacao" class="form-label">Observação (Opcional)</label>
                <textarea class="form-control" id="contato_Observacao" name="Observacao" rows="3" placeholder="Detalhes adicionais sobre o contato"></textarea>
            </div>

            <div id="offcanvasContatoFormErrors" class="text-danger mt-2 mb-3"></div>

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarContato">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirContato" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Contato ========== -->

```
---


- `_offcanvas_dados_bancarios_cliente.cshtml`:

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


- `_offcanvas_dados_bancarios_fornecedor.cshtml`:

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
            <input type="hidden" id="dadosBancarios_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* Pega o PessoaId do fornecedorViewModel principal *@

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


- `_offcanvas_endereco_cliente.cshtml`:

```cshtml

<!-- ========== Offcanvas Endereço ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasEndereco" aria-labelledby="offcanvasEnderecoLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasEnderecoLabel">Endereço</h5>
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="enderecoOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken()

            <input type="hidden" id="endereco_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="endereco_PessoaId" name="PessoaId" value="@Model.PessoaId" />

            <div class="mb-3">
                <label for="endereco_TipoDeEndereco" class="form-label">Tipo de Endereço</label>
                <select class="form-select" id="endereco_TipoDeEndereco" name="TipoDeEndereco" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Entities.Endereco.TipoDeEnderecoEnum>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do endereço.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_PaisCodigoIso" class="form-label">País (Código ISO)</label>
                <input type="text" class="form-control" id="endereco_PaisCodigoIso" name="PaisCodigoIso" required placeholder="Ex: BR, US, PT" maxlength="2">
                <div class="invalid-feedback">Informe o código ISO do país (2 letras).</div>
            </div>

            <div class="mb-3">
                <label for="endereco_LinhaEndereco1" class="form-label">Endereço Linha 1</label>
                <input type="text" class="form-control" id="endereco_LinhaEndereco1" name="LinhaEndereco1" required placeholder="Rua, Número, Bairro/Distrito">
                <div class="invalid-feedback">Informe a linha principal do endereço.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_LinhaEndereco2" class="form-label">Endereço Linha 2 (Opcional)</label>
                <input type="text" class="form-control" id="endereco_LinhaEndereco2" name="LinhaEndereco2" placeholder="Complemento, Apartamento, Bloco">
            </div>

            <div class="mb-3">
                <label for="endereco_Cidade" class="form-label">Cidade / Localidade</label>
                <input type="text" class="form-control" id="endereco_Cidade" name="Cidade" required placeholder="Nome da cidade">
                <div class="invalid-feedback">Informe a cidade.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_EstadoOuProvincia" class="form-label">Estado / Província / Região (Opcional)</label>
                <input type="text" class="form-control" id="endereco_EstadoOuProvincia" name="EstadoOuProvincia" placeholder="UF, Estado, etc.">
            </div>

            <div class="mb-3">
                <label for="endereco_CodigoPostal" class="form-label">CEP / Código Postal / ZIP Code</label>
                <input type="text" class="form-control" id="endereco_CodigoPostal" name="CodigoPostal" required placeholder="00000-000 ou similar">
                <div class="invalid-feedback">Informe o código postal.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_InformacoesAdicionais" class="form-label">Informações Adicionais (Opcional)</label>
                <textarea class="form-control" id="endereco_InformacoesAdicionais" name="InformacoesAdicionais" rows="3" placeholder="Ponto de referência, instruções de entrega..."></textarea>
            </div>

            <div id="offcanvasEnderecoFormErrors" class="text-danger mt-2 mb-3"></div>

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarEndereco">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirEndereco" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Endereço ========== -->

```
---


- `_offcanvas_endereco_fornecedor.cshtml`:

```cshtml

<!-- ========== Offcanvas Endereço ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasEndereco" aria-labelledby="offcanvasEnderecoLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasEnderecoLabel">Endereço</h5>
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="enderecoOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken()

            <input type="hidden" id="endereco_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="endereco_PessoaId" name="PessoaId" value="@Model.PessoaId" />

            <div class="mb-3">
                <label for="endereco_TipoDeEndereco" class="form-label">Tipo de Endereço</label>
                <select class="form-select" id="endereco_TipoDeEndereco" name="TipoDeEndereco" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Entities.Endereco.TipoDeEnderecoEnum>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do endereço.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_PaisCodigoIso" class="form-label">País (Código ISO)</label>
                <input type="text" class="form-control" id="endereco_PaisCodigoIso" name="PaisCodigoIso" required placeholder="Ex: BR, US, PT" maxlength="2">
                <div class="invalid-feedback">Informe o código ISO do país (2 letras).</div>
            </div>

            <div class="mb-3">
                <label for="endereco_LinhaEndereco1" class="form-label">Endereço Linha 1</label>
                <input type="text" class="form-control" id="endereco_LinhaEndereco1" name="LinhaEndereco1" required placeholder="Rua, Número, Bairro/Distrito">
                <div class="invalid-feedback">Informe a linha principal do endereço.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_LinhaEndereco2" class="form-label">Endereço Linha 2 (Opcional)</label>
                <input type="text" class="form-control" id="endereco_LinhaEndereco2" name="LinhaEndereco2" placeholder="Complemento, Apartamento, Bloco">
            </div>

            <div class="mb-3">
                <label for="endereco_Cidade" class="form-label">Cidade / Localidade</label>
                <input type="text" class="form-control" id="endereco_Cidade" name="Cidade" required placeholder="Nome da cidade">
                <div class="invalid-feedback">Informe a cidade.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_EstadoOuProvincia" class="form-label">Estado / Província / Região (Opcional)</label>
                <input type="text" class="form-control" id="endereco_EstadoOuProvincia" name="EstadoOuProvincia" placeholder="UF, Estado, etc.">
            </div>

            <div class="mb-3">
                <label for="endereco_CodigoPostal" class="form-label">CEP / Código Postal / ZIP Code</label>
                <input type="text" class="form-control" id="endereco_CodigoPostal" name="CodigoPostal" required placeholder="00000-000 ou similar">
                <div class="invalid-feedback">Informe o código postal.</div>
            </div>

            <div class="mb-3">
                <label for="endereco_InformacoesAdicionais" class="form-label">Informações Adicionais (Opcional)</label>
                <textarea class="form-control" id="endereco_InformacoesAdicionais" name="InformacoesAdicionais" rows="3" placeholder="Ponto de referência, instruções de entrega..."></textarea>
            </div>

            <div id="offcanvasEnderecoFormErrors" class="text-danger mt-2 mb-3"></div>

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarEndereco">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirEndereco" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Endereço ========== -->

```
---


- `_offcanvas_telefone_cliente.cshtml`:

```cshtml

@* Cole este bloco no final de Views/Cliente/Details.cshtml e Views/Cliente/Edit.cshtml, ANTES de @section Scripts *@

<!-- ========== Offcanvas Telefone ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasTelefone" aria-labelledby="offcanvasTelefoneLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasTelefoneLabel">Telefone</h5> @* Título pode ser alterado via JS *@
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="telefoneOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken() @* Essencial para segurança *@

            <input type="hidden" id="telefone_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="telefone_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* PessoaId do Cliente principal *@

            <div class="mb-3">
                <label for="telefone_DDD" class="form-label">DDD</label>
                <input type="text" class="form-control" id="telefone_DDD" name="DDD" required placeholder="Ex: 21" maxlength="3">
                <div class="invalid-feedback">Por favor, informe o DDD.</div>
                <span id="validation_Telefone_DDD" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="telefone_Numero" class="form-label">Número</label>
                <input type="text" class="form-control" id="telefone_Numero" name="Numero" required placeholder="Ex: 98765-4321" maxlength="15">
                <div class="invalid-feedback">Por favor, informe o número.</div>
                <span id="validation_Telefone_Numero" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="telefone_TipoDeTelefone" class="form-label">Tipo de Telefone</label>
                <select class="form-select" id="telefone_TipoDeTelefone" name="TipoDeTelefone" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Enums.TipoDeTelefone>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do telefone.</div>
                <span id="validation_Telefone_TipoDeTelefone" class="text-danger field-validation-error"></span>
            </div>

            <div id="offcanvasTelefoneFormErrors" class="text-danger mt-2 mb-3"></div> @* Para erros gerais *@

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarTelefone">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirTelefone" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Telefone ========== -->

```
---


- `_offcanvas_telefone_fornecedor.cshtml`:

```cshtml

@* Cole este bloco no final de Views/Fornecedor/Details.cshtml e Views/Fornecedor/Edit.cshtml, ANTES de @section Scripts *@

<!-- ========== Offcanvas Telefone ========== -->
<div class="offcanvas offcanvas-end" tabindex="-1" id="offcanvasTelefone" aria-labelledby="offcanvasTelefoneLabel">
    <div class="offcanvas-header border-bottom">
        <h5 class="offcanvas-title" id="offcanvasTelefoneLabel">Telefone</h5> @* Título pode ser alterado via JS *@
        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas" aria-label="Close"></button>
    </div>
    <div class="offcanvas-body">
        <form id="telefoneOffcanvasForm" novalidate>
            @* Novo ID para o formulário *@
            @Html.AntiForgeryToken() @* Essencial para segurança *@

            <input type="hidden" id="telefone_Id" name="Id" value="00000000-0000-0000-0000-000000000000" />
            <input type="hidden" id="telefone_PessoaId" name="PessoaId" value="@Model.PessoaId" /> @* PessoaId do Fornecedor principal *@

            <div class="mb-3">
                <label for="telefone_DDD" class="form-label">DDD</label>
                <input type="text" class="form-control" id="telefone_DDD" name="DDD" required placeholder="Ex: 21" maxlength="3">
                <div class="invalid-feedback">Por favor, informe o DDD.</div>
                <span id="validation_Telefone_DDD" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="telefone_Numero" class="form-label">Número</label>
                <input type="text" class="form-control" id="telefone_Numero" name="Numero" required placeholder="Ex: 98765-4321" maxlength="15">
                <div class="invalid-feedback">Por favor, informe o número.</div>
                <span id="validation_Telefone_Numero" class="text-danger field-validation-error"></span>
            </div>

            <div class="mb-3">
                <label for="telefone_TipoDeTelefone" class="form-label">Tipo de Telefone</label>
                <select class="form-select" id="telefone_TipoDeTelefone" name="TipoDeTelefone" required>
                    <option value="">-- Selecione --</option>
                    @foreach (var tipo in Html.GetEnumSelectList<GeneralLabSolutions.Domain.Enums.TipoDeTelefone>())
                    {
                        <option value="@tipo.Value">@tipo.Text</option>
                    }
                </select>
                <div class="invalid-feedback">Por favor, selecione o tipo do telefone.</div>
                <span id="validation_Telefone_TipoDeTelefone" class="text-danger field-validation-error"></span>
            </div>

            <div id="offcanvasTelefoneFormErrors" class="text-danger mt-2 mb-3"></div> @* Para erros gerais *@

            <div class="mt-4 d-flex justify-content-end gap-2">
                <button type="submit" class="btn btn-success" id="btnSalvarTelefone">
                    <i class="ri-save-line align-bottom me-1"></i> Salvar
                </button>
                <button type="button" class="btn btn-danger" id="btnExcluirTelefone" style="display: none;">
                    <i class="ri-delete-bin-line align-bottom me-1"></i> Excluir
                </button>
                <button type="button" class="btn btn-light" data-bs-dismiss="offcanvas">
                    <i class="ri-close-line align-bottom me-1"></i> Fechar
                </button>
            </div>
        </form>
    </div>
</div>
<!-- ========== End Offcanvas Telefone ========== -->

```
---


- Ufa! Meus Deus, quanto código duplicado... isso tá me deixando louco!