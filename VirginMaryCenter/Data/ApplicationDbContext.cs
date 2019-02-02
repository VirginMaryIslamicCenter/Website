using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VirginMaryCenter.Models;

namespace VirginMaryCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<EmailSubscription> EmailSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Event>().OwnsOne(x => x.Location);
        }

        public Task<List<Event>> GetFutureEvents()
        {
            return this.Events.Where(x => x.StartDate >= DateTime.UtcNow).ToListAsync();
        }
    }
}
