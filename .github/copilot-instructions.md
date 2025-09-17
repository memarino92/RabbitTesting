# Copilot Instructions

This is a simple proof of concept for a message-driven workflow application using MassTransit, RabbitMQ, and Redis for state persistence.

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

## Working Effectively

### Environment Setup
- Install .NET 10 RC (required for this project):
  ```bash
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --channel 10.0
  export PATH="$HOME/.dotnet:$PATH"
  ```
- Verify .NET 10 installation: `dotnet --version` (should show 10.0.100-rc.1.25451.107 or later)
- Ensure Docker is available: `docker --version`

### Build and Test
- Restore packages: `dotnet restore` -- takes ~8 seconds. NEVER CANCEL.
- Build solution: `dotnet build` -- takes ~8 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- Check code formatting: `dotnet format --verify-no-changes` -- takes ~3 seconds.
- Format code if needed: `dotnet format`
- NO UNIT TESTS: This project has no test projects. Do not attempt to run `dotnet test`.

### Running the Application

#### Option 1: Automated Startup (Windows PowerShell only)
- Use the provided PowerShell scripts on Windows:
  ```powershell
  ./start-application.ps1    # Starts all services and applications
  ./start-services.ps1       # Starts only Docker services
  ```

#### Option 2: Manual Startup (Cross-platform)
1. **Start required Docker services:**
   ```bash
   # Start Redis for saga state persistence
   docker run --name rabbit-redis -p 6379:6379 -d redis
   
   # Start RabbitMQ with management UI
   docker run --name rabbit-mq -p 5672:5672 -p 15672:15672 -d rabbitmq:3-management
   ```

2. **Wait for RabbitMQ to initialize (CRITICAL - takes ~15-30 seconds):**
   ```bash
   # Wait for RabbitMQ to be ready
   sleep 15
   docker exec rabbit-mq rabbitmqctl status
   ```
   NEVER CANCEL - RabbitMQ initialization takes 15-30 seconds. Set timeout to 60+ seconds.

3. **Start Worker (keep running in background):**
   ```bash
   export PATH="$HOME/.dotnet:$PATH"
   dotnet run --project Worker/Worker.csproj
   ```
   The Worker will show configuration logs and then "Bus started: rabbitmq://localhost/"

4. **Test workflow execution:**
   ```bash
   export PATH="$HOME/.dotnet:$PATH"
   echo -e "\n" | dotnet run --project NewTask/NewTask.csproj
   ```

### Validation Scenarios

**ALWAYS test the complete workflow after making changes:**
1. Start Redis and RabbitMQ services
2. Start at least one Worker instance
3. Run NewTask to trigger a workflow
4. Verify in Worker logs that all 4 steps execute successfully:
   - StartWorkflowCommand received
   - WorkflowStartedEvent processed
   - Step1Command → Step1CompletedEvent
   - Step2Command → Step2CompletedEvent  
   - Step3Command → Step3CompletedEvent
   - Step4Command → Step4CompletedEvent
   - "workflow complete!" message appears

**For multi-worker testing:** Start multiple Worker instances in separate terminals to test competing consumer behavior.

### Service URLs and Management
- RabbitMQ: localhost:5672 (AMQP)
- RabbitMQ Management UI: http://localhost:15672 (guest/guest)
- Redis: localhost:6379

### Cleanup
```bash
docker stop rabbit-redis rabbit-mq
docker rm rabbit-redis rabbit-mq
```

## Project Guidelines

- **Use Latest Features:** Always use the latest C# syntax and language features.
- **.NET Version:** This project uses .NET 10 RC. All projects target `net10.0` for compatibility.
- **Use dotnet CLI:** Always prefer using the `dotnet` CLI for managing projects and dependencies (e.g., `dotnet add reference`, `dotnet add package`). Do not manually edit `.csproj` or `.sln` files for these tasks.
- **File-scoped Namespaces:** Always use file-scoped namespace declarations (`namespace X;`) instead of block-scoped namespaces (`namespace X { ... }`).
- **Keep Documentation Updated:** When making changes to project structure, dependencies, or workflow, update the `README.md` and these instructions.

## Project Structure

- **Contracts:** Message contracts (commands and events) and workflow state (`WorkflowState`, `WorkflowStateMachine`)
- **Saga:** State machine that orchestrates the workflow (`WorkflowStateMachine`)  
- **Worker:** Hosts the saga and activity consumers (`StartWorkflowConsumer`, `DirectStep1Consumer`, etc.)
- **NewTask:** Client application that initiates workflows

## Common Issues and Troubleshooting

### Build Issues
- **".NET SDK does not support targeting .NET 10.0"**: Install .NET 10 RC using the curl command above
- **Package restore fails**: Ensure .NET 10 RC is in PATH and try `dotnet restore` again

### Runtime Issues  
- **Events processed out of order or duplicated**: Ensure Redis is running for saga state persistence
- **Worker fails to connect**: Verify RabbitMQ container is running and ready (use `docker exec rabbit-mq rabbitmqctl status`)
- **Multiple saga instances**: Only one saga instance should be defined per Worker application

### Docker Issues
- **Container name conflicts**: Remove existing containers with `docker rm rabbit-redis rabbit-mq`
- **Port conflicts**: Ensure ports 5672, 15672, and 6379 are available

## Scope of Work

- **Follow Instructions Precisely:** Do not perform any tasks that are not explicitly requested.
- **No Extra Initiative:** There is no reward for going beyond the defined scope of a request.
- **Clarify Ambiguity:** If a request is unclear, ask for clarification before proceeding.
- **Suggest, Don't Implement:** If additional changes are required for the project to function after your changes, suggest them but do not implement them unless explicitly instructed to do so.
