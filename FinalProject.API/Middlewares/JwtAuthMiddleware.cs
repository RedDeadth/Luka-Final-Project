using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FinalProject.API.Middlewares
{
    public class JwtAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthMiddleware> _logger;

        public JwtAuthMiddleware(RequestDelegate next, ILogger<JwtAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                var userId = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                context.Items["User"] = new { Id = userId, Role = role };
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Invalid token: {message}", ex.Message);
            }
        }
    }
}