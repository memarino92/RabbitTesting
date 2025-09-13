using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step1Consumer : IConsumer<Step1Command>
{
    private readonly ILogger<Step1Consumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public Step1Consumer(ILogger<Step1Consumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<Step1Command> context)
    {
        _logger.LogInformation("Executing Step 1 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(2000);

        _logger.LogInformation("Step 1 completed, initiating Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await _publishEndpoint.Publish(new Step2Command(context.Message.WorkflowId));
    }
}
