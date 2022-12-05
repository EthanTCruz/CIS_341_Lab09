using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;

namespace Lab8.Models
{
    public class Listing
    {
        public int ListingID { get; set; }
        public int CreatedByID { get; set; }
        public int? ClaimedByID { get; set; }
        public int StoreID { get; set; } 
        public int ConditionID { get; set; } 

        public int TypeID { get; set; }

        public int StatusID { get; set; }

        
        [DisplayName("Description")]
        public string Description { get; set; }
        [DisplayName("Quantity")]
        public int Quantity { get; set; }


        [DisplayName("Status")]
        public Status Status { get; set; }
        [DisplayName("Type")]
        public Type Type { get; set; }
        [DisplayName("CreatedBy")]
        public Customer CreatedBy { get; set; }
        [DisplayName("ClaimedBy")]
        public Customer? ClaimedBy { get; set; }
        [DisplayName("Store")]
        public Store Store { get; set; }
        [DisplayName("Condition")]
        public Condition Condition { get; set; }
    }
}
