# Code Coverage Techniques - Visual Decision Guide

## 🎯 Choose Your Technique

```
START HERE
	↓
What's your situation?
	├─ "I need to check coverage RIGHT NOW"
	│  └─→ Technique 1: Test Explorer (30 sec)
	│      Ctrl+E, T → Analyze Code Coverage
	│
	├─ "I want a detailed report to share"
	│  └─→ Technique 2: Coverlet + ReportGenerator (2 min)
	│      ./scripts/measure-coverage.ps1 -All -GenerateReport
	│
	├─ "I need to validate before commit"
	│  └─→ Technique 6: Batch Scripts (2 min)
	│      ./scripts/pre-commit-coverage-check.ps1
	│
	├─ "I want automatic enforcement"
	│  └─→ Technique 7: GitHub Actions (automatic)
	│      Already configured in .github/workflows/
	│
	└─ "I'm an expert and need max control"
	   └─→ Technique 8: OpenCover (5+ min)
		   Advanced configuration required
```

---

## 📊 Quick Comparison Matrix

```
┌──────────────────────┬─────────┬──────────┬────────┬────────────┐
│ Technique            │ Ease    │ Speed    │ Detail │ Best For   │
├──────────────────────┼─────────┼──────────┼────────┼────────────┤
│ 1. Test Explorer     │ ⭐⭐⭐⭐⭐ │ 30sec    │ Low    │ Quick check│
│ 2. Editor Highlight  │ ⭐⭐⭐⭐⭐ │ Instant  │ Low    │ While code│
│ 3. Coverage Window   │ ⭐⭐⭐⭐  │ 1min     │ High   │ Analysis  │
│ 4. Coverlet CLI      │ ⭐⭐⭐   │ 1min     │ High   │ Scripting │
│ 5. Coverlet + Report │ ⭐⭐⭐   │ 2min     │ VHigh  │ Full report
│ 6. Batch Scripts     │ ⭐⭐⭐⭐  │ 2min     │ High   │ Pre-commit│
│ 7. GitHub Actions    │ ⭐⭐    │ 2-5min   │ VHigh  │ CI/CD      │
│ 8. OpenCover         │ ⭐⭐    │ 3-5min   │ VHigh  │ Advanced  │
└──────────────────────┴─────────┴──────────┴────────┴────────────┘
```

---

## 🔄 Typical Developer Workflow

```
Monday Morning - First Setup
├─ Install tools: ./scripts/setup-coverage-tools.ps1
├─ Time: 2 minutes
└─ Result: Ready for all techniques

Throughout Day - While Coding
├─ Write code/tests
├─ Quick check: Ctrl+E, T → Analyze (30 sec)
├─ See coverage highlighting in editor (instant)
└─ Repeat as needed

End of Day - Before Push
├─ Full measurement: ./scripts/measure-coverage.ps1 -All -GenerateReport
├─ Review HTML report
├─ If < 80%: Add more tests
├─ Validate: ./scripts/validate-coverage.ps1
├─ Time: 5-10 minutes
└─ Result: Confidence to push!

After Push - Automatic
├─ GitHub Actions runs automatically
├─ Coverage report generated
├─ PR comments with results
├─ No manual effort needed
└─ Team sees coverage impact
```

---

## 🎯 Pick Your Technique(s)

### Solo Developer (Focused on Quality)
```
DAILY ROUTINE
├─ While coding:   Use Coverage Highlighting
├─ Quick check:    Ctrl+E, T → Analyze  
├─ Before commit:  ./scripts/measure-coverage.ps1 -All -GenerateReport
├─ Validate:       ./scripts/validate-coverage.ps1
└─ Result:         Never push with low coverage
```

### Team Development (Shared Standards)
```
TEAM ROUTINE
├─ Individual:     Test Explorer + Batch Scripts
├─ Team CI/CD:     GitHub Actions (automatic)
├─ Code review:    HTML report as evidence
├─ Trend tracking: Coverage badges in README
└─ Result:         Consistent quality across team
```

### Quality-Focused Team (Strict Enforcement)
```
ENFORCEMENT ROUTINE
├─ Local:          ./scripts/pre-commit-coverage-check.ps1 (must pass)
├─ CI/CD:          GitHub Actions blocks <80% coverage
├─ PR Review:      Requires coverage improvement details
├─ Metrics:        Track coverage trends
└─ Result:         Guaranteed minimum quality standard
```

---

## 📈 Coverage Goals Visualization

