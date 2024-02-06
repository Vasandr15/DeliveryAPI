using DeliveryAPI.DTO;
using DeliveryAPI.Exceptions;

namespace DeliveryAPI.Middleware;

public class MiddlewareHandler
{
    private readonly RequestDelegate _next;

    public MiddlewareHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ConflictException exception)
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (Unauthorized exception)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (BadRequest exception)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        catch (NotFoundException exception)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new { message = exception.Message });
        }
        // catch (Exception exception)
        // {
        //     var response = new Response
        //     {
        //         Status= "500",
        //         Message = exception.Message
        //     };
        //     context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        //     await context.Response.WriteAsJsonAsync(response);
        // }
    }
}