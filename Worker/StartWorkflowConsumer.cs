using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class StartWorkflowConsumer(ILogger<StartWorkflowConsumer> logger) : IConsumer<StartWorkflowCommand>
{
    public async Task Consume(ConsumeContext<StartWorkflowCommand> context)
    {
        var workflowId = context.Message.WorkflowId;

        logger.LogInformation("[StartWorkflowConsumer] Starting workflow for WorkflowId: {WorkflowId}", workflowId);

        await Task.Delay(1000);

        logger.LogInformation("[StartWorkflowConsumer] Workflow started for WorkflowId: {WorkflowId}", workflowId);
        logger.LogInformation("[StartWorkflowConsumer] Publishing WorkflowStartedEvent for WorkflowId: {WorkflowId}", workflowId);

        try
        {
            // Publish the event to trigger the saga (only publish once)
            await context.Publish(new WorkflowStartedEvent(workflowId));

            logger.LogInformation("[StartWorkflowConsumer] Successfully published WorkflowStartedEvent for WorkflowId: {WorkflowId}", workflowId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[StartWorkflowConsumer] Error publishing WorkflowStartedEvent for WorkflowId: {WorkflowId}", workflowId);
        }
    }
}
