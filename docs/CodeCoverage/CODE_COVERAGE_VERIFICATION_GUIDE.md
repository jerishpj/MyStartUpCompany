# Code Coverage Verification Guide for Visual Studio

This guide provides comprehensive techniques to measure and verify unit test coverage in Visual Studio before pushing to the repository. It covers built-in VS tools, third-party solutions, and best practices for your .NET 10 project.

---

## Table of Contents

1. [Quick Start](#quick-start)
2. [Visual Studio Built-in Coverage Analysis](#visual-studio-built-in-coverage-analysis)
3. [Advanced Coverage Tools](#advanced-coverage-tools)
4. [Measuring Code Coverage](#measuring-code-coverage)
5. [Setting Coverage Targets](#setting-coverage-targets)
6. [CI/CD Integration](#cicd-integration)
7. [Best Practices](#best-practices)

---

## Quick Start

**Prerequisites:**
- Visual Studio 2022+ or Visual Studio Community 2026
- .NET 10 SDK installed
- Test projects in your solution

**30-Second Coverage Check:**
1. Open **Test Explorer** (`Ctrl+E, T`)
2. Select tests to analyze
3. Right-click → **Analyze Code Coverage for Selected Tests**
4. Review coverage report in output window

---

## Visual Studio Built-in Coverage Analysis

### Method 1: Using Test Explorer Coverage Analysis

**Step-by-Step:**

1. **Open Test Explorer**
   ```
   Visual Studio → Test → Test Explorer (Ctrl+E, T)
   ```

2. **Select Tests**
   - Select specific test classes or individual test methods
   - Or right-click on a test project → **Run All Tests** first

3. **Analyze Coverage**
   - Right-click selected test(s) → **Analyze Code Coverage for Selected Tests**
   - Or: `Test → Analyze Code Coverage for Selected Tests`

4. **Review Results**
   - Coverage percentage appears in the **Code Coverage Results** window
   - Breakdown by:
	 - Project
	 - Namespace
	 - Class
	 - Method
	 - Blocks covered/total

**Example Output:**
```
Project: MyStartUpCompany.Api.Tests
├── Coverage: 78.5%
├── MyStartUpCompany.Api (72% - 312/434 blocks)
│   ├── Mappers (85% - 102/120 blocks)
│   │   └── SourceAMapper: 90%
│   │   └── SourceBMapper: 82%
│   ├── Controllers (65% - 98/150 blocks)
│   └── Services (78% - 112/144 blocks)
└── Uncovered classes listed
```

### Method 2: Coverage Reports Window

**Access Coverage Details:**

1. After analysis completes, open **Code Coverage Results** window:
   ```
   Test → Windows → Code Coverage Results
   ```

2. **Analyze by Level:**
   - Click column headers to sort by coverage %
   - Expand nodes to see detailed breakdowns
   - Right-click entries → **Show Uncovered Code**

3. **Visual Indicators:**
   - 🟢 Green: Well covered (>80%)
   - 🟡 Yellow: Partial coverage (50-80%)
   - 🔴 Red: Poor coverage (<50%)
   - ⚪ Gray: Not covered (0%)

### Method 3: Highlighting Uncovered Code in Editor

**Enable Coverage Highlighting:**

1. **Tools → Options → Test Tools → Code Coverage**

2. **Configure:**
   - ✅ Enable **Highlight uncovered code**
   - Choose highlighting color

3. **View in Code:**
   - Open a source file
   - Lines with highlighting = not covered by tests
   - Covered lines appear normal

---

## Advanced Coverage Tools

### OpenCover + ReportGenerator (Free, Powerful)

**Installation:**

```powershell
# Install global tools
dotnet tool install --global OpenCover
dotnet tool install --global ReportGenerator
```

**Usage:**

1. **Generate Coverage Data:**

```powershell
# Navigate to solution root
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# Run tests with OpenCover
OpenCover.Console.exe `
  -target:"dotnet.exe" `
  -targetargs:"test --configuration Release" `
  -filter:"+[MyStartUpCompany.*]* -[MyStartUpCompany.*.Tests]*" `
  -output:"coverage.xml" `
  -oldStyle
```

2. **Generate HTML Report:**

```powershell
ReportGenerator `
  -reports:"coverage.xml" `
  -targetdir:"CoverageReport" `
  -reporttypes:"Html;HtmlSummary"
```

3. **View Report:**
   - Open `CoverageReport/index.htm` in browser
   - Interactive visualization of coverage

**Advantages:**
- More detailed than built-in tool
- HTML reports for documentation
- Cross-platform support
- Trend tracking capability

---

### Coverlet + ReportGenerator (Modern Approach)

**Installation:**

```powershell
# Add NuGet package to test projects
dotnet add MyStartUpCompany.Api.Tests package coverlet.collector
dotnet add MyStartUpCompany.Worker.Tests package coverlet.collector
```

**Usage:**

1. **Run Tests with Coverage:**

```powershell
# Single project
dotnet test MyStartUpCompany.Api.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput=./TestResults/coverage.cobertura.xml

# All projects
dotnet test `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=opencover `
  /p:CoverletOutput=./TestResults/
```

2. **Configuration (coverlet.runsettings):**

Create `coverlet.runsettings` in solution root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <RunConfiguration>
	<TargetPlatform>x64</TargetPlatform>
	<FrameworkVersion>.NET 10</FrameworkVersion>
  </RunConfiguration>
  <DataCollectionRunSettings>
	<DataCollectors>
	  <DataCollector friendlyName="XPlat code coverage">
		<Configuration>
		  <Format>cobertura</Format>
		  <Exclude>[*]*.Tests.*,[*]*.Test.*,[xunit.*]*</Exclude>
		  <UseSourceLink>true</UseSourceLink>
		</Configuration>
	  </DataCollector>
	</DataCollectors>
  </DataCollectionRunSettings>
</RunSettings>
```

3. **Generate Reports:**

```powershell
ReportGenerator `
  -reports:"TestResults/coverage.cobertura.xml" `
  -targetdir:"CoverageReports" `
  -reporttypes:"Html;Badges;MarkdownSummary"
```

**Advantages:**
- Integrates seamlessly with .NET CLI
- Modern, actively maintained
- Better .NET 10 support
- Can exclude test code automatically
- Supports multiple output formats

---

## Measuring Code Coverage

### Strategy 1: Coverage by Project

**Command:**

```powershell
# Test each project individually
dotnet test tests/MyStartUpCompany.Api.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura

dotnet test tests/MyStartUpCompany.Worker.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura

dotnet test tests/MyStartUpCompany.Observability.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura
```

**Analysis Points:**
- Identify project-specific gaps
- Set individual coverage baselines
- Track improvements per project

### Strategy 2: Combined Project Coverage

**Batch Script** (`measure-coverage.ps1`):

```powershell
# Run all tests with coverage collection
$projects = @(
	"tests/MyStartUpCompany.Api.Tests/MyStartUpCompany.Api.Tests.csproj",
	"tests/MyStartUpCompany.Worker.Tests/MyStartUpCompany.Worker.Tests.csproj",
	"tests/MyStartUpCompany.Observability.Tests/MyStartUpCompany.Observability.Tests.csproj"
)

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$reportDir = "CoverageReports_$timestamp"
New-Item -ItemType Directory -Path $reportDir -Force

foreach ($project in $projects) {
	Write-Host "Testing: $project"
	dotnet test $project `
		/p:CollectCoverage=true `
		/p:CoverletOutputFormat=cobertura `
		/p:CoverletOutput="./$reportDir/coverage.xml"
}

# Generate unified report
ReportGenerator `
	-reports:"$reportDir/*.xml" `
	-targetdir:"$reportDir/Report" `
	-reporttypes:"Html;HtmlSummary;MarkdownSummary"

Write-Host "Coverage report generated: $reportDir/Report/index.html"
```

### Strategy 3: IDE-Based Analysis

**In Visual Studio:**

1. **Run → Run Tests with Profiler**
   - Provides code coverage automatically
   - Shows execution flow

2. **Test → Analyze Code Coverage for All Tests**
   - Comprehensive analysis
   - All test projects included

3. **View Results:**
   - **Test Explorer** → Coverage column
   - **Code Coverage Results** window → detailed breakdown

---

## Setting Coverage Targets

### Define Minimum Coverage Requirements

**Create `coverage-targets.json`:**

```json
{
  "coverageTargets": {
	"overall": 80,
	"projectSpecific": {
	  "MyStartUpCompany.Api": 85,
	  "MyStartUpCompany.Worker": 80,
	  "MyStartUpCompany.Observability": 75,
	  "MyStartUpCompany.Persistence": 70
	},
	"classMinimum": 70,
	"criticalClasses": {
	  "MyStartUpCompany.Api.Mappers.*": 95,
	  "MyStartUpCompany.Worker.Services.*": 90,
	  "MyStartUpCompany.Persistence.Repositories.*": 85
	}
  }
}
```

### Enforcement Script

**Coverage Validation Script** (`validate-coverage.ps1`):

```powershell
param(
	[string]$reportPath = "./CoverageReports/coverage.cobertura.xml",
	[string]$targetFile = "./coverage-targets.json"
)

# Load target configuration
$targets = Get-Content $targetFile | ConvertFrom-Json

# Parse coverage XML
[xml]$coverage = Get-Content $reportPath
$lineCoverage = $coverage.coverage.'line-rate'
$currentCoverage = [math]::Round($lineCoverage * 100, 2)

Write-Host "Current Coverage: $currentCoverage%"
Write-Host "Target Coverage: $($targets.coverageTargets.overall)%"

if ($currentCoverage -lt $targets.coverageTargets.overall) {
	Write-Host "❌ Coverage FAILED: $currentCoverage% is below target of $($targets.coverageTargets.overall)%" -ForegroundColor Red
	exit 1
} else {
	Write-Host "✅ Coverage PASSED: $currentCoverage% meets target of $($targets.coverageTargets.overall)%" -ForegroundColor Green
	exit 0
}
```

**Usage Before Commit:**

```powershell
./validate-coverage.ps1
if ($LASTEXITCODE -ne 0) {
	Write-Host "Add more tests before committing!" -ForegroundColor Red
	exit 1
}
```

---

## CI/CD Integration

### GitHub Actions Workflow

**File:** `.github/workflows/code-coverage.yml`

```yaml
name: Code Coverage Analysis

on:
  pull_request:
	branches: [ denormalise-test, main ]
  push:
	branches: [ denormalise-test ]

jobs:
  coverage:
	runs-on: ubuntu-latest

	steps:
	- uses: actions/checkout@v4

	- name: Setup .NET
	  uses: actions/setup-dotnet@v4
	  with:
		dotnet-version: '10.0'

	- name: Restore dependencies
	  run: dotnet restore

	- name: Build
	  run: dotnet build --configuration Release --no-restore

	- name: Run tests with coverage
	  run: |
		dotnet test --configuration Release \
		  /p:CollectCoverage=true \
		  /p:CoverletOutputFormat=cobertura \
		  /p:CoverletOutput=./TestResults/coverage.cobertura.xml

	- name: Install ReportGenerator
	  run: dotnet tool install --global ReportGenerator

	- name: Generate coverage report
	  run: |
		reportgenerator \
		  -reports:"TestResults/coverage.cobertura.xml" \
		  -targetdir:"CoverageReport" \
		  -reporttypes:"Html;MarkdownSummary"

	- name: Upload coverage to Codecov
	  uses: codecov/codecov-action@v4
	  with:
		files: ./TestResults/coverage.cobertura.xml
		fail_ci_if_error: true
		verbose: true

	- name: Check coverage threshold
	  run: |
		coverage=$(grep -oP '(?<=line-rate=")[^"]*' TestResults/coverage.cobertura.xml | head -1)
		threshold=0.80
		if (( $(echo "$coverage < $threshold" | bc -l) )); then
		  echo "❌ Coverage $coverage is below threshold $threshold"
		  exit 1
		fi
		echo "✅ Coverage $coverage meets threshold $threshold"
```

---

## Best Practices

### ✅ DO's

1. **Measure Regularly**
   - Run coverage analysis before every commit
   - Track coverage trends over time
   - Aim for 80%+ coverage on critical paths

2. **Focus on Critical Paths**
   - Data access layers (Repositories)
   - Business logic (Mappers, Services)
   - Error handling
   - Edge cases

3. **Automated Validation**
   - Fail builds if coverage drops
   - Require minimum coverage in PR checks
   - Generate reports for documentation

4. **Test Meaningful Scenarios**
   - Happy path + edge cases
   - Error conditions
   - Null handling
   - Boundary conditions

5. **Keep Tests Maintainable**
   - Use clear test names
   - One assertion per test (or logically grouped)
   - Mock external dependencies
   - Arrange-Act-Assert pattern

### ❌ DON'Ts

1. **Don't Chase 100% Coverage**
   - Unrealistic goal
   - Leads to brittle tests
   - Can indicate over-testing trivial code

2. **Don't Cover Test Code**
   - Exclude `*.Tests` assemblies from coverage
   - Focus on production code only

3. **Don't Ignore Coverage Gaps**
   - Investigate low-coverage areas
   - Understand why code isn't tested

4. **Don't Measure Without Purpose**
   - Know what coverage metric means
   - Track trends, not just numbers
   - Link coverage to quality

5. **Don't Mix Concerns**
   - Keep unit tests unit-focused
   - Integration tests separate
   - Test behavior, not implementation

---

## Practical Workflow for Your Project

### Pre-Commit Checklist

**Before pushing to `denormalise-test` branch:**

1. **Run Test Explorer Coverage:**
   ```
   Ctrl+E, T → Select all tests → Right-click → Analyze Code Coverage
   ```
   - Verify all tests pass
   - Ensure coverage ≥ 80%

2. **Generate Detailed Report:**
   ```powershell
   dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
   ReportGenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./CoverageReport
   ```
   - Open `CoverageReport/index.html`
   - Review uncovered code
   - Identify gaps

3. **Review Specific Areas:**
   - Mappers: Target 90%+
   - Services: Target 85%+
   - Controllers: Target 75%+
   - Utilities: Target 70%+

4. **Document Coverage:**
   - Update this file with new coverage metrics
   - Note any intentional exclusions

### Command Quick Reference

```powershell
# Quick coverage analysis (VS built-in)
# Test Explorer → Select tests → Right-click → Analyze Code Coverage for Selected Tests

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Create HTML report
ReportGenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./CoverageReport -reporttypes:Html

# Validate against threshold
./validate-coverage.ps1

# Full pre-commit workflow
dotnet test /p:CollectCoverage=true && ReportGenerator -reports:./TestResults/coverage.cobertura.xml -targetdir:./CoverageReport && start ./CoverageReport/index.html
```

---

## Coverage by Project (Your Solution)

**Current Status (As of latest test run):**

| Project | Target | Status |
|---------|--------|--------|
| MyStartUpCompany.Api | 85% | 📊 [Measure] |
| MyStartUpCompany.Worker | 80% | 📊 [Measure] |
| MyStartUpCompany.Observability | 75% | 📊 [Measure] |
| MyStartUpCompany.Persistence | 70% | 📊 [Measure] |
| **Overall** | **80%** | 📊 [Measure] |

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Coverage window not appearing | `Test → Windows → Code Coverage Results` |
| Coverage shows 0% | Rebuild solution, ensure test projects reference main projects |
| ReportGenerator not found | `dotnet tool install --global ReportGenerator` |
| XML coverage files missing | Check `TestResults/` directory, ensure `/p:CollectCoverage=true` |
| Performance slow during analysis | Close other applications, use Release build configuration |

---

## Additional Resources

- [Microsoft: Code Coverage Analysis](https://learn.microsoft.com/en-us/visualstudio/test/using-code-coverage-to-determine-how-much-code-is-being-tested)
- [Coverlet Documentation](https://github.com/coverlet-coverage/coverlet)
- [ReportGenerator](https://github.com/danielpalme/ReportGenerator)
- [xUnit Coverage Best Practices](https://xunit.net/)

---

**Last Updated:** 2024
**Applicable to:** .NET 10, Visual Studio 2022+, GitHub Actions
