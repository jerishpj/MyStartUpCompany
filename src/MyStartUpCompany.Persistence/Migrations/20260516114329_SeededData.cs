using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyStartUpCompany.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Id", "Address", "City", "Country", "Description", "Name", "Phone", "PostalCode", "Region" },
                values: new object[,]
                {
                    { 1, "1234 Silicon Valley Blvd", "San Francisco", "United States", "Leading AI and machine learning solutions provider", "TechVision Inc", "+1-415-555-0101", "94102", "CA" },
                    { 2, "5678 Innovation Drive", "San Jose", "United States", "Enterprise cloud infrastructure services", "CloudSync Solutions", "+1-408-555-0102", "95110", "CA" },
                    { 3, "9012 Tech Park Way", "Palo Alto", "United States", "Big data analytics and visualization platform", "DataFlow Analytics", "+1-650-555-0103", "94301", "CA" },
                    { 4, "3456 Cyber Lane", "San Diego", "United States", "Cybersecurity solutions for modern enterprises", "SecureNet Systems", "+1-619-555-0104", "92101", "CA" },
                    { 5, "7890 Automation Street", "Los Angeles", "United States", "CI/CD and DevOps automation specialists", "DevOps Masters", "+1-213-555-0105", "90001", "CA" },
                    { 6, "2468 Research Boulevard", "Mountain View", "United States", "Next-generation quantum computing research", "Quantum Computing Labs", "+1-650-555-0106", "94040", "CA" },
                    { 7, "1357 App Street", "Cupertino", "United States", "iOS and Android application development", "Mobile First Studios", "+1-408-555-0107", "95014", "CA" },
                    { 8, "9753 Solar Avenue", "Sacramento", "United States", "Renewable energy management systems", "Green Energy Tech", "+1-916-555-0108", "95814", "CA" },
                    { 9, "4682 Medical Plaza", "Irvine", "United States", "Medical device software and telemedicine", "HealthTech Innovations", "+1-949-555-0109", "92602", "CA" },
                    { 10, "1111 Gaming Way", "San Francisco", "United States", "Video game development and publishing", "GameDev Studios", "+1-415-555-0110", "94103", "CA" },
                    { 11, "2222 Learning Lane", "Oakland", "United States", "Educational technology and e-learning platforms", "EduTech Solutions", "+1-510-555-0111", "94612", "CA" },
                    { 12, "3333 Crypto Court", "Berkeley", "United States", "Cryptocurrency and blockchain development", "Blockchain Ventures", "+1-510-555-0112", "94704", "CA" },
                    { 13, "4444 Vision Drive", "Santa Clara", "United States", "Computer vision and image recognition", "AI Vision Systems", "+1-408-555-0113", "95050", "CA" },
                    { 14, "5555 Aviation Road", "Fremont", "United States", "Commercial drone software and services", "Drone Dynamics", "+1-510-555-0114", "94538", "CA" },
                    { 15, "6666 Network Street", "Menlo Park", "United States", "Social media analytics and management", "Social Connect Platform", "+1-650-555-0115", "94025", "CA" },
                    { 16, "100 Wall Street", "New York", "United States", "Financial technology and blockchain services", "FinTech Solutions LLC", "+1-212-555-0201", "10005", "NY" },
                    { 17, "250 Broadway", "New York", "United States", "Smart city data analytics platform", "Urban Analytics Corp", "+1-212-555-0202", "10007", "NY" },
                    { 18, "42 Madison Avenue", "New York", "United States", "Video streaming and content delivery network", "MediaStream Technologies", "+1-212-555-0203", "10010", "NY" },
                    { 19, "150 Wall Street", "New York", "United States", "Algorithmic trading and market analysis", "Trading Algorithms Inc", "+1-212-555-0204", "10005", "NY" },
                    { 20, "333 Fifth Avenue", "New York", "United States", "Performance marketing and ad tech", "Digital Marketing Hub", "+1-212-555-0205", "10016", "NY" },
                    { 21, "444 Seventh Avenue", "New York", "United States", "E-commerce and fashion technology", "Fashion Tech Lab", "+1-212-555-0206", "10018", "NY" },
                    { 22, "555 Park Avenue", "New York", "United States", "Property tech and real estate data", "Real Estate Analytics", "+1-212-555-0207", "10022", "NY" },
                    { 23, "666 Lexington Avenue", "New York", "United States", "Insurance technology solutions", "Insurance Tech Group", "+1-212-555-0208", "10022", "NY" },
                    { 24, "777 Third Avenue", "New York", "United States", "Legal practice management software", "Legal Tech Partners", "+1-212-555-0209", "10017", "NY" },
                    { 25, "100 MetroTech Center", "Brooklyn", "United States", "Custom software development agency", "Brooklyn Software Collective", "+1-718-555-0210", "11201", "NY" },
                    { 26, "200 Main Street", "Buffalo", "United States", "Manufacturing automation software", "Buffalo Tech Innovations", "+1-716-555-0211", "14202", "NY" },
                    { 27, "300 East Avenue", "Rochester", "United States", "Enterprise data management solutions", "Rochester Data Systems", "+1-585-555-0212", "14604", "NY" },
                    { 28, "1500 Energy Plaza", "Houston", "United States", "Smart grid and renewable energy solutions", "Energy Grid Innovations", "+1-713-555-0301", "77002", "TX" },
                    { 29, "600 Congress Avenue", "Austin", "United States", "Custom software development and consulting", "Austin Code Labs", "+1-512-555-0302", "78701", "TX" },
                    { 30, "2000 Ross Avenue", "Dallas", "United States", "Enterprise software solutions provider", "Lone Star Tech", "+1-214-555-0303", "75201", "TX" },
                    { 31, "1600 Louisiana Street", "Houston", "United States", "Energy sector data analytics", "Oil & Gas Analytics", "+1-713-555-0304", "77002", "TX" },
                    { 32, "700 Red River Street", "Austin", "United States", "Event management and ticketing software", "Live Music Tech", "+1-512-555-0305", "78701", "TX" },
                    { 33, "2100 Pearl Street", "Dallas", "United States", "Telecom infrastructure and 5G technology", "Telecommunications Solutions", "+1-214-555-0306", "75201", "TX" },
                    { 34, "800 Dolorosa Street", "San Antonio", "United States", "Military-grade security systems", "San Antonio Cybersecurity", "+1-210-555-0307", "78205", "TX" },
                    { 35, "900 Commerce Street", "Fort Worth", "United States", "Supply chain management software", "Fort Worth Logistics Tech", "+1-817-555-0308", "76102", "TX" },
                    { 36, "1000 Space Center Boulevard", "Houston", "United States", "Aerospace software and satellite systems", "Space Tech Ventures", "+1-281-555-0309", "77058", "TX" },
                    { 37, "1100 South Lamar", "Austin", "United States", "Mobile and console game development", "Austin Gaming Studios", "+1-512-555-0310", "78704", "TX" },
                    { 38, "1201 Third Avenue", "Seattle", "United States", "Multi-cloud management platform", "Pacific Cloud Services", "+1-206-555-0401", "98101", "WA" },
                    { 39, "500 Pike Street", "Seattle", "United States", "Healthcare IT solutions", "Emerald Software Group", "+1-206-555-0402", "98101", "WA" },
                    { 40, "410 Terry Avenue North", "Seattle", "United States", "Online marketplace technology", "E-Commerce Platform Inc", "+1-206-555-0403", "98109", "WA" },
                    { 41, "2401 Utah Avenue South", "Seattle", "United States", "Restaurant and cafe POS systems", "Coffee Tech Solutions", "+1-206-555-0404", "98134", "WA" },
                    { 42, "100 North Riverside Avenue", "Renton", "United States", "Flight simulation and training software", "Aerospace Simulation Systems", "+1-425-555-0405", "98057", "WA" },
                    { 43, "1000 Denny Way", "Seattle", "United States", "Colocation and edge computing", "Northwest Data Center", "+1-206-555-0406", "98109", "WA" },
                    { 44, "808 West Spokane Falls Boulevard", "Spokane", "United States", "Custom enterprise applications", "Spokane Software Works", "+1-509-555-0407", "99201", "WA" },
                    { 45, "1950 Pacific Avenue", "Tacoma", "United States", "Industrial Internet of Things solutions", "Tacoma Industrial IoT", "+1-253-555-0408", "98402", "WA" },
                    { 46, "75 State Street", "Boston", "United States", "Biotechnology research and development", "BioTech Innovations", "+1-617-555-0501", "02109", "MA" },
                    { 47, "1 Memorial Drive", "Cambridge", "United States", "Artificial intelligence research lab", "Cambridge AI Research", "+1-617-555-0502", "02142", "MA" },
                    { 48, "200 Berkeley Street", "Boston", "United States", "Medical device software and diagnostics", "MedTech Solutions", "+1-617-555-0503", "02116", "MA" },
                    { 49, "77 Massachusetts Avenue", "Cambridge", "United States", "Autonomous robotics systems", "Robotics Engineering Lab", "+1-617-555-0504", "02139", "MA" },
                    { 50, "300 The Fenway", "Boston", "United States", "Digital textbooks and learning platforms", "EdTech Publishing", "+1-617-555-0505", "02115", "MA" },
                    { 51, "100 Front Street", "Worcester", "United States", "Technology incubator and coworking", "Worcester Innovation Hub", "+1-508-555-0506", "01608", "MA" },
                    { 52, "500 Boylston Street", "Boston", "United States", "Quantum encryption technology", "Quantum Security Systems", "+1-617-555-0507", "02116", "MA" },
                    { 53, "233 South Wacker Drive", "Chicago", "United States", "Enterprise resource planning systems", "WindyCity Solutions", "+1-312-555-0601", "60606", "IL" },
                    { 54, "444 West Lake Street", "Chicago", "United States", "Colocation and managed hosting services", "Midwest Data Center", "+1-312-555-0602", "60661", "IL" },
                    { 55, "141 West Jackson Boulevard", "Chicago", "United States", "Commodities trading software", "Trading Platform Technologies", "+1-312-555-0603", "60604", "IL" },
                    { 56, "225 North Michigan Avenue", "Chicago", "United States", "Rail and freight optimization", "Transportation Logistics AI", "+1-312-555-0604", "60601", "IL" },
                    { 57, "100 West Monroe Street", "Chicago", "United States", "Restaurant management systems", "Food Service Tech", "+1-312-555-0605", "60603", "IL" },
                    { 58, "1 North Dearborn Street", "Chicago", "United States", "Industrial process optimization", "Manufacturing Analytics Pro", "+1-312-555-0606", "60602", "IL" },
                    { 59, "1000 Brickell Avenue", "Miami", "United States", "Mobile app development studio", "Sunshine Software", "+1-305-555-0701", "33131", "FL" },
                    { 60, "100 West Livingston Street", "Orlando", "United States", "Technology incubator and coworking space", "Orlando Tech Hub", "+1-407-555-0702", "32801", "FL" },
                    { 61, "1000 Universal Studios Plaza", "Orlando", "United States", "Attraction management and ticketing", "Theme Park Systems", "+1-407-555-0703", "32819", "FL" },
                    { 62, "2000 Biscayne Boulevard", "Miami", "United States", "Maritime navigation and booking systems", "Cruise Line Technologies", "+1-305-555-0704", "33137", "FL" },
                    { 63, "3000 Bayshore Boulevard", "Tampa", "United States", "Healthcare management platforms", "Tampa Bay HealthTech", "+1-813-555-0705", "33629", "FL" },
                    { 64, "4000 NASA Parkway", "Cape Canaveral", "United States", "Aerospace engineering software", "Space Coast Engineering", "+1-321-555-0706", "32920", "FL" },
                    { 65, "1700 Lincoln Street", "Denver", "United States", "Cloud infrastructure and SaaS", "Mile High Software", "+1-303-555-0801", "80203", "CO" },
                    { 66, "191 Peachtree Street", "Atlanta", "United States", "Payment processing solutions", "Atlanta FinTech Group", "+1-404-555-0802", "30303", "GA" },
                    { 67, "100 West Washington Street", "Phoenix", "United States", "Enterprise data warehousing", "Phoenix Data Solutions", "+1-602-555-0803", "85003", "AZ" },
                    { 68, "1000 Southwest Broadway", "Portland", "United States", "Open source software development", "Portland Open Source", "+1-503-555-0804", "97205", "OR" },
                    { 69, "1 Woodward Avenue", "Detroit", "United States", "Automotive software and IoT", "Detroit Auto Tech", "+1-313-555-0805", "48226", "MI" },
                    { 70, "1234 Market Street", "Philadelphia", "United States", "Electronic health records systems", "Philadelphia HealthCare IT", "+1-215-555-0806", "19107", "PA" },
                    { 71, "3799 Las Vegas Boulevard", "Las Vegas", "United States", "Casino and gaming software", "Las Vegas Gaming Tech", "+1-702-555-0807", "89109", "NV" },
                    { 72, "150 Fourth Avenue North", "Nashville", "United States", "Music production and streaming tech", "Nashville Music Software", "+1-615-555-0808", "37219", "TN" },
                    { 73, "50 East South Temple", "Salt Lake City", "United States", "Outdoor recreation and fitness apps", "Salt Lake Outdoor Tech", "+1-801-555-0809", "84111", "UT" },
                    { 74, "100 North Tryon Street", "Charlotte", "United States", "Core banking software", "Charlotte Banking Systems", "+1-704-555-0810", "28202", "NC" },
                    { 75, "50 South Sixth Street", "Minneapolis", "United States", "Agricultural technology and farm management", "Minneapolis AgriTech", "+1-612-555-0811", "55402", "MN" },
                    { 76, "1100 Main Street", "Kansas City", "United States", "Warehouse automation and robotics", "Kansas City Logistics AI", "+1-816-555-0812", "64105", "MO" },
                    { 77, "10 Downing Street", "London", "United Kingdom", "Digital marketing and web development", "London Digital Agency", "+44-20-7946-0901", "SW1A 2AA", "England" },
                    { 78, "100 King Street West", "Toronto", "Canada", "Software consulting and training", "Toronto Tech Collective", "+1-416-555-0902", "M5X 1A9", "ON" },
                    { 79, "1 Macquarie Place", "Sydney", "Australia", "Cloud migration specialists", "Sydney Cloud Partners", "+61-2-9555-0903", "2000", "NSW" },
                    { 80, "Unter den Linden 77", "Berlin", "Germany", "IoT and embedded systems development", "Berlin Innovation Labs", "+49-30-2555-0904", "10117", "BE" },
                    { 81, "1 Marina Boulevard", "Singapore", "Singapore", "Financial services technology platform", "Singapore FinTech Hub", "+65-6555-0905", "018989", null },
                    { 82, "1-1-1 Shibuya", "Tokyo", "Japan", "Industrial automation and robotics", "Tokyo Robotics Corp", "+81-3-5555-0906", "150-0002", null },
                    { 83, "10 Avenue des Champs-Élysées", "Paris", "France", "Luxury fashion e-commerce platform", "Paris Fashion Tech", "+33-1-4555-0907", "75008", "IDF" },
                    { 84, "Science Park 904", "Amsterdam", "Netherlands", "Machine learning and data analytics", "Amsterdam Data Science", "+31-20-5555-0908", "1098 XH", "NH" },
                    { 85, "70 Sir John Rogerson's Quay", "Dublin", "Ireland", "European cloud infrastructure provider", "Dublin Cloud Services", "+353-1-555-0909", "D02 R296", "D" },
                    { 86, "Sveavägen 46", "Stockholm", "Sweden", "Sustainable technology solutions", "Stockholm GreenTech", "+46-8-555-0910", "111 34", "AB" },
                    { 87, "MG Road 100", "Bangalore", "India", "Offshore development and IT services", "Bangalore Software Solutions", "+91-80-5555-0911", "560001", "KA" },
                    { 88, "Avenida Paulista 1000", "São Paulo", "Brazil", "Latin American e-commerce platform", "São Paulo Digital", "+55-11-5555-0912", "01310-100", "SP" },
                    { 89, "1055 West Hastings Street", "Vancouver", "Canada", "Cross-platform mobile application development", "Vancouver Mobile Apps", "+1-604-555-1001", "V6E 2E9", "BC" },
                    { 90, "120 Collins Street", "Melbourne", "Australia", "Business intelligence and analytics platform", "Melbourne Analytics Hub", "+61-3-9555-1002", "3000", "VIC" },
                    { 91, "Bahnhofstrasse 50", "Zurich", "Switzerland", "Enterprise security and compliance software", "Zurich SecureTech", "+41-44-555-1003", "8001", "ZH" },
                    { 92, "123 Gangnam-daero", "Seoul", "South Korea", "Mobile and PC game development", "Seoul Game Studios", "+82-2-555-1004", "06236", null },
                    { 93, "1 International Finance Centre", "Hong Kong", "Hong Kong", "Financial services technology solutions", "Hong Kong FinServe", "+852-2555-1005", "999077", null },
                    { 94, "Passeig de Gràcia 85", "Barcelona", "Spain", "Urban technology and IoT solutions", "Barcelona Smart City", "+34-93-555-1006", "08008", "CAT" },
                    { 95, "Vesterbrogade 6A", "Copenhagen", "Denmark", "Sustainable technology and clean energy software", "Copenhagen Green Solutions", "+45-33-555-1007", "1620", null },
                    { 96, "Rothschild Boulevard 100", "Tel Aviv", "Israel", "Cybersecurity and AI research", "Tel Aviv Innovation Labs", "+972-3-555-1008", "6688101", null },
                    { 97, "Sheikh Zayed Road", "Dubai", "United Arab Emirates", "Enterprise digital transformation services", "Dubai Tech Hub", "+971-4-555-1009", "00000", null },
                    { 98, "Stephansplatz 1", "Vienna", "Austria", "Data analytics and business intelligence", "Vienna Data Systems", "+43-1-555-1010", "1010", "W" },
                    { 99, "Karl Johans gate 10", "Oslo", "Norway", "Marine navigation and shipping software", "Oslo Maritime Tech", "+47-22-555-1011", "0154", null },
                    { 100, "Rue de la Loi 200", "Brussels", "Belgium", "Government and regulatory compliance software", "Brussels EU Tech", "+32-2-555-1012", "1049", "BRU" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Employees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Employees",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Employees",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Employees",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Employees",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Companies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Companies",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Companies",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Companies",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Companies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
