using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step4Consumer(ILogger<Step4Consumer> logger) : IConsumer<Step4Command>
{
    public async Task Consume(ConsumeContext<Step4Command> context)
    {
        logger.LogInformation("Executing Step 4 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(2500);

        logger.LogInformation("Step 4 completed. Workflow finished for WorkflowId: {WorkflowId}", context.Message.WorkflowId);
    }
}
