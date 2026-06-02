using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedLocationBuildingOfficeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Locations
            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "CompanyId", "Name", "Description", "Address", "City", "Region", "PostalCode", "Country", "Phone", "Email", "ManagerName", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "New York Headquarters", "Main headquarters office", "350 Fifth Avenue", "New York", "NY", "10118", "United States", "+1-212-555-0100", "nyc@techcorp.com", "John Smith", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 1, "San Francisco Innovation Hub", "West Coast innovation center", "1 Market Street", "San Francisco", "CA", "94103", "United States", "+1-415-555-0101", "sf@techcorp.com", "Sarah Johnson", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 1, "Los Angeles West Coast Office", "Southern California office", "2049 Century Park East", "Los Angeles", "CA", "90067", "United States", "+1-310-555-0102", "la@techcorp.com", "Michael Chen", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 2, "Seattle Development Center", "Software development hub", "410 Terry Avenue North", "Seattle", "WA", "98109", "United States", "+1-206-555-0103", "seattle@innovsoft.com", "Emma Davis", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 2, "Portland Office", "Regional office", "1000 SW Broadway", "Portland", "OR", "97205", "United States", "+1-503-555-0104", "portland@innovsoft.com", "David Wilson", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 3, "Austin Headquarters", "Headquarters and main office", "600 Congress Avenue", "Austin", "TX", "78701", "United States", "+1-512-555-0105", "austin@codelab.com", "Lisa Anderson", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 3, "Dallas Regional Office", "Texas region office", "2000 Ross Avenue", "Dallas", "TX", "75201", "United States", "+1-214-555-0106", "dallas@codelab.com", "Robert Miller", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 4, "Boston Research Center", "R&D and research facility", "75 State Street", "Boston", "MA", "02109", "United States", "+1-617-555-0107", "boston@biotech.com", "Jennifer Taylor", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, 5, "Chicago Main Office", "Central region headquarters", "233 South Wacker Drive", "Chicago", "IL", "60606", "United States", "+1-312-555-0108", "chicago@windycity.com", "Thomas Brown", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, 6, "Miami Beach Office", "Southeast region office", "1000 Brickell Avenue", "Miami", "FL", "33131", "United States", "+1-305-555-0109", "miami@sunshine.com", "Maria Garcia", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null }
                });

            // Seed Buildings
            migrationBuilder.InsertData(
                table: "Buildings",
                columns: new[] { "Id", "LocationId", "Name", "BuildingCode", "Description", "Address", "NumberOfFloors", "YearConstructed", "TotalFloorArea", "ContactPerson", "Phone", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Tower A", "NYC-A", "35-story office tower", "350 Fifth Avenue", 35, 1950, 450000m, "Building Manager 1", "+1-212-555-0200", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 1, "Tower B", "NYC-B", "28-story modern office building", "360 Fifth Avenue", 28, 2015, 380000m, "Building Manager 2", "+1-212-555-0201", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 2, "Innovation Tower", "SF-I", "Sustainable green building", "1 Market Street", 42, 2018, 520000m, "Building Manager 3", "+1-415-555-0202", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 3, "Century Plaza Tower", "LA-C", "40-story commercial tower", "2049 Century Park East", 40, 1975, 480000m, "Building Manager 4", "+1-310-555-0203", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 4, "Denny Building", "SEA-D", "Tech campus main building", "410 Terry Avenue North", 22, 2012, 320000m, "Building Manager 5", "+1-206-555-0204", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 5, "Pearl Street Complex", "POR-P", "Mixed-use office complex", "1000 SW Broadway", 18, 2010, 280000m, "Building Manager 6", "+1-503-555-0205", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 6, "Congress Plaza", "AUS-C", "12-story contemporary office", "600 Congress Avenue", 12, 2008, 180000m, "Building Manager 7", "+1-512-555-0206", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 7, "Ross Avenue Tower", "DAL-R", "32-story office tower", "2000 Ross Avenue", 32, 1990, 420000m, "Building Manager 8", "+1-214-555-0207", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, 8, "State Street Building", "BOS-S", "Research and development facility", "75 State Street", 16, 2016, 240000m, "Building Manager 9", "+1-617-555-0208", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, 9, "Wacker Tower", "CHI-W", "Main office headquarters", "233 South Wacker Drive", 44, 1983, 560000m, "Building Manager 10", "+1-312-555-0209", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null }
                });

            // Seed Offices
            migrationBuilder.InsertData(
                table: "Offices",
                columns: new[] { "Id", "BuildingId", "Name", "OfficeCode", "Description", "FloorNumber", "Section", "Capacity", "OfficeType", "SquareMeters", "Department", "Manager", "Phone", "Email", "IsActive", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, "Executive Suite", "NYC-A-E1", "Executive offices", 35, "East", 50, "Private", 2000m, "Executive", "John Smith", "+1-212-555-0300", "exec@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 2, 1, "Sales Department", "NYC-A-S2", "Sales team office", 30, "South", 120, "Open Office", 3500m, "Sales", "Mark Johnson", "+1-212-555-0301", "sales@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 3, 1, "Engineering", "NYC-A-E3", "Software engineering team", 25, "North", 150, "Open Office", 4200m, "Engineering", "Robert Chen", "+1-212-555-0302", "eng@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 4, 2, "Meeting Rooms", "NYC-B-M1", "Conference and meeting spaces", 20, "Central", 8, "Meeting Room", 1500m, "Facilities", "Angela Davis", "+1-212-555-0303", "meetings@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 5, 3, "Innovation Lab", "SF-I-L1", "Research and development lab", 15, "West", 80, "Open Office", 2800m, "R&D", "Sarah Williams", "+1-415-555-0304", "rnd@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 6, 3, "Product Team", "SF-I-P2", "Product management and design", 10, "East", 60, "Cubicles", 2200m, "Product", "Lisa Park", "+1-415-555-0305", "product@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 7, 4, "Operations", "LA-C-O1", "Operations and management", 38, "North", 100, "Open Office", 3000m, "Operations", "Michael Brown", "+1-310-555-0306", "ops@techcorp.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 8, 5, "Development Squad", "SEA-D-D1", "Development team", 12, "South", 90, "Open Office", 2900m, "Engineering", "David Martinez", "+1-206-555-0307", "dev@innovsoft.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 9, 6, "Administrative", "POR-P-A1", "Administrative support", 8, "West", 30, "Cubicles", 900m, "Administration", "Emma Wilson", "+1-503-555-0308", "admin@innovsoft.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null },
                    { 10, 7, "Core Team", "AUS-C-C1", "Main core development team", 6, "Central", 70, "Open Office", 2100m, "Engineering", "Lisa Anderson", "+1-512-555-0309", "core@codelab.com", true, new DateTime(2025, 6, 2, 0, 0, 0, DateTimeKind.Utc), null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Delete in reverse order of dependencies
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 3);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 4);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 5);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 6);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 7);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 8);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 9);
            migrationBuilder.DeleteData(
                table: "Offices",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 3);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 4);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 5);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 6);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 7);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 8);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 9);
            migrationBuilder.DeleteData(
                table: "Buildings",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 9);
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
