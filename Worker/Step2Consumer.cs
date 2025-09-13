using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step2Consumer(ILogger<Step2Consumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<Step2Command>
{
    public async Task Consume(ConsumeContext<Step2Command> context)
    {
        logger.LogInformation("Executing Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1500);

        logger.LogInformation("Step 2 completed, initiating Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await publishEndpoint.Publish(new Step3Command(context.Message.WorkflowId));
    }
}
