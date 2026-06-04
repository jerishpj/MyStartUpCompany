using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDenormalizedFieldsAndTriggers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BuildingName",
                table: "Offices",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCity",
                table: "Offices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationCountry",
                table: "Offices",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationRegion",
                table: "Offices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Office_Active_Building_Department",
                table: "Offices",
                columns: new[] { "IsActive", "BuildingName", "Department" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_Active_Location_Geographic",
                table: "Offices",
                columns: new[] { "IsActive", "LocationCity", "LocationCountry" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_Building_Type_Active",
                table: "Offices",
                columns: new[] { "BuildingName", "OfficeType", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_BuildingName",
                table: "Offices",
                column: "BuildingName");

            migrationBuilder.CreateIndex(
                name: "IX_Office_Location_Department_Active",
                table: "Offices",
                columns: new[] { "LocationCity", "Department", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Office_Location_Geographic",
                table: "Offices",
                columns: new[] { "LocationCountry", "LocationRegion", "LocationCity" });

            // ========== POPULATE INITIAL VALUES FROM EXISTING DATA ==========
            // Join Office -> Building -> Location to populate denormalized fields
            migrationBuilder.Sql(@"
                UPDATE o
                SET 
                    o.BuildingName = b.Name,
                    o.LocationCity = l.City,
                    o.LocationRegion = l.Region,
                    o.LocationCountry = l.Country
                FROM Offices o
                INNER JOIN Buildings b ON o.BuildingId = b.Id
                INNER JOIN Locations l ON b.LocationId = l.Id
                WHERE b.Id IS NOT NULL;
            ");

            // ========== DATABASE TRIGGERS FOR AUTOMATIC SYNCHRONIZATION ==========
            // These triggers ensure denormalized fields stay in sync with Building and Location updates

            // Trigger: Update denormalized fields when a Building is updated
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_Building_Update_Offices_Denormalized
                ON Buildings
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE o
                    SET 
                        o.BuildingName = i.Name,
                        o.UpdatedAt = GETUTCDATE()
                    FROM Offices o
                    INNER JOIN inserted i ON o.BuildingId = i.Id
                    WHERE o.BuildingId IN (SELECT Id FROM inserted);
                END;
            ");

            // Trigger: Update denormalized location fields when a Location is updated
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_Location_Update_Offices_Denormalized
                ON Locations
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE o
                    SET 
                        o.LocationCity = i.City,
                        o.LocationRegion = i.Region,
                        o.LocationCountry = i.Country,
                        o.UpdatedAt = GETUTCDATE()
                    FROM Offices o
                    INNER JOIN Buildings b ON o.BuildingId = b.Id
                    INNER JOIN inserted i ON b.LocationId = i.Id
                    WHERE b.LocationId IN (SELECT Id FROM inserted);
                END;
            ");

            // Trigger: Populate denormalized fields when a new Office is inserted
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_Office_Insert_Populate_Denormalized
                ON Offices
                AFTER INSERT
                AS
                BEGIN
                    SET NOCOUNT ON;

                    UPDATE o
                    SET 
                        o.BuildingName = b.Name,
                        o.LocationCity = l.City,
                        o.LocationRegion = l.Region,
                        o.LocationCountry = l.Country
                    FROM Offices o
                    INNER JOIN inserted i ON o.Id = i.Id
                    INNER JOIN Buildings b ON o.BuildingId = b.Id
                    INNER JOIN Locations l ON b.LocationId = l.Id;
                END;
            ");

            // Trigger: Handle cascading updates when Building's Location changes
            migrationBuilder.Sql(@"
                CREATE TRIGGER tr_Building_LocationId_Update_Offices_Denormalized
                ON Buildings
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Check if LocationId was changed
                    IF UPDATE(LocationId)
                    BEGIN
                        UPDATE o
                        SET 
                            o.LocationCity = l.City,
                            o.LocationRegion = l.Region,
                            o.LocationCountry = l.Country,
                            o.UpdatedAt = GETUTCDATE()
                        FROM Offices o
                        INNER JOIN inserted i ON o.BuildingId = i.Id
                        INNER JOIN Locations l ON i.LocationId = l.Id
                        WHERE i.LocationId IN (SELECT LocationId FROM inserted);
                    END;
                END;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop triggers
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_Office_Insert_Populate_Denormalized;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_Building_LocationId_Update_Offices_Denormalized;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_Location_Update_Offices_Denormalized;");
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS tr_Building_Update_Offices_Denormalized;");

            migrationBuilder.DropIndex(
                name: "IX_Office_Active_Building_Department",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Office_Active_Location_Geographic",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Office_Building_Type_Active",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Office_BuildingName",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Office_Location_Department_Active",
                table: "Offices");

            migrationBuilder.DropIndex(
                name: "IX_Office_Location_Geographic",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "BuildingName",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "LocationCity",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "LocationCountry",
                table: "Offices");

            migrationBuilder.DropColumn(
                name: "LocationRegion",
                table: "Offices");
        }
    }
}
