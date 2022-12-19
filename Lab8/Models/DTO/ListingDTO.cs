using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab8.Models.DTO
{
    public class ListingDTO
    {
        [Key]
        [DisplayName("ListingID")]
        public int ListingID { get; set; }



        [DisplayName("Condition")]
        public string Condition { get; set; } = string.Empty;

        [DisplayName("Description")]
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }


        [DisplayName("CreatedBy")]
        public string? CreatedBy { get; set; }

        [DisplayName("ClaimedBy")]
        public string? ClaimedBy { get; set; } = "Unclaimed";
        
        [DisplayName("Store Name")]
        public string Store { get; set; } = string.Empty;



        [DisplayName("Status")]
        public string Status { get; set; } = string.Empty;

        [DisplayName("Type")]
        public string Type { get; set; } = string.Empty;

        [DisplayName("Type Name")]
        public string? TypeName { get; set; } = string.Empty;

        [DisplayName("Type Description")]
        public string? TypeDescription { get; set; } = string.Empty;


        public ICollection<Listing>? Listings { get; set; }
        [NotMapped]
        public IEnumerable<SelectListItem>? Stores { get; set; }
    }
}
