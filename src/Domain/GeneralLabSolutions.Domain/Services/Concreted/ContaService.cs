using GeneralLabSolutions.Domain.Application.Events;
using GeneralLabSolutions.Domain.Entities;
using GeneralLabSolutions.Domain.Interfaces;
using GeneralLabSolutions.Domain.Mensageria;
using GeneralLabSolutions.Domain.Notifications;
using GeneralLabSolutions.Domain.Validations;

namespace GeneralLabSolutions.Domain.Services
{
    public class ContaService : BaseService, IContaService
    {
        private readonly IContaRepository _contaRepository;
        private readonly IGenericRepository<Conta, Guid> _query; //Para pesquisas!
        private readonly IMediatorHandler _mediatorHandler;

        public ContaService(IContaRepository contaRepository,
                            IGenericRepository<Conta, Guid> query,
                            INotificador notificador,
                            IMediatorHandler mediatorHandler) : base(notificador)
        {
            _contaRepository = contaRepository;
            _query = query;
            _mediatorHandler = mediatorHandler;
        }

        public async Task<bool> AdicionarConta(Conta conta)
        {
            //Validando a Entidade
            if (!ExecutarValidacao(new ContaValidation(), conta))
                return false;

            // Regras de Negócio (exemplo)
            if (await _contaRepository.ExisteConta(conta.Id))
            {
                Notificar("Já existe uma conta com este ID.");
                return false;
            }

            //Outras validações de negócio aqui!

            await _contaRepository.AddAsync(conta);

            return true; //Retorna true se tudo estiver ok!
        }

        public async Task<bool> AtualizarConta(Conta conta)
        {
            if (!ExecutarValidacao(new ContaValidation(), conta))
                return false;

            //Verifica se a conta existe!
            if (!await _contaRepository.ExisteConta(conta.Id))
            {
                Notificar("Conta não encontrada.");
                return false;
            }

            // Regras de Negócio (exemplo)
            var contaExistente = await _query.GetByIdAsync(conta.Id);

            // Impedir de alterar o tipo de conta (regra de negócio de exemplo)
            if (contaExistente.TipoDeConta != conta.TipoDeConta)
            {
                Notificar("Não é permitido alterar o Tipo de Conta.");
                return false;
            }

            // Impedir a alteração se já estiver paga (regra de negócio)
            if (contaExistente.EstaPaga)
            {
                Notificar("Não é possível alterar uma conta que já foi paga.");
                return false;
            }

            await _contaRepository.UpdateAsync(conta);
            return true;

        }

        public async Task<bool> RemoverConta(Conta conta)
        {
            //Verifica se a conta existe!
            if (!await _contaRepository.ExisteConta(conta.Id))
            {
                Notificar("Conta não encontrada.");
                return false;
            }

            // Regras de Negócio (exemplo)
            if (conta.EstaPaga)
            {
                Notificar("Não é possível remover uma conta que já foi paga.");
                return false;
            }

            await _contaRepository.DeleteAsync(conta);
            return true;
        }


        public async Task MarcarComoPaga(Guid contaId)
        {
            var conta = await _query.GetByIdAsync(contaId);
            if (conta == null)
            {
                Notificar("Conta não encontrada.");
                return;
            }

            conta.MarcarComoPaga(); // Chama o método de domínio
            await _contaRepository.UpdateAsync(conta);
            await _mediatorHandler.PublicarEvento(new ContaPagaEvent(contaId));
        }

        public async Task MarcarComoNaoPaga(Guid contaId)
        {
            var conta = await _query.GetByIdAsync(contaId);
            if (conta == null)
            {
                Notificar("Conta não encontrada.");
                return;
            }
            conta.MarcarComoNaoPaga();  // Chama o método de domínio
            await _contaRepository.UpdateAsync(conta);
            await _mediatorHandler.PublicarEvento(new ContaEstornadaEvent(contaId));

        }

        public async Task InativarConta(Guid contaId)
        {
            var conta = await _query.GetByIdAsync(contaId);
            if (conta == null)
            {
                Notificar("Conta não encontrada.");
                return;
            }

            conta.InativarConta(); //Método de Dominio
            await _contaRepository.UpdateAsync(conta);
            //await _mediatorHandler.PublicarEvento(new ContaInativadaEvent(contaId));
        }


        // Outros métodos de serviço (consulta, etc.)
        public void Dispose()
        {
            _contaRepository?.Dispose();
            _query?.Dispose();
        }

    }

}