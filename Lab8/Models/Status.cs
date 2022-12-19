using System.ComponentModel;

namespace Lab8.Models
{
    public class Status
    {

        [DisplayName("Status ID")]
        public int StatusID { get; set; } // PK
        [DisplayName("Description")]
        public string Description { get; set; }

        public ICollection<Listing>? ListingsByCollection { get; set; }

    }
}