```
Your Coverage Target
		│
	100%│     ❌ Unrealistic
		│     (Chasing 100% wastes time)
		│
	 90%│     ✅ Critical paths
		│     (Business logic, mappers, services)
		│
	 85%│ ◄─── 🎯 Aim here for key components
		│
	 80%│ ◄─── 🎯 Overall minimum target
		│      (Test Explorer shows GREEN here)
		│
	 70%│     🟡 Yellow zone
		│     (Acceptable but could improve)
		│
	 50%│     🔴 Red zone
		│     (Critical - add tests)
		│
	  0%│     ⚪ Not tested at all
		│
```

---

## 🔍 Finding Coverage - Visual Guide

### Method 1: Test Explorer (30 seconds)
```
┌─────────────────────────────────────────┐
│ Test Explorer (Ctrl+E, T)              │
├─────────────────────────────────────────┤
│ ✓ MyStartUpCompany.Api.Tests           │ ← Select
│   ✓ MapperTests                        │
│   ✓ ControllerTests                    │
└─────────────────────────────────────────┘
		 ↓
   Right-click ↓
		 ↓
┌─────────────────────────────────────────┐
│ Analyze Code Coverage for Selected... │
└─────────────────────────────────────────┘
		 ↓
		 ↓ (2 seconds)
		 ↓
┌─────────────────────────────────────────┐
│ Code Coverage Results                   │
├─────────────────────────────────────────┤
│ MyStartUpCompany.Api         72% 🟡     │
│ ├─ Mappers                  85% 🟢      │
│ ├─ Controllers              65% 🔴      │
│ └─ Services                 78% 🟡      │
└─────────────────────────────────────────┘
```

### Method 2: HTML Report (2 minutes)
```
PowerShell Command
		 ↓
./scripts/measure-coverage.ps1 -All -GenerateReport
		 ↓ (1-2 minutes)
		 ↓
┌──────────────────────────────────────────────────┐
│ Browser Opens Automatically                      │
├──────────────────────────────────────────────────┤
│                                                  │
│ Code Coverage Report                             │
│ ─────────────────────────────────────           │
│                                                  │
│ Overall: 78% 🟡                                  │
│                                                  │
│ [MyStartUpCompany.Api]         78%  🟡           │
│   Click to expand ↓                              │
│   ├─ Mappers.cs               85%  🟢            │
│   │  Lines 1-50   ✅ Covered                     │
│   │  Lines 51-65  ❌ NOT COVERED ← Red           │
│   ├─ Controllers/              65%  🔴            │
│   └─ Services/                78%  🟡            │
│                                                  │
│ [Actionable] Uncovered lines shown in RED        │
│                                                  │
└──────────────────────────────────────────────────┘
```

---

## 💻 Command Flowchart

```
START
  │
  ├─ Setup? (first time)
  │  ├─ YES → ./scripts/setup-coverage-tools.ps1
  │  │        └─ Done ✓
  │  └─ NO → Continue
  │
  ├─ Need coverage DATA only?
  │  ├─ YES → ./scripts/measure-coverage.ps1 -All
  │  │        (creates XML files, no report)
  │  └─ NO → Continue
  │
  ├─ Need BEAUTIFUL HTML REPORT?
  │  ├─ YES → ./scripts/measure-coverage.ps1 -All -GenerateReport
  │  │        (opens report in browser)
  │  └─ NO → Continue
  │
  ├─ Need PRE-COMMIT VALIDATION?
  │  ├─ YES → ./scripts/pre-commit-coverage-check.ps1
  │  │        (passes/fails automatically)
  │  └─ NO → Continue
  │
  ├─ Need QUICK CHECK in VS?
  │  ├─ YES → Ctrl+E, T → Right-click → Analyze
  │  │        (30 seconds)
  │  └─ NO → Continue
  │
  ├─ Need TO VALIDATE THRESHOLD?
  │  ├─ YES → ./scripts/validate-coverage.ps1
  │  │        (check if >= 80%)
  │  └─ NO → Continue
  │
  └─ Need AUTOMATED ENFORCEMENT?
	 ├─ YES → Push to GitHub
	 │        (GitHub Actions runs automatically)
	 └─ NO → Done ✓

END
```

---

## 📊 Sample Report Output

