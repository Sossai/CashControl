using Transactions.Domain.Entities;
using Transactions.Domain.Interfaces;
using Transactions.Infrastructure.Entities;


namespace Transactions.Infrastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly TransactionsDbContext _transactionsDbContext;

        public TransactionRepository(TransactionsDbContext transactionsDbContext)
        {
            _transactionsDbContext = transactionsDbContext;
        }

        public async Task<Guid> Create(Transaction transaction)
        {
            _transactionsDbContext.Transactions.Add(transaction);
            await _transactionsDbContext.SaveChangesAsync();
            return transaction.Id;
        }

        public async Task AddAsync(Transaction transaction)
        {
            _transactionsDbContext.Transactions.Add(transaction);
        }
    }
}
