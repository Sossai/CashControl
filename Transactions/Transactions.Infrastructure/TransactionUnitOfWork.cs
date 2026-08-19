
using Transactions.Infrastructure.Interfaces;

namespace Transactions.Infrastructure
{
    public class TransactionUnitOfWork : ITransactionUnitOfWork
    {
        private readonly TransactionsDbContext _transactionsDbContext;

        public TransactionUnitOfWork(TransactionsDbContext transactionsDbContext)
        {
            _transactionsDbContext = transactionsDbContext;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _transactionsDbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
