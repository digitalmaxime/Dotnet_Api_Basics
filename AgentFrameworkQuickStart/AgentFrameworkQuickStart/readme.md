# Microsoft Agent Framework

See [Microsoft Agent Framework](https://learn.microsoft.com/en-us/agent-framework/overview/agent-framework-overview)

The ChatClientAgent is built on top of any IChatClient implementation. 

## Running the agent with a multi-turn conversation

Agents are stateless and do not maintain any state internally between calls. 
To have a multi-turn conversation with an agent, you need to create an object
using `GetNewThread` to hold the conversation state and pass this object to the agent when running it.

`AgentThread thread = agent.GetNewThread();`

## Using function tools with an agent

You can turn any C# method into a function tool, by using the `AIFunctionFactory.Create`
method to create an `AIFunction` instance from the method.

## human in the loop approvals

When using functions, it's possible to indicate for each function whether it requires human approval before being executed.
This is done by wrapping the AIFunction instance in an ApprovalRequiredAIFunction instance.

```csharp
AIFunction weatherFunction = AIFunctionFactory.Create(GetWeather);
AIFunction approvalRequiredWeatherFunction = new ApprovalRequiredAIFunction(weatherFunction);
```

Whenever you are using function tools with human in the loop approvals, remember to check for 
FunctionApprovalRequestContent instances in the response, after each agent run, 
until all function calls have been approved or rejected.

## Structured Output with Agents

The ChatClientAgent uses the support for structured output that's provided by the underlying chat client.

When creating the agent, you can provide the default ChatOptions instance to use for the underlying chat client. 
This ChatOptions instance allows you to pick a preferred ChatResponseFormat.

- A built-in `ChatResponseFormat.Text property`: The response will be plain text.
- A built-in `ChatResponseFormat.Json property`: The response will be a JSON object without any particular schema.
- A custom   `ChatResponseFormatJson instance` The response will be a JSON object that conforms to a specific schema.

See [Create the agent with structured output](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/structured-output?pivots=programming-language-csharp#create-the-agent-with-structured-output)

## Using an agent as a function tool

You can use an `AIAgent` as a function tool by calling `.AsAIFunction()` 
on the agent and providing it as a tool to another agent. 
This allows you to compose agents and build more advanced workflows.

- Create a function tool as a C# method.
- Create an agent that uses the function tool.
- Create a main agent that uses the agent as a function tool.
  - `tools: [weatherAgent.AsAIFunction()]`

See [Create a workflow with multiple agents](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/agent-as-function-tool?pivots=programming-language-csharp#create-and-use-an-agent-as-a-function-tool)

## Expose an agent as an MCP tool

TBD

packages 
```
dotnet add package Microsoft.Extensions.Hosting --prerelease
dotnet add package ModelContextProtocol --prerelease
```

## Enabling observability for Agents

```
dotnet add package OpenTelemetry
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol
```

Enable Agent Framework telemetry and create an OpenTelemetry TracerProvider that exports to the console.
The TracerProvider must remain alive while you run the agent so traces are exported.

using Jaeger Tracing though docker container

Use Jaeger either by spinning up the docker compose file below or by running it through [Testcontainers for .NET](https://dotnet.testcontainers.org/)
```yaml
# docker-compose.yml
networks:
  network:
    name: network
volumes:
  jaeger:
    driver: local
services:
  jaeger:
    image: jaegertracing/all-in-one:latest
    networks:
      - network
    volumes:
      - jaeger:/tmp
    container_name: telemetry-jaeger
    ports:
      - "16686:16686" // UI
      - "4317:4317" // OTLP gRPC
    environment:
      COLLECTOR_OTLP_ENABLED: true
```

```csharp
// programs.cs
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;

// creates a new temporary test container scoped to the current test method
    var jaeger = new ContainerBuilder("jaegertracing/all-in-one:latest")
        .WithPortBinding(16686, 16686) // UI will be available on port 16686
        .WithPortBinding(4317, 4317) // OTLP gRPC
        .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true")
        .WithWaitStrategy(Wait.ForUnixContainer())
        .Build();
```

See [Microsoft's documentation on Observability for Agents](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/enable-observability?pivots=programming-language-csharp#enable-opentelemetry-in-your-app) and [Microsoft Reactor's youtube video](https://www.youtube.com/watch?v=TvgUim_3vrU&t=2596s)