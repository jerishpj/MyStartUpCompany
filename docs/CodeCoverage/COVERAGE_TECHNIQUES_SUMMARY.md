# Code Coverage Techniques Summary

## Overview

As a developer, you have **multiple techniques** to verify unit test coverage in Visual Studio before pushing to your repository. This document summarizes all available approaches, from simple to advanced.

---

## Technique 1: Visual Studio Test Explorer (Built-in) ⭐ EASIEST

**When to use:** Quick coverage check before commit

**Steps:**
1. Open **Test Explorer**: `Ctrl+E, T`
2. Select tests you want to analyze
3. Right-click → **Analyze Code Coverage for Selected Tests**
4. Results appear in **Code Coverage Results** window

**Pros:**
- ✅ No setup required
- ✅ Built into Visual Studio
- ✅ Interactive UI
- ✅ Drill-down by class/method
- ✅ Shows uncovered code inline

**Cons:**
- ❌ Limited reporting options
- ❌ Manual process
- ❌ Can't easily share reports

**Time to execute:** 30 seconds - 2 minutes

**Sample output:**
```
MyStartUpCompany.Api (72% - 312/434 blocks)
├─ Mappers (85%)
│  ├─ SourceAMapper: 90% ✅
│  ├─ SourceBMapper: 82% 🟡
│  └─ SourceCMapper: 85% ✅
├─ Controllers (65%) 🔴
└─ Services (78%)
```

---

## Technique 2: Coverage Highlighting in Editor

**When to use:** While writing/reviewing code

**Setup (one-time):**
1. `Tools → Options → Test Tools → Code Coverage`
2. Enable: **Highlight uncovered code**
3. Choose a color (typically red/orange)

**Usage:**
1. Run any test
2. Open source file
3. Uncovered lines appear highlighted

**Pros:**
- ✅ Visual feedback while coding
- ✅ Identifies gaps immediately
- ✅ No extra window needed

**Cons:**
- ❌ Only highlights, no metrics
- ❌ Requires running tests first

**Time to execute:** Instant (after first test run)

---

## Technique 3: Code Coverage Results Window

**When to use:** Deep analysis after running coverage

**Steps:**
1. Run coverage analysis (Test Explorer → Analyze)
2. `Test → Windows → Code Coverage Results`
3. Explore hierarchically:
   - Click to expand projects
   - View classes and methods
   - See coverage percentages
   - Right-click → **Show Uncovered Code**

**Pros:**
- ✅ Comprehensive metrics
- ✅ Hierarchical view (Project > Class > Method)
- ✅ Sort by coverage %
- ✅ Direct link to uncovered code

**Cons:**
- ❌ Windows-only (Visual Studio)
- ❌ Not shareable

**Time to execute:** 1-3 minutes

**Data shown:**
- Line coverage %
- Blocks covered/total
- Namespace hierarchy
- Uncovered methods list

---

## Technique 4: Coverlet + Command Line ⭐ MOST POWERFUL

**When to use:** Automated checks, CI/CD, detailed reports

**Installation:**
```powershell
# Add to test projects (done once)
dotnet add tests/MyStartUpCompany.Api.Tests package coverlet.collector
dotnet add tests/MyStartUpCompany.Worker.Tests package coverlet.collector
```

**Usage - Single Project:**
```powershell
dotnet test tests/MyStartUpCompany.Api.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput=./TestResults/coverage.cobertura.xml
```

**Usage - All Projects:**
```powershell
dotnet test `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput=./TestResults/coverage.xml
```

**Pros:**
- ✅ Scriptable and automated
- ✅ Works cross-platform
- ✅ Multiple output formats
- ✅ Excludes test code automatically
- ✅ Integrates with CI/CD

**Cons:**
- ❌ Requires PowerShell or terminal knowledge
- ❌ No visual UI (but outputs XML/JSON)

**Time to execute:** 30 seconds - 2 minutes

**Output files:**
- `coverage.cobertura.xml` - Machine-readable format
- Can be processed by other tools

---

## Technique 5: Coverlet + ReportGenerator 🌟 BEST FOR ANALYSIS

**When to use:** Comprehensive analysis before push, documentation

**Installation:**
```powershell
# Setup once
./scripts/setup-coverage-tools.ps1

