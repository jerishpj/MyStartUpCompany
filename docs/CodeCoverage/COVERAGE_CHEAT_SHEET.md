# Code Coverage Cheat Sheet

## 🚀 Start Here

### The 30-Second Way (Built-in)
```
Ctrl+E, T  →  Select Tests  →  Right-click  →  Analyze Code Coverage
```

### The 2-Minute Way (Best Reports)
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Opens beautiful HTML report automatically
```

### The Pre-Commit Way (Automation)
```powershell
./scripts/pre-commit-coverage-check.ps1
# Passes/fails automatically
```

---

## 📋 All Available Methods

| Method | Command/Action | Time | Output |
|--------|---|---|---|
| **Quick** | `Ctrl+E, T → Analyze` | 30s | Visual window |
| **Report** | `./scripts/measure-coverage.ps1 -All -GenerateReport` | 2m | HTML report |
| **Validate** | `./scripts/validate-coverage.ps1` | 10s | Pass/Fail |
| **Single Project** | `./scripts/measure-coverage.ps1 -Project "ProjectName.Tests"` | 1m | XML + Optional HTML |
| **Raw CLI** | `dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura` | 1m | XML only |
| **Pre-commit** | `./scripts/pre-commit-coverage-check.ps1` | 2m | Pass/Fail + Report |
| **CI/CD** | GitHub Actions (automatic) | 2-5m | PR Comment + Artifacts |

---

## 🎯 Coverage Targets

```
🎯 Overall Target:              80%
🎯 Mappers & Business Logic:    90%+
🎯 Services:                    85%+
🎯 Controllers/APIs:            75%+
🎯 Data Access:                 85%+
🎯 Utilities:                   70%+
```

---

## 📊 Reading Coverage Reports

### Visual Indicators
```
🟢 Green  = 80%+   (Excellent)
🟡 Yellow = 50-80% (OK, could improve)
🔴 Red    = <50%   (Critical, add tests)
⚪ Gray   = 0%     (Not tested at all)
```

### What Each Metric Means
```
Line Coverage:     % of lines executed
Branch Coverage:   % of decision paths (if/else)
Block Coverage:    % of code blocks
Method Coverage:   % of methods called
```

---

## 🔍 Finding Uncovered Code

### Method 1: Visual Studio UI
```
Test Explorer → Right-click test/project
→ Analyze Code Coverage for Selected Tests
→ Code Coverage Results window
→ Click "Show Uncovered Code" on red items
→ Jump to exact lines in editor
```

### Method 2: HTML Report
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Open ./CoverageReport/index.html
# Red lines = uncovered
# Click file → shows exactly which lines need tests
```

### Method 3: Editor Highlighting
```
Tools → Options → Test Tools → Code Coverage
Enable: Highlight uncovered code
→ Run any test
→ Gray/red lines in editor = uncovered
```

---

## 💻 Command Reference

### Setup (Run Once)
```powershell
./scripts/setup-coverage-tools.ps1
```

### Daily/Pre-Push Usage
```powershell
# Quick check with report
./scripts/measure-coverage.ps1 -All -GenerateReport

# Check if it meets standards
./scripts/validate-coverage.ps1

# Pre-commit validation
./scripts/pre-commit-coverage-check.ps1
```

### Measure Individual Projects
```powershell
# Just one project
./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests"

# With report
./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests" -GenerateReport
```

### Raw Commands (No Scripts)
```powershell
# Collect coverage data
dotnet test tests/MyStartUpCompany.Api.Tests `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura

# Generate HTML report
ReportGenerator `
  -reports:"./TestResults/coverage.cobertura.xml" `
  -targetdir:"./CoverageReport" `
  -reporttypes:"Html"
```

---

## 🧪 Test Examples

### ❌ BAD - Not Testing Error Cases
```csharp
[Fact]
public void Mapper_WithValidData_MapsCorrectly()
{
	// Only tests happy path
	var mapper = new SourceAMapper();
	var result = mapper.Map(validMessage);
	Assert.NotNull(result);
}
```

