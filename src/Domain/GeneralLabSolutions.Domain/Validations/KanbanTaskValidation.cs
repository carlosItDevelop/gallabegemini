using FluentValidation;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Validations
{
    public class KanbanTaskValidation : AbstractValidator<KanbanTask>
    {
        public KanbanTaskValidation()
        {

            RuleFor(x => x.Title)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");

            RuleFor(x => x.Description)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");

            RuleFor(x => x.DueDate)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo")
                .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("O campo '{PropertyName}' não pode estar no passado!");

        }

    }
}
