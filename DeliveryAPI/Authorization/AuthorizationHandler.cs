using DeliveryAPI.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;

namespace DeliveryAPI.Authorization;

public class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirements>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public AuthorizationHandler(IHttpContextAccessor httpContextAccessor,
        IServiceScopeFactory serviceScopeFactory)
    {
        _httpContextAccessor = httpContextAccessor;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AuthorizationRequirements requirement)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            string? authorizationString = _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Authorization];
            if (authorizationString == null) throw new Unauthorized();
            
            var token = authorizationString.Replace("Bearer ", "");

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContexts>();

            var tokenEntity = await dbContext
                .Tokens
                .Where(x => x.ValidToken == token)
                .FirstOrDefaultAsync();
            
            if (tokenEntity != null) throw new Unauthorized();
            
            context.Succeed(requirement);
        }
        else
        {
            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "Bad request"
            );
            throw ex;
        }
    }
}