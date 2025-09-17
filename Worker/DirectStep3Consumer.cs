using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep3Consumer(ILogger<DirectStep3Consumer> logger, IPublishEndpoint publishEndpoint)
    : IConsumer<Step3Command>
{
    public async Task Consume(ConsumeContext<Step3Command> context)
    {
        logger.LogInformation("Executing Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(3000);

        logger.LogInformation("Step 3 completed for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        // Publish completion event for the saga
        await publishEndpoint.Publish(new Step3CompletedEvent(context.Message.WorkflowId));
    }
}
