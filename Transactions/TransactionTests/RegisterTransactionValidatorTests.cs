using FluentValidation.TestHelper;
using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Transactions.Application.DTOs;
using Transactions.Application.Validators;
using Xunit;


namespace TransactionTests
{
    public class RegisterTransactionValidatorTests
    {
        private readonly RegisterTransactionValidator _validator;

        public RegisterTransactionValidatorTests()
        {
            _validator = new RegisterTransactionValidator();
        }

        [Fact]
        public void Should_have_error_when_amount_is_zero()
        {
            var request = new RegisterTransactionDTO
            {

                Amount = 0,
                Type = 0,
                Date = DateOnly.Parse("2026-08-10"),
                Description = ""
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Amount);
        }

        [Theory]
        [InlineData(TransactionType.Credit)]
        [InlineData(TransactionType.Debit)]
        public void Should_not_have_error_when_request_is_valid(TransactionType type)
        {
            var request = new RegisterTransactionDTO
            {
                Amount = 10,
                Type = TransactionType.Credit,
                Date = DateOnly.Parse("2026-08-10"),
                Description = ""
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(TransactionType.Credit)]
        [InlineData(TransactionType.Debit)]
        public void Should_not_have_error_when_request_TransactionType_is_valid(TransactionType type)
        {
            var request = new RegisterTransactionDTO
            {
                Amount = 10,
                Type = type,
                Date = DateOnly.Parse("2026-08-10"),
                Description = ""
            };

            var result = _validator.TestValidate(request);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(TransactionType.None)]
        public void Should_have_error_when_request_TransactionType_is_invalid(TransactionType type)
        {
            var request = new RegisterTransactionDTO
            {
                Amount = 10,
                Type = type,
                Date = DateOnly.Parse("2026-08-10"),
                Description = ""
            };

            var result = _validator.TestValidate(request);

            result.ShouldHaveValidationErrorFor(x => x.Type);
        }
    }
}