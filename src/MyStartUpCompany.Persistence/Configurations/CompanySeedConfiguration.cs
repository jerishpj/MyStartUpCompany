using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyStartUpCompany.Persistence.Entities;

namespace MyStartUpCompany.Persistence.Configurations;

/// <summary>
/// Seed data configuration for Company entity with 100 records
/// </summary>
public class CompanySeedConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasData(GetSeedData());
    }

    private static IEnumerable<Company> GetSeedData()
    {
        var companies = new List<Company>();
        int id = 1;

        // ========== CALIFORNIA - 15 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "TechVision Inc",
                Description = "Leading AI and machine learning solutions provider",
                Address = "1234 Silicon Valley Blvd",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94102",
                Country = "United States",
                Phone = "+1-415-555-0101"
            },
            new Company
            {
                Id = id++,
                Name = "CloudSync Solutions",
                Description = "Enterprise cloud infrastructure services",
                Address = "5678 Innovation Drive",
                City = "San Jose",
                Region = "CA",
                PostalCode = "95110",
                Country = "United States",
                Phone = "+1-408-555-0102"
            },
            new Company
            {
                Id = id++,
                Name = "DataFlow Analytics",
                Description = "Big data analytics and visualization platform",
                Address = "9012 Tech Park Way",
                City = "Palo Alto",
                Region = "CA",
                PostalCode = "94301",
                Country = "United States",
                Phone = "+1-650-555-0103"
            },
            new Company
            {
                Id = id++,
                Name = "SecureNet Systems",
                Description = "Cybersecurity solutions for modern enterprises",
                Address = "3456 Cyber Lane",
                City = "San Diego",
                Region = "CA",
                PostalCode = "92101",
                Country = "United States",
                Phone = "+1-619-555-0104"
            },
            new Company
            {
                Id = id++,
                Name = "DevOps Masters",
                Description = "CI/CD and DevOps automation specialists",
                Address = "7890 Automation Street",
                City = "Los Angeles",
                Region = "CA",
                PostalCode = "90001",
                Country = "United States",
                Phone = "+1-213-555-0105"
            },
            new Company
            {
                Id = id++,
                Name = "Quantum Computing Labs",
                Description = "Next-generation quantum computing research",
                Address = "2468 Research Boulevard",
                City = "Mountain View",
                Region = "CA",
                PostalCode = "94040",
                Country = "United States",
                Phone = "+1-650-555-0106"
            },
            new Company
            {
                Id = id++,
                Name = "Mobile First Studios",
                Description = "iOS and Android application development",
                Address = "1357 App Street",
                City = "Cupertino",
                Region = "CA",
                PostalCode = "95014",
                Country = "United States",
                Phone = "+1-408-555-0107"
            },
            new Company
            {
                Id = id++,
                Name = "Green Energy Tech",
                Description = "Renewable energy management systems",
                Address = "9753 Solar Avenue",
                City = "Sacramento",
                Region = "CA",
                PostalCode = "95814",
                Country = "United States",
                Phone = "+1-916-555-0108"
            },
            new Company
            {
                Id = id++,
                Name = "HealthTech Innovations",
                Description = "Medical device software and telemedicine",
                Address = "4682 Medical Plaza",
                City = "Irvine",
                Region = "CA",
                PostalCode = "92602",
                Country = "United States",
                Phone = "+1-949-555-0109"
            },
            new Company
            {
                Id = id++,
                Name = "GameDev Studios",
                Description = "Video game development and publishing",
                Address = "1111 Gaming Way",
                City = "San Francisco",
                Region = "CA",
                PostalCode = "94103",
                Country = "United States",
                Phone = "+1-415-555-0110"
            },
            new Company
            {
                Id = id++,
                Name = "EduTech Solutions",
                Description = "Educational technology and e-learning platforms",
                Address = "2222 Learning Lane",
                City = "Oakland",
                Region = "CA",
                PostalCode = "94612",
                Country = "United States",
                Phone = "+1-510-555-0111"
            },
            new Company
            {
                Id = id++,
                Name = "Blockchain Ventures",
                Description = "Cryptocurrency and blockchain development",
                Address = "3333 Crypto Court",
                City = "Berkeley",
                Region = "CA",
                PostalCode = "94704",
                Country = "United States",
                Phone = "+1-510-555-0112"
            },
            new Company
            {
                Id = id++,
                Name = "AI Vision Systems",
                Description = "Computer vision and image recognition",
                Address = "4444 Vision Drive",
                City = "Santa Clara",
                Region = "CA",
                PostalCode = "95050",
                Country = "United States",
                Phone = "+1-408-555-0113"
            },
            new Company
            {
                Id = id++,
                Name = "Drone Dynamics",
                Description = "Commercial drone software and services",
                Address = "5555 Aviation Road",
                City = "Fremont",
                Region = "CA",
                PostalCode = "94538",
                Country = "United States",
                Phone = "+1-510-555-0114"
            },
            new Company
            {
                Id = id++,
                Name = "Social Connect Platform",
                Description = "Social media analytics and management",
                Address = "6666 Network Street",
                City = "Menlo Park",
                Region = "CA",
                PostalCode = "94025",
                Country = "United States",
                Phone = "+1-650-555-0115"
            }
        });

        // ========== NEW YORK - 12 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "FinTech Solutions LLC",
                Description = "Financial technology and blockchain services",
                Address = "100 Wall Street",
                City = "New York",
                Region = "NY",
                PostalCode = "10005",
                Country = "United States",
                Phone = "+1-212-555-0201"
            },
            new Company
            {
                Id = id++,
                Name = "Urban Analytics Corp",
                Description = "Smart city data analytics platform",
                Address = "250 Broadway",
                City = "New York",
                Region = "NY",
                PostalCode = "10007",
                Country = "United States",
                Phone = "+1-212-555-0202"
            },
            new Company
            {
                Id = id++,
                Name = "MediaStream Technologies",
                Description = "Video streaming and content delivery network",
                Address = "42 Madison Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10010",
                Country = "United States",
                Phone = "+1-212-555-0203"
            },
            new Company
            {
                Id = id++,
                Name = "Trading Algorithms Inc",
                Description = "Algorithmic trading and market analysis",
                Address = "150 Wall Street",
                City = "New York",
                Region = "NY",
                PostalCode = "10005",
                Country = "United States",
                Phone = "+1-212-555-0204"
            },
            new Company
            {
                Id = id++,
                Name = "Digital Marketing Hub",
                Description = "Performance marketing and ad tech",
                Address = "333 Fifth Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10016",
                Country = "United States",
                Phone = "+1-212-555-0205"
            },
            new Company
            {
                Id = id++,
                Name = "Fashion Tech Lab",
                Description = "E-commerce and fashion technology",
                Address = "444 Seventh Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10018",
                Country = "United States",
                Phone = "+1-212-555-0206"
            },
            new Company
            {
                Id = id++,
                Name = "Real Estate Analytics",
                Description = "Property tech and real estate data",
                Address = "555 Park Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10022",
                Country = "United States",
                Phone = "+1-212-555-0207"
            },
            new Company
            {
                Id = id++,
                Name = "Insurance Tech Group",
                Description = "Insurance technology solutions",
                Address = "666 Lexington Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10022",
                Country = "United States",
                Phone = "+1-212-555-0208"
            },
            new Company
            {
                Id = id++,
                Name = "Legal Tech Partners",
                Description = "Legal practice management software",
                Address = "777 Third Avenue",
                City = "New York",
                Region = "NY",
                PostalCode = "10017",
                Country = "United States",
                Phone = "+1-212-555-0209"
            },
            new Company
            {
                Id = id++,
                Name = "Brooklyn Software Collective",
                Description = "Custom software development agency",
                Address = "100 MetroTech Center",
                City = "Brooklyn",
                Region = "NY",
                PostalCode = "11201",
                Country = "United States",
                Phone = "+1-718-555-0210"
            },
            new Company
            {
                Id = id++,
                Name = "Buffalo Tech Innovations",
                Description = "Manufacturing automation software",
                Address = "200 Main Street",
                City = "Buffalo",
                Region = "NY",
                PostalCode = "14202",
                Country = "United States",
                Phone = "+1-716-555-0211"
            },
            new Company
            {
                Id = id++,
                Name = "Rochester Data Systems",
                Description = "Enterprise data management solutions",
                Address = "300 East Avenue",
                City = "Rochester",
                Region = "NY",
                PostalCode = "14604",
                Country = "United States",
                Phone = "+1-585-555-0212"
            }
        });

        // ========== TEXAS - 10 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "Energy Grid Innovations",
                Description = "Smart grid and renewable energy solutions",
                Address = "1500 Energy Plaza",
                City = "Houston",
                Region = "TX",
                PostalCode = "77002",
                Country = "United States",
                Phone = "+1-713-555-0301"
            },
            new Company
            {
                Id = id++,
                Name = "Austin Code Labs",
                Description = "Custom software development and consulting",
                Address = "600 Congress Avenue",
                City = "Austin",
                Region = "TX",
                PostalCode = "78701",
                Country = "United States",
                Phone = "+1-512-555-0302"
            },
            new Company
            {
                Id = id++,
                Name = "Lone Star Tech",
                Description = "Enterprise software solutions provider",
                Address = "2000 Ross Avenue",
                City = "Dallas",
                Region = "TX",
                PostalCode = "75201",
                Country = "United States",
                Phone = "+1-214-555-0303"
            },
            new Company
            {
                Id = id++,
                Name = "Oil & Gas Analytics",
                Description = "Energy sector data analytics",
                Address = "1600 Louisiana Street",
                City = "Houston",
                Region = "TX",
                PostalCode = "77002",
                Country = "United States",
                Phone = "+1-713-555-0304"
            },
            new Company
            {
                Id = id++,
                Name = "Live Music Tech",
                Description = "Event management and ticketing software",
                Address = "700 Red River Street",
                City = "Austin",
                Region = "TX",
                PostalCode = "78701",
                Country = "United States",
                Phone = "+1-512-555-0305"
            },
            new Company
            {
                Id = id++,
                Name = "Telecommunications Solutions",
                Description = "Telecom infrastructure and 5G technology",
                Address = "2100 Pearl Street",
                City = "Dallas",
                Region = "TX",
                PostalCode = "75201",
                Country = "United States",
                Phone = "+1-214-555-0306"
            },
            new Company
            {
                Id = id++,
                Name = "San Antonio Cybersecurity",
                Description = "Military-grade security systems",
                Address = "800 Dolorosa Street",
                City = "San Antonio",
                Region = "TX",
                PostalCode = "78205",
                Country = "United States",
                Phone = "+1-210-555-0307"
            },
            new Company
            {
                Id = id++,
                Name = "Fort Worth Logistics Tech",
                Description = "Supply chain management software",
                Address = "900 Commerce Street",
                City = "Fort Worth",
                Region = "TX",
                PostalCode = "76102",
                Country = "United States",
                Phone = "+1-817-555-0308"
            },
            new Company
            {
                Id = id++,
                Name = "Space Tech Ventures",
                Description = "Aerospace software and satellite systems",
                Address = "1000 Space Center Boulevard",
                City = "Houston",
                Region = "TX",
                PostalCode = "77058",
                Country = "United States",
                Phone = "+1-281-555-0309"
            },
            new Company
            {
                Id = id++,
                Name = "Austin Gaming Studios",
                Description = "Mobile and console game development",
                Address = "1100 South Lamar",
                City = "Austin",
                Region = "TX",
                PostalCode = "78704",
                Country = "United States",
                Phone = "+1-512-555-0310"
            }
        });

        // ========== WASHINGTON - 8 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "Pacific Cloud Services",
                Description = "Multi-cloud management platform",
                Address = "1201 Third Avenue",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98101",
                Country = "United States",
                Phone = "+1-206-555-0401"
            },
            new Company
            {
                Id = id++,
                Name = "Emerald Software Group",
                Description = "Healthcare IT solutions",
                Address = "500 Pike Street",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98101",
                Country = "United States",
                Phone = "+1-206-555-0402"
            },
            new Company
            {
                Id = id++,
                Name = "E-Commerce Platform Inc",
                Description = "Online marketplace technology",
                Address = "410 Terry Avenue North",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98109",
                Country = "United States",
                Phone = "+1-206-555-0403"
            },
            new Company
            {
                Id = id++,
                Name = "Coffee Tech Solutions",
                Description = "Restaurant and cafe POS systems",
                Address = "2401 Utah Avenue South",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98134",
                Country = "United States",
                Phone = "+1-206-555-0404"
            },
            new Company
            {
                Id = id++,
                Name = "Aerospace Simulation Systems",
                Description = "Flight simulation and training software",
                Address = "100 North Riverside Avenue",
                City = "Renton",
                Region = "WA",
                PostalCode = "98057",
                Country = "United States",
                Phone = "+1-425-555-0405"
            },
            new Company
            {
                Id = id++,
                Name = "Northwest Data Center",
                Description = "Colocation and edge computing",
                Address = "1000 Denny Way",
                City = "Seattle",
                Region = "WA",
                PostalCode = "98109",
                Country = "United States",
                Phone = "+1-206-555-0406"
            },
            new Company
            {
                Id = id++,
                Name = "Spokane Software Works",
                Description = "Custom enterprise applications",
                Address = "808 West Spokane Falls Boulevard",
                City = "Spokane",
                Region = "WA",
                PostalCode = "99201",
                Country = "United States",
                Phone = "+1-509-555-0407"
            },
            new Company
            {
                Id = id++,
                Name = "Tacoma Industrial IoT",
                Description = "Industrial Internet of Things solutions",
                Address = "1950 Pacific Avenue",
                City = "Tacoma",
                Region = "WA",
                PostalCode = "98402",
                Country = "United States",
                Phone = "+1-253-555-0408"
            }
        });

        // ========== MASSACHUSETTS - 7 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "BioTech Innovations",
                Description = "Biotechnology research and development",
                Address = "75 State Street",
                City = "Boston",
                Region = "MA",
                PostalCode = "02109",
                Country = "United States",
                Phone = "+1-617-555-0501"
            },
            new Company
            {
                Id = id++,
                Name = "Cambridge AI Research",
                Description = "Artificial intelligence research lab",
                Address = "1 Memorial Drive",
                City = "Cambridge",
                Region = "MA",
                PostalCode = "02142",
                Country = "United States",
                Phone = "+1-617-555-0502"
            },
            new Company
            {
                Id = id++,
                Name = "MedTech Solutions",
                Description = "Medical device software and diagnostics",
                Address = "200 Berkeley Street",
                City = "Boston",
                Region = "MA",
                PostalCode = "02116",
                Country = "United States",
                Phone = "+1-617-555-0503"
            },
            new Company
            {
                Id = id++,
                Name = "Robotics Engineering Lab",
                Description = "Autonomous robotics systems",
                Address = "77 Massachusetts Avenue",
                City = "Cambridge",
                Region = "MA",
                PostalCode = "02139",
                Country = "United States",
                Phone = "+1-617-555-0504"
            },
            new Company
            {
                Id = id++,
                Name = "EdTech Publishing",
                Description = "Digital textbooks and learning platforms",
                Address = "300 The Fenway",
                City = "Boston",
                Region = "MA",
                PostalCode = "02115",
                Country = "United States",
                Phone = "+1-617-555-0505"
            },
            new Company
            {
                Id = id++,
                Name = "Worcester Innovation Hub",
                Description = "Technology incubator and coworking",
                Address = "100 Front Street",
                City = "Worcester",
                Region = "MA",
                PostalCode = "01608",
                Country = "United States",
                Phone = "+1-508-555-0506"
            },
            new Company
            {
                Id = id++,
                Name = "Quantum Security Systems",
                Description = "Quantum encryption technology",
                Address = "500 Boylston Street",
                City = "Boston",
                Region = "MA",
                PostalCode = "02116",
                Country = "United States",
                Phone = "+1-617-555-0507"
            }
        });

        // ========== ILLINOIS - 6 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "WindyCity Solutions",
                Description = "Enterprise resource planning systems",
                Address = "233 South Wacker Drive",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60606",
                Country = "United States",
                Phone = "+1-312-555-0601"
            },
            new Company
            {
                Id = id++,
                Name = "Midwest Data Center",
                Description = "Colocation and managed hosting services",
                Address = "444 West Lake Street",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60661",
                Country = "United States",
                Phone = "+1-312-555-0602"
            },
            new Company
            {
                Id = id++,
                Name = "Trading Platform Technologies",
                Description = "Commodities trading software",
                Address = "141 West Jackson Boulevard",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60604",
                Country = "United States",
                Phone = "+1-312-555-0603"
            },
            new Company
            {
                Id = id++,
                Name = "Transportation Logistics AI",
                Description = "Rail and freight optimization",
                Address = "225 North Michigan Avenue",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60601",
                Country = "United States",
                Phone = "+1-312-555-0604"
            },
            new Company
            {
                Id = id++,
                Name = "Food Service Tech",
                Description = "Restaurant management systems",
                Address = "100 West Monroe Street",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60603",
                Country = "United States",
                Phone = "+1-312-555-0605"
            },
            new Company
            {
                Id = id++,
                Name = "Manufacturing Analytics Pro",
                Description = "Industrial process optimization",
                Address = "1 North Dearborn Street",
                City = "Chicago",
                Region = "IL",
                PostalCode = "60602",
                Country = "United States",
                Phone = "+1-312-555-0606"
            }
        });

        // ========== FLORIDA - 6 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "Sunshine Software",
                Description = "Mobile app development studio",
                Address = "1000 Brickell Avenue",
                City = "Miami",
                Region = "FL",
                PostalCode = "33131",
                Country = "United States",
                Phone = "+1-305-555-0701"
            },
            new Company
            {
                Id = id++,
                Name = "Orlando Tech Hub",
                Description = "Technology incubator and coworking space",
                Address = "100 West Livingston Street",
                City = "Orlando",
                Region = "FL",
                PostalCode = "32801",
                Country = "United States",
                Phone = "+1-407-555-0702"
            },
            new Company
            {
                Id = id++,
                Name = "Theme Park Systems",
                Description = "Attraction management and ticketing",
                Address = "1000 Universal Studios Plaza",
                City = "Orlando",
                Region = "FL",
                PostalCode = "32819",
                Country = "United States",
                Phone = "+1-407-555-0703"
            },
            new Company
            {
                Id = id++,
                Name = "Cruise Line Technologies",
                Description = "Maritime navigation and booking systems",
                Address = "2000 Biscayne Boulevard",
                City = "Miami",
                Region = "FL",
                PostalCode = "33137",
                Country = "United States",
                Phone = "+1-305-555-0704"
            },
            new Company
            {
                Id = id++,
                Name = "Tampa Bay HealthTech",
                Description = "Healthcare management platforms",
                Address = "3000 Bayshore Boulevard",
                City = "Tampa",
                Region = "FL",
                PostalCode = "33629",
                Country = "United States",
                Phone = "+1-813-555-0705"
            },
            new Company
            {
                Id = id++,
                Name = "Space Coast Engineering",
                Description = "Aerospace engineering software",
                Address = "4000 NASA Parkway",
                City = "Cape Canaveral",
                Region = "FL",
                PostalCode = "32920",
                Country = "United States",
                Phone = "+1-321-555-0706"
            }
        });

        // ========== OTHER US STATES - 12 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "Mile High Software",
                Description = "Cloud infrastructure and SaaS",
                Address = "1700 Lincoln Street",
                City = "Denver",
                Region = "CO",
                PostalCode = "80203",
                Country = "United States",
                Phone = "+1-303-555-0801"
            },
            new Company
            {
                Id = id++,
                Name = "Atlanta FinTech Group",
                Description = "Payment processing solutions",
                Address = "191 Peachtree Street",
                City = "Atlanta",
                Region = "GA",
                PostalCode = "30303",
                Country = "United States",
                Phone = "+1-404-555-0802"
            },
            new Company
            {
                Id = id++,
                Name = "Phoenix Data Solutions",
                Description = "Enterprise data warehousing",
                Address = "100 West Washington Street",
                City = "Phoenix",
                Region = "AZ",
                PostalCode = "85003",
                Country = "United States",
                Phone = "+1-602-555-0803"
            },
            new Company
            {
                Id = id++,
                Name = "Portland Open Source",
                Description = "Open source software development",
                Address = "1000 Southwest Broadway",
                City = "Portland",
                Region = "OR",
                PostalCode = "97205",
                Country = "United States",
                Phone = "+1-503-555-0804"
            },
            new Company
            {
                Id = id++,
                Name = "Detroit Auto Tech",
                Description = "Automotive software and IoT",
                Address = "1 Woodward Avenue",
                City = "Detroit",
                Region = "MI",
                PostalCode = "48226",
                Country = "United States",
                Phone = "+1-313-555-0805"
            },
            new Company
            {
                Id = id++,
                Name = "Philadelphia HealthCare IT",
                Description = "Electronic health records systems",
                Address = "1234 Market Street",
                City = "Philadelphia",
                Region = "PA",
                PostalCode = "19107",
                Country = "United States",
                Phone = "+1-215-555-0806"
            },
            new Company
            {
                Id = id++,
                Name = "Las Vegas Gaming Tech",
                Description = "Casino and gaming software",
                Address = "3799 Las Vegas Boulevard",
                City = "Las Vegas",
                Region = "NV",
                PostalCode = "89109",
                Country = "United States",
                Phone = "+1-702-555-0807"
            },
            new Company
            {
                Id = id++,
                Name = "Nashville Music Software",
                Description = "Music production and streaming tech",
                Address = "150 Fourth Avenue North",
                City = "Nashville",
                Region = "TN",
                PostalCode = "37219",
                Country = "United States",
                Phone = "+1-615-555-0808"
            },
            new Company
            {
                Id = id++,
                Name = "Salt Lake Outdoor Tech",
                Description = "Outdoor recreation and fitness apps",
                Address = "50 East South Temple",
                City = "Salt Lake City",
                Region = "UT",
                PostalCode = "84111",
                Country = "United States",
                Phone = "+1-801-555-0809"
            },
            new Company
            {
                Id = id++,
                Name = "Charlotte Banking Systems",
                Description = "Core banking software",
                Address = "100 North Tryon Street",
                City = "Charlotte",
                Region = "NC",
                PostalCode = "28202",
                Country = "United States",
                Phone = "+1-704-555-0810"
            },
            new Company
            {
                Id = id++,
                Name = "Minneapolis AgriTech",
                Description = "Agricultural technology and farm management",
                Address = "50 South Sixth Street",
                City = "Minneapolis",
                Region = "MN",
                PostalCode = "55402",
                Country = "United States",
                Phone = "+1-612-555-0811"
            },
            new Company
            {
                Id = id++,
                Name = "Kansas City Logistics AI",
                Description = "Warehouse automation and robotics",
                Address = "1100 Main Street",
                City = "Kansas City",
                Region = "MO",
                PostalCode = "64105",
                Country = "United States",
                Phone = "+1-816-555-0812"
            }
        });

        // ========== INTERNATIONAL - 12 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "London Digital Agency",
                Description = "Digital marketing and web development",
                Address = "10 Downing Street",
                City = "London",
                Region = "England",
                PostalCode = "SW1A 2AA",
                Country = "United Kingdom",
                Phone = "+44-20-7946-0901"
            },
            new Company
            {
                Id = id++,
                Name = "Toronto Tech Collective",
                Description = "Software consulting and training",
                Address = "100 King Street West",
                City = "Toronto",
                Region = "ON",
                PostalCode = "M5X 1A9",
                Country = "Canada",
                Phone = "+1-416-555-0902"
            },
            new Company
            {
                Id = id++,
                Name = "Sydney Cloud Partners",
                Description = "Cloud migration specialists",
                Address = "1 Macquarie Place",
                City = "Sydney",
                Region = "NSW",
                PostalCode = "2000",
                Country = "Australia",
                Phone = "+61-2-9555-0903"
            },
            new Company
            {
                Id = id++,
                Name = "Berlin Innovation Labs",
                Description = "IoT and embedded systems development",
                Address = "Unter den Linden 77",
                City = "Berlin",
                Region = "BE",
                PostalCode = "10117",
                Country = "Germany",
                Phone = "+49-30-2555-0904"
            },
            new Company
            {
                Id = id++,
                Name = "Singapore FinTech Hub",
                Description = "Financial services technology platform",
                Address = "1 Marina Boulevard",
                City = "Singapore",
                Region = null,
                PostalCode = "018989",
                Country = "Singapore",
                Phone = "+65-6555-0905"
            },
            new Company
            {
                Id = id++,
                Name = "Tokyo Robotics Corp",
                Description = "Industrial automation and robotics",
                Address = "1-1-1 Shibuya",
                City = "Tokyo",
                Region = null,
                PostalCode = "150-0002",
                Country = "Japan",
                Phone = "+81-3-5555-0906"
            },
            new Company
            {
                Id = id++,
                Name = "Paris Fashion Tech",
                Description = "Luxury fashion e-commerce platform",
                Address = "10 Avenue des Champs-Élysées",
                City = "Paris",
                Region = "IDF",
                PostalCode = "75008",
                Country = "France",
                Phone = "+33-1-4555-0907"
            },
            new Company
            {
                Id = id++,
                Name = "Amsterdam Data Science",
                Description = "Machine learning and data analytics",
                Address = "Science Park 904",
                City = "Amsterdam",
                Region = "NH",
                PostalCode = "1098 XH",
                Country = "Netherlands",
                Phone = "+31-20-5555-0908"
            },
            new Company
            {
                Id = id++,
                Name = "Dublin Cloud Services",
                Description = "European cloud infrastructure provider",
                Address = "70 Sir John Rogerson's Quay",
                City = "Dublin",
                Region = "D",
                PostalCode = "D02 R296",
                Country = "Ireland",
                Phone = "+353-1-555-0909"
            },
            new Company
            {
                Id = id++,
                Name = "Stockholm GreenTech",
                Description = "Sustainable technology solutions",
                Address = "Sveavägen 46",
                City = "Stockholm",
                Region = "AB",
                PostalCode = "111 34",
                Country = "Sweden",
                Phone = "+46-8-555-0910"
            },
            new Company
            {
                Id = id++,
                Name = "Bangalore Software Solutions",
                Description = "Offshore development and IT services",
                Address = "MG Road 100",
                City = "Bangalore",
                Region = "KA",
                PostalCode = "560001",
                Country = "India",
                Phone = "+91-80-5555-0911"
            },
            new Company
            {
                Id = id++,
                Name = "São Paulo Digital",
                Description = "Latin American e-commerce platform",
                Address = "Avenida Paulista 1000",
                City = "São Paulo",
                Region = "SP",
                PostalCode = "01310-100",
                Country = "Brazil",
                Phone = "+55-11-5555-0912"
            }
        });

        // ========== ADDITIONAL COMPANIES - 12 companies ==========
        companies.AddRange(new[]
        {
            new Company
            {
                Id = id++,
                Name = "Vancouver Mobile Apps",
                Description = "Cross-platform mobile application development",
                Address = "1055 West Hastings Street",
                City = "Vancouver",
                Region = "BC",
                PostalCode = "V6E 2E9",
                Country = "Canada",
                Phone = "+1-604-555-1001"
            },
            new Company
            {
                Id = id++,
                Name = "Melbourne Analytics Hub",
                Description = "Business intelligence and analytics platform",
                Address = "120 Collins Street",
                City = "Melbourne",
                Region = "VIC",
                PostalCode = "3000",
                Country = "Australia",
                Phone = "+61-3-9555-1002"
            },
            new Company
            {
                Id = id++,
                Name = "Zurich SecureTech",
                Description = "Enterprise security and compliance software",
                Address = "Bahnhofstrasse 50",
                City = "Zurich",
                Region = "ZH",
                PostalCode = "8001",
                Country = "Switzerland",
                Phone = "+41-44-555-1003"
            },
            new Company
            {
                Id = id++,
                Name = "Seoul Game Studios",
                Description = "Mobile and PC game development",
                Address = "123 Gangnam-daero",
                City = "Seoul",
                Region = null,
                PostalCode = "06236",
                Country = "South Korea",
                Phone = "+82-2-555-1004"
            },
            new Company
            {
                Id = id++,
                Name = "Hong Kong FinServe",
                Description = "Financial services technology solutions",
                Address = "1 International Finance Centre",
                City = "Hong Kong",
                Region = null,
                PostalCode = "999077",
                Country = "Hong Kong",
                Phone = "+852-2555-1005"
            },
            new Company
            {
                Id = id++,
                Name = "Barcelona Smart City",
                Description = "Urban technology and IoT solutions",
                Address = "Passeig de Gràcia 85",
                City = "Barcelona",
                Region = "CAT",
                PostalCode = "08008",
                Country = "Spain",
                Phone = "+34-93-555-1006"
            },
            new Company
            {
                Id = id++,
                Name = "Copenhagen Green Solutions",
                Description = "Sustainable technology and clean energy software",
                Address = "Vesterbrogade 6A",
                City = "Copenhagen",
                Region = null,
                PostalCode = "1620",
                Country = "Denmark",
                Phone = "+45-33-555-1007"
            },
            new Company
            {
                Id = id++,
                Name = "Tel Aviv Innovation Labs",
                Description = "Cybersecurity and AI research",
                Address = "Rothschild Boulevard 100",
                City = "Tel Aviv",
                Region = null,
                PostalCode = "6688101",
                Country = "Israel",
                Phone = "+972-3-555-1008"
            },
            new Company
            {
                Id = id++,
                Name = "Dubai Tech Hub",
                Description = "Enterprise digital transformation services",
                Address = "Sheikh Zayed Road",
                City = "Dubai",
                Region = null,
                PostalCode = "00000",
                Country = "United Arab Emirates",
                Phone = "+971-4-555-1009"
            },
            new Company
            {
                Id = id++,
                Name = "Vienna Data Systems",
                Description = "Data analytics and business intelligence",
                Address = "Stephansplatz 1",
                City = "Vienna",
                Region = "W",
                PostalCode = "1010",
                Country = "Austria",
                Phone = "+43-1-555-1010"
            },
            new Company
            {
                Id = id++,
                Name = "Oslo Maritime Tech",
                Description = "Marine navigation and shipping software",
                Address = "Karl Johans gate 10",
                City = "Oslo",
                Region = null,
                PostalCode = "0154",
                Country = "Norway",
                Phone = "+47-22-555-1011"
            },
            new Company
            {
                Id = id++,
                Name = "Brussels EU Tech",
                Description = "Government and regulatory compliance software",
                Address = "Rue de la Loi 200",
                City = "Brussels",
                Region = "BRU",
                PostalCode = "1049",
                Country = "Belgium",
                Phone = "+32-2-555-1012"
            }
        });

        return companies;
    }
}