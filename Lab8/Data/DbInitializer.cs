using Lab8.Models;

namespace Lab8.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CommunityStoreContext context)
        {
            context.Database.EnsureCreated();

            if(context.Listings.Any())
            {
                return;
            }


            var customer = new Customer
            {
                FirstName = "Customer",
                LastName = "Person",
                Email = "Customer@gmail.com",
                Password = "Customer" 
            };


            context.Customers.Add(customer);

            var store = new Store
            {
                Name = "Target",
                Address = "500 Division St"
            };

            context.Stores.Add(store);

    

            var condition = new Condition
            {
                Description = "Used"
            };

            context.Conditions.Add(condition);

            var listing = new Listing
            {
                Condition = condition,
                Customer = customer,
                Store = store,
                Quantity = 1,
                Description = "Table"
            };


            context.Listings.Add(listing);

            context.SaveChanges();


            var customer2 = new Customer
            {
                FirstName = "Ethan",
                LastName = "Cruz",
                Email = "ecruz@uwsp.edu",
                Password = "password" 
            };


            context.Customers.Add(customer2);


            var customer3 = new Customer
            {

                FirstName = "Null",
                LastName = "Null",
                Email = "Null@gmail.com",
                Password = "Null"
            };


            context.Customers.Add(customer3);
            context.SaveChanges();

            var store2 = new Store
            {
                Name = "Walmart",
                Address = "200 4th St"
            };

            context.Stores.Add(store2);

            var condition2 = new Condition
            {
                Description = "New"
            };

            context.Conditions.Add(condition2);

            var listing2 = new Listing
            {
                Condition = condition2,
                Customer = customer2,
                Store = store2,
                Quantity = 2,
                Description = "Chair"
            };


            context.Listings.Add(listing2);

            context.SaveChanges();
        }
    }
}
