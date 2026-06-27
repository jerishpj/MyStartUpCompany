# Code Coverage Quick Reference

## 🚀 Quick Start (2 minutes)

### Option 1: Visual Studio Built-in (Easiest)
```
1. Ctrl+E, T                          (Open Test Explorer)
2. Select your tests or test project
3. Right-click → Analyze Code Coverage for Selected Tests
4. View results in Code Coverage Results window
```

### Option 2: Command Line (Most Powerful)
```powershell
# Setup once
./scripts/setup-coverage-tools.ps1

# Measure coverage
./scripts/measure-coverage.ps1 -All -GenerateReport

# Validate
./scripts/validate-coverage.ps1
```

---

## 📊 Key Metrics to Understand

| Metric | Meaning | Good Range |
|--------|---------|-----------|
| **Line Coverage** | % of lines executed during tests | 80%+ |
| **Branch Coverage** | % of decision paths taken | 75%+ |
| **Block Coverage** | % of code blocks executed | 80%+ |
| **Method Coverage** | % of methods called | 85%+ |

---

## 🎯 Target Coverage by Component

| Component | Target | Why |
|-----------|--------|-----|
| Business Logic (Mappers, Services) | 90%+ | Core functionality, high risk |
| Controllers/APIs | 75%+ | Higher-level routing, delegating logic |
| Repository/DAL | 85%+ | Data concerns, important for integrity |
| Utilities/Helpers | 70%+ | Often simple, lower risk |
| Tests themselves | Exclude | Don't measure test code |

---

## 🔍 Finding Uncovered Code

### Method 1: Code Coverage Results Window
```
Test Explorer → Right-click any test
→ Analyze Code Coverage for Selected Tests
→ Code Coverage Results window appears
→ Click "Show Uncovered Code" on red items
```

### Method 2: HTML Report
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Opens ./CoverageReport/index.html in browser
# Shows uncovered lines highlighted in red
```

### Method 3: Visual Highlighting in Editor
```
Tools → Options → Test Tools → Code Coverage
Enable: Highlight uncovered code
→ Gray lines in editor = uncovered
```

---

## ⚙️ Configuration Files

### `coverlet.runsettings`
Controls what gets measured:
- Exclude test assemblies: `[*]*.Tests*`
- Output format: `cobertura` (XML)
- Framework: `.NET 10`

### `coverage-targets.json`
Defines your quality gates:
```json
{
  "coverageTargets": {
	"overall": 80,           // Minimum for whole solution
	"projectSpecific": {
	  "MyStartUpCompany.Api": 85,
	  "MyStartUpCompany.Worker": 80
	}
  }
}
```

---

## 🛠️ Common Tasks

### Run Coverage for Single Project
```powershell
./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests"
```

### Run Coverage for All Projects
```powershell
./scripts/measure-coverage.ps1 -All
```

### Run Coverage + Generate HTML Report
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
```

### Validate Against Threshold
```powershell
./scripts/validate-coverage.ps1
```

### Check Coverage Before Commit
```powershell
./scripts/pre-commit-coverage-check.ps1
```

---

## 📈 Interpreting Results

### Visual Indicators
- 🟢 **Green (>80%)**: Good, keep it up
- 🟡 **Yellow (50-80%)**: Acceptable, could improve
- 🔴 **Red (<50%)**: Critical, add tests
- ⚪ **Gray (0%)**: Not tested at all

### If Coverage is Low
1. Open HTML report: `CoverageReport/index.html`
2. Look for red lines (untested code)
3. Identify the uncovered logic
4. Write tests to cover:
   - Main scenario (happy path)
   - Error cases
   - Boundary conditions
   - Null handling

### Example Gaps
```csharp
// ❌ Uncovered - No tests for edge case
if (string.IsNullOrEmpty(name)) {
	throw new ArgumentException("Name required");  // Not tested!
}

// ✅ Fix - Add test
[Fact]
public void Constructor_WithNullName_ThrowsArgumentException()
{
	Assert.Throws<ArgumentException>(() => new Person(null));
}
```

---

## 💡 Tips & Tricks

### Increase Coverage Quickly
1. **Identify Gaps** → Run report, find red lines
2. **High-Impact First** → Test critical paths first
3. **Use Mocks** → Focus on unit tests (not integration)
4. **Copy Patterns** → Look at existing test examples

### Avoid Common Pitfalls
- ❌ Don't test trivial getters/setters
- ❌ Don't chase 100% coverage
- ❌ Don't write bad tests to hit coverage
- ✅ Do test business logic
- ✅ Do test error paths
- ✅ Do test edge cases

---

## 🔗 Coverage in CI/CD

Your GitHub Actions can automatically:
1. Run tests with coverage collection
2. Generate HTML reports
3. Upload to Codecov
4. Block PRs if coverage drops
5. Add badges to README

See: `.github/workflows/code-coverage.yml`

---

## 📚 More Information

- **Full Guide**: `docs/CODE_COVERAGE_VERIFICATION_GUIDE.md`
- **Measurement Scripts**: `scripts/measure-coverage.ps1`
- **Validation Script**: `scripts/validate-coverage.ps1`
- **Setup Script**: `scripts/setup-coverage-tools.ps1`

---

## ✅ Pre-Push Checklist

Before pushing to `denormalise-test`:

- [ ] All tests pass (Run tests in Test Explorer)
- [ ] Code coverage >= 80% (Run validation script)
- [ ] No new warnings or errors
- [ ] Coverage report reviewed (Check HTML report)
- [ ] Commit message references coverage improvements (if applicable)

**Quick Command:**
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport; ./scripts/validate-coverage.ps1
```

**Result:** Green checkmarks = Safe to push! 🚀
