using Example_POS.Data;
using Example_POS.Helper;
using Example_POS.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Example_POS.Middleware
{
    public class JwtRefreshMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public JwtRefreshMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var _jwtHelper = scope.ServiceProvider.GetRequiredService<IJwtHelper>();
                var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // อ่าน access token จาก Header (หรือ Cookie แล้วแต่ config)
                var accessToken = context.Request.Cookies["access_token"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    var principal = _jwtHelper.ValidateToken(accessToken);

                    if (principal != null)
                    {
                        if (DateTime.Now > GetTokenExpiration(principal))
                        {
                            // token หมดอายุ หรือไม่ถูกต้อง
                            // ลอง refresh ด้วย refresh token
                            var refreshToken = context.Request.Cookies["refresh_token"];

                            if (!string.IsNullOrEmpty(refreshToken))
                            {
                                const string getUserByRefreshTokenCommand = @"
                                                                    SELECT * from Users 
                                                                    WHERE RefreshToken = @refreshToken and
                                                                          RefreshTokenExpiryTime > @refreshTokenExpiryTime and
                                                                          IsDelete = @isDelete and
                                                                          IsActive = @isActive";


                                User? user = _db.Users.FromSqlRaw(getUserByRefreshTokenCommand,
                                                                  new SqlParameter("@refreshToken", refreshToken),
                                                                  new SqlParameter("@refreshTokenExpiryTime", DateTime.Now),
                                                                  new SqlParameter("@IsDelete", false),
                                                                  new SqlParameter("@isActive", true)).FirstOrDefault();

                                if (user != null)
                                {
                                    // สร้าง access token ใหม่
                                    var newAccessToken = _jwtHelper.GenerateJwtToken(user.Email, user.Username);

                                    // สร้าง refresh token ใหม่ และอัพเดต DB
                                    var newRefreshToken = _jwtHelper.GenerateRefreshToken();
                                    var refreshExpiry = DateTime.Now.AddDays(7);

                                    const string updateUserCommand = @"
                                                                        UPDATE Users 
                                                                        SET RefreshToken = @refreshToken, 
                                                                            RefreshTokenExpiryTime = @refreshTokenExpiryTime 
                                                                        WHERE Id = @userId";


                                    _db.Database.ExecuteSqlRaw(updateUserCommand,
                                        new SqlParameter("@userId", user.Id),
                                        new SqlParameter("@refreshToken", refreshToken),
                                        new SqlParameter("@refreshTokenExpiryTime", refreshExpiry));


                                    // ส่ง token ใหม่ใน cookie
                                    context.Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
                                    {
                                        HttpOnly = true,
                                        Secure = true,
                                        SameSite = SameSiteMode.Strict,
                                        Expires = DateTime.Now.AddMinutes(30)
                                    });

                                    context.Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                                    {
                                        HttpOnly = true,
                                        Secure = true,
                                        SameSite = SameSiteMode.Strict,
                                        Expires = refreshExpiry
                                    });

                                    // update principal ใน HttpContext.User
                                    var principalUser = _jwtHelper.ValidateToken(newAccessToken);

                                    if (principalUser != null)
                                    {
                                        context.User = principalUser;
                                    }
                                }
                                else
                                {
                                    // refresh token หมดอายุหรือไม่ถูกต้อง
                                    // ไม่ต้องทำอะไร รอให้ controller จัดการ (เช่น redirect login)
                                }
                            }
                        }
                    }
                    else
                    {
                        // token ยัง valid ปกติ ไม่ต้อง refresh
                    }
                }

            }



            await _next(context);
        }


        public DateTime? GetTokenExpiration(ClaimsPrincipal user)
        {
            var expClaim = user.Claims.FirstOrDefault(c => c.Type == "exp");

            if (expClaim == null)
                return null;

            // แปลงจาก Unix Timestamp เป็น DateTime
            var secondsSinceEpoch = long.Parse(expClaim.Value);
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(secondsSinceEpoch).DateTime.ToLocalTime();
            return expirationDate;
        }
    }
}
