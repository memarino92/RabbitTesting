using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;


namespace Worker;

public class Step2Consumer : IConsumer<Step2Command>
{
    private readonly ILogger<Step2Consumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public Step2Consumer(ILogger<Step2Consumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<Step2Command> context)
    {
        _logger.LogInformation("Executing Step 2 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1500);

        _logger.LogInformation("Step 2 completed, initiating Step 3 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await _publishEndpoint.Publish(new Step3Command(context.Message.WorkflowId));
    }
}
