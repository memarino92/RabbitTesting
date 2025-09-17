using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep1Consumer(ILogger<DirectStep1Consumer> logger, IPublishEndpoint publishEndpoint)
    : IConsumer<Step1Command>
{
    public async Task Consume(ConsumeContext<Step1Command> context)
    {
        logger.LogInformation("Executing Step 1 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1000);

        logger.LogInformation("Step 1 completed for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await publishEndpoint.Publish(new Step1CompletedEvent(context.Message.WorkflowId));
    }
}
