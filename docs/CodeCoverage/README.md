# Code Coverage Documentation & Scripts

This folder contains all documentation and resources for measuring and verifying unit test code coverage in the MyStartUpCompany project.

---

## 📂 Folder Structure

```
docs/CodeCoverage/
├── README.md (this file)
│
├── 📚 Documentation/
│   ├── COVERAGE_START_HERE.md                ⭐ Start here!
│   ├── COVERAGE_CHEAT_SHEET.md               Quick reference (1 page)
│   ├── COVERAGE_QUICK_REFERENCE.md           Essential metrics
│   ├── COVERAGE_TECHNIQUES_SUMMARY.md        Compare all 8 techniques
│   ├── CODE_COVERAGE_VERIFICATION_GUIDE.md   Comprehensive guide (3000+ words)
│   ├── COVERAGE_VISUAL_GUIDE.md              Visual diagrams & flowcharts
│   └── COVERAGE_DOCUMENTATION_INDEX.md       Complete navigation
│
└── ⚙️ Configuration/
	├── coverlet.runsettings                  Coverage collection config
	└── coverage-targets.json                 Coverage threshold targets

scripts/coverage/
├── setup-coverage-tools.ps1                  Setup tools (run once)
├── measure-coverage.ps1                      Measure coverage (daily use)
├── validate-coverage.ps1                     Validate thresholds (pre-push)
└── pre-commit-coverage-check.ps1             Full workflow (before commit)
```

---

## 🚀 Quick Start

### 1. Read This First (5 minutes)
```
docs/CodeCoverage/COVERAGE_START_HERE.md
```

### 2. Setup Tools (2 minutes - one time only)
```powershell
./scripts/coverage/setup-coverage-tools.ps1
```

### 3. Measure Coverage (2 minutes - daily)
```powershell
./scripts/coverage/measure-coverage.ps1 -All -GenerateReport
```

### 4. Review Report
- Beautiful HTML report opens automatically
- Red lines = uncovered code
- Green = well covered

---

## 📚 Documentation Guide

### For Quick Answers
👉 **`COVERAGE_CHEAT_SHEET.md`** (1 page)
- All commands at a glance
- Coverage targets
- Common tasks
- Troubleshooting table

### For Getting Started
👉 **`COVERAGE_START_HERE.md`** (2 pages)
- 60-second setup
- First-day workflow
- Quick Q&A

### For Essential Knowledge
👉 **`COVERAGE_QUICK_REFERENCE.md`** (2 pages)
- Coverage metrics explained
- Visual indicators
- Finding uncovered code
- Coverage targets by component

### For Understanding All Options
👉 **`COVERAGE_TECHNIQUES_SUMMARY.md`** (5 pages)
- 8 different techniques compared
- Pros/cons for each
- Recommended workflows
- Decision tree

### For Comprehensive Learning
👉 **`CODE_COVERAGE_VERIFICATION_GUIDE.md`** (12 pages)
- Every technique with examples
- Step-by-step instructions
- CLI commands reference
- Troubleshooting guide
- Best practices

### For Visual Learners
👉 **`COVERAGE_VISUAL_GUIDE.md`** (6 pages)
- Decision flowchart
- Workflow diagrams
- Visual comparisons
- Sample outputs

### For Navigation
👉 **`COVERAGE_DOCUMENTATION_INDEX.md`**
- Complete file map
- What to read for each scenario
- Quick help reference

---

## 🛠️ Available Scripts

### `setup-coverage-tools.ps1`
- **When:** First time only
- **Duration:** 2 minutes
- **Does:** Installs tools, creates config files, sets up directories
```powershell
./scripts/coverage/setup-coverage-tools.ps1
```

### `measure-coverage.ps1` ⭐ Main Script
- **When:** Daily, before commits
- **Duration:** 1-3 minutes
- **Does:** Runs tests, collects coverage, generates HTML report
```powershell
# All projects with report
./scripts/coverage/measure-coverage.ps1 -All -GenerateReport

# Single project
./scripts/coverage/measure-coverage.ps1 -Project "ProjectName.Tests" -GenerateReport
```

### `validate-coverage.ps1`
- **When:** Pre-push validation
- **Duration:** 10 seconds
- **Does:** Checks if coverage meets 80% threshold
```powershell
./scripts/coverage/validate-coverage.ps1
```

### `pre-commit-coverage-check.ps1` 🎯 All-in-One
- **When:** Right before committing
- **Duration:** 2-3 minutes
- **Does:** Full workflow - tests, coverage, validation, report
```powershell
./scripts/coverage/pre-commit-coverage-check.ps1
```

---

## 🎯 Coverage Targets

| Component | Target | Importance |
|-----------|--------|-----------|
| Overall Solution | 80%+ | Minimum standard |
| Business Logic (Mappers, Services) | 90%+ | Critical |
| Controllers/APIs | 75%+ | Important |
| Data Access (Repositories) | 85%+ | Important |
| Utilities/Helpers | 70%+ | Lower priority |

---

## 📊 Visual Indicators

