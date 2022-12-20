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
            var customer1 = new Customer
            {
                Name = "Jeff Ray",
                Email = "creatore@gmail.com"
            };
            context.Customers.Add(customer1);

            var customer2 = new Customer
            {
                Name = "Customer",
                Email = "customer@gmail.com"

            };
            context.Customers.Add(customer2);

            var customer3 = new Customer
            {
                Name = "Gary Wart",
                Email = "Gary@gmail.com"

            };
            context.Customers.Add(customer3);

            var customer4 = new Customer
            {
                Name = "Zach Dumas",
                Email = "zach@gmail.com"

            };
            context.Customers.Add(customer4);

            var customer5 = new Customer
            {
                Name = "Alex Rant",
                Email = "Alex@gmail.com"

            };
            context.Customers.Add(customer5);

            var condition1 = new Condition
            {
                Description = "Good"
            };
            context.Conditions.Add(condition1);


            var condition2 = new Condition
            {
                Description = "Good"
            };
            context.Conditions.Add(condition2);

            var condition3 = new Condition
            {
                Description = "Poor"
            };
            context.Conditions.Add(condition3);

            var condition4 = new Condition
            {
                Description = "Used"
            };
            context.Conditions.Add(condition4);

            var condition5 = new Condition
            {
                Description = "Perfect"
            };
            context.Conditions.Add(condition5);


            var type1 = new Models.Type
            {
                Name = "Furniture",
                Description = "Chair"

            };
            context.Types.Add(type1);

            var type2 = new Models.Type
            {
                Name = "Book",
                Description = "Science Fiction"

            };
            context.Types.Add(type2);

            var type3 = new Models.Type
            {
                Name = "Furniture",
                Description = "Stool"

            };
            context.Types.Add(type3);

            var type4 = new Models.Type
            {
                Name = "Clothing",
                Description = "t shirt"

            };
            context.Types.Add(type4);

            var type5 = new Models.Type
            {
                Name = "Furniture",
                Description = "Chair"

            };
            context.Types.Add(type5);


            var manager1 = new Manager
            {
                Name = "Manager",
                Email = "manager@gmail.com"
            };
            context.Managers.Add(manager1);

            var manager2 = new Manager
            {
                Name = "Jeff Musk",
                Email = "jm@gmail.com"
            };
            context.Managers.Add(manager2);

            var manager3 = new Manager
            {
                Name = "Elon Bezos",
                Email = "eb@gmail.com"
            };
            context.Managers.Add(manager3);

            var store1 = new Store
            {
                Name = "Target",
                Manager = manager1,

            };
            context.Stores.Add(store1);
            var store2 = new Store
            {
                Name = "Walmart",
                Manager = manager2,

            };
            context.Stores.Add(store2);
            var store3 = new Store
            {
                Name = "Kmart",
                Manager = manager3,

            };
            context.Stores.Add(store3);

            var status1 = new Status
            {
                Description = "Unapproved"
            };
            context.Status.Add(status1);

            var status2 = new Status
            {
                Description = "Recieved"
            };
            context.Status.Add(status2);

            var status3 = new Status
            {
                Description = "Approved"
            };
            context.Status.Add(status3);

            var status4 = new Status
            {
                Description = "Unapproved"
            };
            context.Status.Add(status4);

            var status5 = new Status
            {
                Description = "Unapproved"
            };
            context.Status.Add(status5);

            var listing1 = new Listing
            {
                Description = "Nice wooden chair",
                Store = store1,
                Condition = condition1,
                Type = type1,
                Quantity = 1,
                ClaimedBy = customer1,
                CreatedBy = customer2,
                Status = status1
            };
            context.Listings.Add(listing1);
            context.SaveChanges();





            var listing2 = new Listing
            {
                Description = "1984",
                Store = store2,
                Condition = condition2,
                Type = type2,
                Quantity = 1,
                CreatedBy = customer2,
                Status = status2
            };
            context.Listings.Add(listing2);

            context.SaveChanges();


            var listing3 = new Listing
            {
                Description = "Wooden stool",
                Store = store3,
                Condition = condition3,
                Type = type3,
                Quantity = 3,
                CreatedBy = customer3,
                Status = status3
            };
            context.Listings.Add(listing3);

            context.SaveChanges();

            var listing4 = new Listing
            {
                Description = "regular tshirt",
                Store = store3,
                Condition = condition4,
                Type = type4,
                Quantity = 1,
                CreatedBy = customer4,
                Status = status4
            };
            context.Listings.Add(listing4);

            context.SaveChanges();

            var listing5 = new Listing
            {
                Description = "Another Nice wooden chair",
                Store = store1,
                Condition = condition5,
                Type = type5,
                Quantity = 1,
                CreatedBy = customer5,
                Status = status5
            };
            context.Listings.Add(listing5);

            context.SaveChanges();
        }
        }
}
