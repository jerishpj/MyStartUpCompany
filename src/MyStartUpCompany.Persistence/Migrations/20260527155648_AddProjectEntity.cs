using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectIdentifier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Unique project identifier, e.g., PROJ-2024-001"),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Project name/title"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Project code/abbreviation, e.g., DVP, MKT"),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Project location/site, e.g., San Francisco, Remote"),
                    CompanyId = table.Column<int>(type: "int", nullable: false, comment: "Foreign key to Company"),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false, comment: "Project details stored as JSON: Budget, Status, Dates, Team, Tags, Metrics, Metadata"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "When the project record was created"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When the project record was last updated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Code", "CompanyId", "CreatedAt", "Details", "Location", "Name", "ProjectIdentifier", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "AIPL", 1, new DateTime(2024, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":250000,\"Status\":\"Active\",\"StartDate\":\"2024-05-27T00:00:00Z\",\"EndDate\":\"2025-05-27T00:00:00Z\",\"Description\":\"Develop a machine learning-powered analytics platform for enterprise data analysis\",\"ProjectManager\":\"Sarah Chen\",\"TeamMembers\":[\"Sarah Chen\",\"John Developer\",\"Alice ML Specialist\",\"Bob DevOps\"],\"Priority\":\"Critical\",\"Tags\":[\"AI/ML\",\"Analytics\",\"Enterprise\"],\"Metrics\":{\"Accuracy\":\"94.5%\",\"Response Time\":\"200ms\"},\"Metadata\":{},\"ProgressPercentage\":65,\"Notes\":null,\"BudgetSpent\":162500,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"MVP\",\"Data Pipeline\",\"ML Models\",\"Dashboard\"],\"Dependencies\":[]}", "San Francisco", "AI-Powered Analytics Platform", "PROJ-2024-001", null },
                    { 2, "CLDM", 1, new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":180000,\"Status\":\"Planning\",\"StartDate\":\"2024-12-27T00:00:00Z\",\"EndDate\":\"2025-06-27T00:00:00Z\",\"Description\":\"Migrate existing infrastructure to cloud-native architecture\",\"ProjectManager\":\"Michael Torres\",\"TeamMembers\":[\"Michael Torres\",\"Cloud Architect\",\"Network Specialist\"],\"Priority\":\"High\",\"Tags\":[\"Cloud\",\"Infrastructure\",\"Migration\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":15,\"Notes\":null,\"BudgetSpent\":null,\"Outcome\":null,\"RiskLevel\":\"High\",\"Deliverables\":[\"Architecture Design\",\"Migration Plan\",\"Testing Strategy\"],\"Dependencies\":[]}", "San Francisco", "Cloud Infrastructure Migration", "PROJ-2024-002", null },
                    { 3, "APISEC", 1, new DateTime(2023, 11, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":95000,\"Status\":\"Completed\",\"StartDate\":\"2023-11-27T00:00:00Z\",\"EndDate\":\"2024-05-27T00:00:00Z\",\"Description\":\"Implement OAuth 2.0 and advanced security protocols for all public APIs\",\"ProjectManager\":\"Dr. Alex Kumar\",\"TeamMembers\":[\"Dr. Alex Kumar\",\"Security Lead\",\"Backend Engineer\"],\"Priority\":\"Critical\",\"Tags\":[\"Security\",\"API\",\"Compliance\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":100,\"Notes\":null,\"BudgetSpent\":95000,\"Outcome\":\"Successfully implemented OAuth 2.0, reducing security incidents by 87%\",\"RiskLevel\":\"Low\",\"Deliverables\":[\"OAuth 2.0 Implementation\",\"Security Audit Report\",\"API Documentation\"],\"Dependencies\":[]}", "Palo Alto", "API Security Enhancement", "PROJ-2024-003", null },
                    { 4, "CONT", 2, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":320000,\"Status\":\"Active\",\"StartDate\":\"2024-08-27T00:00:00Z\",\"EndDate\":\"2025-08-27T00:00:00Z\",\"Description\":\"Build a Kubernetes-based platform for managing containerized applications\",\"ProjectManager\":\"Emma Rodriguez\",\"TeamMembers\":[\"Emma Rodriguez\",\"DevOps Lead\",\"Platform Engineer\",\"SRE\"],\"Priority\":\"Critical\",\"Tags\":[\"Kubernetes\",\"DevOps\",\"Containers\"],\"Metrics\":{\"Uptime\":\"99.95%\",\"Deployment Time\":\"5min\"},\"Metadata\":{},\"ProgressPercentage\":45,\"Notes\":null,\"BudgetSpent\":144000,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"Platform Core\",\"Dashboard\",\"CLI Tools\",\"Documentation\"],\"Dependencies\":[]}", "San Jose", "Container Orchestration Platform", "PROJ-2024-004", null },
                    { 5, "MCMON", 2, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":155000,\"Status\":\"Active\",\"StartDate\":\"2024-07-27T00:00:00Z\",\"EndDate\":\"2025-07-27T00:00:00Z\",\"Description\":\"Create unified monitoring solution for multi-cloud environments\",\"ProjectManager\":\"James Park\",\"TeamMembers\":[\"James Park\",\"Monitoring Specialist\",\"Data Engineer\"],\"Priority\":\"High\",\"Tags\":[\"Monitoring\",\"Cloud\",\"Analytics\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":52,\"Notes\":null,\"BudgetSpent\":80600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Monitoring Agent\",\"Dashboard\",\"Alert System\"],\"Dependencies\":[]}", "San Jose", "Multi-Cloud Monitoring Solution", "PROJ-2024-005", null },
                    { 6, "DREC", 2, new DateTime(2024, 10, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":210000,\"Status\":\"Planning\",\"StartDate\":\"2025-01-27T00:00:00Z\",\"EndDate\":\"2025-07-27T00:00:00Z\",\"Description\":\"Implement comprehensive disaster recovery and business continuity infrastructure\",\"ProjectManager\":\"Lisa Wang\",\"TeamMembers\":[\"Lisa Wang\",\"Infrastructure Manager\"],\"Priority\":\"High\",\"Tags\":[\"Disaster Recovery\",\"Infrastructure\",\"Business Continuity\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":5,\"Notes\":null,\"BudgetSpent\":null,\"Outcome\":null,\"RiskLevel\":\"High\",\"Deliverables\":[\"DR Plan\",\"Infrastructure Setup\",\"Testing Procedures\"],\"Dependencies\":[]}", "Remote", "Disaster Recovery Infrastructure", "PROJ-2024-006", null },
                    { 7, "RTDP", 3, new DateTime(2024, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":280000,\"Status\":\"Active\",\"StartDate\":\"2024-06-27T00:00:00Z\",\"EndDate\":\"2025-06-27T00:00:00Z\",\"Description\":\"Build a real-time data ingestion and processing pipeline using Apache Kafka\",\"ProjectManager\":\"Priya Sharma\",\"TeamMembers\":[\"Priya Sharma\",\"Data Engineer\",\"Stream Processing Specialist\",\"DevOps\"],\"Priority\":\"Critical\",\"Tags\":[\"Big Data\",\"Kafka\",\"Real-Time\"],\"Metrics\":{\"Throughput\":\"1M events/sec\",\"Latency\":\"100ms\"},\"Metadata\":{},\"ProgressPercentage\":72,\"Notes\":null,\"BudgetSpent\":201600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Kafka Cluster\",\"Processing Engine\",\"Monitoring\"],\"Dependencies\":[]}", "Palo Alto", "Real-Time Data Pipeline", "PROJ-2024-007", null },
                    { 8, "AAD", 3, new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":120000,\"Status\":\"Active\",\"StartDate\":\"2024-09-27T00:00:00Z\",\"EndDate\":\"2025-03-27T00:00:00Z\",\"Description\":\"Develop interactive analytics dashboard with real-time insights\",\"ProjectManager\":\"Robert Johnson\",\"TeamMembers\":[\"Robert Johnson\",\"UI/UX Designer\",\"Frontend Developer\",\"Data Analyst\"],\"Priority\":\"High\",\"Tags\":[\"Analytics\",\"Dashboard\",\"UI/UX\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":58,\"Notes\":null,\"BudgetSpent\":69600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Dashboard App\",\"Data Connectors\",\"User Documentation\"],\"Dependencies\":[]}", "Palo Alto", "Advanced Analytics Dashboard", "PROJ-2024-008", null },
                    { 9, "DQF", 3, new DateTime(2024, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":95000,\"Status\":\"Completed\",\"StartDate\":\"2024-01-27T00:00:00Z\",\"EndDate\":\"2024-08-27T00:00:00Z\",\"Description\":\"Implement comprehensive data quality validation and monitoring framework\",\"ProjectManager\":\"Nina Patel\",\"TeamMembers\":[\"Nina Patel\",\"Data Quality Lead\"],\"Priority\":\"High\",\"Tags\":[\"Data Quality\",\"Validation\",\"Monitoring\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":100,\"Notes\":null,\"BudgetSpent\":95000,\"Outcome\":\"Implemented automated quality checks, reducing data issues by 92%\",\"RiskLevel\":\"Low\",\"Deliverables\":[\"Quality Framework\",\"Rules Engine\",\"Reporting\"],\"Dependencies\":[]}", "Remote", "Data Quality Framework", "PROJ-2024-009", null },
                    { 10, "ZTRST", 4, new DateTime(2024, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":420000,\"Status\":\"Active\",\"StartDate\":\"2024-03-27T00:00:00Z\",\"EndDate\":\"2025-03-27T00:00:00Z\",\"Description\":\"Implement comprehensive zero-trust security model across organization\",\"ProjectManager\":\"Colonel Michael Hayes\",\"TeamMembers\":[\"Colonel Michael Hayes\",\"Security Architect\",\"Network Security Lead\",\"Compliance Officer\"],\"Priority\":\"Critical\",\"Tags\":[\"Security\",\"Zero-Trust\",\"Compliance\"],\"Metrics\":{\"Compliance Score\":\"98.5%\",\"Threat Detection Rate\":\"99.2%\"},\"Metadata\":{},\"ProgressPercentage\":68,\"Notes\":null,\"BudgetSpent\":285600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Architecture Design\",\"Implementation\",\"Security Audit\",\"Training\"],\"Dependencies\":[]}", "San Diego", "Zero-Trust Security Architecture", "PROJ-2024-010", null },
                    { 11, "EKMS", 4, new DateTime(2024, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":175000,\"Status\":\"Active\",\"StartDate\":\"2024-08-27T00:00:00Z\",\"EndDate\":\"2025-08-27T00:00:00Z\",\"Description\":\"Develop HSM-integrated key management system for enterprise encryption\",\"ProjectManager\":\"Victoria Thompson\",\"TeamMembers\":[\"Victoria Thompson\",\"Cryptography Expert\",\"Backend Developer\"],\"Priority\":\"Critical\",\"Tags\":[\"Encryption\",\"HSM\",\"Security\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":41,\"Notes\":null,\"BudgetSpent\":71750,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"KMS Platform\",\"HSM Integration\",\"API Documentation\"],\"Dependencies\":[]}", "San Diego", "Encryption Key Management System", "PROJ-2024-011", null },
                    { 12, "GITOPS", 5, new DateTime(2024, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":140000,\"Status\":\"Active\",\"StartDate\":\"2024-09-27T00:00:00Z\",\"EndDate\":\"2025-05-27T00:00:00Z\",\"Description\":\"Implement GitOps-based CI/CD pipeline with ArgoCD and Flux\",\"ProjectManager\":\"David Kim\",\"TeamMembers\":[\"David Kim\",\"DevOps Engineer\",\"Platform Engineer\"],\"Priority\":\"High\",\"Tags\":[\"GitOps\",\"CI/CD\",\"DevOps\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":35,\"Notes\":null,\"BudgetSpent\":49000,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"GitOps Setup\",\"ArgoCD Configuration\",\"Documentation\"],\"Dependencies\":[]}", "Los Angeles", "GitOps Pipeline Implementation", "PROJ-2024-012", null },
                    { 13, "IACF", 5, new DateTime(2024, 5, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":165000,\"Status\":\"Active\",\"StartDate\":\"2024-05-27T00:00:00Z\",\"EndDate\":\"2025-05-27T00:00:00Z\",\"Description\":\"Create reusable Terraform and Ansible modules for infrastructure provisioning\",\"ProjectManager\":\"Elena Rodriguez\",\"TeamMembers\":[\"Elena Rodriguez\",\"Infrastructure Specialist\",\"Automation Engineer\"],\"Priority\":\"High\",\"Tags\":[\"Terraform\",\"IaC\",\"Infrastructure\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":58,\"Notes\":null,\"BudgetSpent\":95700,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Terraform Modules\",\"Ansible Playbooks\",\"Testing Suite\"],\"Dependencies\":[]}", "Los Angeles", "Infrastructure as Code Framework", "PROJ-2024-013", null },
                    { 14, "QAD", 6, new DateTime(2023, 11, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":550000,\"Status\":\"Active\",\"StartDate\":\"2023-11-27T00:00:00Z\",\"EndDate\":\"2025-11-27T00:00:00Z\",\"Description\":\"Research and develop novel quantum algorithms for optimization problems\",\"ProjectManager\":\"Dr. Richard Chen\",\"TeamMembers\":[\"Dr. Richard Chen\",\"Quantum Researcher\",\"Theoretical Physicist\",\"Software Engineer\"],\"Priority\":\"Critical\",\"Tags\":[\"Quantum Computing\",\"Research\",\"Algorithms\"],\"Metrics\":{\"Algorithm Accuracy\":\"96.7%\",\"Speedup Factor\":\"2.3x\"},\"Metadata\":{},\"ProgressPercentage\":52,\"Notes\":null,\"BudgetSpent\":286000,\"Outcome\":null,\"RiskLevel\":\"High\",\"Deliverables\":[\"Algorithm Papers\",\"Implementation\",\"Benchmarks\",\"Documentation\"],\"Dependencies\":[]}", "Mountain View", "Quantum Algorithm Development", "PROJ-2024-014", null },
                    { 15, "QSP", 6, new DateTime(2024, 10, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":320000,\"Status\":\"Planning\",\"StartDate\":\"2025-01-27T00:00:00Z\",\"EndDate\":\"2026-01-27T00:00:00Z\",\"Description\":\"Build high-performance quantum simulator for algorithm testing\",\"ProjectManager\":\"Dr. Lisa Wong\",\"TeamMembers\":[\"Dr. Lisa Wong\",\"Platform Architect\"],\"Priority\":\"High\",\"Tags\":[\"Quantum\",\"Simulation\",\"Platform\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":10,\"Notes\":null,\"BudgetSpent\":null,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"Simulator Core\",\"API\",\"Performance Benchmarks\"],\"Dependencies\":[]}", "Mountain View", "Quantum Simulator Platform", "PROJ-2024-015", null },
                    { 16, "CPFX", 7, new DateTime(2024, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":280000,\"Status\":\"Active\",\"StartDate\":\"2024-03-27T00:00:00Z\",\"EndDate\":\"2025-03-27T00:00:00Z\",\"Description\":\"Develop modern cross-platform mobile framework supporting iOS and Android\",\"ProjectManager\":\"Sofia Martinez\",\"TeamMembers\":[\"Sofia Martinez\",\"Mobile Lead\",\"iOS Developer\",\"Android Developer\",\"QA Engineer\"],\"Priority\":\"Critical\",\"Tags\":[\"Mobile\",\"Cross-Platform\",\"iOS\",\"Android\"],\"Metrics\":{\"Code Coverage\":\"85%\",\"App Store Rating\":\"4.8/5\"},\"Metadata\":{},\"ProgressPercentage\":62,\"Notes\":null,\"BudgetSpent\":173600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Framework Core\",\"Sample Apps\",\"Documentation\",\"Testing Tools\"],\"Dependencies\":[]}", "San Francisco", "Cross-Platform Mobile Framework", "PROJ-2024-016", null },
                    { 17, "ARVR", 7, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":195000,\"Status\":\"Active\",\"StartDate\":\"2024-07-27T00:00:00Z\",\"EndDate\":\"2025-07-27T00:00:00Z\",\"Description\":\"Integrate AR/VR capabilities into mobile platform\",\"ProjectManager\":\"James Wilson\",\"TeamMembers\":[\"James Wilson\",\"AR/VR Specialist\",\"Graphics Programmer\"],\"Priority\":\"High\",\"Tags\":[\"AR\",\"VR\",\"Mobile\",\"3D\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":45,\"Notes\":null,\"BudgetSpent\":87750,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"AR Engine\",\"VR Engine\",\"Demo Applications\"],\"Dependencies\":[]}", "San Francisco", "AR/VR Mobile Integration", "PROJ-2024-017", null },
                    { 18, "ERPU", 8, new DateTime(2023, 8, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":500000,\"Status\":\"Active\",\"StartDate\":\"2023-08-27T00:00:00Z\",\"EndDate\":\"2025-08-27T00:00:00Z\",\"Description\":\"Upgrade legacy ERP system to modern cloud-based solution\",\"ProjectManager\":\"Paul Anderson\",\"TeamMembers\":[\"Paul Anderson\",\"ERP Consultant\",\"Migration Lead\",\"Database Admin\",\"Business Analyst\"],\"Priority\":\"Critical\",\"Tags\":[\"ERP\",\"Migration\",\"Enterprise\"],\"Metrics\":{\"User Adoption\":\"87%\",\"Process Efficiency\":\"\\u002B35%\"},\"Metadata\":{},\"ProgressPercentage\":61,\"Notes\":null,\"BudgetSpent\":305000,\"Outcome\":null,\"RiskLevel\":\"High\",\"Deliverables\":[\"System Setup\",\"Data Migration\",\"Training\",\"Support Plan\"],\"Dependencies\":[]}", "New York", "ERP System Upgrade", "PROJ-2024-018", null },
                    { 19, "BIP", 8, new DateTime(2024, 6, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":220000,\"Status\":\"Active\",\"StartDate\":\"2024-06-27T00:00:00Z\",\"EndDate\":\"2025-06-27T00:00:00Z\",\"Description\":\"Build comprehensive business intelligence and reporting platform\",\"ProjectManager\":\"Catherine Lee\",\"TeamMembers\":[\"Catherine Lee\",\"BI Architect\",\"Data Analyst\",\"Report Developer\"],\"Priority\":\"High\",\"Tags\":[\"BI\",\"Reporting\",\"Analytics\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":53,\"Notes\":null,\"BudgetSpent\":116600,\"Outcome\":null,\"RiskLevel\":\"Low\",\"Deliverables\":[\"Data Warehouse\",\"Reports\",\"Dashboards\",\"Training\"],\"Dependencies\":[]}", "New York", "Business Intelligence Platform", "PROJ-2024-019", null },
                    { 20, "BCPAY", 9, new DateTime(2024, 1, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":600000,\"Status\":\"Active\",\"StartDate\":\"2024-01-27T00:00:00Z\",\"EndDate\":\"2026-01-27T00:00:00Z\",\"Description\":\"Develop blockchain-based payment system for global settlements\",\"ProjectManager\":\"Marcus Johnson\",\"TeamMembers\":[\"Marcus Johnson\",\"Blockchain Architect\",\"Smart Contract Dev\",\"Security Expert\",\"Finance Dev\"],\"Priority\":\"Critical\",\"Tags\":[\"Blockchain\",\"Cryptocurrency\",\"FinTech\",\"Payments\"],\"Metrics\":{\"Transaction Throughput\":\"10K TPS\",\"Settlement Time\":\"2 seconds\"},\"Metadata\":{},\"ProgressPercentage\":48,\"Notes\":null,\"BudgetSpent\":288000,\"Outcome\":null,\"RiskLevel\":\"High\",\"Deliverables\":[\"Smart Contracts\",\"Payment API\",\"Wallet System\",\"Compliance Framework\"],\"Dependencies\":[]}", "San Francisco", "Blockchain Payment System", "PROJ-2024-020", null },
                    { 21, "FDAI", 9, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":320000,\"Status\":\"Active\",\"StartDate\":\"2024-07-27T00:00:00Z\",\"EndDate\":\"2025-03-27T00:00:00Z\",\"Description\":\"Implement advanced ML-based fraud detection system\",\"ProjectManager\":\"Dr. Rachel Green\",\"TeamMembers\":[\"Dr. Rachel Green\",\"ML Engineer\",\"Data Scientist\",\"Backend Developer\"],\"Priority\":\"Critical\",\"Tags\":[\"AI/ML\",\"Fraud Detection\",\"FinTech\"],\"Metrics\":{\"Detection Accuracy\":\"98.7%\",\"False Positive Rate\":\"0.3%\"},\"Metadata\":{},\"ProgressPercentage\":64,\"Notes\":null,\"BudgetSpent\":204800,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"ML Models\",\"Detection Engine\",\"Real-time API\",\"Dashboard\"],\"Dependencies\":[]}", "San Francisco", "Fraud Detection AI System", "PROJ-2024-021", null },
                    { 22, "SGMS", 10, new DateTime(2023, 12, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":480000,\"Status\":\"Active\",\"StartDate\":\"2023-12-27T00:00:00Z\",\"EndDate\":\"2025-12-27T00:00:00Z\",\"Description\":\"Build comprehensive smart grid management and optimization system\",\"ProjectManager\":\"Jennifer Foster\",\"TeamMembers\":[\"Jennifer Foster\",\"Energy Systems Lead\",\"IoT Engineer\",\"Data Scientist\",\"Electrical Engineer\"],\"Priority\":\"Critical\",\"Tags\":[\"Smart Grid\",\"IoT\",\"Energy\",\"Optimization\"],\"Metrics\":{\"Grid Efficiency\":\"\\u002B22%\",\"Energy Savings\":\"18%\"},\"Metadata\":{},\"ProgressPercentage\":55,\"Notes\":null,\"BudgetSpent\":264000,\"Outcome\":null,\"RiskLevel\":\"Medium\",\"Deliverables\":[\"SCADA System\",\"Analytics Engine\",\"Mobile App\",\"Training\"],\"Dependencies\":[]}", "Austin", "Smart Grid Management System", "PROJ-2024-022", null },
                    { 23, "SFMP", 10, new DateTime(2023, 9, 27, 0, 0, 0, 0, DateTimeKind.Utc), "{\"Budget\":185000,\"Status\":\"Completed\",\"StartDate\":\"2023-09-27T00:00:00Z\",\"EndDate\":\"2024-09-27T00:00:00Z\",\"Description\":\"Develop real-time monitoring system for solar farm operations\",\"ProjectManager\":\"Mark Stevens\",\"TeamMembers\":[\"Mark Stevens\",\"IoT Specialist\"],\"Priority\":\"High\",\"Tags\":[\"Solar\",\"IoT\",\"Monitoring\",\"Renewable\"],\"Metrics\":{},\"Metadata\":{},\"ProgressPercentage\":100,\"Notes\":null,\"BudgetSpent\":185000,\"Outcome\":\"Successfully deployed to 50 solar farms, achieving 99.2% uptime\",\"RiskLevel\":\"Low\",\"Deliverables\":[\"Monitoring Platform\",\"Mobile App\",\"Alert System\"],\"Dependencies\":[]}", "Austin", "Solar Farm Monitoring Platform", "PROJ-2024-023", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_Code",
                table: "Projects",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId",
                table: "Projects",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId_Code",
                table: "Projects",
                columns: new[] { "CompanyId", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_CompanyId_Location_Code",
                table: "Projects",
                columns: new[] { "CompanyId", "Location", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_Location",
                table: "Projects",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_Project_Location_Code",
                table: "Projects",
                columns: new[] { "Location", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_Location_Name_Id",
                table: "Projects",
                columns: new[] { "Location", "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Project_Name",
                table: "Projects",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectIdentifier",
                table: "Projects",
                column: "ProjectIdentifier",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
