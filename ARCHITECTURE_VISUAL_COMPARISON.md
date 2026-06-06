# Architecture Comparison: Visual Guide

**Type:** Visual Architecture Reference  
**Purpose:** Side-by-side comparison of mapping strategies  
**Audience:** Technical leads, architects, development teams  

---

## 1. Current Architecture: Centralized Mapping

### Structure Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      API Project                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Features/                                                   │
│  ├── CompanyDetails/                                        │
│  │   ├── CompanyController.cs                              │
│  │   ├── Models/                                           │
│  │   │   └── CompanyResponse.cs                            │
│  │   └── Queries/                                          │
│  │       ├── GetAllCompaniesQueryHandler.cs ──┐            │
│  │       ├── GetCompanyQueryHandler.cs ────┐  │            │
│  │       └── GetFilteredCompaniesQueryHandler.cs ──┐       │
│  │                                              │  │        │
│  ├── Locations/                                │  │        │
│  │   ├── LocationsController.cs               │  │        │
│  │   ├── Models/                              │  │        │
│  │   │   └── LocationResponse.cs              │  │        │
│  │   └── Queries/                             │  │        │
│  │       ├── GetAllLocationsQueryHandler.cs ──┤  │        │
│  │       ├── GetLocationQueryHandler.cs ────┤  │        │
│  │       └── GetFilteredLocationsQueryHandler.cs ┤        │
│  │                                           │  │        │
│  ├── Buildings/                              │  │        │
│  │   ├── BuildingsController.cs              │  │        │
│  │   ├── Models/                             │  │        │
│  │   │   └── BuildingResponse.cs             │  │        │
│  │   └── Queries/                            │  │        │
│  │       └── Get*BuildingQueryHandler.cs ────┤  │        │
│  │                                            │  │        │
│  ├── Offices/                                 │  │        │
│  │   ├── OfficesController.cs                │  │        │
│  │   ├── Models/                             │  │        │
│  │   │   └── OfficeResponse.cs               │  │        │
│  │   └── Queries/                            │  │        │
│  │       └── Get*OfficeQueryHandler.cs ──────┤  │        │
│  │                                            │  │        │
│  ├── Projects/                                │  │        │
│  │   ├── ProjectController.cs                │  │        │
│  │   ├── Models/                             │  │        │
│  │   │   └── ProjectResponse.cs              │  │        │
│  │   └── Queries/                            │  │        │
│  │       └── Get*ProjectQueryHandler.cs ─────┤  │        │
│  │                                            │  │        │
│  └── Shared/  ◄────────────────────────────────┴──┴─────┐  │
│      ├── MappingExtensions.cs ◄──────────────────────────┴─┐│
│      │   ├── ProjectToCompanyResponse()                    ││
│      │   ├── ProjectToLocationResponse()                   ││
│      │   ├── ProjectToBuildingResponse()                   ││
│      │   ├── ProjectToOfficeResponse()                     ││
│      │   ├── ProjectToProjectResponse()                    ││
│      │   └── [All MapTo*Response() methods]                ││
│      ├── ModelBinders/                                     ││
│      ├── Constants/                                        ││
│      └── Exceptions/                                       ││
│                                                            ││
└────────────────────────────────────────────────────────────┘│
															   │
		 ◄─────────── CROSS-FEATURE COUPLING ─────────────────┘

ISSUES WITH THIS DESIGN:
❌ All features depend on Features/Shared/MappingExtensions
❌ Changes to one mapping affect all features
❌ Cannot deploy features independently
❌ Shared ownership problems
❌ Violates vertical slice principle
```

### Dependency Graph

```
CompanyDetails Feature
├── Depends on: MappingExtensions (shared)
├── Depends on: ValidationConstants (shared)
└── Depends on: Shared/Exceptions (legitimate)

Locations Feature  
├── Depends on: MappingExtensions (shared) ◄─── PROBLEM
├── Depends on: ValidationConstants (shared)
└── Depends on: Shared/Exceptions (legitimate)

Buildings Feature
├── Depends on: MappingExtensions (shared) ◄─── PROBLEM
├── Depends on: ValidationConstants (shared)
└── Depends on: Shared/Exceptions (legitimate)

