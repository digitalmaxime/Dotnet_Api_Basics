using FluentValidation;

namespace MinimalApiStructured.Validation;

public class CreateCustomerRequestValidator: AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
    }
}