using System.ComponentModel;

namespace Lab8.Models
{
    public class Manager
    {
        [DisplayName("Manager ID")]
        public int ManagerID { get; set; }
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Password")]
        public string Password { get; set; }


        public ICollection<Listing> ManagerListings { get; set; }
    }
}
