### Start-Services.ps1
### This script ensures RabbitMQ and Redis are running before starting the application

# Script configuration
$redisContainerName = "rabbit-redis"
$redisPort = 6379
$rabbitContainerName = "rabbit-mq"
$rabbitPort = 5672
$rabbitManagementPort = 15672

Write-Host "Starting required services for RabbitTesting application..." -ForegroundColor Cyan

# Check if Docker is available
try {
    $dockerVersion = docker --version
    Write-Host "Docker detected: $dockerVersion" -ForegroundColor Green
}
catch {
    Write-Host "ERROR: Docker is not installed or not available in PATH. Please install Docker to continue." -ForegroundColor Red
    exit 1
}

# Function to check if a container exists and is running
function Test-ContainerRunning {
    param (
        [string]$containerName
    )
    
    $container = docker ps -a --filter "name=^/$containerName$" --format "{{.Status}}"
    
    if ($container -match "Up") {
        return $true
    }
    return $false
}

# Function to check if a container exists (running or not)
function Test-ContainerExists {
    param (
        [string]$containerName
    )
    
    $container = docker ps -a --filter "name=^/$containerName$" --format "{{.Names}}"
    
    if ($container -eq $containerName) {
        return $true
    }
    return $false
}

# 1. Check and start Redis
Write-Host "Checking Redis..." -ForegroundColor Yellow
if (Test-ContainerRunning -containerName $redisContainerName) {
    Write-Host "Redis is already running." -ForegroundColor Green
} 
elseif (Test-ContainerExists -containerName $redisContainerName) {
    Write-Host "Starting existing Redis container..." -ForegroundColor Yellow
    docker start $redisContainerName
    Write-Host "Redis started successfully." -ForegroundColor Green
} 
else {
    Write-Host "Creating and starting Redis container..." -ForegroundColor Yellow
    docker run --name $redisContainerName -p ${redisPort}:6379 -d redis redis-server --appendonly yes
    Write-Host "Redis started successfully." -ForegroundColor Green
}

# 2. Check and start RabbitMQ
Write-Host "Checking RabbitMQ..." -ForegroundColor Yellow
if (Test-ContainerRunning -containerName $rabbitContainerName) {
    Write-Host "RabbitMQ is already running." -ForegroundColor Green
} 
elseif (Test-ContainerExists -containerName $rabbitContainerName) {
    Write-Host "Starting existing RabbitMQ container..." -ForegroundColor Yellow
    docker start $rabbitContainerName
    Write-Host "RabbitMQ started successfully." -ForegroundColor Green
} 
else {
    Write-Host "Creating and starting RabbitMQ container..." -ForegroundColor Yellow
    docker run --name $rabbitContainerName -p ${rabbitPort}:5672 -p ${rabbitManagementPort}:15672 -d rabbitmq:3-management
    Write-Host "RabbitMQ started successfully." -ForegroundColor Green
}

# 3. Verify services are responding
Write-Host "Verifying service health..." -ForegroundColor Yellow

# Wait for RabbitMQ to be fully initialized (can take a few seconds)
$rabbitReady = $false
$attempts = 0
$maxAttempts = 30

while (-not $rabbitReady -and $attempts -lt $maxAttempts) {
    $attempts++
    Write-Host "Waiting for RabbitMQ to initialize (attempt $attempts/$maxAttempts)..." -ForegroundColor Yellow
    
    try {
        $rabbitStatus = docker exec $rabbitContainerName rabbitmqctl status
        if ($LASTEXITCODE -eq 0) {
            $rabbitReady = $true
            Write-Host "RabbitMQ is ready!" -ForegroundColor Green
        }
    }
    catch {
        # Continue waiting
    }
    
    if (-not $rabbitReady) {
        Start-Sleep -Seconds 2
    }
}

if (-not $rabbitReady) {
    Write-Host "WARNING: RabbitMQ may not be fully initialized yet." -ForegroundColor Yellow
}

# All services should be running now
Write-Host "`nAll required services are now running:" -ForegroundColor Cyan
Write-Host "- Redis: localhost:$redisPort" -ForegroundColor Cyan
Write-Host "- RabbitMQ: localhost:$rabbitPort" -ForegroundColor Cyan
Write-Host "- RabbitMQ Management UI: http://localhost:$rabbitManagementPort" -ForegroundColor Cyan

Write-Host "`nYou can now start your application." -ForegroundColor Green
Write-Host "To run two workers for testing, open two terminals and execute:" -ForegroundColor Green
Write-Host "Terminal 1: cd Worker && dotnet run" -ForegroundColor DarkGray
Write-Host "Terminal 2: cd Worker && dotnet run" -ForegroundColor DarkGray
Write-Host "`nThen open a third terminal and execute:" -ForegroundColor Green
Write-Host "cd NewTask && dotnet run" -ForegroundColor DarkGray
