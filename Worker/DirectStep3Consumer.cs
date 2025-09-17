using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class DirectStep3Consumer(
    ILogger<DirectStep3Consumer> logger, 
    IPublishEndpoint publishEndpoint,
    IResultStore resultStore)
    : IConsumer<Step3Command>
{
    private static readonly Random _random = new();

    public async Task Consume(ConsumeContext<Step3Command> context)
    {
        logger.LogInformation("Executing Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        // Simulate work
        await Task.Delay(1000);
        
        // Generate a random result and store it in Redis
        var result = _random.Next(1, 100);
        await resultStore.StoreStepResult(context.Message.WorkflowId, "Step3", result);
        
        logger.LogInformation("Step 3 completed for WorkflowId: {WorkflowId} with result {Result}", 
            context.Message.WorkflowId, result);

        // Only publish the completion event without the result
        await publishEndpoint.Publish(new Step3CompletedEvent(context.Message.WorkflowId));
    }
}
