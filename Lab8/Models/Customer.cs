using System.ComponentModel;

namespace Lab8.Models
{
    public class Customer
    {
        [DisplayName("Customer ID")]
        public int CustomerID { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; } = "None";
        [DisplayName("Last Name")]
        public string LastName { get; set; } = string.Empty;
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }


        public ICollection<Listing> CustomerListings { get; set; }
    }
}
