#!/bin/bash

# AKS Deployment Helper for MyStartUpCompany
# This script automates common deployment operations

set -e

# Configuration
RESOURCE_GROUP="mystartupcorp-rg"
AKS_CLUSTER_NAME="mystartupcorp-aks"
NAMESPACE_APP="mystartupcorp"
NAMESPACE_OBS="observability"

# Colors
GREEN='\033[0;32m'
BLUE='\033[0;34m'
NC='\033[0m'

print_header() {
	echo -e "${BLUE}=== $1 ===${NC}"
}

print_success() {
	echo -e "${GREEN}✓ $1${NC}"
}

deploy_manifests() {
	print_header "Deploying Kubernetes Manifests"

	kubectl apply -f k8s/00-namespaces.yaml
	kubectl apply -f k8s/01-observability-config.yaml
	kubectl apply -f k8s/02-otel-collector.yaml
	kubectl apply -f k8s/03-jaeger.yaml
	kubectl apply -f k8s/04-prometheus.yaml
	kubectl apply -f k8s/05-grafana.yaml
	kubectl apply -f k8s/06-api-deployment.yaml
	kubectl apply -f k8s/07-worker-deployment.yaml
	kubectl apply -f k8s/08-notifier-deployment.yaml
	kubectl apply -f k8s/09-ingress-and-network-policies.yaml

	print_success "Deployment complete"
}

case "${1:-help}" in
	deploy)
		deploy_manifests
		;;
	*)
		echo "Usage: $0 {deploy}"
		;;
esac
