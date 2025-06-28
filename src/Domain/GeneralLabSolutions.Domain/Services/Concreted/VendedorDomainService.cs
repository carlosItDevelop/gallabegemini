using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Enums;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Services.Abstractions;
using GeneralLabSolutions.Domain.Validations;

namespace GeneralLabSolutions.Domain.Services.Concreted
{
    public class VendedorDomainService : IVendedorDomainService
    {
        private readonly IVendedorRepository _vendedorRepository;
        private readonly INotificador _notificador;

        public VendedorDomainService(IVendedorRepository vendedorRepository, INotificador notificador)
        {
            _vendedorRepository = vendedorRepository;
            _notificador = notificador;
        }

        public async Task<Vendedor> AdicionarVendedor(Vendedor vendedor)
        {
            if (!ExecutarValidacao(new VendedorValidation(), vendedor)) return vendedor;

            await _vendedorRepository.AddAsync(vendedor);
            return vendedor;
        }

        public async Task<Vendedor> AtualizarVendedor(Vendedor vendedor)
        {
            if (!ExecutarValidacao(new VendedorValidation(), vendedor)) return vendedor;

            await _vendedorRepository.UpdateAsync(vendedor);
            return vendedor;
        }

        public async Task ExcluirVendedor(Guid id)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(id);
            if (vendedor == null)
            {
                _notificador.Handle(new Notificacao("Vendedor não encontrado."));
                return;
            }
            await _vendedorRepository.DeleteAsync(vendedor);
        }

        public async Task<Vendedor?> ObterVendedorPorId(Guid id)
        {
            return await _vendedorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Vendedor>> ObterTodosVendedores()
        {
            return await _vendedorRepository.GetAllAsync();
        }

        // Métodos para gerenciar agregados (implementação detalhada virá após a validação da entidade Vendedor)
        public async Task AdicionarDadosBancarios(Guid vendedorId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para adicionar dados bancários ao vendedor
            // vendedor.AdicionarDadosBancarios(banco, agencia, conta, tipoConta);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AtualizarDadosBancarios(Guid vendedorId, Guid dadosBancariosId, string banco, string agencia, string conta, TipoDeContaBancaria tipoConta)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para atualizar dados bancários do vendedor
            // vendedor.AtualizarDadosBancarios(dadosBancariosId, banco, agencia, conta, tipoConta);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task RemoverDadosBancarios(Guid vendedorId, Guid dadosBancariosId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para remover dados bancários do vendedor
            // vendedor.RemoverDadosBancarios(dadosBancariosId);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AdicionarTelefone(Guid vendedorId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para adicionar telefone ao vendedor
            // vendedor.AdicionarTelefone(ddd, numero, tipoTelefone);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AtualizarTelefone(Guid vendedorId, Guid telefoneId, string ddd, string numero, TipoDeTelefone tipoTelefone)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para atualizar telefone do vendedor
            // vendedor.AtualizarTelefone(telefoneId, ddd, numero, tipoTelefone);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task RemoverTelefone(Guid vendedorId, Guid telefoneId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para remover telefone do vendedor
            // vendedor.RemoverTelever(telefoneId);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AdicionarContato(Guid vendedorId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para adicionar contato ao vendedor
            // vendedor.AdicionarContato(nome, email, telefone, tipoDeContato, emailAlt, telAlt, obs);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AtualizarContato(Guid vendedorId, Guid contatoId, string nome, string email, string telefone, TipoDeContato tipoDeContato, string emailAlt = "", string telAlt = "", string obs = "")
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para atualizar contato do vendedor
            // vendedor.AtualizarContato(contatoId, nome, email, telefone, tipoDeContato, emailAlt, telAlt, obs);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task RemoverContato(Guid vendedorId, Guid contatoId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para remover contato do vendedor
            // vendedor.RemoverContato(contatoId);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AdicionarEndereco(Guid vendedorId, string pais, string linha1, string cidade, string cep, Endereco.TipoDeEnderecoEnum tipo, string? linha2, string? estado, string? info)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para adicionar endereço ao vendedor
            // vendedor.AdicionarEndereco(pais, linha1, cidade, cep, tipo, linha2, estado, info);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task AtualizarEndereco(Guid vendedorId, Guid enderecoId, string paisCodigoIso, string linhaEndereco1, string cidade, string codigoPostal, Endereco.TipoDeEnderecoEnum tipoDeEndereco, string? linhaEndereco2 = null, string? estadoOuProvincia = null, string? informacoesAdicionais = null)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para atualizar endereço do vendedor
            // vendedor.AtualizarEndereco(enderecoId, paisCodigoIso, linhaEndereco1, cidade, codigoPostal, tipoDeEndereco, linhaEndereco2, estadoOuProvincia, informacoesAdicionais);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        public async Task RemoverEndereco(Guid vendedorId, Guid enderecoId)
        {
            var vendedor = await _vendedorRepository.GetByIdAsync(vendedorId);
            if (vendedor == null) { _notificador.Handle(new Notificacao("Vendedor não encontrado.")); return; }
            // Lógica para remover endereço do vendedor
            // vendedor.RemoverEndereco(enderecoId);
            await _vendedorRepository.UpdateAsync(vendedor);
        }

        private bool ExecutarValidacao<TValidation, TEntity>(TValidation validacao, TEntity entidade)
            where TValidation : VendedorValidation
            where TEntity : Vendedor
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid) return true;

            foreach (var error in validator.Errors)
            {
                _notificador.Handle(new Notificacao(error.ErrorMessage));
            }

            return false;
        }
    }
}