Projects Feature
├── Depends on: MappingExtensions (shared) ◄─── PROBLEM
├── Depends on: ValidationConstants (shared)
└── Depends on: Shared/Exceptions (legitimate)

Shared/MappingExtensions
└── Depends on: ALL DTO models from ALL features
	├── CompanyDetails.Models.CompanyResponse
	├── Locations.Models.LocationResponse
	├── Buildings.Models.BuildingResponse
	├── Offices.Models.OfficeResponse
	└── Projects.Models.ProjectResponse

╔════════════════════════════════════════════╗
║ RESULT: EVERYTHING DEPENDS ON EVERYTHING  ║
║ (Hidden circular coupling through Shared)  ║
╚════════════════════════════════════════════╝
```

### Data Flow

```
Request → Handler → Local Code → MappingExtensions → Database
								  (Shared location)
								  ▲
								  │
				┌─────────────────┼─────────────────┐
				│                 │                 │
		 CompanyHandler    LocationHandler    ProjectHandler
				│                 │                 │
				└─────────────────┼─────────────────┘
								  │
						All handlers import
						from same Shared class
```

---

## 2. Recommended Architecture: Feature-Scoped Mapping

### Structure Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                      API Project                             │
├─────────────────────────────────────────────────────────────┤
│                                                              │
│  Features/                                                   │
│  ├── CompanyDetails/ ◄─── VERTICAL SLICE (Self-contained)  │
│  │   ├── CompanyController.cs                              │
│  │   ├── Models/                                           │
│  │   │   └── CompanyResponse.cs                            │
│  │   └── Queries/                                          │
│  │       ├── CompanyMappingExtensions.cs (INTERNAL) ◄──────┤
│  │       │   ├── ProjectToCompanyResponse()                │
│  │       │   └── MapToCompanyResponse()                    │
│  │       ├── GetAllCompaniesQueryHandler.cs                │
│  │       ├── GetCompanyQueryHandler.cs                     │
│  │       └── GetFilteredCompaniesQueryHandler.cs           │
│  │                                                          │
│  ├── Locations/ ◄─── VERTICAL SLICE (Self-contained)      │
│  │   ├── LocationsController.cs                           │
│  │   ├── Models/                                          │
│  │   │   └── LocationResponse.cs                          │
│  │   └── Queries/                                         │
│  │       ├── LocationMappingExtensions.cs (INTERNAL) ◄───┤
│  │       │   ├── ProjectToLocationResponse()              │
│  │       │   └── MapToLocationResponse()                  │
│  │       ├── GetAllLocationsQueryHandler.cs               │
│  │       ├── GetLocationQueryHandler.cs                   │
│  │       └── GetFilteredLocationsQueryHandler.cs          │
│  │                                                         │
│  ├── Buildings/ ◄─── VERTICAL SLICE (Self-contained)     │
│  │   ├── BuildingsController.cs                          │
│  │   ├── Models/                                         │
│  │   │   └── BuildingResponse.cs                         │
│  │   └── Queries/                                        │
│  │       ├── BuildingMappingExtensions.cs (INTERNAL) ◄──┤
│  │       │   ├── ProjectToBuildingResponse()              │
│  │       │   └── MapToBuildingResponse()                  │
│  │       └── Get*BuildingQueryHandler.cs                  │
│  │                                                        │
│  ├── Offices/ ◄─── VERTICAL SLICE (Self-contained)      │
│  │   ├── OfficesController.cs                           │
│  │   ├── Models/                                        │
│  │   │   └── OfficeResponse.cs                          │
│  │   └── Queries/                                       │
│  │       ├── OfficeMappingExtensions.cs (INTERNAL) ◄───┤
│  │       │   ├── ProjectToOfficeResponse()               │
│  │       │   └── MapToOfficeResponse()                   │
│  │       └── Get*OfficeQueryHandler.cs                   │
│  │                                                       │
│  ├── Projects/ ◄─── VERTICAL SLICE (Self-contained)    │
│  │   ├── ProjectController.cs                          │
│  │   ├── Models/                                       │
│  │   │   └── ProjectResponse.cs                        │
│  │   └── Queries/                                      │
│  │       ├── ProjectMappingExtensions.cs (INTERNAL) ◄─┤
│  │       │   ├── ProjectToProjectResponse()             │
│  │       │   └── MapToProjectResponse()                 │
│  │       └── Get*ProjectQueryHandler.cs                │
│  │                                                     │
│  └── Shared/  ◄────── Infrastructure Only              │
│      ├── ModelBinders/                                 │
│      ├── Constants/                                    │
│      ├── Exceptions/ (Legitimate cross-feature)        │
│      └── Filters/ (Legitimate cross-feature)           │
│                                                        │
└────────────────────────────────────────────────────────┘

BENEFITS OF THIS DESIGN:
✅ Each feature self-contained (Vertical Slice)
✅ Feature mappings private (internal) to the slice
✅ Features can be deployed independently
✅ Clear ownership (Feature team owns its slice)
✅ Aligns with vertical slice principle
✅ Easy to extract to microservice later
```

