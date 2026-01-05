using Microsoft.Agents.AI.Workflows;

namespace AgentFrameworkQuickStart.features;

public static class SimpleSequentialWorkflow
{
    public async static Task Call()
    {
        var uppercase = new UppercaseExecutor();
        var reverse = new ReverseTextExecutor();
        // Build the workflow by connecting executors sequentially
        WorkflowBuilder builder = new(uppercase);
        builder.AddEdge(uppercase, reverse).WithOutputFrom(reverse);
        var workflow = builder.Build();
        
        // Execute the workflow with input data
        await using Run run = await InProcessExecution.RunAsync(workflow, "Hello, World!");
        foreach (WorkflowEvent evt in run.NewEvents)
        {
            switch (evt)
            {
                case ExecutorCompletedEvent executorComplete:
                    Console.WriteLine($"{executorComplete.ExecutorId}: {executorComplete.Data}");
                    break;
            }
        }
    }
}

internal sealed class ReverseTextExecutor() : Executor<string, string>("ReverseTextExecutor")
{
    public override ValueTask<string> HandleAsync(string input, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(new string(input.Reverse().ToArray()));
    }
}
internal sealed class UppercaseExecutor() : Executor<string, string>("UppercaseExecutor")
{
    public override ValueTask<string> HandleAsync(string input, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        return new ValueTask<string>(input.ToUpperInvariant());
    }
}