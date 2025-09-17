using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep4Consumer(ILogger<DirectStep4Consumer> logger, IPublishEndpoint publishEndpoint)
    : IConsumer<Step4Command>
{
    public async Task Consume(ConsumeContext<Step4Command> context)
    {
        logger.LogInformation("Executing Step 4 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1000);

        logger.LogInformation("Step 4 completed for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        // Publish completion event for the saga
        await publishEndpoint.Publish(new Step4CompletedEvent(context.Message.WorkflowId));
    }
}