### Dependency Graph

```
CompanyDetails Feature (Independent)
├── Internal: CompanyMappingExtensions
├── Depends on: ValidationConstants (legitimate shared)
└── Depends on: Shared/Exceptions (legitimate shared)

Locations Feature (Independent)
├── Internal: LocationMappingExtensions
├── Depends on: ValidationConstants (legitimate shared)
└── Depends on: Shared/Exceptions (legitimate shared)

Buildings Feature (Independent)
├── Internal: BuildingMappingExtensions
├── Depends on: ValidationConstants (legitimate shared)
└── Depends on: Shared/Exceptions (legitimate shared)

Projects Feature (Independent)
├── Internal: ProjectMappingExtensions
├── Depends on: ValidationConstants (legitimate shared)
└── Depends on: Shared/Exceptions (legitimate shared)

Shared (Infrastructure only)
├── Constants (Legitimate cross-cutting)
├── Exceptions (Legitimate cross-cutting)
├── Filters (Legitimate cross-cutting)
└── ModelBinders (Legitimate cross-cutting)

╔════════════════════════════════════════════╗
║ RESULT: CLEAN DEPENDENCIES                 ║
║ Features depend on infrastructure only     ║
║ No domain coupling between features        ║
╚════════════════════════════════════════════╝
```

### Data Flow

```
Request → Handler → Local MappingExtensions → Database
					 (Feature-scoped)

┌──────────────────────────────────────┐
│   Each Feature Independent            │
│   CompanyDetails                       │
│   ├── Uses local CompanyMapping        │
│   ├── (no dependency on other mapping) │
│   └── Can be deployed independently    │
└──────────────────────────────────────┘

┌──────────────────────────────────────┐
│   Each Feature Independent            │
│   Locations                           │
│   ├── Uses local LocationMapping       │
│   ├── (no dependency on other mapping) │
│   └── Can be deployed independently    │
└──────────────────────────────────────┘
```

---

## 3. Side-by-Side Comparison

### Scenario: Change Company Mapping Logic

#### Current (Centralized)

```
Change Request: "Update company address handling"

Step 1: Modify Features/Shared/MappingExtensions.cs
		└─ Line 35: Change address projection logic

Step 2: Who else uses this?
		├─ CompanyDetails handlers (expected)
		├─ But MappingExtensions is public...
		├─ Could be used by Admin feature
		├─ Could be used by other features
		└─ Need to check entire codebase!

Step 3: Rebuild and test
		├─ Run all tests (everything depends on Shared)
		├─ Regression risk: HIGH
		└─ Test impact: Company + Location + Building + Office + Project tests

Step 4: Risk Assessment
		❌ HIGH - Shared class affects all features
		❌ MEDIUM - One mapping change might break others
		❌ LOW - Onboarding new devs on impact scope
```

#### Recommended (Feature-Scoped)

```
Change Request: "Update company address handling"

Step 1: Modify Features/CompanyDetails/Queries/CompanyMappingExtensions.cs
		└─ Line 35: Change address projection logic
		└─ INTERNAL class, only used in CompanyDetails

Step 2: Who else uses this?
		├─ Only CompanyDetails handlers (obvious)
		├─ Extension is internal (compile-time check)
		└─ No risk of accidental use elsewhere

Step 3: Rebuild and test
		├─ Run CompanyDetails tests only
		├─ Regression risk: NONE (only this feature affected)
		└─ Test impact: CompanyDetails tests only

Step 4: Risk Assessment
		✅ LOW - Change isolated to one feature
		✅ LOW - Other features unaffected
		✅ EASY - Onboarding new devs (obvious scope)
```