```
🟢 Green (80%+):     Excellent - maintain this level
🟡 Yellow (50-80%):  Acceptable - could improve
🔴 Red (<50%):       Critical - add tests urgently
⚪ Gray (0%):        Not tested - highest priority
```

---

## ✅ Pre-Push Checklist

Before pushing to repository:

- [ ] Read: `COVERAGE_START_HERE.md`
- [ ] Setup: `./scripts/coverage/setup-coverage-tools.ps1` (once)
- [ ] Measure: `./scripts/coverage/measure-coverage.ps1 -All -GenerateReport`
- [ ] Review: `CoverageReport/index.html`
- [ ] Validate: `./scripts/coverage/validate-coverage.ps1`
- [ ] If < 80%: Add tests, then re-run validation
- [ ] Commit with coverage info: `"Tests: improved coverage from X% to Y%"`

---

## 8️⃣ Coverage Verification Techniques

You have **8 different techniques** available:

1. **Test Explorer** (30 sec) - Easiest, built-in
2. **Editor Highlighting** (Instant) - Visual while coding
3. **Coverage Results Window** (1 min) - Detailed analysis
4. **Coverlet CLI** (1 min) - Command line
5. **Coverlet + ReportGenerator** (2 min) - **Best reports** ⭐
6. **Batch Scripts** (2 min) - Automated
7. **GitHub Actions** (Auto) - CI/CD enforcement
8. **OpenCover** (5 min) - Advanced users

**See:** `COVERAGE_TECHNIQUES_SUMMARY.md` for detailed comparison

---

## 🔍 Finding Uncovered Code

### Method 1: HTML Report (Easiest)
```powershell
./scripts/coverage/measure-coverage.ps1 -All -GenerateReport
# Open: CoverageReport/index.html
# Red lines = uncovered
```

### Method 2: Visual Studio UI
```
Test Explorer → Right-click test → Analyze Code Coverage
→ Code Coverage Results window → Show Uncovered Code
```

### Method 3: Editor Highlighting
```
Tools → Options → Test Tools → Code Coverage
Enable: Highlight uncovered code
→ Run tests → See gray/red lines in editor
```

---

## 📈 Your Solution Structure

Test projects in your solution:

```
tests/
├── MyStartUpCompany.Api.Tests               [Target: 85%]
├── MyStartUpCompany.Worker.Tests            [Target: 80%]
└── MyStartUpCompany.Observability.Tests     [Target: 75%]
```

**Measure each project:**
```powershell
./scripts/coverage/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests" -GenerateReport
./scripts/coverage/measure-coverage.ps1 -Project "MyStartUpCompany.Worker.Tests" -GenerateReport
./scripts/coverage/measure-coverage.ps1 -Project "MyStartUpCompany.Observability.Tests" -GenerateReport
```

---

## 🆘 Troubleshooting

| Issue | Solution |
|-------|----------|
| Scripts won't run | `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| Coverage shows 0% | Rebuild: `dotnet clean; dotnet build` |
| Can't find report | Check: `CoverageReport/` directory |
| Tools not installed | Run: `./scripts/coverage/setup-coverage-tools.ps1` |
| Tests fail | Fix tests first (same way as without coverage) |

**More help:** See troubleshooting sections in individual docs

---

## 🎓 Key Concepts

### Line Coverage
Percentage of code lines that were executed by your tests.

### Branch Coverage
Percentage of decision paths (if/else statements) that were taken.

### Uncovered Code
Code lines that were never executed by any test - shown in red.

### Coverage Report
Visual HTML document showing exactly what is/isn't tested.

### Threshold
Minimum acceptable coverage percentage (default: 80%).

---

## 💡 Best Practices

✅ DO:
- Run coverage before every commit
- Focus on business logic first
- Test error paths and edge cases
- Review coverage trends over time
- Share reports with team

❌ DON'T:
- Chase 100% coverage (unrealistic)
- Test trivial getters/setters
- Write bad tests just to hit numbers
- Ignore uncovered critical code

---

## 📞 Quick Reference

**Fastest check (30 seconds):**
```
Ctrl+E, T → Right-click → Analyze Code Coverage
```

**Best reports (2 minutes):**
```powershell
./scripts/coverage/measure-coverage.ps1 -All -GenerateReport
```

**Pre-commit check (2 minutes):**
```powershell
./scripts/coverage/pre-commit-coverage-check.ps1
```

**Validate threshold (10 seconds):**
```powershell
./scripts/coverage/validate-coverage.ps1
```

---

## 📚 Next Steps

1. **Start:** Read `COVERAGE_START_HERE.md`
2. **Setup:** Run `./scripts/coverage/setup-coverage-tools.ps1`
3. **Measure:** Run `./scripts/coverage/measure-coverage.ps1 -All -GenerateReport`
4. **Review:** Open `CoverageReport/index.html`
5. **Learn:** Check other docs as needed

---

## 📝 File Locations

- **Documentation:** `docs/CodeCoverage/`
- **Scripts:** `scripts/coverage/`
- **Configuration:** `docs/CodeCoverage/`
- **Reports Generated:** `CoverageReport/` (created during measurement)

---

**Ready to measure coverage?** Start with `COVERAGE_START_HERE.md` 🚀
