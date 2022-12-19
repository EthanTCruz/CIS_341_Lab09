using Lab8.Models.DTO;
using Lab8.Models;
using Microsoft.AspNetCore.Mvc;
using Lab8.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Lab8.Data;
using Microsoft.EntityFrameworkCore;

namespace Lab8.Controllers
{
    [Authorize]
    public class ListingsController : Controller
    {

        //Just made controller for the two methods as they are frequently used in both manager and customer controllers

        public ListingDTO ConvertToListingDTO(Listing listing)
        {
            return new ListingDTO
            {
                ListingID = listing.ListingID,
                Condition = listing.Condition.Description,
                Description = listing.Description,
                Quantity = listing.Quantity,
                CreatedBy = listing.CreatedBy.Name,
                ClaimedBy = listing.ClaimedBy?.Name ?? "Unclaimed",
                Store = listing.Store.Name,
                Status = listing.Status.Description,
                Type = listing.Type.Name,
                TypeName = listing.Type.Name,
                TypeDescription = listing.Type.Description
            };
        }

        public Listing ConvertToListing(ListingDTO listingDTO)
        {
            return new Listing
            {
                ListingID = listingDTO.ListingID,
                Description = listingDTO.Description,
                Quantity = listingDTO.Quantity,
                Condition = new Condition { Description = listingDTO.Condition },
                Type = new Models.Type { Name = listingDTO.Type, Description = listingDTO.TypeDescription },
                Status = new Status { Description = listingDTO.Status },
                Store = new Store { Name = listingDTO.Store },
                CreatedBy = new Customer { Name = listingDTO.CreatedBy },
                ClaimedBy = listingDTO.ClaimedBy == "Unclaimed" ? null : new Customer { Name = listingDTO.ClaimedBy }
            };
        }



    }
}
