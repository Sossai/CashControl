namespace Consolidation.Infrastructure.Interfaces
{
    public interface IConsolidateUnitOfWork
    {
        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
