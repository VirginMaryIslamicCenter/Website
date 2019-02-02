using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace VirginMaryCenter.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100), Display(Name = "Title")]
        public string Title { get; set; }

        [Required, MaxLength(400), Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Image")]
        public byte[] Image { get; set; }

        [Required, Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Location")]
        public Location Location { get; set; }


        [NotMapped]
        public IFormFile Picture { get; set; }

        public override string ToString()
        {
            return this.Title;
        }
    }
}
