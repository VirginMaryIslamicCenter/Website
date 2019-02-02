using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace VirginMaryCenter.Models
{
    public class EmailSubscription
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(255)]
        public string Email { get; set; }

        public DateTime DateAdded { get; set; }

        public override string ToString()
        {
            return this.Email;
        }
    }
}
