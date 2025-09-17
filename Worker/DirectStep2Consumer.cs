using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep2Consumer(
    ILogger<DirectStep2Consumer> logger, 
    IPublishEndpoint publishEndpoint,
    IResultStore resultStore)
    : IConsumer<Step2Command>
{
    private static readonly Random _random = new();

    public async Task Consume(ConsumeContext<Step2Command> context)
    {
        logger.LogInformation("Executing Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        // Simulate work
        await Task.Delay(1000);
        
        // Generate a random result and store it in Redis
        var result = _random.Next(1, 100);
        await resultStore.StoreStepResult(context.Message.WorkflowId, "Step2", result);
        
        logger.LogInformation("Step 2 completed for WorkflowId: {WorkflowId} with result {Result}", 
            context.Message.WorkflowId, result);

        // Only publish the completion event without the result
        await publishEndpoint.Publish(new Step2CompletedEvent(context.Message.WorkflowId));
    }
}
