using CloudObjects.App.Interfaces;
using CloudObjects.Models;
using FluentValidation;

namespace CloudObjects.App.Validators
{
    public class AccountDtoValidator : AbstractValidator<Account>, IDtoValidator
    {
        public AccountDtoValidator()
        {
            RuleFor(e => e.Key)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(e => e.Name)
                .MinimumLength(5)
                    .WithMessage("Account name must be least 5 characters long")
                .MaximumLength(50)
                    .WithMessage("Account name must be less than 50 characters long")
                .Matches("^[a-zA-Z0-9\\.\\-]+$")
                    .WithMessage("Account name contains one or more illegal characters, allowed only letters, digits, dots and dashes");
        }
    }
}
