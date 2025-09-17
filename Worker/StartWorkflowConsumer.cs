using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class StartWorkflowConsumer(ILogger<StartWorkflowConsumer> logger) : IConsumer<StartWorkflowCommand>
{
    public async Task Consume(ConsumeContext<StartWorkflowCommand> context)
    {
        var workflowId = context.Message.WorkflowId;

        logger.LogInformation("Starting workflow for WorkflowId: {WorkflowId}", workflowId);
        Console.WriteLine($"[StartWorkflowConsumer] Starting workflow for WorkflowId: {workflowId}");

        await Task.Delay(1000);

        logger.LogInformation("Workflow started for WorkflowId: {WorkflowId}", workflowId);
        Console.WriteLine($"[StartWorkflowConsumer] Workflow started for WorkflowId: {workflowId}");
        Console.WriteLine($"[StartWorkflowConsumer] Publishing WorkflowStartedEvent for WorkflowId: {workflowId}");

        try
        {
            // Publish the event to trigger the saga (only publish once)
            await context.Publish(new WorkflowStartedEvent(workflowId));

            Console.WriteLine($"[StartWorkflowConsumer] Successfully published WorkflowStartedEvent for WorkflowId: {workflowId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StartWorkflowConsumer] Error publishing WorkflowStartedEvent: {ex.Message}");
            logger.LogError(ex, "Error publishing WorkflowStartedEvent for WorkflowId: {WorkflowId}", workflowId);
        }
    }
}
