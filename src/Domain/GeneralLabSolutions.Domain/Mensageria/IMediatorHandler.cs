using FluentValidation.Results;

namespace GeneralLabSolutions.Domain.Mensageria
{
    public interface IMediatorHandler
    {
        Task PublicarEvento<T>(T evento) where T : Event;
    }
}