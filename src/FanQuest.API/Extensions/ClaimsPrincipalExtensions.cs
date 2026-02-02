using System.Security.Claims;

namespace FanQuest.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)
                              ?? principal.FindFirst("sub")
                              ?? principal.FindFirst("userId");

            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token");

            if (!Guid.TryParse(userIdClaim.Value, out var userId))
                throw new UnauthorizedAccessException("Invalid user ID format");

            return userId;
        }

        public static string GetPhoneNumber(this ClaimsPrincipal principal)
        {
            var phoneNumberClaim = principal.FindFirst(ClaimTypes.MobilePhone)
                                   ?? principal.FindFirst("phone_number")
                                   ?? principal.FindFirst("phoneNumber");

            if (phoneNumberClaim == null)
                throw new UnauthorizedAccessException("Phone number not found in token");

            return phoneNumberClaim.Value;
        }

        public static string GetDisplayName(this ClaimsPrincipal principal)
        {
            var Name = principal.FindFirst(ClaimTypes.Name)
                                   ?? principal.FindFirst("phone_number")
                                   ?? principal.FindFirst("phoneNumber");
            if (Name == null)
                throw new UnauthorizedAccessException("Display name not found in token");

            return Name.Value;
        }
    }
}
