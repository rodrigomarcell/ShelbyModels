using System;

namespace ShelbyModels.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }

        // Optional foreign key for the User
        public int? UserId { get; set; } // Nullable foreign key
       
    }
}
