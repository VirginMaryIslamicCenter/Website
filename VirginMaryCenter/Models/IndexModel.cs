using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VirginMaryCenter.Models
{
    public class IndexModel
    {
        public IEnumerable<Event> FutureEvents { get; set; }
        public UserLocation Location { get; set; }
    }

    public class UserLocation
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }

        [Required]
        public string Address { get; set; }
    }
}
