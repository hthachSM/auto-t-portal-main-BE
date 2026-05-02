using System.Net;
using System.Text.Json;

namespace auto_t_portal_main_BE.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponse(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, HttpStatusCode.InternalServerError, "Đã xảy ra lỗi hệ thống.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.StatusCode = (int)code;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
