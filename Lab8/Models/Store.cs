using System.ComponentModel;

namespace Lab8.Models
{
    public class Store
    {
        [DisplayName("Store ID")]
        public int StoreID { get; set; } 
        [DisplayName("Name")]
        public string Name { get; set; }
        [DisplayName("Address")]
        public string Address { get; set; }
        [DisplayName("ManagerID")]
        public string ManagerID { get; set; }

        [DisplayName("Manager")]
        public Manager Manager { get; set; }

        public ICollection<Listing> StoreListings { get; set; }
    }
}
