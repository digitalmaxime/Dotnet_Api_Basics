using DishAppPluralsight.Models;
using FluentValidation;

namespace DishAppPluralsight.Validators;

public class DishForCreationDtoValidator : AbstractValidator<DishForCreationDto>
{
    public DishForCreationDtoValidator()
    {
        RuleFor(x => x.Name).NotNull();
        RuleFor(x => x.Name).Length(3, 10);
    }
}