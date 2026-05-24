#!/bin/bash

###############################################################################
# Database Migration Script for MyStartUpCompany
#
# This script runs database migrations using the MigrationRunner console app.
# It's useful for local development, CI/CD pipelines, and container startup.
#
# Usage:
#   ./scripts/migrate.sh                                    # Development
#   ./scripts/migrate.sh production                         # Production
#   ./scripts/migrate.sh --connection-string "Server=..."  # Override connection
#   ./scripts/migrate.sh --list-migrations                 # List pending only
#
# Environment Variables:
#   ASPNETCORE_ENVIRONMENT           Environment (Development/Staging/Production)
#   ConnectionStrings__DefaultConnection Database connection string
#   DEBUG_MIGRATIONS                 Enable debug logging
###############################################################################

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Defaults
ENVIRONMENT="${1:-Development}"
CONNECTION_STRING=""
LIST_ONLY=false
DOTNET_PROJECT_PATH="./src/MigrationRunner"

# Parse arguments
while [[ $# -gt 0 ]]; do
	case $1 in
		development|Development|dev)
			ENVIRONMENT="Development"
			shift
			;;
		staging|Staging|stage)
			ENVIRONMENT="Staging"
			shift
			;;
		production|Production|prod)
			ENVIRONMENT="Production"
			shift
			;;
		--connection-string)
			CONNECTION_STRING="$2"
			shift 2
			;;
		--list-migrations|--list)
			LIST_ONLY=true
			shift
			;;
		--help|-h)
			echo "MyStartUpCompany Database Migration Tool"
			echo ""
			echo "Usage: $0 [environment] [options]"
			echo ""
			echo "Environments:"
			echo "  development    - Development environment (default)"
			echo "  staging        - Staging environment"
			echo "  production     - Production environment"
			echo ""
			echo "Options:"
			echo "  --connection-string <value>  Override database connection string"
			echo "  --list-migrations            List pending migrations without applying"
			echo "  --help, -h                   Show this help message"
			echo ""
			echo "Examples:"
			echo "  $0                                          # Development"
			echo "  $0 production                               # Production"
			echo "  $0 --connection-string \"Server=...;\"      # Override connection"
			echo ""
			exit 0
			;;
		*)
			echo "Unknown option: $1"
			echo "Use --help for usage information"
			exit 1
			;;
	esac
done

# Get script directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
REPO_ROOT="$(dirname "$SCRIPT_DIR")"

# Resolve paths
PROJECT_PATH="$REPO_ROOT/$DOTNET_PROJECT_PATH"

echo -e "${CYAN}========================================"
echo "MyStartUpCompany Database Migration Tool"
echo "========================================${NC}"
echo ""

# Validate paths
if [ ! -d "$PROJECT_PATH" ]; then
	echo -e "${RED}ERROR: MigrationRunner project not found at: $PROJECT_PATH${NC}"
	exit 1
fi

echo -e "${YELLOW}Environment: $ENVIRONMENT${NC}"
echo -e "${YELLOW}MigrationRunner: $PROJECT_PATH${NC}"

# Build arguments array
ARGUMENTS=('run' '--project' "$PROJECT_PATH" '--')
ARGUMENTS+=('--environment' "$ENVIRONMENT")

# Add connection string if provided
if [ -n "$CONNECTION_STRING" ]; then
	echo -e "${YELLOW}Connection String: (provided via parameter)${NC}"
	ARGUMENTS+=('--connection-string' "$CONNECTION_STRING")
fi

# Add list-only flag if requested
if [ "$LIST_ONLY" = true ]; then
	echo -e "${YELLOW}Mode: List pending migrations only${NC}"
	ARGUMENTS+=('--list-migrations')
fi

echo ""

# Run migration
echo -e "${CYAN}Running migrations...${NC}"
echo ""

export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"

# Execute dotnet with arguments
if dotnet "${ARGUMENTS[@]}"; then
	EXIT_CODE=0
else
	EXIT_CODE=$?
fi

echo ""
if [ $EXIT_CODE -eq 0 ]; then
	echo -e "${GREEN}========================================"
	echo "Migration completed successfully!"
	echo "========================================${NC}"
	exit 0
else
	echo -e "${RED}========================================"
	echo "Migration failed with exit code: $EXIT_CODE"
	echo "========================================${NC}"
	exit $EXIT_CODE
fi
