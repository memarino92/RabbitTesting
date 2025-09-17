using StackExchange.Redis;
using Microsoft.Extensions.Logging;

namespace Worker;

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

    public async Task StoreStepResult(Guid correlationId, string stepName, int result)
    {
        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}{correlationId}:step:{stepName}";
        
        _logger.LogInformation("Storing result {Result} for workflow {CorrelationId} step {StepName}", 
            result, correlationId, stepName);
        
        await db.StringSetAsync(key, result.ToString());
    }

    public async Task<int?> GetStepResult(Guid correlationId, string stepName)
    {
        var db = _redis.GetDatabase();
        var key = $"{_keyPrefix}{correlationId}:step:{stepName}";
        
        var value = await db.StringGetAsync(key);
        
        if (value.IsNullOrEmpty)
        {
            _logger.LogWarning("No result found for workflow {CorrelationId} step {StepName}", 
                correlationId, stepName);
            return null;
        }
        
        if (int.TryParse(value.ToString(), out var result))
        {
            return result;
        }
        
        _logger.LogError("Could not parse result for workflow {CorrelationId} step {StepName}", 
            correlationId, stepName);
        return null;
    }
}
