using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FanQuest.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
               ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                throw new UnauthorizedAccessException("User ID not found in token");

            return Guid.Parse(userIdClaim);
        }

        public static string GetPhoneNumber(this ClaimsPrincipal principal)
        {
            return principal.FindFirst("phone_number")?.Value
                ?? throw new UnauthorizedAccessException("Phone number not found in token");
        }

        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Name)?.Value
                ?? throw new UnauthorizedAccessException("Display name not found in token");
        }
    }
}
