using EventBooking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Application.Common.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}