# Or manually:
dotnet tool install --global ReportGenerator
dotnet add tests/* package coverlet.collector
```

**Usage:**
```powershell
# Step 1: Generate coverage data
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura

# Step 2: Generate HTML report
ReportGenerator `
  -reports:"./TestResults/coverage.cobertura.xml" `
  -targetdir:"./CoverageReport" `
  -reporttypes:"Html"

# Step 3: Open in browser
Start-Process "./CoverageReport/index.html"
```

**Or use provided script:**
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
```

**Pros:**
- ✅ Beautiful HTML report
- ✅ Interactive visualization
- ✅ Drill-down to line-level
- ✅ Shows exactly which lines aren't covered
- ✅ Shareable (email HTML report)
- ✅ Highlights uncovered lines in red
- ✅ Multiple report types (HTML, Markdown, PDF)

**Cons:**
- ❌ Requires installation
- ❌ 2-step process

**Time to execute:** 1-3 minutes (including installation first time)

**Report includes:**
- Overall coverage %
- Coverage by file/class/method
- Line-by-line uncovered code
- Trend charts
- Summary statistics

---

## Technique 6: Batch Script Automation

**When to use:** Pre-commit validation, repeatable checks

**Provided Scripts:**

### `scripts/measure-coverage.ps1`
```powershell
# Measure single project
./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests"

# Measure all projects
./scripts/measure-coverage.ps1 -All

# Measure and generate report
./scripts/measure-coverage.ps1 -All -GenerateReport
```

### `scripts/validate-coverage.ps1`
```powershell
# Check if coverage meets threshold (80%)
./scripts/validate-coverage.ps1

# Returns exit code 0 (pass) or 1 (fail) - useful in automation
```

### `scripts/pre-commit-coverage-check.ps1`
```powershell
# Full pre-commit workflow
./scripts/pre-commit-coverage-check.ps1
```

**Pros:**
- ✅ One-command execution
- ✅ Automated threshold checking
- ✅ Can fail builds if coverage drops
- ✅ Repeatable and consistent

**Cons:**
- ❌ Requires PowerShell

**Time to execute:** 30 seconds - 2 minutes

---

## Technique 7: GitHub Actions CI/CD

**When to use:** Automatic checks on pull requests, enforcement

**Setup:**
```yaml
# .github/workflows/code-coverage.yml (already created)
# Runs automatically on push and pull requests
```

**What it does:**
1. ✅ Runs all tests with coverage
2. ✅ Generates HTML reports
3. ✅ Uploads to Codecov.io
4. ✅ Validates threshold
5. ✅ Comments on PRs with results
6. ✅ Archives reports

**Viewing Results:**
- PR comment with coverage summary
- Artifacts tab → Coverage report
- Codecov.io dashboard

**Pros:**
- ✅ Fully automated
- ✅ Can block PRs if coverage drops
- ✅ Historical trend tracking
- ✅ Team-wide visibility
- ✅ No manual steps needed

**Cons:**
- ❌ Only on push/PR
- ❌ Delayed feedback (minutes)

**Time to execute:** Automatic (2-5 minutes after push)

---

## Technique 8: OpenCover (Alternative)

**When to use:** Advanced users wanting maximum control

**Installation:**
```powershell
dotnet tool install --global OpenCover
```

**Usage:**
```powershell
OpenCover.Console.exe `
  -target:"dotnet.exe" `
  -targetargs:"test --configuration Release" `
  -filter:"+[MyStartUpCompany.*]* -[*.Tests]*" `
  -output:"coverage.xml"

# Then use ReportGenerator to create reports
```

**Pros:**
- ✅ Very granular control
- ✅ Advanced filtering options
- ✅ Cross-platform

**Cons:**
- ❌ Steeper learning curve
- ❌ More configuration needed

**Time to execute:** 2-5 minutes (including setup)

---

## Comparison Table

| Technique | Ease | Speed | Detail | Automation | Sharing | Best For |
|-----------|------|-------|--------|-----------|---------|----------|
| **Test Explorer** | ⭐⭐⭐⭐⭐ | Fast | Medium | ❌ | ❌ | Quick checks |
| **Coverage Highlighting** | ⭐⭐⭐⭐⭐ | Very Fast | Low | Auto | ❌ | While coding |
| **Coverage Results Window** | ⭐⭐⭐⭐ | Medium | High | ❌ | ❌ | Deep analysis |
| **Coverlet CLI** | ⭐⭐⭐ | Fast | High | ✅ | Limited | Scripting |
| **Coverlet + ReportGenerator** | ⭐⭐⭐ | Medium | Very High | ✅ | ✅ | Team communication |
| **Batch Scripts** | ⭐⭐⭐⭐ | Fast | High | ✅ | ✅ | Pre-commit checks |
| **GitHub Actions** | ⭐⭐ | Slow | Very High | ✅ | ✅ | PR enforcement |
| **OpenCover** | ⭐⭐ | Medium | Very High | ✅ | ✅ | Advanced control |

---

## Recommended Workflows

### For Solo Development (Pre-Push)
```
1. Write/modify code
2. Run tests: Ctrl+E, T → Run All
3. Check coverage: Right-click → Analyze Code Coverage
4. If < 80%: ./scripts/measure-coverage.ps1 -All -GenerateReport
5. Review HTML report, add tests
6. Repeat until coverage >= 80%
7. Push to repository
```

### For Team Development (PR Review)
```
1. Local: ./scripts/measure-coverage.ps1 -All -GenerateReport
2. Verify coverage meets standards
3. Push to branch
4. GitHub Actions automatically:
   - Runs tests with coverage
   - Generates report
   - Comments on PR
   - Archives artifacts
5. Reviewers see coverage impact
```

### For Pre-Commit Hook
```
1. Modify code
2. Run: ./scripts/pre-commit-coverage-check.ps1
3. If passes → Commit allowed
4. If fails → Fix tests, run again
```

---

## Quick Decision Tree

**Q: Want fastest coverage check?**
→ Use **Test Explorer** (Ctrl+E, T → Analyze)

**Q: Need detailed analysis?**
→ Use **Coverlet + ReportGenerator** (./scripts/measure-coverage.ps1 -All -GenerateReport)

**Q: Automating for CI/CD?**
→ Use **GitHub Actions** (.github/workflows/code-coverage.yml)

**Q: Validating before commit?**
→ Use **Batch Scripts** (./scripts/pre-commit-coverage-check.ps1)

**Q: Want visual feedback while coding?**
→ Use **Coverage Highlighting** (Tools → Options)

---

## Implementation Guide for Your Project

### Step 1: Setup (5 minutes)
```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany
./scripts/setup-coverage-tools.ps1
```

### Step 2: First Measurement (2 minutes)
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Opens CoverageReport/index.html automatically
```

### Step 3: Identify Gaps (5-10 minutes)
- Review HTML report
- Note uncovered code
- Prioritize by importance

### Step 4: Add Tests (15-30 minutes)
- Write tests for uncovered code
- Focus on business logic first
- Use existing test patterns

### Step 5: Validate (2 minutes)
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
./scripts/validate-coverage.ps1
```

### Step 6: Push (1 minute)
```powershell
git add .
git commit -m "Add tests: improve coverage from X% to Y%"
git push origin denormalise-test
```

---

## Key Metrics for Your Projects

| Project | Status | Technique |
|---------|--------|-----------|
| MyStartUpCompany.Api | 📊 TBD | Run: `./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests" -GenerateReport` |
| MyStartUpCompany.Worker | 📊 TBD | Run: `./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Worker.Tests" -GenerateReport` |
| MyStartUpCompany.Observability | 📊 TBD | Run: `./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Observability.Tests" -GenerateReport` |

---

## Next Steps

1. **Read**: `docs/COVERAGE_QUICK_REFERENCE.md` for cheat sheet
2. **Setup**: Run `./scripts/setup-coverage-tools.ps1` 
3. **Measure**: Run `./scripts/measure-coverage.ps1 -All -GenerateReport`
4. **Analyze**: Open the HTML report
5. **Improve**: Add tests for uncovered code
6. **Validate**: Run `./scripts/validate-coverage.ps1`

---

**Questions?** See `docs/CODE_COVERAGE_VERIFICATION_GUIDE.md` for detailed explanations.
