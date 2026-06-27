# CODE COVERAGE SETUP - START HERE

This document was created on your request to provide comprehensive techniques for verifying unit test coverage in Visual Studio before pushing to your repository.

---

## 📍 You Are Here

Your workspace: `C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany`
Branch: `denormalise-test`
Framework: `.NET 10`
IDE: Visual Studio Community 2026

---

## ⚡ 60-Second Setup

```powershell
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# Run once to setup
./scripts/setup-coverage-tools.ps1

# Then measure
./scripts/measure-coverage.ps1 -All -GenerateReport
```

**Result:** Beautiful HTML coverage report opens automatically! 🎉

---

## 📚 Documentation Created

You now have 4 comprehensive guides:

1. **`docs/COVERAGE_CHEAT_SHEET.md`** ⭐ START HERE
   - One-page quick reference
   - All commands at a glance
   - 5-minute read

2. **`docs/COVERAGE_QUICK_REFERENCE.md`** 
   - Common tasks and metrics
   - Visual indicators explained
   - Finding uncovered code

3. **`docs/COVERAGE_TECHNIQUES_SUMMARY.md`**
   - 8 different techniques explained
   - Comparison table
   - Decision tree

4. **`docs/CODE_COVERAGE_VERIFICATION_GUIDE.md`**
   - Comprehensive 3000+ word guide
   - Every technique with examples
   - CI/CD integration guide
   - Troubleshooting

---

## 🛠️ Automation Scripts Created

All in `scripts/` directory:

1. **`setup-coverage-tools.ps1`** - Setup once
2. **`measure-coverage.ps1`** - Run daily
3. **`validate-coverage.ps1`** - Pre-push validation
4. **`pre-commit-coverage-check.ps1`** - Full workflow

---

## 8️⃣ Coverage Verification Techniques

### Quick (VS Built-in)
```
Ctrl+E, T → Select tests → Right-click → Analyze Code Coverage
```

### Best (Coverlet + ReportGenerator)
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
```

### Automated (Validation)
```powershell
./scripts/validate-coverage.ps1
```

### Pre-Commit (Full Check)
```powershell
./scripts/pre-commit-coverage-check.ps1
```

### CI/CD (GitHub Actions)
```yaml
# Automatically runs on every push/PR
# See: .github/workflows/code-coverage.yml
```

Plus 3 more advanced techniques documented in the guides.

---

## 📊 Coverage Targets

```
Overall:        80%
Mappers/Logic:  90%+
Services:       85%+
Controllers:    75%+
Data Access:    85%+
Utilities:      70%+
```

---

## 🎯 Your First Day Workflow

### Morning: Setup
```powershell
./scripts/setup-coverage-tools.ps1
# Takes 2 minutes, one time only
```

### After Coding: Measure
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Opens beautiful HTML report
# Shows exactly which lines aren't covered
```

### Before Commit: Validate
```powershell
./scripts/validate-coverage.ps1
# Checks if coverage meets 80% target
# Passes/fails automatically
```

### Push Confidently
```powershell
git push origin denormalise-test
# GitHub Actions runs coverage check automatically
```

---

## 📖 What to Read Next

**Choose based on your needs:**

### "I just want to get started"
→ Read: `docs/COVERAGE_CHEAT_SHEET.md` (1 page, 5 min)
→ Run: `./scripts/setup-coverage-tools.ps1`
→ Run: `./scripts/measure-coverage.ps1 -All -GenerateReport`

### "I want to understand all options"
→ Read: `docs/COVERAGE_TECHNIQUES_SUMMARY.md` (comparison of all 8)

### "I need comprehensive reference"
→ Read: `docs/CODE_COVERAGE_VERIFICATION_GUIDE.md` (detailed, 3000+ words)

### "I need quick lookup during work"
→ Keep open: `docs/COVERAGE_QUICK_REFERENCE.md`

---

## 🚀 Common Commands

```powershell
# First time
./scripts/setup-coverage-tools.ps1

# Daily measurement
./scripts/measure-coverage.ps1 -All -GenerateReport

# Pre-push validation
./scripts/validate-coverage.ps1

# One-command pre-commit check
./scripts/pre-commit-coverage-check.ps1

# Measure single project
./scripts/measure-coverage.ps1 -Project "MyStartUpCompany.Api.Tests" -GenerateReport
```

---

## 🎓 Key Concepts

| Concept | Meaning |
|---------|---------|
| **Line Coverage** | % of code lines executed by tests |
| **Branch Coverage** | % of if/else paths taken |
| **Uncovered Code** | Lines highlighted in red - not executed by any test |
| **Coverage Report** | HTML showing exactly what's/isn't tested |
| **Threshold** | Minimum acceptable coverage % (e.g., 80%) |

