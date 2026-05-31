@echo off
REM Script to start the observability stack locally (Windows)
REM Usage: start-observability-stack.bat

setlocal enabledelayedexpansion

echo.
echo 🚀 Starting MyStartUpCompany Observability Stack...
echo.

REM Check if docker is available
where docker >nul 2>nul
if !errorlevel! neq 0 (
	echo ❌ Docker is not installed. Please install Docker Desktop first.
	exit /b 1
)

REM Check if docker-compose is available
where docker-compose >nul 2>nul
if !errorlevel! neq 0 (
	echo ❌ docker-compose is not installed. Please install Docker Compose first.
	exit /b 1
)

echo 📦 Building services...
docker-compose -f docker-compose.observability.yml build

echo.
echo 🎯 Starting services...
docker-compose -f docker-compose.observability.yml up -d

echo.
echo ⏳ Waiting for services to be healthy...
timeout /t 10 /nobreak

echo.
echo 🔍 Service Status:
echo ================================
docker-compose -f docker-compose.observability.yml ps

echo.
echo ✅ Observability Stack Started!
echo.
echo 📊 Access Points:
echo ================================
echo 🔍 Jaeger UI (Distributed Tracing):
echo    → http://localhost:16686
echo.
echo 📈 Prometheus (Metrics):
echo    → http://localhost:9090
echo.
echo 📉 Grafana (Dashboards):
echo    → http://localhost:3000 (admin/admin)
echo.
echo 🌐 API Service:
echo    → http://localhost:8080/swagger/index.html
echo.
echo 🔔 Notifier Service:
echo    → http://localhost:8081/swagger/index.html
echo.
echo 📡 OpenTelemetry Collector:
echo    → gRPC: localhost:4317
echo    → HTTP: localhost:4318
echo.
echo 💾 SQL Server:
echo    → localhost:1433
echo    → User: sa
echo    → Password: P@ssw0rd123!
echo.
echo ================================
echo To stop all services, run:
echo   docker-compose -f docker-compose.observability.yml down
echo.
echo To view logs:
echo   docker-compose -f docker-compose.observability.yml logs -f
echo.
echo To view specific service logs:
echo   docker-compose -f docker-compose.observability.yml logs -f [service-name]
echo.

pause