---

## 4. Scale Comparison

### Feature Growth Scenario

#### Current Design (Centralized): As More Features Added

```
Features/Shared/MappingExtensions.cs (Growth over time)

Month 1: 321 lines (5 features)
Month 2: 380 lines (6 features)
Month 3: 450 lines (7 features)
Month 6: 620 lines (10 features)
Month 12: 950 lines (15 features)
Month 24: 1,400+ lines (20+ features)

📈 GROWTH TREND: +50-60 lines per new feature

PROBLEMS:
❌ File becomes too large to maintain
❌ Hard to find specific mapping
❌ Increased merge conflicts
❌ Cognitive load on developers
❌ Performance: Reflection/compilation of huge class
❌ Testing: ALL tests depend on this file
```

#### Recommended Design (Feature-Scoped): As More Features Added

```
Features/Feature1/Queries/Feature1MappingExtensions.cs (50 lines)
Features/Feature2/Queries/Feature2MappingExtensions.cs (50 lines)
Features/Feature3/Queries/Feature3MappingExtensions.cs (50 lines)
...
Features/FeatureN/Queries/FeatureNMappingExtensions.cs (50 lines)

📈 GROWTH TREND: Each feature = isolated 50 lines

BENEFITS:
✅ Each file stays small (50-60 lines max)
✅ Easy to locate specific mapping
✅ No merge conflicts in shared file
✅ Clear ownership (one team per feature)
✅ Linear scalability
✅ Independent feature evolution
```

---

## 5. Microservices Extraction: Future View

### Scenario: Extract CompanyDetails to Microservice

#### Current Architecture (Centralized Mapping)

```
MONOLITH (Current)
├── Features/CompanyDetails/
│   ├── Models/CompanyResponse
│   ├── Queries/GetAllCompaniesQueryHandler
│   └── Controllers/CompanyController
├── Features/Shared/
│   └── MappingExtensions
│       ├── ProjectToCompanyResponse() ◄─ PART OF MONOLITH
│       ├── ProjectToLocationResponse()
│       ├── ProjectToBuildingResponse()
│       └── [Other mappings]
└── Database (Shared)

EXTRACTION PROBLEM:
❌ Can't extract CompanyDetails alone
❌ Mapping code is in shared/monolith
❌ Locations/Buildings mappings mixed in same file
❌ Must extract entire MappingExtensions class
❌ Creates duplication in multiple services
❌ Hard to maintain consistency across services
```

#### Recommended Architecture (Feature-Scoped Mapping)

```
MONOLITH (Before extraction)
├── Features/CompanyDetails/
│   ├── Models/CompanyResponse
│   ├── Queries/
│   │   ├── CompanyMappingExtensions ◄─ SELF-CONTAINED
│   │   └── GetAllCompaniesQueryHandler
│   └── Controllers/CompanyController
├── Features/Locations/
│   ├── Models/LocationResponse
│   ├── Queries/
│   │   ├── LocationMappingExtensions ◄─ SELF-CONTAINED
│   │   └── Get*LocationQueryHandler
│   └── Controllers/LocationController
└── Database (Shared)

EXTRACTION BENEFIT:
✅ Can extract CompanyDetails feature directly
✅ Mapping code goes with feature
✅ No shared mapping code to split
✅ Creates CompanyDetails Microservice:
   └── Features/CompanyDetails/ (all of it)
	   ├── Models/
	   ├── Queries/
	   │   ├── CompanyMappingExtensions ◄─ TRAVELS WITH FEATURE
	   │   └── Handlers
	   └── Controllers/

RESULT:
✅ Clean extraction
✅ Feature becomes independent service
✅ Mapping logic travels with code
✅ Easy migration path
```

---

## 6. Code Review Perspective

### Review Comment: Centralized Mapping

