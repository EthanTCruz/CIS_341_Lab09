using System.ComponentModel;

namespace Lab8.Models
{
    public class Customer
    {
        [DisplayName("Customer ID")]
        public int CustomerID { get; set; }
        [DisplayName("First Name")]
        public string Name { get; set; } = "None";

        [DisplayName("Email")]
        public string Email { get; set; }


    }
}
