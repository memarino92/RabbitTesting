using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step4Consumer : IConsumer<Step4Command>
{
    private readonly ILogger<Step4Consumer> _logger;

    public Step4Consumer(ILogger<Step4Consumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Step4Command> context)
    {
        _logger.LogInformation("Executing Step 4 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(2500);

        _logger.LogInformation("Step 4 completed. Workflow finished for WorkflowId: {WorkflowId}", context.Message.WorkflowId);
    }
}
