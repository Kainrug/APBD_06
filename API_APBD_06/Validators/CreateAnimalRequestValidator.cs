using API_APBD_06.DTO;
using FluentValidation;

namespace API_APBD_06.Validators;

public class CreateAnimalRequestValidator : AbstractValidator<CreateAnimalsRequest>
{
    public CreateAnimalRequestValidator()
    {
        RuleFor(e => e.Name).MaximumLength(30).NotNull();
        RuleFor(e => e.Descripton).MaximumLength(50);
        RuleFor(e => e.Category).MaximumLength(50);
        RuleFor(e => e.Area).MaximumLength(50);
    }
}