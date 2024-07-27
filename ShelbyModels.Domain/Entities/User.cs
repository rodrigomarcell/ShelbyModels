﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ShelbyModels.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfileType { get; set; }
        public string SexualOrientation { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Ethnicity { get; set; }
        public string HairColor { get; set; }
        public string HairLength { get; set; }
        public string BreastSize { get; set; }
        public string BreastType { get; set; }
        public bool IsAvailableForOutcall { get; set; }
        public bool IsAvailableForIncall { get; set; }
        public bool IsAvailableForOutcallAndIncall { get; set; }
        public string MeetingWith { get; set; }
        public string Languages { get; set; }
        public bool HasTattoos { get; set; }
        public bool HasPiercings { get; set; }
        public bool IsSmoker { get; set; }
        public string EyeColor { get; set; }
        public string ServiceSpecifications { get; set; }
        public string Nationality { get; set; }

        // Navigation property for addresses        
        public Address Address { get; set; }
        public int? AddressId { get; set; } // Nul
    }

}