# ============================================================================
# AKS Deployment Script for MyStartUpCompany (PowerShell)
# This script automates the deployment to Azure Kubernetes Service
# ============================================================================

param(
	[string]$ResourceGroup = "mystartup-rg",
	[string]$ClusterName = "mystartup-aks",
	[string]$RegistryName = "mystartupregistry",
	[string]$ImageTag = "latest",
	[string]$Namespace = "mystartup"
)

# ============================================================================
# FUNCTIONS
# ============================================================================

function Write-Info {
	param([string]$Message)
	Write-Host "[INFO] $Message" -ForegroundColor Green
}

function Write-Warn {
	param([string]$Message)
	Write-Host "[WARN] $Message" -ForegroundColor Yellow
}

function Write-Error-Custom {
	param([string]$Message)
	Write-Host "[ERROR] $Message" -ForegroundColor Red
	exit 1
}

function Test-Prerequisites {
	Write-Info "Checking prerequisites..."

	# Check Azure CLI
	if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
		Write-Error-Custom "Azure CLI is not installed"
	}
	Write-Info "✓ Azure CLI found"

	# Check kubectl
	if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
		Write-Error-Custom "kubectl is not installed"
	}
	Write-Info "✓ kubectl found"
}

function Get-AKSCredentials {
	Write-Info "Getting AKS cluster credentials..."

	az aks get-credentials `
		--resource-group $ResourceGroup `
		--name $ClusterName `
		--overwrite-existing

	Write-Info "✓ Credentials retrieved and configured"
}

function Verify-Cluster {
	Write-Info "Verifying AKS cluster..."

	$clusterInfo = kubectl cluster-info 2>&1
	if ($LASTEXITCODE -ne 0) {
		Write-Error-Custom "Cannot connect to AKS cluster"
	}

	Write-Info "✓ AKS cluster is accessible"
	Write-Info $clusterInfo
}

function Create-Namespace {
	Write-Info "Creating namespace '$Namespace'..."

	$ns = kubectl get namespace $Namespace 2>&1
	if ($LASTEXITCODE -eq 0) {
		Write-Warn "Namespace '$Namespace' already exists"
	} else {
		kubectl create namespace $Namespace
		Write-Info "✓ Namespace created"
	}
}

function Deploy-Secrets {
	Write-Info "Checking for secrets in namespace..."

	$secret = kubectl get secret app-secrets -n $Namespace 2>&1
	if ($LASTEXITCODE -eq 0) {
		Write-Warn "Secret 'app-secrets' already exists"

		$update = Read-Host "Do you want to update the secrets? (yes/no)"
		if ($update -eq "yes") {
			kubectl delete secret app-secrets -n $Namespace
			Deploy-Secrets
		}
	} else {
		Write-Warn "Please create the secrets first using:"
		Write-Warn "kubectl create secret generic app-secrets \\"
		Write-Warn "  --from-literal=database-connection-string='YOUR_CONNECTION_STRING' \\"
		Write-Warn "  --from-literal=azure-servicebus-connectionstring='YOUR_SERVICEBUS_STRING' \\"
		Write-Warn "  -n $Namespace"

		$secretsCreated = Read-Host "Have you created the secrets? (yes/no)"
		if ($secretsCreated -ne "yes") {
			Write-Error-Custom "Secrets must be created before deployment"
		}
	}
}

function Deploy-Manifests {
	Write-Info "Deploying Kubernetes manifests..."

	$RegistryUrl = "$RegistryName.azurecr.io"
	Write-Info "Using registry: $RegistryUrl"

	# Apply ConfigMap
	Write-Info "Applying ConfigMap..."
	kubectl apply -f k8s/02-configmap.yml

	# Deploy MigrationRunner job
	Write-Info "Deploying MigrationRunner job..."
	$migrationContent = Get-Content k8s/06-migration-job.yml
	$migrationContent -replace "IMAGE_TAG", $ImageTag | kubectl apply -f -

	Write-Info "Waiting for migrations to complete..."
	$jobComplete = $false
	$maxWait = 300
	$elapsed = 0

	while (-not $jobComplete -and $elapsed -lt $maxWait) {
		$job = kubectl get job migration-runner -n $Namespace -o json | ConvertFrom-Json
		if ($job.status.succeeded -eq 1) {
			$jobComplete = $true
		} else {
			Start-Sleep -Seconds 5
			$elapsed += 5
		}
	}

	if (-not $jobComplete) {
		Write-Warn "Migration job may still be running"
		$continueDeploy = Read-Host "Continue with deployment? (yes/no)"
		if ($continueDeploy -ne "yes") {
			Write-Error-Custom "Deployment cancelled"
		}
	} else {
		Write-Info "✓ Migrations completed"
	}

	# Deploy API
	Write-Info "Deploying API service..."
	$apiContent = Get-Content k8s/04-api-deployment.yml
	$apiContent -replace "IMAGE_TAG", $ImageTag | kubectl apply -f -

	# Deploy Worker
	Write-Info "Deploying Worker service..."
	$workerContent = Get-Content k8s/05-worker-deployment.yml
	$workerContent -replace "IMAGE_TAG", $ImageTag | kubectl apply -f -

	# Deploy Pod Disruption Budgets
	Write-Info "Deploying Pod Disruption Budgets..."
	kubectl apply -f k8s/08-pod-disruption-budget.yml

	Write-Info "✓ All manifests deployed"
}

function Wait-ForDeployments {
	Write-Info "Waiting for deployments to be ready..."

	Write-Info "Waiting for API deployment..."
	kubectl rollout status deployment/api -n $Namespace --timeout=300s

	Write-Info "Waiting for Worker deployment..."
	kubectl rollout status deployment/worker -n $Namespace --timeout=300s

	Write-Info "✓ All deployments are ready"
}

function Verify-Services {
	Write-Info "Verifying services..."

	Write-Info "Checking pods..."
	kubectl get pods -n $Namespace -o wide

	Write-Info "Checking services..."
	kubectl get services -n $Namespace -o wide

	Write-Info "Checking deployments..."
	kubectl get deployments -n $Namespace -o wide
}

function Get-ServiceEndpoint {
	Write-Info "Getting service endpoint..."

	$externalIp = ""
	$attempts = 0
	$maxAttempts = 30

	while ([string]::IsNullOrEmpty($externalIp) -and $attempts -lt $maxAttempts) {
		Write-Info "Waiting for LoadBalancer IP (attempt $($attempts + 1)/$maxAttempts)..."

		$svc = kubectl get svc api-service -n $Namespace -o json | ConvertFrom-Json
		if ($svc.status.loadBalancer.ingress.Length -gt 0) {
			$externalIp = $svc.status.loadBalancer.ingress[0].ip
		}

		if ([string]::IsNullOrEmpty($externalIp)) {
			Start-Sleep -Seconds 10
			$attempts++
		}
	}

	if ([string]::IsNullOrEmpty($externalIp)) {
		Write-Warn "Could not retrieve external IP. Service may still be initializing."
	} else {
		Write-Info "✓ Service is accessible at: http://$externalIp"
	}
}

function Show-PodLogs {
	Write-Info "Recent pod logs..."

	Write-Info "API pod logs:"
	kubectl logs -n $Namespace -l app=api --tail=20

	Write-Info "Worker pod logs:"
	kubectl logs -n $Namespace -l app=worker --tail=20
}

# ============================================================================
# MAIN EXECUTION
# ============================================================================

Write-Info "Starting AKS deployment for MyStartUpCompany"
Write-Info "Resource Group: $ResourceGroup"
Write-Info "Cluster: $ClusterName"
Write-Info "Namespace: $Namespace"

$proceed = Read-Host "Continue with deployment? (yes/no)"
if ($proceed -ne "yes") {
	Write-Error-Custom "Deployment cancelled"
}

Test-Prerequisites
Get-AKSCredentials
Verify-Cluster
Create-Namespace
Deploy-Secrets
Deploy-Manifests
Wait-ForDeployments
Verify-Services
Get-ServiceEndpoint
Show-PodLogs

Write-Info ""
Write-Info "✓ Deployment completed successfully!"
Write-Info ""
Write-Info "Next steps:"
Write-Info "1. Test the API: curl http://<EXTERNAL-IP>/health/ready"
Write-Info "2. Monitor pods: kubectl get pods -n $Namespace -w"
Write-Info "3. View logs: kubectl logs -f deployment/api -n $Namespace"
Write-Info "4. Check services: kubectl get services -n $Namespace"
