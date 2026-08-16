using FluentValidation;
using Shared.Domain.Enums;
using Transactions.Application.DTOs;

namespace Transactions.Application.Validators
{
    public class RegisterTransactionValidator : AbstractValidator<RegisterTransactionDTO>
    {

        public RegisterTransactionValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty()
                .NotEqual(default(DateOnly))
                .WithMessage("The date is mandatory.");

            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .NotEqual(TransactionType.None)
                .WithMessage("Invalid type.");
        }
    }
}
