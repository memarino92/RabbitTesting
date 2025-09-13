using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step3Consumer(ILogger<Step3Consumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<Step3Command>
{
    public async Task Consume(ConsumeContext<Step3Command> context)
    {
        logger.LogInformation("Executing Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(3000);

        logger.LogInformation("Step 3 completed, initiating Step 4 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await publishEndpoint.Publish(new Step4Command(context.Message.WorkflowId));
    }
}
