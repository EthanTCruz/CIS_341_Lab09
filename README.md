# CIS_341_Lab09
Community Marketplace
Welcome to the Community Marketplace! This application is a web front-end for a community marketplace that allows community members to list items that they want to give away. This process is facilitated by participating local stores that can be designated as pick-up locations. Store managers are able to approve or disapprove of the listings based on their capacity and update the availability of the listings once they receive the items from community members.

User Groups
There are two main user groups in the Community Marketplace:

Community members who can browse, view details, claim, add, and remove listings.
Store managers who can view, approve or disapprove, update availability, and remove listings.
Controller Classes
The Community Marketplace has the following controller classes to implement the functionality described in the user stories for each user group:

Community Member Controller
The CommunityMemberController class provides the following action methods:

Task<IActionResult> BrowseListings(string type, string condition) - retrieves the listings that match the given type and condition criteria and returns a view with the list of listings.
Task<IActionResult> ViewListingDetails(int listingId) - retrieves the details of the listing with the given ID and returns a view with the listing details.
Task<IActionResult> ClaimListing(int listingId) - marks the listing with the given ID as claimed by the current community member and saves the changes to the database. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> AddListing(Listing listing) - adds a new listing to the database and saves the changes. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> ConfigureListing(int listingId, string description, string type, string condition, int quantity) - updates the listing with the given ID with the given description, type, condition, and quantity and saves the changes to the database. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> Login(string email, string password) - authenticates the user with the given email and password and returns a view with a confirmation message or an error message if the login fails.

Manager Controller
The ManagerController class provides the following action methods:

Task<IActionResult> ViewListings(string status) - retrieves the listings that match the given status and returns a view with the list of listings.
Task<IActionResult> ViewListingDetails(int listingId) - retrieves the details of the listing with the given ID and returns a view with the listing details.
Task<IActionResult> ApproveListing(int listingId) - marks the listing with the given ID as approved and saves the changes to the database. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> DisapproveListing(int listingId) - marks the listing with the given ID as disapproved and saves the changes to the database. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> UpdateAvailability(int listingId, bool isAvailable) - updates the availability of the listing with the given ID and saves the changes to the database. Returns a view with a confirmation message or an error message if the save operation fails.
Task<IActionResult> RemoveListing(int listingId) - removes the listing with the given ID from the database. Returns a view with a confirmation message or an error message if the remove operation fails.
Task<IActionResult> Login(string email, string password) - authenticates the user with the given email and password and returns a view with a confirmation message or an error message if the login fails.
Additional Notes

Both the CommunityMemberController and ManagerController classes include a ConvertToListingDTO method that converts a Listing entity to a ListingDTO object, which is a data transfer object used to display the relevant information about a listing in the views.

In addition, both controllers include an Authorize attribute to restrict access to certain actions to only users with the specified roles. In the CommunityMemberController, the Authorize attribute is set to allow only users with the role "Customer" to access the actions, while in the ManagerController, the attribute is set to allow only users with the role "Manager" to access the actions.

It is important to note that the UserManager<ApplicationUser> object is being injected into both controllers in their constructors. This object is used to manage users and their roles, and is essential for implementing the authorization and authentication features of the application.

Finally, both controllers rely on the CommunityStoreContext object to access and manipulate data in the database. The CommunityStoreContext includes the Listings entity set, which is used to store the listings in the community marketplace. The Include method is used to load related entities when querying the Listings set, which allows for efficient and accurate data retrieval.
