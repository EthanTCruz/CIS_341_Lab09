using System.ComponentModel;

namespace Lab8.Models
{
    public class Type
    {
        [DisplayName("Type ID")]
        public int TypeID { get; set; } 
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Name")]
        public string Name { get; set; }

        public ICollection<Listing> ListingsByCollection { get; set; }
    }
}
