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

In this demo, we are using *Jaeger* Tracing though docker container
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
    await using var jaeger = new ContainerBuilder("jaegertracing/all-in-one:latest")
    .WithPortBinding(16686, 16686)
    .WithPortBinding(4317, 4317)
    .WithEnvironment("COLLECTOR_OTLP_ENABLED", "true")
    .WithWaitStrategy(Wait.ForUnixContainer())
    .Build();

    await jaeger.StartAsync();
```

See [Microsoft's documentation on Observability for Agents](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/enable-observability?pivots=programming-language-csharp#enable-opentelemetry-in-your-app) and [Microsoft Reactor's youtube video](https://www.youtube.com/watch?v=TvgUim_3vrU&t=2596s)

## Persisting and Resuming Agent Conversations

Demo uses DotNet.Testcontainers to spin up a Postgres container. 

The Postgres container is then used to store the serialized thread state.



### Simple approach using thread state (in memory)

Steps:
- Create an agent and obtain a new thread that will hold the conversation state.
  - `AIAgent agent = new AzureOpenAIClient(...);`
  - `AIAgentThread thread = agent.GetNewThread();`
- Run the agent, passing in the thread, so that the AgentThread includes this exchange.
  - `await agent.RunAsync("Tell me a joke.", thread)`
- Save the serialized thread state to a database or file.
  - `string serializedJson = thread.Serialize(JsonSerializerOptions.Web).GetRawText();`
  - `await _storage.SaveAsync(serializedJson);`
- When the agent is invoked again, pass the thread to the agent.
  - Load the persisted JSON from storage 
    - `string loadedJson = await _storage.LoadAsync();` 
    - `JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(loadedJson, JsonSerializerOptions.Web);`
  - Recreate the AgentThread instance from it. The thread must be deserialized using an agent instance (of the same type e.g. AzureOpenAIClient).
    - `AgentThread resumedThread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);`
- Resume the conversation by passing the thread to the agent.
  - `await agent.RunAsync("now tell that same joke but in french", resumedThread);`
  
See [Microsoft's documentation on Persisting and Resuming Agent Conversations](https://learn.microsoft.com/en-us/agent-framework/tutorials/agents/persisted-conversation?pivots=programming-language-csharp#persisting-and-resuming-the-conversation)

### Using a Message Store

package for using *ChatMessage Store* : 
`dotnet add package Microsoft.SemanticKernel.Connectors.InMemory --prerelease`

package for using Postgres vector store : `dotnet add package Microsoft.SemanticKernel.Connectors.PgVector --prerelease`

#### Message storage and retrieval methods
The most important methods to implement are:

`AddMessagesAsync` - called to add new messages to the store.

`GetMessagesAsync` - called to retrieve the messages from the store.
- Any chat history reduction logic, such as summarization or trimming, should be done before returning messages from GetMessagesAsync.

#### Chat History Reduction

Strategies
- Truncation
- Summarization
- Token-Based

#### Storage Best Practice
Use a combination approach:
- Always save full history to the database (for audit/legal)
- Generate summaries for conversations longer than 20 messages
- Create embeddings of summaries for semantic search
- Load optimized context when resuming (summary + recent messages)
- Implement retention policies (archive old conversations)
- Tiered Storage
  - Hot storage: Recent full conversations (last 24 hours)
  - Warm storage: Summaries (last 30 days)
  - Cold storage: Archives (older than 30 days)
- Context Window Management

#### Recommended Database Schema
```sql
-- Full conversation history
CREATE TABLE conversation_history (
    id UUID PRIMARY KEY,
    conversation_id VARCHAR(255),
    created_at TIMESTAMP,
    full_json JSONB,
    message_count INT
);

-- Summaries for efficient retrieval
CREATE TABLE conversation_summaries (
    id UUID PRIMARY KEY,
    conversation_id VARCHAR(255),
    summary TEXT,
    created_at TIMESTAMP,
    summary_embedding VECTOR(1536) -- For pgvector
);