### ✅ GOOD - Testing Multiple Scenarios
```csharp
[Fact]
public void Mapper_WithValidData_MapsCorrectly()
{
	// Happy path
	var mapper = new SourceAMapper();
	var result = mapper.Map(validMessage);
	Assert.NotNull(result);
}

[Fact]
public void Mapper_WithNullInput_ReturnsNull()
{
	// Edge case
	var mapper = new SourceAMapper();
	var result = mapper.Map(null);
	Assert.Null(result);
}

[Fact]
public void Mapper_WithEmptyString_Throws()
{
	// Error case
	var mapper = new SourceAMapper();
	Assert.Throws<ArgumentException>(() => mapper.Map(emptyMessage));
}

[Fact]
public void Mapper_TrimsWhitespace_AndMaps()
{
	// Boundary condition
	var mapper = new SourceAMapper();
	var messageWithWhitespace = new SourceAMessage { Name = "  John  " };
	var result = mapper.Map(messageWithWhitespace);
	Assert.Equal("John", result.Name);
}
```

---

## 📈 Coverage Workflow

### 1️⃣ Write Code
```csharp
public class SourceAMapper
{
	public SourceAEntity Map(SourceAMessage msg)
	{
		if (msg?.Name == null)
			throw new ArgumentNullException(nameof(msg.Name));

		return new SourceAEntity 
		{ 
			Name = msg.Name.Trim() 
		};
	}
}
```

### 2️⃣ Check Coverage
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Opens report showing which lines aren't covered
```

### 3️⃣ Identify Gaps
```
Report shows:
- ✅ Line 5: if (msg?.Name == null) - COVERED
- ✅ Line 6: throw new ArgumentNullException - COVERED
- ✅ Line 8: return new SourceAEntity - COVERED
- ✅ Line 11: msg.Name.Trim() - COVERED
Coverage: 100% ✅
```

### 4️⃣ Add More Tests (if needed)
```csharp
[Theory]
[InlineData("John")]      // Normal case
[InlineData("  Jane  ")]  // With whitespace
[InlineData("X")]         // Single char
public void Mapper_MapsVariouNames(string name)
{
	// Test different inputs
}
```

### 5️⃣ Validate Before Push
```powershell
./scripts/validate-coverage.ps1
# ✅ PASSED: Coverage 95% exceeds threshold (15% above)
# Safe to push!
```

---

## ⚡ Quick Tips

### Make Coverage Higher Faster
1. **Run Test Explorer Coverage** - See what's not covered
2. **Open HTML Report** - Get line-by-line view
3. **Copy Test Pattern** - Look at existing tests in same file
4. **Add Edge Cases** - Test null, empty, boundaries
5. **Run Again** - Validate improvement

### Don't Waste Time On
- ❌ Testing trivial auto-properties
- ❌ Chasing 100% coverage
- ❌ Testing framework/library code
- ❌ Complex integration scenarios (separate tests for that)

### Focus On
- ✅ Business logic (mappers, services)
- ✅ Error handling
- ✅ Edge cases
- ✅ Data validation

---

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| "Report not found" | Run: `./scripts/measure-coverage.ps1 -All -GenerateReport` |
| "Coverage shows 0%" | Rebuild: `dotnet clean; dotnet build` |
| "ReportGenerator not found" | Run: `./scripts/setup-coverage-tools.ps1` |
| "Tests fail during coverage" | Same tests that fail normally - fix those first |
| "Script execution disabled" | Run: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser` |

---

## 📚 Full Documentation

```
Quick Reference:        docs/COVERAGE_QUICK_REFERENCE.md
All Techniques:         docs/COVERAGE_TECHNIQUES_SUMMARY.md
Detailed Guide:         docs/CODE_COVERAGE_VERIFICATION_GUIDE.md
Setup:                  ./scripts/setup-coverage-tools.ps1
Measure:                ./scripts/measure-coverage.ps1
Validate:               ./scripts/validate-coverage.ps1
```

---

## ✅ Pre-Push Checklist

- [ ] Ran tests: `Ctrl+E, T → Run All`
- [ ] Checked coverage: `./scripts/measure-coverage.ps1 -All -GenerateReport`
- [ ] Coverage >= 80%: `./scripts/validate-coverage.ps1`
- [ ] Reviewed HTML report for gaps
- [ ] Added tests for critical uncovered code
- [ ] Committed with message like: "Add tests: improved coverage from 72% to 85%"

**One command to do all of this:**
```powershell
./scripts/pre-commit-coverage-check.ps1
```

---

## 🎓 Learning Resources

- **Video**: "Code Coverage in Visual Studio" on YouTube
- **Docs**: `docs/CODE_COVERAGE_VERIFICATION_GUIDE.md` (comprehensive)
- **CLI Docs**: `dotnet test --help`
- **Coverlet**: https://github.com/coverlet-coverage/coverlet

---

**Start now:** `./scripts/setup-coverage-tools.ps1`
