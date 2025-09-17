namespace Worker;

/// <summary>
/// Interface for storing and retrieving step results securely outside of the message bus
/// </summary>
public interface IResultStore
{
    /// <summary>
    /// Stores a result for a specific workflow step
    /// </summary>
    /// <param name="correlationId">The workflow correlation ID</param>
    /// <param name="stepName">The name of the step</param>
    /// <param name="result">The result value</param>
    /// <returns>A task representing the async operation</returns>
    Task StoreStepResult(Guid correlationId, string stepName, int result);

    /// <summary>
    /// Retrieves a result for a specific workflow step
    /// </summary>
    /// <param name="correlationId">The workflow correlation ID</param>
    /// <param name="stepName">The name of the step</param>
    /// <returns>The result value, or null if not found</returns>
    Task<int?> GetStepResult(Guid correlationId, string stepName);
}
