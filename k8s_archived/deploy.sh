#!/bin/bash

# ============================================================================
# AKS Deployment Script for MyStartUpCompany
# This script automates the deployment to Azure Kubernetes Service
# ============================================================================

set -e

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# ============================================================================
# CONFIGURATION
# ============================================================================

# Azure settings
RESOURCE_GROUP="${RESOURCE_GROUP:-mystartup-rg}"
CLUSTER_NAME="${CLUSTER_NAME:-mystartup-aks}"
REGISTRY_NAME="${REGISTRY_NAME:-mystartupregistry}"

# Kubernetes settings
NAMESPACE="mystartup"
IMAGE_TAG="${IMAGE_TAG:-latest}"
REGISTRY_URL="${REGISTRY_NAME}.azurecr.io"

# Database settings
SQL_SERVER="${SQL_SERVER:-mystartup-sql}"
SQL_DATABASE="${SQL_DATABASE:-MyStartUpCompanyDb}"

# ============================================================================
# FUNCTIONS
# ============================================================================

log_info() {
	echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
	echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
	echo -e "${RED}[ERROR]${NC} $1"
	exit 1
}

check_prerequisites() {
	log_info "Checking prerequisites..."

	# Check Azure CLI
	if ! command -v az &> /dev/null; then
		log_error "Azure CLI is not installed"
	fi
	log_info "✓ Azure CLI found"

	# Check kubectl
	if ! command -v kubectl &> /dev/null; then
		log_error "kubectl is not installed"
	fi
	log_info "✓ kubectl found"

	# Check Docker
	if ! command -v docker &> /dev/null; then
		log_warn "Docker is not installed (needed for building images)"
	else
		log_info "✓ Docker found"
	fi
}

get_aks_credentials() {
	log_info "Getting AKS cluster credentials..."

	az aks get-credentials \
		--resource-group "$RESOURCE_GROUP" \
		--name "$CLUSTER_NAME" \
		--overwrite-existing

	log_info "✓ Credentials retrieved and configured"
}

verify_cluster() {
	log_info "Verifying AKS cluster..."

	if ! kubectl cluster-info &> /dev/null; then
		log_error "Cannot connect to AKS cluster"
	fi

	log_info "✓ AKS cluster is accessible"
	kubectl cluster-info
}

create_namespace() {
	log_info "Creating namespace '$NAMESPACE'..."

	if kubectl get namespace "$NAMESPACE" &> /dev/null; then
		log_warn "Namespace '$NAMESPACE' already exists"
	else
		kubectl create namespace "$NAMESPACE"
		log_info "✓ Namespace created"
	fi
}

deploy_secrets() {
	log_info "Checking for secrets in namespace..."

	# Check if secrets already exist
	if kubectl get secret app-secrets -n "$NAMESPACE" &> /dev/null; then
		log_warn "Secret 'app-secrets' already exists"
		read -p "Do you want to update the secrets? (yes/no): " update_secrets

		if [ "$update_secrets" = "yes" ]; then
			kubectl delete secret app-secrets -n "$NAMESPACE"
			deploy_secrets
		fi
	else
		log_warn "Please create the secrets first:"
		log_warn "kubectl create secret generic app-secrets \\"
		log_warn "  --from-literal=database-connection-string='YOUR_CONNECTION_STRING' \\"
		log_warn "  --from-literal=azure-servicebus-connectionstring='YOUR_SERVICEBUS_STRING' \\"
		log_warn "  -n $NAMESPACE"

		read -p "Have you created the secrets? (yes/no): " secrets_created
		if [ "$secrets_created" != "yes" ]; then
			log_error "Secrets must be created before deployment"
		fi
	fi
}