---

## ✅ Before Pushing to Repository

Checklist:
- [ ] Tests pass: `Ctrl+E, T → Run All`
- [ ] Coverage measured: `./scripts/measure-coverage.ps1 -All -GenerateReport`
- [ ] Report reviewed: Checked `CoverageReport/index.html`
- [ ] Validation passed: `./scripts/validate-coverage.ps1`
- [ ] Uncovered code understood: Identified gaps
- [ ] New tests added: For critical uncovered code (if needed)

**One command for all:** `./scripts/pre-commit-coverage-check.ps1`

---

## 🔍 Finding Uncovered Code

3 ways:

1. **Visual Studio UI** - Fastest
   ```
   Test Explorer → Right-click → Analyze Coverage
   → Code Coverage Results window → Show Uncovered Code
   ```

2. **HTML Report** - Most detailed
   ```
   CoverageReport/index.html → Search files
   → Red lines = uncovered
   → Click to see exact code
   ```

3. **Editor Highlighting** - While coding
   ```
   Tools → Options → Test Tools → Code Coverage
   Enable highlighting → Run tests → See gray/red lines
   ```

---

## 🛠️ Configuration Files

Already created for you:

- `coverlet.runsettings` - Coverage collection config
- `coverage-targets.json` - Coverage goals
- `.github/workflows/code-coverage.yml` - GitHub Actions

No additional setup needed!

---

## 💡 Tips for Success

1. **Run before every commit** - Make it routine
2. **Focus on critical paths** - Not 100%, but 80%+
3. **Use HTML reports** - Easiest to understand
4. **Copy test patterns** - Look at existing tests
5. **Test edge cases** - Not just happy path
6. **Share reports** - Team communication
7. **Set alarms** - Don't let coverage drop
8. **Celebrate progress** - Track improvements

---

## 🆘 Troubleshooting

| Problem | Fix |
|---------|-----|
| Scripts won't run | `Set-ExecutionPolicy RemoteSigned -Scope CurrentUser` |
| Tests fail | Fix them first (same way as without coverage) |
| Coverage 0% | Rebuild: `dotnet clean; dotnet build` |
| Report missing | Check: `CoverageReport/` directory exists |
| Tools not found | Run: `./scripts/setup-coverage-tools.ps1` |

---

## 📞 Quick Questions

**Q: Where's the coverage report?**
→ `CoverageReport/index.html` (after running measure script)

**Q: How often should I check coverage?**
→ Before every commit (2 minute habit)

**Q: What's a good coverage target?**
→ 80% overall, 90%+ for business logic

**Q: Will GitHub Actions enforce coverage?**
→ Yes, it validates on every push/PR

**Q: Do I need to understand all 8 techniques?**
→ No, most people just use: Test Explorer + Measure Script

---

## 🎯 Right Now, Go Do This

```powershell
# 1. Navigate to project
cd C:\Jerish\Lab-POC\SharedEFMigrations\MyStartUpCompany

# 2. Setup tools (one-time, 2 min)
./scripts/setup-coverage-tools.ps1

# 3. Measure coverage (2 min)
./scripts/measure-coverage.ps1 -All -GenerateReport

# 4. Review report
# (Opens automatically in browser)

# 5. Read quick reference
Get-Content docs/COVERAGE_CHEAT_SHEET.md
```

**Time to complete:** ~10 minutes
**Result:** You'll understand exactly which code is/isn't tested ✅

---

## 📍 Navigation

All files created:

```
docs/
├─ COVERAGE_CHEAT_SHEET.md              ← Start here!
├─ COVERAGE_QUICK_REFERENCE.md
├─ COVERAGE_TECHNIQUES_SUMMARY.md
├─ CODE_COVERAGE_VERIFICATION_GUIDE.md
└─ COVERAGE_START_HERE.md               ← You are reading this

scripts/
├─ setup-coverage-tools.ps1             ← Run once
├─ measure-coverage.ps1                 ← Use daily
├─ validate-coverage.ps1                ← Pre-push
└─ pre-commit-coverage-check.ps1        ← Full workflow

.github/workflows/
└─ code-coverage.yml                    ← Automatic on push/PR
```

---

## ✨ What You Can Now Do

✅ Measure code coverage with Visual Studio
✅ Find uncovered code lines
✅ Generate beautiful HTML reports
✅ Validate against thresholds
✅ Automate pre-commit checks
✅ Integrate with GitHub Actions
✅ Share reports with team
✅ Track coverage trends
✅ Improve code quality
✅ Push with confidence

---

**You're all set! 🚀**

Start with: `docs/COVERAGE_CHEAT_SHEET.md` (5 min read)
Then run: `./scripts/setup-coverage-tools.ps1`