### Test Explorer Output
```
┌──────────────────────────────────────┐
│ Coverage Results                     │
├──────────────────────────────────────┤
│                                      │
│ MyStartUpCompany.Api      78%        │
│ ├─ Mappers.cs           85% (85/100)│
│ ├─ Controllers.cs       65% (98/150)│
│ └─ Services.cs          78% (112/144│
│                                      │
│ MyStartUpCompany.Worker  72%         │
│ ├─ Handlers.cs          80% (80/100)│
│ └─ Processors.cs        68% (85/125)│
│                                      │
│ MyStartUpCompany.Observability  75%  │
│ ├─ Telemetry.cs         82% (82/100)│
│ └─ Metrics.cs           70% (70/100)│
│                                      │
│ OVERALL                  75%         │
│ ─────────────────────────────────    │
│ ✅ All tests passed                   │
│ ⚠️  Below 80% target - add tests     │
└──────────────────────────────────────┘
```

### HTML Report Visualization
```
┌────────────────────────────────────────────┐
│ CoverageReport/index.html (In Browser)     │
├────────────────────────────────────────────┤
│                                            │
│ Summary                                    │
│ ────────                                   │
│ Line Coverage:    78% 🟡                   │
│ Branch Coverage:  75% 🟡                   │
│ Method Coverage:  82% 🟢                   │
│                                            │
│ By Component                               │
│ ────────────                               │
│ 🟢 Mappers:        85% ████████░░ Good    │
│ 🟡 Controllers:    65% █████░░░░ OK       │
│ 🟡 Services:       78% ███████░░░ OK      │
│ 🔴 Utilities:      45% ████░░░░░░ Low     │
│                                            │
│ Uncovered Code Details                     │
│ ──────────────────────                     │
│ File: MapperA.cs                           │
│ ├─ Line 25: NOT COVERED ← Click for fix    │
│ ├─ Line 40: NOT COVERED                    │
│ └─ Line 52: NOT COVERED                    │
│                                            │
└────────────────────────────────────────────┘
```

---

## ⏱️ Time Estimates

```
First Time (Setup)
├─ Install tools:              2 min
└─ Create config files:        0 min (automatic)
Total: ~2 minutes

Ongoing Use (Per Day)
├─ Quick check (VS):          30 sec
├─ Full measure + report:      2 min
├─ Review report:              3 min
├─ Add tests (if needed):      10+ min
└─ Validate + commit:          1 min
Total: 5-20 minutes depending on coverage gaps

Before Major Push
├─ Full suite measurement:     2 min
├─ Generate HTML report:       1 min
├─ Team review of report:      5 min
└─ Improvement iterations:     10+ min
Total: 15-30 minutes
```

---

## 🎓 Understanding the Numbers

```
What you see        What it means              Action needed
──────────────────────────────────────────────────────────────
95% 🟢              Excellent                  Maintain level
					(Keep doing what you're doing)

82% 🟡              Good                       Optional improvement
					(Acceptable, but could test more)

68% 🔴              Needs work                 Add tests
					(Too many untested paths)

25% ⚪              Critical                   Major testing effort
					(Too little testing)

Coverage = X%
Lines covered = 78
Total lines = 100
					Math: 78 ÷ 100 × 100 = 78%
					Meaning: 78 of 100 lines executed by tests
```

---

## ✅ Pre-Push Checklist

```
BEFORE PUSH TO REPOSITORY

□ Code compiles
  └─ No compilation errors

□ Tests pass
  └─ Ctrl+E, T → Run All → All GREEN ✓

□ Coverage measured
  └─ ./scripts/measure-coverage.ps1 -All -GenerateReport

□ Coverage reviewed
  └─ Opened CoverageReport/index.html
  └─ Understood the red (uncovered) lines

□ Coverage validated
  └─ ./scripts/validate-coverage.ps1
  └─ Result: PASSED ✓ (>= 80%)

□ If failed validation:
  └─ Added tests for critical uncovered code
  └─ Re-ran validation → PASSED ✓

□ Commit message includes coverage info
  └─ "Add tests: improved coverage from X% to Y%"

✅ Ready to push!
   git push origin denormalise-test
```

---

## 🚨 Emergency Quick Reference

**"I need to check coverage RIGHT NOW"**
```powershell
Ctrl+E, T → Right-click test → Analyze Code Coverage
# Result in 30 seconds
```

**"I need a report to show my team"**
```powershell
./scripts/measure-coverage.ps1 -All -GenerateReport
# Beautiful HTML report in 2 minutes
```

**"I need to validate before commit"**
```powershell
./scripts/validate-coverage.ps1
# Pass/Fail in 10 seconds
```

**"All-in-one pre-commit check"**
```powershell
./scripts/pre-commit-coverage-check.ps1
# Runs tests, measures, validates, shows report
```

---

**Choose your technique and GO! 🚀**
