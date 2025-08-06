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
            var refreshToken = context.Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await authService.ValidateRefreshToken(refreshToken);

                if (token != null)
                {
                    context.Response.Cookies.Append("accessToken", token.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(10)
                    });

                    context.User = authService.ValidateAccessToken(token.AccessToken)!;

                    await _next(context);
                    return;
                }
            }

            await _next(context);
            return;
        }
    }
}
