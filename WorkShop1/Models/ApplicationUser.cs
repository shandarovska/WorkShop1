using Microsoft.AspNetCore.Identity;

namespace WorkShop1.Models
{
    public class ApplicationUser : IdentityUser
    {
        //kje ima nasleduvanje za student i teacher
        public long? StudentId {  get; set; }
        public Student? Student { get; set; }

        public int? TeacherId {  get; set; }
        public Teacher? Teacher { get; set; }
    }
}
