using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step1Consumer(ILogger<Step1Consumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<Step1Command>
{
    public async Task Consume(ConsumeContext<Step1Command> context)
    {
        logger.LogInformation("Executing Step 1 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(2000);

        logger.LogInformation("Step 1 completed, initiating Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await publishEndpoint.Publish(new Step2Command(context.Message.WorkflowId));
    }
}
