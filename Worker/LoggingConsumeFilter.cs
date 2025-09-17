using MassTransit;
using Microsoft.Extensions.Logging;

namespace Worker;

public class LoggingConsumeFilter<T>(ILogger<LoggingConsumeFilter<T>> logger) : IFilter<ConsumeContext<T>> where T : class
{
    private readonly ILogger<LoggingConsumeFilter<T>> _logger = logger;
    private readonly string _instanceId = string.Concat(Environment.MachineName, "-", Guid.NewGuid().ToString("N").AsSpan(0, 4));

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var messageType = typeof(T).Name;
        var correlationId = context.CorrelationId ?? Guid.Empty;

        // Try to extract the WorkflowId property if it exists
        Guid workflowId = Guid.Empty;
        var workflowIdProperty = typeof(T).GetProperty("WorkflowId");
        if (workflowIdProperty != null)
        {
            var value = workflowIdProperty.GetValue(context.Message);
            if (value != null)
            {
                workflowId = (Guid)value;
            }
        }

        _logger.LogInformation("[{InstanceId}] Receiving {MessageType} with WorkflowId: {WorkflowId}, CorrelationId: {CorrelationId}",
            _instanceId, messageType, workflowId, correlationId);

        try
        {
            // Continue with the pipeline
            await next.Send(context);

            _logger.LogInformation("[{InstanceId}] Completed {MessageType} with WorkflowId: {WorkflowId}",
                _instanceId, messageType, workflowId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{InstanceId}] Failed processing {MessageType} with CorrelationId: {CorrelationId}",
                _instanceId, messageType, correlationId);

            // Re-throw to ensure the exception is handled by MassTransit
            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("logging-consume-filter");
    }
}