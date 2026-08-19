namespace Transactions.Infrastructure.Interfaces
{
    public interface ITransactionUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
