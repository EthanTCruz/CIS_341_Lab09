using System.ComponentModel;

namespace Lab8.Models
{
    public class Store
    {
        [DisplayName("Store ID")]
        public int StoreID { get; set; } 
        [DisplayName("Name")]
        public string Name { get; set; }


        public int ManagerID { get; set; }


        public Manager Manager { get; set; }

        public ICollection<Listing> StoreListings { get; set; }
    }
}
