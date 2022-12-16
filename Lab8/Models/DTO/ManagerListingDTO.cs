using System.ComponentModel;

namespace Lab8.Models.DTO
{
    public class ManagerListingDTO
    {

        [DisplayName("ListingID")]
        public int ListingID { get; set; }


        [DisplayName("StoreID")]
        public int StoreID { get; set; }
        [DisplayName("Condition")]
        public string Condition { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }


        [DisplayName("CreatedBy")]
        public string CreatedBy { get; set; }

        [DisplayName("Store Name")]
        public string Store { get; set; } = string.Empty;

        [DisplayName("Status")]
        public string Status { get; set; } = string.Empty;

        public ICollection<Listing> Listings { get; set; }

    }
}
