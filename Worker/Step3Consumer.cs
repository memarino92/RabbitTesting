using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step3Consumer : IConsumer<Step3Command>
{
    private readonly ILogger<Step3Consumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public Step3Consumer(ILogger<Step3Consumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<Step3Command> context)
    {
        _logger.LogInformation("Executing Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(3000);

        _logger.LogInformation("Step 3 completed, initiating Step 4 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await _publishEndpoint.Publish(new Step4Command(context.Message.WorkflowId));
    }
}
