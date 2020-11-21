using Microsoft.AspNetCore.Identity;

namespace web.Models

{
    public class ApplicationUsers : IdentityUser
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string City { get; set; }
        
    }
}