using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class StartWorkflowConsumer(ILogger<StartWorkflowConsumer> logger, IPublishEndpoint publishEndpoint) : IConsumer<StartWorkflowCommand>
{
    public async Task Consume(ConsumeContext<StartWorkflowCommand> context)
    {
        logger.LogInformation("Starting workflow for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1000);

        logger.LogInformation("Workflow started, initiating Step 1 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await publishEndpoint.Publish(new Step1Command(context.Message.WorkflowId));
    }
}