-- Individual messages (for granular search)
CREATE TABLE messages (
    id UUID PRIMARY KEY,
    conversation_id VARCHAR(255),
    role VARCHAR(50),
    content TEXT,
    timestamp TIMESTAMP,
    embedding VECTOR(1536)
);
```

## Create a Simple Sequential Workflow

Sequential workflows are the foundation of building complex AI agent systems.

packages `dotnet add package Microsoft.Agents.AI.Workflows --prerelease`

- Creating a custom executor with one handler
- Creating a custom executor from a function
- Using WorkflowBuilder to connect executors with edges
- Processing data through sequential steps
- Observing workflow execution through events


See [Create a Simple Sequential Workflow](https://learn.microsoft.com/en-us/agent-framework/tutorials/workflows/simple-sequential-workflow?pivots=programming-language-csharp#overview)

## Create RAG tooled agent

Retrieval Augmented Generation, RAG for short, can be achieved in MS Agent Framework using
vector embedding. 

packages `Microsoft.SemanticKernel.Connectors.PgVector` or `Microsoft.SemanticKernel.Connectors.InMemory`


## Agent as Function Tool

Conceptually flattening an agent into a tool/function. 

The tool agent acts more like a stateless capability, like a Function, Plugin or Helper module.

Key characteristics : 
- Invoked like a function call
- Synchronous eexecution
- Stateless (typically)
- Executed on the same process / thread
- Used for deterministic tasks with narrow expertise

`tools: [toolAgent.AsAIFunction()]`

## Agent to Agent

Unlike _Agent as Function Tool_, A2A exposes autonomous agent over the Agent-to-Agent protocol. 

A2A is a standardized protocol that supports:

- Agent discovery through agent cards
- Message-based communication between agents
- Long-running agentic processes via tasks
- Cross-platform interoperability between different agent frameworks

Key characteristics:
- The agent is network addressable
- It has its own lifecycle
- It can reason, plan 
- It can be stateful
- Talked over by A2A messages, not function calls

packages 
- `Microsoft.Agents.AI.Hosting.A2A.AspNetCore` for the 'host' side (the one being called)
- `Microsoft.Agents.AI.Hosting.A2A` for the client side (the one making the external call)

To communicate with agent over http, the POST body needs to comply with the A2A specification.
- `messageId`
  - A unique identifier for this specific message. You can create your own ID (e.g., a GUID) 
  - or set it to null to let the agent generate one automatically.
- `contextId`
  - The conversation identifier. Provide your own ID to start a new conversation or continue an existing one by reusing a previous contextId. 
  - The agent will maintain conversation history for the same contextId. Agent will generate one for you as well, if none is provided.

The response includes the contextId (conversation identifier), messageId (message identifier), and the actual content from the  agent.
  
see example in `a2aclient.http` file

see [A2A documentation](https://learn.microsoft.com/en-us/agent-framework/user-guide/agents/agent-types/a2a-agent?pivots=programming-language-csharp#getting-started)

#### A2A Protocol "Message"

In my code example, `/a2a/pizza/v1/message:send` and `/a2a/pizza/v1/message:stream`

**What A2A actually defines**

- `message`
- `parts`
- `roles`
- `contextId`
- streaming
- tool calls
- metadata

That’s it.

So this:
```json
{
  "message": {
    "kind": "message",
    "role": "user",
    "parts": [
      {
        "kind": "text",
        "text": "Order me a pizza"
      }
    ]
  }
}
```


✅ Is the protocol

There is nothing lower-level than this.

#### A2A Protocol "Task"

In A2A, a task is a convention. A message is the protocol.
“A message (or small set of messages) that represents a single unit of delegated work.”

Semantically: “Do this. Return a result. Stop.”

Protocol-wise: “Here is a message.”

#### AgentCard Configuration

The AgentCard provides metadata about your agent for discovery and integration:

```csharp
app.MapA2A(agent, "/a2a/pizza", agentCard: new()
{
    Name = "My Pizza Agent",
    Description = "A helpful agent that assists with tasks.",
    Version = "1.0",
});
```
You can access the agent card by sending this request:
```http request
  # Send A2A request to the pirate agent
  GET {{baseAddress}}/a2a/pizza/v1/card
```
AgentCard Properties

- Name: Display name of the agent
- Description: Brief description of the agent
- Version: Version string for the agent
- Url: Endpoint URL (automatically assigned if not specified)
- Capabilities: Optional metadata about streaming, push notifications, and other features

#### Exposing Multiple Agents

You can expose multiple agents in a single application, as long as their endpoints don't collide. 

```csharp
var mathAgent = builder.AddAIAgent("math", instructions: "You are a math expert.");
var scienceAgent = builder.AddAIAgent("science", instructions: "You are a science expert.");

app.MapA2A(mathAgent, "/a2a/math");
app.MapA2A(scienceAgent, "/a2a/science");
```

see [Microsoft's A2A documentation](https://learn.microsoft.com/en-us/agent-framework/user-guide/hosting/agent-to-agent-integration?tabs=dotnet-cli%2Cuser-secrets#what-is-a2a)

## A2A vs Agent as Tool

A2A for:
- Planner / Manager agents
- External collaborators

Tools for:
- Specialists
- Validators
- IO-bound operations


| Question                                         | A2A | Tool |
| ------------------------------------------------ | --- | ---- |
| Can it refuse or negotiate?                      | ✅   | ❌    |
| Can it decide *how* to solve the task?           | ✅   | ❌    |
| Can it be replaced by another system?            | ✅   | ❌    |
| Can it run independently?                        | ✅   | ❌    |
| Is it part of the main agent’s chain-of-thought? | ❌   | ✅    |



## Best Practices

- Rate limiting
- Guard Rails