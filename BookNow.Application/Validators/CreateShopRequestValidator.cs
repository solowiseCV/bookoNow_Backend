using BookNow.Application.DTOs.Shop;
using FluentValidation;

namespace BookNow.Application.Validators;

public class CreateShopRequestValidator : AbstractValidator<CreateShopRequestDto>
{
    public CreateShopRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
