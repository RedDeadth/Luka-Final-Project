using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FinalProject.API.Middlewares
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                await AttachUserToContextAsync(context, token);
            }

            await _next(context);
        }

        private async Task AttachUserToContextAsync(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = await Task.Run(() => 
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken));

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var role = principal.FindFirst(ClaimTypes.Role)?.Value;
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                context.Items["User"] = new { Id = userId, Role = role, Email = email };
                context.Items["IsAuthenticated"] = true;

                _logger.LogDebug("Token validated successfully for user {UserId}", userId);
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogWarning("Token expired");
                context.Items["IsAuthenticated"] = false;
                context.Items["TokenError"] = "Token expired";
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                _logger.LogWarning("Invalid token signature");
                context.Items["IsAuthenticated"] = false;
                context.Items["TokenError"] = "Invalid signature";
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                context.Items["IsAuthenticated"] = false;
                context.Items["TokenError"] = "Invalid token";
            }
        }
    }
}
