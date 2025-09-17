using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Worker
{
    public class LoggingConsumeFilter<T> : IFilter<ConsumeContext<T>> where T : class
    {
        private readonly ILogger<LoggingConsumeFilter<T>> _logger;
        private readonly string _instanceId;

        public LoggingConsumeFilter(ILogger<LoggingConsumeFilter<T>> logger)
        {
            _logger = logger;
            _instanceId = Environment.MachineName + "-" + Guid.NewGuid().ToString("N").Substring(0, 4);
        }

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

            _logger.LogInformation("Worker {InstanceId} receiving {MessageType} with WorkflowId: {WorkflowId}, CorrelationId: {CorrelationId}",
                _instanceId, messageType, workflowId, correlationId);

            Console.WriteLine($"[{_instanceId}] Receiving {messageType} with WorkflowId: {workflowId}, CorrelationId: {correlationId}");

            try
            {
                // Continue with the pipeline
                await next.Send(context);

                _logger.LogInformation("Worker {InstanceId} completed {MessageType} with WorkflowId: {WorkflowId}",
                    _instanceId, messageType, workflowId);

                Console.WriteLine($"[{_instanceId}] Completed {messageType} with WorkflowId: {workflowId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker {InstanceId} failed processing {MessageType} with CorrelationId: {CorrelationId}",
                    _instanceId, messageType, correlationId);

                Console.WriteLine($"[{_instanceId}] ERROR processing {messageType}: {ex.Message}");

                // Re-throw to ensure the exception is handled by MassTransit
                throw;
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateFilterScope("logging-consume-filter");
        }
    }
}
