using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProjectTypeReferenceMaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProjectTypeReferences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnumName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Enum member name matching ProjectType enum"),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, comment: "User-friendly display name for UI/reports"),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Detailed description of the project type"),
                    IconIdentifier = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "Icon or emoji identifier for UI (e.g., '🎮', '☁️')"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, comment: "Sort order for UI dropdowns"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, comment: "Is this project type currently active/usable"),
                    ColorCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true, comment: "Hex color code for UI categorization (e.g., '#FF5733')"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "When this reference was created"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When this reference was last updated")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTypeReferences", x => x.Id);
                },
                comment: "Reference data for all valid ProjectType enum values");

            migrationBuilder.InsertData(
                table: "ProjectTypeReferences",
                columns: new[] { "Id", "ColorCode", "CreatedAt", "Description", "DisplayName", "DisplayOrder", "EnumName", "IconIdentifier", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "#FF6B6B", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Game design, engine development, art, audio, and game programming projects", "Game Development", 1, "GameDevelopment", "🎮", true, null },
                    { 2, "#4ECDC4", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cloud infrastructure, migration, deployment, and cloud-native application projects", "Cloud Service", 2, "CloudService", "☁️", true, null },
                    { 3, "#95E1D3", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Customer support platforms, ticketing systems, and customer engagement tools", "Customer Support", 3, "CustomerSupport", "🎧", true, null },
                    { 4, "#F38181", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Data pipelines, analytics engines, reporting, BI tools, and data warehouse projects", "Data Analytics", 4, "DataAnalytics", "📊", true, null },
                    { 5, "#AA96DA", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CI/CD pipelines, monitoring systems, DevOps, container orchestration, and infrastructure management", "Infrastructure", 5, "Infrastructure", "⚙️", true, null },
                    { 6, "#FCBAD3", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Security implementation, compliance, auditing, penetration testing, and security infrastructure", "Security", 6, "Security", "🔒", true, null },
                    { 7, "#FFFFD2", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "iOS, Android, and cross-platform mobile application development projects", "Mobile App", 7, "MobileApp", "📱", true, null },
                    { 8, "#80D8FF", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Frontend and backend web application development, SPA projects, and web services", "Web Application", 8, "WebApplication", "🌐", true, null },
                    { 9, "#B2DFDB", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "REST APIs, GraphQL, gRPC, and other API interface development and integration projects", "API Development", 9, "ApiDevelopment", "🔗", true, null },
                    { 10, "#FFB74D", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ML model development, AI implementations, training pipelines, and predictive analytics", "Machine Learning", 10, "MachineLearning", "🤖", true, null },
                    { 11, "#CE93D8", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System integration, middleware development, data interchange, and enterprise integration projects", "Integration", 11, "Integration", "🔀", true, null },
                    { 12, "#64B5F6", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Research and POC (Proof of Concept) projects, experimentation, and prototyping", "Research", 12, "Research", "🔬", true, null },
                    { 13, "#B0BEC5", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Miscellaneous projects that don't fit into standard categories", "Other", 13, "Other", "📦", true, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTypeReferences_EnumName",
                table: "ProjectTypeReferences",
                column: "EnumName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProjectTypeReferences");
        }
    }
}
