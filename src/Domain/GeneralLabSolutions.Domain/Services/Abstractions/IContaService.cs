using GeneralLabSolutions.Domain.Entities;

public interface IContaService : IDisposable
{
    Task<bool> AdicionarConta(Conta conta);
    Task<bool> AtualizarConta(Conta conta);
    Task<bool> RemoverConta(Conta conta);
    Task MarcarComoPaga(Guid contaId);
    Task MarcarComoNaoPaga(Guid contaId);
    Task InativarConta(Guid contaId);
}
