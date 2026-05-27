using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;
using MyStartUpCompany.Persistence.Entities.ValueObjects;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Project entity with 23 diverse projects
/// covering various companies, locations, and statuses.
/// Uses hardcoded static dates to avoid dynamic value warnings from EF Core.
/// </summary>
public class ProjectSeedConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasData(GetSeedData());
    }

    private static IEnumerable<Project> GetSeedData()
    {
        var projects = new List<Project>();
        int id = 1;

        // ========== COMPANY 1 (TechVision Inc) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-001",
                Name = "AI-Powered Analytics Platform",
                Code = "AIPL",
                Location = "San Francisco",
                CompanyId = 1,
                Details = new ProjectDetails
                {
                    Budget = 250000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop a machine learning-powered analytics platform for enterprise data analysis",
                    ProjectManager = "Sarah Chen",
                    TeamMembers = new List<string> { "Sarah Chen", "John Developer", "Alice ML Specialist", "Bob DevOps" },
                    Priority = "Critical",
                    Tags = new List<string> { "AI/ML", "Analytics", "Enterprise" },
                    Metrics = new Dictionary<string, string> { { "Accuracy", "94.5%" }, { "Response Time", "200ms" } },
                    ProgressPercentage = 65,
                    BudgetSpent = 162500,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "MVP", "Data Pipeline", "ML Models", "Dashboard" },
                    Dependencies = new List<string>()
                },
                CreatedAt = new DateTime(2024, 5, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-002",
                Name = "Cloud Infrastructure Migration",
                Code = "CLDM",
                Location = "San Francisco",
                CompanyId = 1,
                Details = new ProjectDetails
                {
                    Budget = 180000,
                    Status = "Planning",
                    StartDate = new DateTime(2024, 12, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Migrate existing infrastructure to cloud-native architecture",
                    ProjectManager = "Michael Torres",
                    TeamMembers = new List<string> { "Michael Torres", "Cloud Architect", "Network Specialist" },
                    Priority = "High",
                    Tags = new List<string> { "Cloud", "Infrastructure", "Migration" },
                    ProgressPercentage = 15,
                    RiskLevel = "High",
                    Deliverables = new List<string> { "Architecture Design", "Migration Plan", "Testing Strategy" },
                    Dependencies = new List<string>()
                },
                CreatedAt = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-003",
                Name = "API Security Enhancement",
                Code = "APISEC",
                Location = "Palo Alto",
                CompanyId = 1,
                Details = new ProjectDetails
                {
                    Budget = 95000,
                    Status = "Completed",
                    StartDate = new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement OAuth 2.0 and advanced security protocols for all public APIs",
                    ProjectManager = "Dr. Alex Kumar",
                    TeamMembers = new List<string> { "Dr. Alex Kumar", "Security Lead", "Backend Engineer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Security", "API", "Compliance" },
                    ProgressPercentage = 100,
                    BudgetSpent = 95000,
                    Outcome = "Successfully implemented OAuth 2.0, reducing security incidents by 87%",
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "OAuth 2.0 Implementation", "Security Audit Report", "API Documentation" }
                },
                CreatedAt = new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 2 (CloudSync Solutions) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-004",
                Name = "Container Orchestration Platform",
                Code = "CONT",
                Location = "San Jose",
                CompanyId = 2,
                Details = new ProjectDetails
                {
                    Budget = 320000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Build a Kubernetes-based platform for managing containerized applications",
                    ProjectManager = "Emma Rodriguez",
                    TeamMembers = new List<string> { "Emma Rodriguez", "DevOps Lead", "Platform Engineer", "SRE" },
                    Priority = "Critical",
                    Tags = new List<string> { "Kubernetes", "DevOps", "Containers" },
                    Metrics = new Dictionary<string, string> { { "Uptime", "99.95%" }, { "Deployment Time", "5min" } },
                    ProgressPercentage = 45,
                    BudgetSpent = 144000,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "Platform Core", "Dashboard", "CLI Tools", "Documentation" }
                },
                CreatedAt = new DateTime(2024, 8, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-005",
                Name = "Multi-Cloud Monitoring Solution",
                Code = "MCMON",
                Location = "San Jose",
                CompanyId = 2,
                Details = new ProjectDetails
                {
                    Budget = 155000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Create unified monitoring solution for multi-cloud environments",
                    ProjectManager = "James Park",
                    TeamMembers = new List<string> { "James Park", "Monitoring Specialist", "Data Engineer" },
                    Priority = "High",
                    Tags = new List<string> { "Monitoring", "Cloud", "Analytics" },
                    ProgressPercentage = 52,
                    BudgetSpent = 80600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Monitoring Agent", "Dashboard", "Alert System" }
                },
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-006",
                Name = "Disaster Recovery Infrastructure",
                Code = "DREC",
                Location = "Remote",
                CompanyId = 2,
                Details = new ProjectDetails
                {
                    Budget = 210000,
                    Status = "Planning",
                    StartDate = new DateTime(2025, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement comprehensive disaster recovery and business continuity infrastructure",
                    ProjectManager = "Lisa Wang",
                    TeamMembers = new List<string> { "Lisa Wang", "Infrastructure Manager" },
                    Priority = "High",
                    Tags = new List<string> { "Disaster Recovery", "Infrastructure", "Business Continuity" },
                    ProgressPercentage = 5,
                    RiskLevel = "High",
                    Deliverables = new List<string> { "DR Plan", "Infrastructure Setup", "Testing Procedures" }
                },
                CreatedAt = new DateTime(2024, 10, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 3 (DataFlow Analytics) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-007",
                Name = "Real-Time Data Pipeline",
                Code = "RTDP",
                Location = "Palo Alto",
                CompanyId = 3,
                Details = new ProjectDetails
                {
                    Budget = 280000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Build a real-time data ingestion and processing pipeline using Apache Kafka",
                    ProjectManager = "Priya Sharma",
                    TeamMembers = new List<string> { "Priya Sharma", "Data Engineer", "Stream Processing Specialist", "DevOps" },
                    Priority = "Critical",
                    Tags = new List<string> { "Big Data", "Kafka", "Real-Time" },
                    Metrics = new Dictionary<string, string> { { "Throughput", "1M events/sec" }, { "Latency", "100ms" } },
                    ProgressPercentage = 72,
                    BudgetSpent = 201600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Kafka Cluster", "Processing Engine", "Monitoring" }
                },
                CreatedAt = new DateTime(2024, 6, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-008",
                Name = "Advanced Analytics Dashboard",
                Code = "AAD",
                Location = "Palo Alto",
                CompanyId = 3,
                Details = new ProjectDetails
                {
                    Budget = 120000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop interactive analytics dashboard with real-time insights",
                    ProjectManager = "Robert Johnson",
                    TeamMembers = new List<string> { "Robert Johnson", "UI/UX Designer", "Frontend Developer", "Data Analyst" },
                    Priority = "High",
                    Tags = new List<string> { "Analytics", "Dashboard", "UI/UX" },
                    ProgressPercentage = 58,
                    BudgetSpent = 69600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Dashboard App", "Data Connectors", "User Documentation" }
                },
                CreatedAt = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-009",
                Name = "Data Quality Framework",
                Code = "DQF",
                Location = "Remote",
                CompanyId = 3,
                Details = new ProjectDetails
                {
                    Budget = 95000,
                    Status = "Completed",
                    StartDate = new DateTime(2024, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement comprehensive data quality validation and monitoring framework",
                    ProjectManager = "Nina Patel",
                    TeamMembers = new List<string> { "Nina Patel", "Data Quality Lead" },
                    Priority = "High",
                    Tags = new List<string> { "Data Quality", "Validation", "Monitoring" },
                    ProgressPercentage = 100,
                    BudgetSpent = 95000,
                    Outcome = "Implemented automated quality checks, reducing data issues by 92%",
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Quality Framework", "Rules Engine", "Reporting" }
                },
                CreatedAt = new DateTime(2024, 1, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 4 (SecureNet Systems) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-010",
                Name = "Zero-Trust Security Architecture",
                Code = "ZTRST",
                Location = "San Diego",
                CompanyId = 4,
                Details = new ProjectDetails
                {
                    Budget = 420000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement comprehensive zero-trust security model across organization",
                    ProjectManager = "Colonel Michael Hayes",
                    TeamMembers = new List<string> { "Colonel Michael Hayes", "Security Architect", "Network Security Lead", "Compliance Officer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Security", "Zero-Trust", "Compliance" },
                    Metrics = new Dictionary<string, string> { { "Compliance Score", "98.5%" }, { "Threat Detection Rate", "99.2%" } },
                    ProgressPercentage = 68,
                    BudgetSpent = 285600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Architecture Design", "Implementation", "Security Audit", "Training" }
                },
                CreatedAt = new DateTime(2024, 3, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-011",
                Name = "Encryption Key Management System",
                Code = "EKMS",
                Location = "San Diego",
                CompanyId = 4,
                Details = new ProjectDetails
                {
                    Budget = 175000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop HSM-integrated key management system for enterprise encryption",
                    ProjectManager = "Victoria Thompson",
                    TeamMembers = new List<string> { "Victoria Thompson", "Cryptography Expert", "Backend Developer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Encryption", "HSM", "Security" },
                    ProgressPercentage = 41,
                    BudgetSpent = 71750,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "KMS Platform", "HSM Integration", "API Documentation" }
                },
                CreatedAt = new DateTime(2024, 8, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 5 (DevOps Masters) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-012",
                Name = "GitOps Pipeline Implementation",
                Code = "GITOPS",
                Location = "Los Angeles",
                CompanyId = 5,
                Details = new ProjectDetails
                {
                    Budget = 140000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement GitOps-based CI/CD pipeline with ArgoCD and Flux",
                    ProjectManager = "David Kim",
                    TeamMembers = new List<string> { "David Kim", "DevOps Engineer", "Platform Engineer" },
                    Priority = "High",
                    Tags = new List<string> { "GitOps", "CI/CD", "DevOps" },
                    ProgressPercentage = 35,
                    BudgetSpent = 49000,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "GitOps Setup", "ArgoCD Configuration", "Documentation" }
                },
                CreatedAt = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-013",
                Name = "Infrastructure as Code Framework",
                Code = "IACF",
                Location = "Los Angeles",
                CompanyId = 5,
                Details = new ProjectDetails
                {
                    Budget = 165000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 5, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Create reusable Terraform and Ansible modules for infrastructure provisioning",
                    ProjectManager = "Elena Rodriguez",
                    TeamMembers = new List<string> { "Elena Rodriguez", "Infrastructure Specialist", "Automation Engineer" },
                    Priority = "High",
                    Tags = new List<string> { "Terraform", "IaC", "Infrastructure" },
                    ProgressPercentage = 58,
                    BudgetSpent = 95700,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Terraform Modules", "Ansible Playbooks", "Testing Suite" }
                },
                CreatedAt = new DateTime(2024, 5, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 6 (Quantum Research Lab) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-014",
                Name = "Quantum Algorithm Development",
                Code = "QAD",
                Location = "Mountain View",
                CompanyId = 6,
                Details = new ProjectDetails
                {
                    Budget = 550000,
                    Status = "Active",
                    StartDate = new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 11, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Research and develop novel quantum algorithms for optimization problems",
                    ProjectManager = "Dr. Richard Chen",
                    TeamMembers = new List<string> { "Dr. Richard Chen", "Quantum Researcher", "Theoretical Physicist", "Software Engineer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Quantum Computing", "Research", "Algorithms" },
                    Metrics = new Dictionary<string, string> { { "Algorithm Accuracy", "96.7%" }, { "Speedup Factor", "2.3x" } },
                    ProgressPercentage = 52,
                    BudgetSpent = 286000,
                    RiskLevel = "High",
                    Deliverables = new List<string> { "Algorithm Papers", "Implementation", "Benchmarks", "Documentation" }
                },
                CreatedAt = new DateTime(2023, 11, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-015",
                Name = "Quantum Simulator Platform",
                Code = "QSP",
                Location = "Mountain View",
                CompanyId = 6,
                Details = new ProjectDetails
                {
                    Budget = 320000,
                    Status = "Planning",
                    StartDate = new DateTime(2025, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Build high-performance quantum simulator for algorithm testing",
                    ProjectManager = "Dr. Lisa Wong",
                    TeamMembers = new List<string> { "Dr. Lisa Wong", "Platform Architect" },
                    Priority = "High",
                    Tags = new List<string> { "Quantum", "Simulation", "Platform" },
                    ProgressPercentage = 10,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "Simulator Core", "API", "Performance Benchmarks" }
                },
                CreatedAt = new DateTime(2024, 10, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 7 (NextGen Mobile) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-016",
                Name = "Cross-Platform Mobile Framework",
                Code = "CPFX",
                Location = "San Francisco",
                CompanyId = 7,
                Details = new ProjectDetails
                {
                    Budget = 280000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop modern cross-platform mobile framework supporting iOS and Android",
                    ProjectManager = "Sofia Martinez",
                    TeamMembers = new List<string> { "Sofia Martinez", "Mobile Lead", "iOS Developer", "Android Developer", "QA Engineer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Mobile", "Cross-Platform", "iOS", "Android" },
                    Metrics = new Dictionary<string, string> { { "Code Coverage", "85%" }, { "App Store Rating", "4.8/5" } },
                    ProgressPercentage = 62,
                    BudgetSpent = 173600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Framework Core", "Sample Apps", "Documentation", "Testing Tools" }
                },
                CreatedAt = new DateTime(2024, 3, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-017",
                Name = "AR/VR Mobile Integration",
                Code = "ARVR",
                Location = "San Francisco",
                CompanyId = 7,
                Details = new ProjectDetails
                {
                    Budget = 195000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Integrate AR/VR capabilities into mobile platform",
                    ProjectManager = "James Wilson",
                    TeamMembers = new List<string> { "James Wilson", "AR/VR Specialist", "Graphics Programmer" },
                    Priority = "High",
                    Tags = new List<string> { "AR", "VR", "Mobile", "3D" },
                    ProgressPercentage = 45,
                    BudgetSpent = 87750,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "AR Engine", "VR Engine", "Demo Applications" }
                },
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 8 (Enterprise Solutions) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-018",
                Name = "ERP System Upgrade",
                Code = "ERPU",
                Location = "New York",
                CompanyId = 8,
                Details = new ProjectDetails
                {
                    Budget = 500000,
                    Status = "Active",
                    StartDate = new DateTime(2023, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 8, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Upgrade legacy ERP system to modern cloud-based solution",
                    ProjectManager = "Paul Anderson",
                    TeamMembers = new List<string> { "Paul Anderson", "ERP Consultant", "Migration Lead", "Database Admin", "Business Analyst" },
                    Priority = "Critical",
                    Tags = new List<string> { "ERP", "Migration", "Enterprise" },
                    Metrics = new Dictionary<string, string> { { "User Adoption", "87%" }, { "Process Efficiency", "+35%" } },
                    ProgressPercentage = 61,
                    BudgetSpent = 305000,
                    RiskLevel = "High",
                    Deliverables = new List<string> { "System Setup", "Data Migration", "Training", "Support Plan" }
                },
                CreatedAt = new DateTime(2023, 8, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-019",
                Name = "Business Intelligence Platform",
                Code = "BIP",
                Location = "New York",
                CompanyId = 8,
                Details = new ProjectDetails
                {
                    Budget = 220000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 6, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Build comprehensive business intelligence and reporting platform",
                    ProjectManager = "Catherine Lee",
                    TeamMembers = new List<string> { "Catherine Lee", "BI Architect", "Data Analyst", "Report Developer" },
                    Priority = "High",
                    Tags = new List<string> { "BI", "Reporting", "Analytics" },
                    ProgressPercentage = 53,
                    BudgetSpent = 116600,
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Data Warehouse", "Reports", "Dashboards", "Training" }
                },
                CreatedAt = new DateTime(2024, 6, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 9 (FinTech Innovations) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-020",
                Name = "Blockchain Payment System",
                Code = "BCPAY",
                Location = "San Francisco",
                CompanyId = 9,
                Details = new ProjectDetails
                {
                    Budget = 600000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2026, 1, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop blockchain-based payment system for global settlements",
                    ProjectManager = "Marcus Johnson",
                    TeamMembers = new List<string> { "Marcus Johnson", "Blockchain Architect", "Smart Contract Dev", "Security Expert", "Finance Dev" },
                    Priority = "Critical",
                    Tags = new List<string> { "Blockchain", "Cryptocurrency", "FinTech", "Payments" },
                    Metrics = new Dictionary<string, string> { { "Transaction Throughput", "10K TPS" }, { "Settlement Time", "2 seconds" } },
                    ProgressPercentage = 48,
                    BudgetSpent = 288000,
                    RiskLevel = "High",
                    Deliverables = new List<string> { "Smart Contracts", "Payment API", "Wallet System", "Compliance Framework" }
                },
                CreatedAt = new DateTime(2024, 1, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-021",
                Name = "Fraud Detection AI System",
                Code = "FDAI",
                Location = "San Francisco",
                CompanyId = 9,
                Details = new ProjectDetails
                {
                    Budget = 320000,
                    Status = "Active",
                    StartDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 3, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Implement advanced ML-based fraud detection system",
                    ProjectManager = "Dr. Rachel Green",
                    TeamMembers = new List<string> { "Dr. Rachel Green", "ML Engineer", "Data Scientist", "Backend Developer" },
                    Priority = "Critical",
                    Tags = new List<string> { "AI/ML", "Fraud Detection", "FinTech" },
                    Metrics = new Dictionary<string, string> { { "Detection Accuracy", "98.7%" }, { "False Positive Rate", "0.3%" } },
                    ProgressPercentage = 64,
                    BudgetSpent = 204800,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "ML Models", "Detection Engine", "Real-time API", "Dashboard" }
                },
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        // ========== COMPANY 10 (Green Energy Solutions) - Projects ==========
        projects.AddRange(new[]
        {
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-022",
                Name = "Smart Grid Management System",
                Code = "SGMS",
                Location = "Austin",
                CompanyId = 10,
                Details = new ProjectDetails
                {
                    Budget = 480000,
                    Status = "Active",
                    StartDate = new DateTime(2023, 12, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2025, 12, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Build comprehensive smart grid management and optimization system",
                    ProjectManager = "Jennifer Foster",
                    TeamMembers = new List<string> { "Jennifer Foster", "Energy Systems Lead", "IoT Engineer", "Data Scientist", "Electrical Engineer" },
                    Priority = "Critical",
                    Tags = new List<string> { "Smart Grid", "IoT", "Energy", "Optimization" },
                    Metrics = new Dictionary<string, string> { { "Grid Efficiency", "+22%" }, { "Energy Savings", "18%" } },
                    ProgressPercentage = 55,
                    BudgetSpent = 264000,
                    RiskLevel = "Medium",
                    Deliverables = new List<string> { "SCADA System", "Analytics Engine", "Mobile App", "Training" }
                },
                CreatedAt = new DateTime(2023, 12, 27, 0, 0, 0, DateTimeKind.Utc)
            },
            new Project
            {
                Id = id++,
                ProjectIdentifier = "PROJ-2024-023",
                Name = "Solar Farm Monitoring Platform",
                Code = "SFMP",
                Location = "Austin",
                CompanyId = 10,
                Details = new ProjectDetails
                {
                    Budget = 185000,
                    Status = "Completed",
                    StartDate = new DateTime(2023, 9, 27, 0, 0, 0, DateTimeKind.Utc),
                    EndDate = new DateTime(2024, 9, 27, 0, 0, 0, DateTimeKind.Utc),
                    Description = "Develop real-time monitoring system for solar farm operations",
                    ProjectManager = "Mark Stevens",
                    TeamMembers = new List<string> { "Mark Stevens", "IoT Specialist" },
                    Priority = "High",
                    Tags = new List<string> { "Solar", "IoT", "Monitoring", "Renewable" },
                    ProgressPercentage = 100,
                    BudgetSpent = 185000,
                    Outcome = "Successfully deployed to 50 solar farms, achieving 99.2% uptime",
                    RiskLevel = "Low",
                    Deliverables = new List<string> { "Monitoring Platform", "Mobile App", "Alert System" }
                },
                CreatedAt = new DateTime(2023, 9, 27, 0, 0, 0, DateTimeKind.Utc)
            }
        });

        return projects;
    }
}
