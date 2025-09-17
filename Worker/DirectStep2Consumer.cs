using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep2Consumer(ILogger<DirectStep2Consumer> logger, IPublishEndpoint publishEndpoint)
    : IConsumer<Step2Command>
{
    public async Task Consume(ConsumeContext<Step2Command> context)
    {
        logger.LogInformation("Executing Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1500);

        logger.LogInformation("Step 2 completed for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        // Publish completion event for the saga
        await publishEndpoint.Publish(new Step2CompletedEvent(context.Message.WorkflowId));
    }
}
