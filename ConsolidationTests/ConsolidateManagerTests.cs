using Consolidation.Application;
using Consolidation.Domain.Entities;
using Consolidation.Domain.Interfaces;
using Consolidation.Infrastructure.Interfaces;
using Moq;
using Shared.Domain.Entities;
using Shared.Domain.Enums;

namespace ConsolidationTests
{
    public class ConsolidateManagerTests
    {
        private readonly Mock<IDailyConsolidateRepository> _mockDailyConsolidateRepository;
        private readonly Mock<IProcessedEventRepository> _mockProcessedEventRepository;
        private readonly Mock<IConsolidateUnitOfWork> _mockConsolidateUnitOfWork;

        private readonly ConsolidateManager _consolidateManager;

        public ConsolidateManagerTests()
        {
            _mockDailyConsolidateRepository = new Mock<IDailyConsolidateRepository>();
            _mockProcessedEventRepository = new Mock<IProcessedEventRepository>();
            _mockConsolidateUnitOfWork = new Mock<IConsolidateUnitOfWork>();
            _consolidateManager = new ConsolidateManager(
                _mockDailyConsolidateRepository.Object,
                _mockProcessedEventRepository.Object,
                _mockConsolidateUnitOfWork.Object);
        }

        [Fact]
        public async Task ConsolidateTransaction_has_Success()
        {
            _mockProcessedEventRepository.Setup(s => s.IsProcessedAsync(It.IsAny<Guid>())).ReturnsAsync(false);
            _mockDailyConsolidateRepository.Setup(s=> s.Process(It.IsAny<DateOnly>(), It.IsAny<decimal>()));
            _mockProcessedEventRepository.Setup(s=> s.RegisterProcessedAsync(It.IsAny<Guid>()));
            _mockConsolidateUnitOfWork.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>())).Returns((Func<Task> action) => action());

            var processTransaction = new ProcessTransaction(Guid.NewGuid(), DateOnly.Parse("2026-10-10"), TransactionType.Debit, 10, DateTime.UtcNow);

            await _consolidateManager.ConsolidateTransaction(processTransaction);

            _mockProcessedEventRepository.Verify(s => s.IsProcessedAsync(It.IsAny<Guid>()), Times.Once);
            _mockDailyConsolidateRepository.Verify(s => s.Process(It.IsAny<DateOnly>(), It.IsAny<decimal>()), Times.Once);
            _mockProcessedEventRepository.Verify(s => s.RegisterProcessedAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task ConsolidateTransaction_with_duplicated_idempotency()
        {
            _mockProcessedEventRepository.Setup(s => s.IsProcessedAsync(It.IsAny<Guid>())).ReturnsAsync(true);
            _mockDailyConsolidateRepository.Setup(s => s.Process(It.IsAny<DateOnly>(), It.IsAny<decimal>()));
            _mockProcessedEventRepository.Setup(s => s.RegisterProcessedAsync(It.IsAny<Guid>()));
            _mockConsolidateUnitOfWork.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task>>())).Returns((Func<Task> action) => action());

            var processTransaction = new ProcessTransaction(Guid.NewGuid(), DateOnly.Parse("2026-10-10"), TransactionType.Debit, 10, DateTime.UtcNow);

            await _consolidateManager.ConsolidateTransaction(processTransaction);

            _mockProcessedEventRepository.Verify(s => s.IsProcessedAsync(It.IsAny<Guid>()), Times.Once);
            _mockDailyConsolidateRepository.Verify(s => s.Process(It.IsAny<DateOnly>(), It.IsAny<decimal>()), Times.Never);
            _mockProcessedEventRepository.Verify(s => s.RegisterProcessedAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetConsolidate_has_Success()
        {
            var dailyConsolidate = new DailyConsolidate
            {
                Date = DateOnly.Parse("2026-10-10"),
                AccumulatedBalance = 100,
                UpdatedAt = DateTime.UtcNow

            };
            _mockDailyConsolidateRepository.Setup(s => s.GetConsolidate(It.IsAny<DateOnly>())).ReturnsAsync(dailyConsolidate);

            var response = await _consolidateManager.GetConsolidate(It.IsAny<DateOnly>());

            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetConsolidate_has_Failure()
        {

            _mockDailyConsolidateRepository.Setup(s => s.GetConsolidate(It.IsAny<DateOnly>())).ReturnsAsync(default(DailyConsolidate));

            var response = await _consolidateManager.GetConsolidate(It.IsAny<DateOnly>());

            Assert.False(response.Success);
        }

        [Theory]
        [InlineData (TransactionType.Credit)]
        [InlineData (TransactionType.Debit)]
        public async Task ResolveAmount_has_Success(TransactionType type)
        {
            var processTransaction = new ProcessTransaction(
            
                Guid.NewGuid(),
                DateOnly.Parse("2026-10-10"),
                type,
                100,
                DateTime.UtcNow

            );

            var amount = ConsolidateManager.ResolveAmount(processTransaction);

            var isValid = type switch
            {
                TransactionType.Credit => amount > 0,
                TransactionType.Debit => amount < 0,
                _ => false
            };
            Assert.True(isValid);
        }
    }
}
