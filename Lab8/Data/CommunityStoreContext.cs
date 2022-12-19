using Microsoft.EntityFrameworkCore;
using Lab8.Models;
using Lab8.Models.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Lab8.Data
{
    public class CommunityStoreContext : DbContext
    {
        public CommunityStoreContext() { }
        public CommunityStoreContext(DbContextOptions<CommunityStoreContext> options) : base(options)
        {
        }

        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Models.Type> Types { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Store> Stores { get; set; }
        //delete this line for dto later
        public DbSet<Lab8.Models.DTO.ListingDTO> ListingDTO { get; set; }

        public DbSet<SelectListGroup> SelectListGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure other entities

            modelBuilder.Entity<SelectListGroup>().HasNoKey();
        }
    }
}
