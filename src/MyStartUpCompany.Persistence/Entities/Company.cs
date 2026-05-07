namespace MyStartUpCompany.Persistence.Entities
{
    public class Company
    {
        // Company properties
        public int Id { get; set; }

        public required string Name { get; set; }

        public string? Description { get; set; }

        public required string Address { get; set; }

        public required string City { get; set; }

        public string? Region { get; set; }

        public required string PostalCode { get; set; }

        public required string Country { get; set; }

        public required string Phone { get; set; }
    }
}
