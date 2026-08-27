
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Domain.Entities
{
    public class ApplicationUser:IdentityUser<Guid>

    {
        public required string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
