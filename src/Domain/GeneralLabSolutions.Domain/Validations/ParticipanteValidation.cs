using FluentValidation;
using GeneralLabSolutions.Domain.Entities;

namespace GeneralLabSolutions.Domain.Validations
{
    public class ParticipanteValidation : AbstractValidator<Participante>
    {
        public ParticipanteValidation()
        {

            RuleFor(x => x.Id)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");

            RuleFor(x => x.Name)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");

            RuleFor(x => x.Email)
                .NotNull().WithMessage("O campo '{PropertyName}' não pode ser Nulo");

        }

    }
}
