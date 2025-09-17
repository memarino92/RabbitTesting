using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Frontend.Services;

public interface IResultStore
{
    Task<int?> GetStepResult(Guid correlationId, string stepName);
}

public class RedisResultStore : IResultStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisResultStore> _logger;
    private readonly string _keyPrefix = "workflow-result:";

    public RedisResultStore(IConnectionMultiplexer redis, ILogger<RedisResultStore> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<int?> GetStepResult(Guid correlationId, string stepName)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}{correlationId}:step:{stepName}";
            
            var value = await db.StringGetAsync(key);
            
            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("No result found for workflow {CorrelationId} step {StepName}", 
                    correlationId, stepName);
                return null;
            }
            
            if (int.TryParse(value.ToString(), out var result))
            {
                _logger.LogDebug("Retrieved result {Result} for workflow {CorrelationId} step {StepName}", 
                    result, correlationId, stepName);
                return result;
            }
            
            _logger.LogWarning("Could not parse result for workflow {CorrelationId} step {StepName}", 
                correlationId, stepName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving result for workflow {CorrelationId} step {StepName}", 
                correlationId, stepName);
            return null;
        }
    }
}
