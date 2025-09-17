# Frontend Integration Guide

When merging the webapp branch, the Frontend project will need to be updated to use the shared `IResultStore` interface from the Contracts project.

## Current Frontend Implementation (webapp branch)

The Frontend project currently has its own local `IResultStore` interface:

```csharp
// Frontend/Services/RedisResultStore.cs (current webapp branch)
public interface IResultStore
{
    Task<int?> GetStepResult(Guid correlationId, string stepName);
    // Missing: StoreStepResult method
}
```

## Required Changes

1. **Remove the local interface definition** from `Frontend/Services/RedisResultStore.cs`
2. **Add Contracts project reference** to Frontend.csproj (if not already present)
3. **Update the using statements** to reference the shared interface
4. **Implement the missing StoreStepResult method** in Frontend's RedisResultStore

## Updated Frontend Implementation

```csharp
// Frontend/Services/RedisResultStore.cs (updated)
using StackExchange.Redis;
using Microsoft.Extensions.Logging;
using Contracts; // Reference shared interface

namespace Frontend.Services;

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

    // Existing method - keep implementation
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

    // NEW METHOD: Implement the missing StoreStepResult method
    public async Task StoreStepResult(Guid correlationId, string stepName, int result)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = $"{_keyPrefix}{correlationId}:step:{stepName}";
            
            _logger.LogDebug("Storing result {Result} for workflow {CorrelationId} step {StepName}", 
                result, correlationId, stepName);
            
            await db.StringSetAsync(key, result.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing result for workflow {CorrelationId} step {StepName}", 
                correlationId, stepName);
            throw; // Re-throw to maintain error handling contract
        }
    }
}
```

## Frontend.csproj Updates

Ensure the Frontend project references the Contracts project:

```xml
<ItemGroup>
  <ProjectReference Include="..\Contracts\Contracts.csproj" />
  <!-- other references -->
</ItemGroup>
```

This change ensures both Frontend and Worker projects use the same interface contract, eliminating the API inconsistency described in issue #3.