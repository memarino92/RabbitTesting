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

    public Step1CompletedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<Step1CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        _workflowTracker.UpdateWorkflowStatus(workflowId, status => 
        {
            status.IsStep1Completed = true;
        });
        
        return Task.CompletedTask;
    }
}

public class Step2CompletedConsumer : IConsumer<Step2CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;

    public Step2CompletedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<Step2CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        _workflowTracker.UpdateWorkflowStatus(workflowId, status => 
        {
            status.IsStep2Completed = true;
        });
        
        return Task.CompletedTask;
    }
}

public class Step3CompletedConsumer : IConsumer<Step3CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;

    public Step3CompletedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<Step3CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        _workflowTracker.UpdateWorkflowStatus(workflowId, status => 
        {
            status.IsStep3Completed = true;
        });
        
        return Task.CompletedTask;
    }
}

public class Step4CompletedConsumer : IConsumer<Step4CompletedEvent>
{
    private readonly WorkflowTracker _workflowTracker;

    public Step4CompletedConsumer(WorkflowTracker workflowTracker)
    {
        _workflowTracker = workflowTracker;
    }

    public Task Consume(ConsumeContext<Step4CompletedEvent> context)
    {
        var workflowId = context.Message.WorkflowId;
        
        _workflowTracker.UpdateWorkflowStatus(workflowId, status => 
        {
            status.IsStep4Completed = true;
        });
        
        return Task.CompletedTask;
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
