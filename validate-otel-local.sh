#!/bin/bash
# Local OTEL Validation Script
# Validates that the local OTEL observability stack is working correctly
# Run this after docker-compose is up and applications are running

set +e  # Don't exit on errors, we want to report all issues

VERBOSE=false
TIMEOUT_SECONDS=30

# Parse arguments
while [[ $# -gt 0 ]]; do
	case $1 in
		-v|--verbose)
			VERBOSE=true
			shift
			;;
		*)
			shift
			;;
	esac
done

ALL_PASSED=true

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

echo ""
echo -e "${CYAN}==========================================${NC}"
echo -e "${CYAN}Local OTEL Stack Validation${NC}"
echo -e "${CYAN}==========================================${NC}"
echo ""

# Function to test HTTP endpoint
test_endpoint() {
	local url=$1
	local service_name=$2
	local success_indicator=$3

	echo -e "${GRAY}Testing ${service_name} at ${url}...${NC}"

	response=$(curl -s -o /dev/null -w "%{http_code}" -m 5 "$url" 2>&1)

	if [ "$response" = "200" ]; then
		if [ -z "$success_indicator" ] || curl -s "$url" | grep -q "$success_indicator"; then
			echo -e "  ${GREEN}✅ Accessible and responding${NC}"
			return 0
		else
			echo -e "  ${RED}❌ Connected but unexpected content${NC}"
			return 1
		fi
	elif [ -z "$response" ]; then
		echo -e "  ${RED}❌ Not accessible${NC}"
		return 1
	else
		echo -e "  ${YELLOW}⚠️  Status: ${response}${NC}"
		return 1
	fi
}

# Function to test Docker container status
test_docker_container() {
	local container_name=$1

	echo -e "${GRAY}Checking Docker container: ${container_name}...${NC}"

	status=$(docker-compose ps "$container_name" 2>&1 | grep -E "(Up|Exited)")

	if echo "$status" | grep -q "Up"; then
		echo -e "  ${GREEN}✅ Container is running${NC}"
		return 0
	elif echo "$status" | grep -q "Exited"; then
		echo -e "  ${RED}❌ Container has exited${NC}"
		return 1
	else
		echo -e "  ${YELLOW}⚠️  Status unknown${NC}"
		return 1
	fi
}

echo -e "${CYAN}📊 STEP 1: Validate Docker Compose Services${NC}"
echo ""

for container in jaeger prometheus grafana; do
	if ! test_docker_container "$container"; then
		ALL_PASSED=false
	fi
done

echo ""
echo -e "${CYAN}🌐 STEP 2: Validate Service Endpoints${NC}"
echo ""

if ! test_endpoint "http://localhost:16686" "Jaeger UI" "traces"; then
	ALL_PASSED=false
fi

if ! test_endpoint "http://localhost:9090" "Prometheus" "prometheus"; then
	ALL_PASSED=false
fi

if ! test_endpoint "http://localhost:3000" "Grafana" "grafana"; then
	ALL_PASSED=false
fi

echo ""
echo -e "${CYAN}📈 STEP 3: Validate Application Metrics Endpoints${NC}"
echo ""

for port in 9091 9092 9093; do
	echo -e "${GRAY}Checking metrics at http://localhost:${port}/metrics...${NC}"

	response=$(curl -s -o /dev/null -w "%{http_code}" -m 5 "http://localhost:${port}/metrics" 2>&1)

	if [ "$response" = "200" ]; then
		content=$(curl -s "http://localhost:${port}/metrics" 2>/dev/null)
		if echo "$content" | grep -q "^# HELP"; then
			echo -e "  ${GREEN}✅ Metrics endpoint is active (found Prometheus format)${NC}"
		else
			echo -e "  ${GREEN}✅ Metrics endpoint responds${NC}"
		fi
	elif [ -z "$response" ]; then
		echo -e "  ${YELLOW}ℹ️  Endpoint not yet available (app may not be running)${NC}"
	else
		echo -e "  ${YELLOW}⚠️  Status: ${response}${NC}"
	fi
done

echo ""
echo -e "${CYAN}🔗 STEP 4: Validate Prometheus Scrape Targets${NC}"
echo ""

echo -e "${GRAY}Checking Prometheus targets...${NC}"

targets_response=$(curl -s "http://localhost:9090/api/v1/targets" 2>/dev/null)

if command -v jq &> /dev/null; then
	active_targets=$(echo "$targets_response" | jq '.data.activeTargets | length' 2>/dev/null)

	if [ -n "$active_targets" ] && [ "$active_targets" -gt 0 ]; then
		echo -e "  ${GREEN}✅ Found ${active_targets} active targets${NC}"
		echo "$targets_response" | jq -r '.data.activeTargets[] | "    • \(.labels.job) (\(.scrapeUrl))"' 2>/dev/null
	else
		echo -e "  ${YELLOW}⚠️  No active scrape targets found yet${NC}"
	fi
else
	if echo "$targets_response" | grep -q "activeTargets"; then
		echo -e "  ${GREEN}✅ Prometheus API is responding${NC}"
	else
		echo -e "  ${YELLOW}ℹ️  Cannot query Prometheus targets yet${NC}"
	fi
fi

echo ""
echo -e "${CYAN}📊 STEP 5: Network Connectivity Test${NC}"
echo ""

echo -e "${GRAY}Testing Docker network connectivity...${NC}"

if docker network ls | grep -q "observability"; then
	echo -e "  ${GREEN}✅ Observability network exists${NC}"
else
	echo -e "  ${YELLOW}⚠️  Observability network not found${NC}"
fi

echo ""
echo -e "${CYAN}==========================================${NC}"

if [ "$ALL_PASSED" = true ]; then
	echo -e "${GREEN}✅ All validation checks passed!${NC}"
	echo ""
	echo -e "${CYAN}Next Steps:${NC}"
	echo "  1. Start your .NET applications (API, Worker, Notifier)"
	echo "  2. Make HTTP requests to generate traffic"
	echo "  3. Check Jaeger at http://localhost:16686 for traces"
	echo "  4. Check Prometheus at http://localhost:9090 for metrics"
	echo "  5. Create dashboards in Grafana at http://localhost:3000"
	echo ""
else
	echo -e "${YELLOW}⚠️  Some validation checks did not pass${NC}"
	echo ""
	echo -e "${CYAN}Troubleshooting:${NC}"
	echo "  • Ensure Docker is running"
	echo "  • Ensure docker-compose is up: cd docker-compose && ./startup.sh"
	echo "  • Check logs: docker-compose logs <service_name>"
	echo "  • See LOCAL_OTEL_TESTING_GUIDE.md for detailed troubleshooting"
	echo ""
fi

echo -e "${CYAN}📚 Documentation:${NC}"
echo "  • docs/LOCAL_OTEL_TESTING_GUIDE.md - Complete testing guide"
echo "  • docs/OPENTELEMETRY_ARCHITECTURE.md - Architecture overview"
echo "  • docker-compose/README.md - Docker Compose documentation"
echo ""
