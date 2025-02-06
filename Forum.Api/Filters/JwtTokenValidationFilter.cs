using Forum.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Forum.Api.Filters
{
    public class JwtTokenValidationFilter : IAuthorizationFilter
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public JwtTokenValidationFilter(IJwtTokenGenerator jwtTokenGenerator)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var accessToken = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                context.Result = new UnauthorizedObjectResult("Access token bulunamadı.");
                return;
            }

            var (isValid, principal) = _jwtTokenGenerator.ValidateTokenAsync(accessToken).Result;
            if (!isValid || principal == null)
            {
                context.Result = new UnauthorizedObjectResult("Geçersiz veya süresi dolmuş token.");
                return;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out Guid userId))
            {
                context.HttpContext.Items["userId"] = userId;
            }
            else
            {
                context.Result = new UnauthorizedObjectResult("Geçersiz kullanıcı kimliği.");
            }
        }
    }
}