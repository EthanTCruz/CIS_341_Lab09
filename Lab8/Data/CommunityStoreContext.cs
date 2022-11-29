using Microsoft.EntityFrameworkCore;
using Lab8.Models;

namespace Lab8.Data
{
    public class CommunityStoreContext : DbContext
    {
        public CommunityStoreContext(DbContextOptions<CommunityStoreContext> options) : base(options)
        {
        }

        public DbSet<Condition> Conditions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Listing> Listings { get; set; }
        public DbSet<Store> Stores { get; set; }
    }
}