deploy_manifests() {
	log_info "Deploying Kubernetes manifests..."

	# Update image references in manifests
	log_info "Updating image references to: $REGISTRY_URL"

	# Apply manifests in order
	log_info "Applying ConfigMap..."
	kubectl apply -f k8s/02-configmap.yml

	log_info "Applying Secrets (if using local file)..."
	# kubectl apply -f k8s/03-secrets.yml  # Uncomment if using file-based secrets

	log_info "Deploying MigrationRunner job..."
	sed "s|IMAGE_TAG|$IMAGE_TAG|g" k8s/06-migration-job.yml | kubectl apply -f -

	log_info "Waiting for migrations to complete..."
	kubectl wait --for=condition=complete job/migration-runner -n "$NAMESPACE" --timeout=300s 2>/dev/null || {
		log_warn "Migration job may still be running"
		log_info "Check status with: kubectl get job migration-runner -n $NAMESPACE"
		read -p "Continue with deployment? (yes/no): " continue_deploy
		if [ "$continue_deploy" != "yes" ]; then
			log_error "Deployment cancelled"
		fi
	}

	log_info "Deploying API service..."
	sed "s|IMAGE_TAG|$IMAGE_TAG|g" k8s/04-api-deployment.yml | kubectl apply -f -

	log_info "Deploying Worker service..."
	sed "s|IMAGE_TAG|$IMAGE_TAG|g" k8s/05-worker-deployment.yml | kubectl apply -f -

	log_info "Deploying Pod Disruption Budgets..."
	kubectl apply -f k8s/08-pod-disruption-budget.yml

	log_info "✓ All manifests deployed"
}

wait_for_deployments() {
	log_info "Waiting for deployments to be ready..."

	log_info "Waiting for API deployment..."
	kubectl rollout status deployment/api -n "$NAMESPACE" --timeout=300s

	log_info "Waiting for Worker deployment..."
	kubectl rollout status deployment/worker -n "$NAMESPACE" --timeout=300s

	log_info "✓ All deployments are ready"
}

verify_services() {
	log_info "Verifying services..."

	log_info "Checking pods..."
	kubectl get pods -n "$NAMESPACE" -o wide

	log_info "Checking services..."
	kubectl get services -n "$NAMESPACE" -o wide

	log_info "Checking deployments..."
	kubectl get deployments -n "$NAMESPACE" -o wide
}

get_service_endpoint() {
	log_info "Getting service endpoint..."

	EXTERNAL_IP=""
	while [ -z $EXTERNAL_IP ]; do
		log_info "Waiting for LoadBalancer IP..."
		EXTERNAL_IP=$(kubectl get svc api-service -n "$NAMESPACE" --template="{{range .status.loadBalancer.ingress}}{{.ip}}{{end}}")
		[ -z "$EXTERNAL_IP" ] && sleep 10
	done

	log_info "✓ Service is accessible at: http://$EXTERNAL_IP"
}

show_pod_logs() {
	log_info "Recent pod logs..."

	log_info "API pod logs:"
	kubectl logs -n "$NAMESPACE" -l app=api --tail=20

	log_info "Worker pod logs:"
	kubectl logs -n "$NAMESPACE" -l app=worker --tail=20
}

# ============================================================================
# MAIN EXECUTION
# ============================================================================

main() {
	log_info "Starting AKS deployment for MyStartUpCompany"
	log_info "Resource Group: $RESOURCE_GROUP"
	log_info "Cluster: $CLUSTER_NAME"
	log_info "Namespace: $NAMESPACE"
	log_info "Registry: $REGISTRY_URL"

	echo ""
	read -p "Continue with deployment? (yes/no): " proceed
	if [ "$proceed" != "yes" ]; then
		log_error "Deployment cancelled"
	fi

	check_prerequisites
	get_aks_credentials
	verify_cluster
	create_namespace
	deploy_secrets
	deploy_manifests
	wait_for_deployments
	verify_services
	get_service_endpoint
	show_pod_logs

	log_info "✓ Deployment completed successfully!"
	log_info ""
	log_info "Next steps:"
	log_info "1. Test the API: curl http://$EXTERNAL_IP/health/ready"
	log_info "2. Monitor pods: kubectl get pods -n $NAMESPACE -w"
	log_info "3. View logs: kubectl logs -f deployment/api -n $NAMESPACE"
	log_info "4. Check services: kubectl get services -n $NAMESPACE"
}

# Run main
main
