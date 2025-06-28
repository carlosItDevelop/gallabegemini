using FluentValidation;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Validations
{
    public class DeleteClienteValidation : AbstractValidator<Cliente>
    {
        public DeleteClienteValidation()
        {
            RuleFor(x => x.Nome)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");
        }
    }
}