```
REVIEW COMMENT:
❌ "Why do we have a 321-line MappingExtensions class?
   This violates our vertical slice architecture.
   Each feature should own its mappings.

   Impact:
   - All features depend on this shared class
   - Hard to evolve feature independently
   - Blocks future microservices extraction
   - Creates single point of failure

   Recommendation:
   - Move mappings to feature-scoped extensions
   - Keep extension methods for performance
   - Make mappings internal to features
   - Estimated effort: 2-4 hours

   Blocker: NO (performance is good)
   Nice-to-have: NO (this is best practice)
   Should-do: YES (foundational architecture)"

RESPONSE EXPECTATIONS:
Author: "Understood. We'll refactor in next sprint."
Reviewer: "Great. We have a migration guide prepared."
```

### Review Comment: Feature-Scoped Mapping

```
REVIEW COMMENT:
✅ "Great! I see mappings are scoped to features now.

   Benefits verified:
   - Each feature owns its mappings ✅
   - Internal visibility ensures proper encapsulation ✅
   - Database-level projections for performance ✅
   - No cross-feature coupling ✅
   - Aligns with vertical slice architecture ✅

   Questions:
   - Is the namespace correct? (Yes ✅)
   - Have you tested performance? (Yes, same SQL ✅)
   - Do all handlers use local mapping? (Yes ✅)

   APPROVED"
```

---

## 7. Team Onboarding Flowchart

### Centralized Mapping (Current)

```
NEW DEVELOPER JOINS
│
├─ Week 1: Learn project structure
│  └─ "Where do I put new mapping code?"
│     ├─ Look at Features/Shared/MappingExtensions.cs
│     └─ "It's a 321-line file... why is everything here?"
│
├─ Week 2: First feature implementation
│  └─ Add new entity mapping
│     ├─ "Which section? Bottom?"
│     ├─ "Should I notify other teams?"
│     └─ "Will my change break anything?"
│
├─ Week 3: First bug
│  └─ "Company mapping changed, broke Location feature"
│     ├─ "But I only touched Company code..."
│     ├─ "Oh, the mapping file is shared..."
│     └─ "Now I understand the coupling problem"
│
├─ Month 2: Frustrated
│  └─ "Why can't we extract Company to microservice?"
│     ├─ "Because mapping code is in shared file"
│     └─ "Let's refactor to feature-scoped"
│
└─ Result: Knowledge of architecture pain points
   Time: 2+ months to understand full implications
```

### Feature-Scoped Mapping (Recommended)

```
NEW DEVELOPER JOINS
│
├─ Week 1: Learn project structure
│  └─ "Where do I put new mapping code?"
│     ├─ Look at Features/CompanyDetails/Queries/
│     ├─ See: CompanyMappingExtensions.cs
│     └─ "Ah! Each feature has its own mapping class ✅"
│
├─ Week 2: First feature implementation
│  └─ Add new entity mapping
│     ├─ "I add a new method to feature's mapping class ✅"
│     ├─ "No need to notify other teams ✅"
│     └─ "My change only affects this feature ✅"
│
├─ Week 3: First bug
│  └─ "Company mapping changed, Company feature works"
│     ├─ "Location feature unaffected ✅"
│     ├─ "Other features unaffected ✅"
│     └─ "Architecture makes sense now"
│
├─ Month 2: Understanding microservices
│  └─ "Can we extract Company to microservice?"
│     ├─ "Yes! Just take the entire Feature/CompanyDetails/ folder ✅"
│     └─ "Everything needed is self-contained ✅"
│
└─ Result: Quickly understands architecture
   Time: 1 week to grasp full implications
```

---

## 8. Migration Path Visualization

