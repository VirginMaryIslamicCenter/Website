using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VirginMaryCenter.Models
{
    public class Location
    {
        [MaxLength(100), Display(Name = "Venue")]
        public string Venue { get; set; }

        [MaxLength(100), Display(Name = "Address")]
        public string Address { get; set; }

        [MaxLength(100), Display(Name = "City")]
        public string City { get; set; }

        [MaxLength(100), Display(Name = "State")]
        public string State { get; set; }

        [MaxLength(100), Display(Name = "Zip Code")]
        public string PostalCode { get; set; }
    }
}
