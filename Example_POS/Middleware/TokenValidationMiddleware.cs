using Example_POS.Services.Interfaces;

namespace Example_POS.Middleware
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IAuthService authService)
        {
            var accessToken = context.Request.Cookies["accessToken"];
            if (!string.IsNullOrEmpty(accessToken))
            {
                
                var principal = authService.ValidateAccessToken(accessToken);

                if (principal != null)
                {
                    context.User = principal;
                    await _next(context);
                    return;
                }
            }

            // Try refresh token
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await authService.ValidateRefreshToken(refreshToken);

                //if (result != null)
                //{
                //    // Save new token in cookie
                //    context.Response.Cookies.Append("accessToken", result.AccessToken, new CookieOptions
                //    {
                //        HttpOnly = true,
                //        Secure = true,
                //        SameSite = SameSiteMode.Strict,
                //        Expires = DateTime.UtcNow.AddMinutes(15)
                //    });

                //    context.User = result.Principal;
                //}
            }

            await _next(context);
        }
    }
}
