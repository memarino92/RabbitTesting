using Contracts;
using MassTransit;

namespace Frontend.Services;

public class WorkflowStartedConsumer : IConsumer<WorkflowStartedEvent>
{
    private readonly WorkflowTracker _workflowTracker;

    public WorkflowStartedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<WorkflowStartedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsStarted = true;
        });

        return Task.CompletedTask;
    }
}

public class Step1CompletedConsumer : IConsumer<Step1CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;
    private readonly IResultStore _resultStore;

    public Step1CompletedConsumer(WorkflowTracker workflowTracker, IResultStore resultStore)
    {
        _workflowTracker = workflowTracker;
        _resultStore = resultStore;
    }

    public async Task Consume(ConsumeContext<Step1CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        // Retrieve the result from Redis
        var result = await _resultStore.GetStepResult(workflowId, "Step1");

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsStep1Completed = true;
            status.Step1Result = result;
        });
    }
}

public class Step2CompletedConsumer : IConsumer<Step2CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;
    private readonly IResultStore _resultStore;

    public Step2CompletedConsumer(WorkflowTracker workflowTracker, IResultStore resultStore)
    {
        _workflowTracker = workflowTracker;
        _resultStore = resultStore;
    }

    public async Task Consume(ConsumeContext<Step2CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        // Retrieve the result from Redis
        var result = await _resultStore.GetStepResult(workflowId, "Step2");

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsStep2Completed = true;
            status.Step2Result = result;
        });
    }
}

public class Step3CompletedConsumer : IConsumer<Step3CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;
    private readonly IResultStore _resultStore;

    public Step3CompletedConsumer(WorkflowTracker workflowTracker, IResultStore resultStore)
    {
        _workflowTracker = workflowTracker;
        _resultStore = resultStore;
    }

    public async Task Consume(ConsumeContext<Step3CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        // Retrieve the result from Redis
        var result = await _resultStore.GetStepResult(workflowId, "Step3");

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsStep3Completed = true;
            status.Step3Result = result;
        });
    }
}

public class Step4CompletedConsumer : IConsumer<Step4CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;
    private readonly IResultStore _resultStore;

    public Step4CompletedConsumer(WorkflowTracker workflowTracker, IResultStore resultStore)
    {
        _workflowTracker = workflowTracker;
        _resultStore = resultStore;
    }

    public async Task Consume(ConsumeContext<Step4CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        // Retrieve the result from Redis
        var result = await _resultStore.GetStepResult(workflowId, "Step4");

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsStep4Completed = true;
            status.Step4Result = result;
        });
    }
}

public class WorkflowCompletedConsumer : IConsumer<WorkflowCompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;

    public WorkflowCompletedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<WorkflowCompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;

        _workflowTracker.UpdateWorkflowStatus(workflowId, status =>
        {
            status.IsCompleted = true;
            status.CompletionTime = DateTime.UtcNow;
        });

        return Task.CompletedTask;
    }
}
