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

            //get rid of manager id once figured how to make it pk
            var manager = new Manager
            {
                Name = "test manager",
                Email = "manager@gmail.com"
            };
            context.Managers.Add(manager);
            
            var store = new Store
            {
                Name = "Target",
                Manager = manager,

            };
            context.Stores.Add(store);

            var status = new Status
            {
                Description = "Unapproved"
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


            var condition2 = new Condition
            {
                Description = "Good"
            };
            context.Conditions.Add(condition2);


            var type2 = new Models.Type
            {
                Name = "Furniture",
                Description = "Chair"

            };
            context.Types.Add(type2);






            var status2 = new Status
            {
                Description = "Unapproved"
            };
            context.Status.Add(status2);
            var listing2 = new Listing
            {
                Description = "Nice wooden chair",
                Store = store,
                Condition = condition2,
                Type = type2,
                Quantity = 1,
                CreatedBy = claimedby,
                Status = status2
            };
            context.Listings.Add(listing2);

            context.SaveChanges();

            var condition3 = new Condition
            {
                Description = "Good"
            };
            context.Conditions.Add(condition3);


            var type3 = new Models.Type
            {
                Name = "Furniture",
                Description = "Chair"

            };
            context.Types.Add(type3);




            var status3 = new Status
            {
                Description = "Unapproved"
            };
            context.Status.Add(status3);

            var listing3 = new Listing
            {
                Description = "Another Nice wooden chair",
                Store = store,
                Condition = condition3,
                Type = type3,
                Quantity = 1,
                CreatedBy = claimedby,
                Status = status3
            };
            context.Listings.Add(listing3);

            context.SaveChanges();
        }
        }
}
