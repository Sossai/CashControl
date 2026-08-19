using Consolidation.Application;
using Consolidation.Application.Interfaces;
using MassTransit;
using Shared.Domain.Entities;

namespace Consolidation.Worker.Consumer
{
    public class TransactionConsumer : IConsumer<ProcessTransaction>
    {
        private readonly IConsolidateManager _consolidateManager;

        public TransactionConsumer(IConsolidateManager consolidateManager)
        {
            _consolidateManager = consolidateManager;
        }

        public async Task Consume(ConsumeContext<ProcessTransaction> context)
        {
            var message = context.Message;

            await _consolidateManager.ConsolidateTransaction(message);

            await Task.CompletedTask;
        }
    }
}
