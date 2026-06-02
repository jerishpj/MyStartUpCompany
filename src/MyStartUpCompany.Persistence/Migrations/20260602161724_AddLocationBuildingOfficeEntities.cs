using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationBuildingOfficeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ManagerName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BuildingCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NumberOfFloors = table.Column<int>(type: "int", nullable: true),
                    YearConstructed = table.Column<int>(type: "int", nullable: true),
                    TotalFloorArea = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Buildings_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Offices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OfficeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FloorNumber = table.Column<int>(type: "int", nullable: true),
                    Section = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    OfficeType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SquareMeters = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Manager = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Offices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Offices_Buildings_BuildingId",
                        column: x => x.BuildingId,
                        principalTable: "Buildings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Building_Active_Location",
                table: "Buildings",
                columns: new[] { "IsActive", "LocationId" });

            migrationBuilder.CreateIndex(
                name: "IX_Building_Code",
                table: "Buildings",
                column: "BuildingCode");

            migrationBuilder.CreateIndex(
                name: "IX_Building_Location_Active",
                table: "Buildings",
                columns: new[] { "LocationId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Building_LocationId",
                table: "Buildings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Building_Name",
                table: "Buildings",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Active_Company",
                table: "Locations",
                columns: new[] { "IsActive", "CompanyId" });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Company_Active",
                table: "Locations",
                columns: new[] { "CompanyId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Location_CompanyId",
                table: "Locations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Location_Geographic",
                table: "Locations",
                columns: new[] { "Country", "Region", "City" });

            migrationBuilder.CreateIndex(
                name: "IX_Location_Name",
                table: "Locations",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Active_Building",
                table: "Offices",
                columns: new[] { "IsActive", "BuildingId" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_Building_Active",
                table: "Offices",
                columns: new[] { "BuildingId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_BuildingId",
                table: "Offices",
                column: "BuildingId");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Code",
                table: "Offices",
                column: "OfficeCode");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Department",
                table: "Offices",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Name",
                table: "Offices",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Type",
                table: "Offices",
                column: "OfficeType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Offices");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
