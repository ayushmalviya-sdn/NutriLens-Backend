using App.Application.Dto;
using FluentValidation;

namespace App.Api.Validation
{
    public class UsersValidator : AbstractValidator<UserDto>
    {
        public UsersValidator()
        {
            RuleFor(u => u.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(u => u.LastName)
                .NotEmpty().WithMessage("Last name is required.");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be valid.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}