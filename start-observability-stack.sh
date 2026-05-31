#!/bin/bash

# Script to start the observability stack locally
# Usage: ./start-observability-stack.sh

set -e

echo "🚀 Starting MyStartUpCompany Observability Stack..."
echo ""

# Check if docker and docker-compose are available
if ! command -v docker &> /dev/null; then
	echo "❌ Docker is not installed. Please install Docker first."
	exit 1
fi

if ! command -v docker-compose &> /dev/null; then
	echo "❌ docker-compose is not installed. Please install Docker Compose first."
	exit 1
fi

echo "📦 Building services..."
docker-compose -f docker-compose.observability.yml build

echo ""
echo "🎯 Starting services..."
docker-compose -f docker-compose.observability.yml up -d

echo ""
echo "⏳ Waiting for services to be healthy..."
sleep 10

# Check service health
echo ""
echo "🔍 Service Status:"
echo "================================"
docker-compose -f docker-compose.observability.yml ps

echo ""
echo "✅ Observability Stack Started!"
echo ""
echo "📊 Access Points:"
echo "================================"
echo "🔍 Jaeger UI (Distributed Tracing):"
echo "   → http://localhost:16686"
echo ""
echo "📈 Prometheus (Metrics):"
echo "   → http://localhost:9090"
echo ""
echo "📉 Grafana (Dashboards):"
echo "   → http://localhost:3000 (admin/admin)"
echo ""
echo "🌐 API Service:"
echo "   → http://localhost:8080/swagger/index.html"
echo ""
echo "🔔 Notifier Service:"
echo "   → http://localhost:8081/swagger/index.html"
echo ""
echo "📡 OpenTelemetry Collector:"
echo "   → gRPC: localhost:4317"
echo "   → HTTP: localhost:4318"
echo ""
echo "💾 SQL Server:"
echo "   → localhost:1433"
echo "   → User: sa"
echo "   → Password: P@ssw0rd123!"
echo ""
echo "================================"
echo "To stop all services, run:"
echo "  docker-compose -f docker-compose.observability.yml down"
echo ""
echo "To view logs:"
echo "  docker-compose -f docker-compose.observability.yml logs -f"
echo ""
echo "To view specific service logs:"
echo "  docker-compose -f docker-compose.observability.yml logs -f <service-name>"
