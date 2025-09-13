using MassTransit;
using Microsoft.Extensions.Logging;
using Contracts;

namespace Worker;

public class StartWorkflowConsumer : IConsumer<StartWorkflowCommand>
{
    private readonly ILogger<StartWorkflowConsumer> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public StartWorkflowConsumer(ILogger<StartWorkflowConsumer> logger, IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Consume(ConsumeContext<StartWorkflowCommand> context)
    {
        _logger.LogInformation("Starting workflow for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await Task.Delay(1000);

        _logger.LogInformation("Workflow started, initiating Step 1 for WorkflowId: {WorkflowId}", context.Message.WorkflowId);

        await _publishEndpoint.Publish(new Step1Command(context.Message.WorkflowId));
    }
}