```
CURRENT STATE
┌─────────────────────────────────┐
│ Features/Shared/               │
│ MappingExtensions.cs (321 lines)│
│ - Company mapping              │
│ - Location mapping             │
│ - Building mapping             │
│ - Office mapping               │
│ - Project mapping              │
└─────────────────────────────────┘
		▲
		│ All features depend
		│
	┌───┴───┬───────┬─────────┬──────────┐
	│       │       │         │          │
CompanyDetails Locations Buildings Offices Projects
Features  Features  Features Features Features

						↓

		   MIGRATION (2-4 hours)

						↓

TARGET STATE
┌─────────────────────────────────────────────────────────┐
│ Features/CompanyDetails/Queries/                        │
│ CompanyMappingExtensions.cs (internal)                 │
└─────────────────────────────────────────────────────────┘
		│
		└─ CompanyDetails Feature (independent)

┌─────────────────────────────────────────────────────────┐
│ Features/Locations/Queries/                             │
│ LocationMappingExtensions.cs (internal)                │
└─────────────────────────────────────────────────────────┘
		│
		└─ Locations Feature (independent)

┌─────────────────────────────────────────────────────────┐
│ Features/Buildings/Queries/                             │
│ BuildingMappingExtensions.cs (internal)                │
└─────────────────────────────────────────────────────────┘
		│
		└─ Buildings Feature (independent)

┌─────────────────────────────────────────────────────────┐
│ Features/Offices/Queries/                               │
│ OfficeMappingExtensions.cs (internal)                  │
└─────────────────────────────────────────────────────────┘
		│
		└─ Offices Feature (independent)

┌─────────────────────────────────────────────────────────┐
│ Features/Projects/Queries/                              │
│ ProjectMappingExtensions.cs (internal)                 │
└─────────────────────────────────────────────────────────┘
		│
		└─ Projects Feature (independent)

┌─────────────────────────────────────────────────────────┐
│ Features/Shared/                                        │
│ - Constants (legitimate cross-feature)                  │
│ - Exceptions (legitimate cross-feature)                │
│ - Filters (legitimate cross-feature)                   │
└─────────────────────────────────────────────────────────┘
		│
		└─ Infrastructure layer (shared utilities)
```

---

## 9. Test Coverage Impact

### Centralized Mapping

```
Features/Shared/MappingExtensions.cs changes
		│
		├─ Affects: CompanyDetails tests
		├─ Affects: Locations tests
		├─ Affects: Buildings tests
		├─ Affects: Offices tests
		└─ Affects: Projects tests

One mapping change = Run 5+ test suites
Test execution time: LONG
Coverage scope: BROAD
Regression risk: HIGH
```

### Feature-Scoped Mapping

```
Features/CompanyDetails/Queries/
CompanyMappingExtensions.cs changes
		│
		└─ Affects: CompanyDetails tests only

One mapping change = Run 1 test suite
Test execution time: FAST
Coverage scope: NARROW
Regression risk: LOW
```

---

## 10. Architecture Evolution Path

```
YEAR 1: Monolith with Vertical Slices
├── Features organized by domain
├── Feature-scoped mappings
└── Shared infrastructure only

		↓

YEAR 2: Prepare for Growth
├── Continue vertical slice pattern
├── Ensure feature independence
├── Document extraction boundaries
└── Feature-scoped approach pays off

		↓

YEAR 3: Extract to Microservices (if needed)
├── CompanyDetails → CompanyService
├── Locations → LocationService
├── Buildings → BuildingService
├── Projects → ProjectService
└── Shared infrastructure → API Gateway

Each microservice carries:
├── Feature code (with scoped mappings)
├── Models and DTOs
├── Query handlers
└── Validators

NO REFACTORING NEEDED - already structured correctly!
```

---

## Conclusion

### Visual Summary

| Aspect | Centralized | Feature-Scoped |
|--------|-------------|----------------|
| **Structure** | 🔴 God Class | 🟢 Distributed |
| **Dependencies** | 🔴 Circular | 🟢 Linear |
| **Maintainability** | 🟡 Growing difficult | 🟢 Consistent |
| **Scalability** | 🔴 Poor | 🟢 Excellent |
| **Team Structure** | 🟡 Shared ownership | 🟢 Clear ownership |
| **Microservices** | 🔴 Blocked | 🟢 Enabled |
| **Testing** | 🟡 Broad impact | 🟢 Isolated tests |
| **Onboarding** | 🟡 Hard to explain | 🟢 Self-evident |

### Decision

```
CURRENT: Works, but architectural compromise
RECOMMENDED: Feature-scoped mapping (best practice)
EFFORT: 2-4 hours
RISK: Very low (no functional changes)
BENEFIT: Foundational architecture improvement
```

**Implementation documents provided:**
1. ARCHITECTURAL_REVIEW_MAPPING_EXTENSIONS.md (detailed analysis)
2. FEATURE_SCOPED_MAPPING_MIGRATION_GUIDE.md (step-by-step)
3. MAPPING_EXTENSIONS_SUMMARY.md (executive summary)
4. This document (visual reference)

