using Microsoft.AspNetCore.Identity;

namespace TravelingApp.Domain.Entities
{
    public class User : IdentityUser
    {
        public bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public uint Version { get; set; }
    }
}
