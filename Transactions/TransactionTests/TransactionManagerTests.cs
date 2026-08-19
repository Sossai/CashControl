using MassTransit;
using Moq;
using Shared.Domain.Entities;
using Shared.Domain.Enums;
using Transactions.Application;
using Transactions.Application.DTOs;
using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Interfaces;

namespace TransactionTests
{
    public class TransactionManagerTests
    {
        private readonly Mock<ITransactionRepository> _mockTransactionRepository;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly Mock<ITransactionUnitOfWork> _mockTransactionUnitOfWork;
        private readonly TransactionManager _transactionManager;

        public TransactionManagerTests()
        {
            _mockTransactionRepository = new Mock<ITransactionRepository>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _mockTransactionUnitOfWork = new Mock<ITransactionUnitOfWork>();

            _transactionManager = new TransactionManager(_mockTransactionRepository.Object, _mockPublishEndpoint.Object, _mockTransactionUnitOfWork.Object);
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

            _mockTransactionRepository.Setup(s => s.AddAsync(It.IsAny<Transaction>()));
            _mockTransactionUnitOfWork.Setup(s => s.SaveChangesAsync());
            _mockPublishEndpoint.Setup(s => s.Publish(It.IsAny<ProcessTransaction>()));

            var response = await _transactionManager.RegisterTransaction(registerTransactionDTO);

            Assert.True(response.Success);
            _mockPublishEndpoint.Verify(s => s.Publish(It.IsAny<ProcessTransaction>()), Times.Once);
            _mockTransactionRepository.Verify(s => s.AddAsync(It.IsAny<Transaction>()), Times.Once);
            _mockTransactionUnitOfWork.Verify(s => s.SaveChangesAsync(), Times.Once);
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

            _mockTransactionRepository.Setup(s => s.AddAsync(It.IsAny<Transaction>()));
            _mockTransactionUnitOfWork.Setup(s => s.SaveChangesAsync()).ThrowsAsync(new Exception("Exception test"));
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

            _mockTransactionRepository.Setup(s => s.AddAsync(It.IsAny<Transaction>()));
            _mockTransactionUnitOfWork.Setup(s => s.SaveChangesAsync());
            _mockPublishEndpoint.Setup(s => s.Publish(It.IsAny<ProcessTransaction>())).ThrowsAsync(new Exception("Exception test"));

            var response = await _transactionManager.RegisterTransaction(registerTransactionDTO);

            Assert.False(response.Success);
        }
    }
}
