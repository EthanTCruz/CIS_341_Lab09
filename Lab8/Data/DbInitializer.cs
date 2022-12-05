using Lab8.Models;
using System.Reflection.Metadata.Ecma335;

namespace Lab8.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CommunityStoreContext context)
        {
            context.Database.EnsureCreated();

            if (context.Listings.Any())
            {
                return;
            }
            var createdby = new Customer
            {
                Name = "Creating Customer",
                Email = "Creatore@gmail.com"
            };
            context.Customers.Add(createdby);

            var claimedby = new Customer
            {
                Name = "Customer Person",
                Email = "Customer@gmail.com"

            };
            context.Customers.Add(claimedby);


            var condition = new Condition
            {
                Description = "Good"
            };
            context.Conditions.Add(condition);

            var type = new Models.Type
            {
                Name = "Furniture",
                Description = "Chair"

            };
            context.Types.Add(type);

            var manager = new Manager
            {
                Name = "Manager1",
                Email = "Manager@gmail.com"
            };
            context.Managers.Add(manager);

            var store = new Store
            {
                Name = "Target",
                Address = "1234 fake st"
            };
            context.Stores.Add(store);
            var status = new Status
            {
                Description = "Unclaimed"
            };
            context.Status.Add(status);

            var listing = new Listing
            {
                Description = "Nice wooden chair",
                Store = store,
                Condition = condition,
                Type = type,
                Quantity = 1,
                ClaimedBy = claimedby,
                CreatedBy = createdby,
                Status = status
            };
            context.Listings.Add(listing);
            context.SaveChanges();

        }
        }
}
