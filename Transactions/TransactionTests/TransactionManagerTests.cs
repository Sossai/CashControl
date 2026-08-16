using MassTransit;
using Moq;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Transactions.Application;
using Transactions.Application.DTOs;
using Transactions.Application.Responses;
using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransactionTests
{
    public class TransactionManagerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly TransactionManager _transactionManager;

        public TransactionManagerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _transactionManager = new TransactionManager(_mockTransactionRepository.Object, _mockPublishEndpoint.Object);
        }

        [Fact]
        public async Task RegisterTransaction_has_Success()
        {
            var registerTransactionDTO = new RegisterTransactionDTO
            {
                Date = DateOnly.Parse("2026-10-10"),
                Type = TransactionType.Credit,
                Amount = 100,
                Description = "Test Success"
            };

            _mockTransactionRepository.Setup(s => s.Create(It.IsAny<Transaction>())).ReturnsAsync(It.IsAny<Guid>());
            _mockPublishEndpoint.Setup(s => s.Publish(It.IsAny<ProcessTransaction>()));

            var response = await _transactionManager.RegisterTransaction(registerTransactionDTO);

            Assert.True(response.Success);
        }

        [Fact]
        public async Task RegisterTransaction_has_Failure_on_save_data()
        {
            var registerTransactionDTO = new RegisterTransactionDTO
            {
                Date = DateOnly.Parse("2026-10-10"),
                Type = TransactionType.Credit,
                Amount = 100,
                Description = "Test Failure"
            };

            _mockTransactionRepository.Setup(s => s.Create(It.IsAny<Transaction>()))
                .ThrowsAsync(new Exception("Exception test"));
            _mockPublishEndpoint.Setup(s => s.Publish(It.IsAny<ProcessTransaction>()));

            var response = await _transactionManager.RegisterTransaction(registerTransactionDTO);

            Assert.False(response.Success);
        }


        [Fact]
        public async Task RegisterTransaction_has_Failure_on_publish()
        {
            var registerTransactionDTO = new RegisterTransactionDTO
            {
                Date = DateOnly.Parse("2026-10-10"),
                Type = TransactionType.Credit,
                Amount = 100,
                Description = "Test Failure"
            };

            _mockTransactionRepository.Setup(s => s.Create(It.IsAny<Transaction>())).ReturnsAsync(It.IsAny<Guid>());
            _mockPublishEndpoint.Setup(s => s.Publish(It.IsAny<ProcessTransaction>()))
                .ThrowsAsync(new Exception("Exception test"));

            var response = await _transactionManager.RegisterTransaction(registerTransactionDTO);

            Assert.False(response.Success);
        }
    }
}
