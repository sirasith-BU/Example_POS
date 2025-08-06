using Example_POS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using NuGet.Common;

namespace Example_POS.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpcontext, IAuthService authService)
        {
            var accessToken = httpcontext.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                var principal = authService.ValidateAccessToken(accessToken);
                if (principal != null)
                {
                    httpcontext.User = principal;
                    await _next(httpcontext);
                    return;
                }
            }
            else
            {
                var refreshToken = httpcontext.Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var newAccessToken = await authService.ValidateRefreshToken(refreshToken);

                    if (newAccessToken != null)
                    {
                        // Set Cookies
                        httpcontext.Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.UtcNow.AddMinutes(10)
                        });

                        var principal = authService.ValidateAccessToken(newAccessToken);
                        if (principal != null)
                        {
                            httpcontext.User = principal;
                        }

                        await _next(httpcontext);
                        return;
                    }
                }
            }


                await _next(httpcontext);
            return;
        }
    }
}
