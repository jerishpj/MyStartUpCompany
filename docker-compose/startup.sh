#!/bin/bash
# Quick Start Script for Local OTEL Testing Stack

set -e

echo "=========================================="
echo "MyStartUpCompany OTEL Local Stack Startup"
echo "=========================================="
echo ""

# Check if docker is running
echo "Checking Docker installation..."
if ! command -v docker &> /dev/null; then
	echo "❌ Docker not found. Please install Docker Desktop."
	exit 1
fi

echo "✅ Docker found"
echo ""

# Check if docker daemon is running
echo "Checking Docker daemon..."
if ! docker ps &> /dev/null; then
	echo "❌ Docker daemon not running. Please start Docker Desktop."
	exit 1
fi

echo "✅ Docker daemon is running"
echo ""

# Navigate to docker-compose directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo "Starting Docker Compose stack in: $SCRIPT_DIR"
echo ""

# Start containers
echo "Starting containers..."
docker-compose up -d

echo ""
echo "Waiting for services to be healthy..."
sleep 10

# Check service status
echo ""
echo "=========================================="
echo "Service Status:"
echo "=========================================="

for service in jaeger prometheus grafana; do
	if docker-compose ps | grep -q "$service"; then
		echo "✅ $service is running"
	else
		echo "❌ $service failed to start"
	fi
done

echo ""
echo "=========================================="
echo "Local OTEL Stack Ready!"
echo "=========================================="
echo ""
echo "📊 Access URLs:"
echo "  • Jaeger UI (Traces):   http://localhost:16686"
echo "  • Prometheus (Metrics): http://localhost:9090"
echo "  • Grafana (Dashboards): http://localhost:3000 (admin/admin)"
echo ""
echo "📝 Next Steps:"
echo "  1. Run your .NET applications"
echo "  2. Make HTTP requests to generate traffic"
echo "  3. Check Jaeger for distributed traces"
echo "  4. Check Prometheus for metrics"
echo "  5. Create Grafana dashboards"
echo ""
echo "📚 Logs:"
echo "  docker-compose logs -f jaeger      # Follow Jaeger logs"
echo "  docker-compose logs -f prometheus  # Follow Prometheus logs"
echo "  docker-compose logs -f grafana     # Follow Grafana logs"
echo ""
echo "🛑 To Stop:"
echo "  docker-compose down"
echo ""
