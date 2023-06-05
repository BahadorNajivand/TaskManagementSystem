using TaskManagementSystem.Exceptions;
using TaskManagementSystem.Models;
using Newtonsoft.Json;
using Serilog;
using Serilog.Context;
using System.Net;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace TaskManagementSystem.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            Log.Logger = new LoggerConfiguration()

                // Add console (Sink) as logging target
                .WriteTo.Console()

                // Write logs to a file for warning and logs with a higher severity
                // Logs are written in JSON
                .WriteTo.File(new JsonFormatter(),
                    "important-logs.json",
                    restrictedToMinimumLevel: LogEventLevel.Warning)

                // Add a log file that will be replaced by a new log file each day
                .WriteTo.File("all-daily-.logs",
                    rollingInterval: RollingInterval.Day)

                .MinimumLevel.Debug()

                // Create the actual logger
                .CreateLogger();

            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                string errorId = Guid.NewGuid().ToString();
                LogContext.PushProperty("ErrorId", errorId);
                LogContext.PushProperty("StackTrace", exception.StackTrace);
                var errorResult = new ErrorResult
                {
                    Source = exception.TargetSite?.DeclaringType?.FullName,
                    Exception = exception.Message.Trim(),
                    ErrorId = errorId,
                    SupportMessage = $"Provide the Error Id: {errorId} to the support team for further analysis."
                };
                errorResult.Messages.Add(exception.Message);

                if (exception is not CustomException && exception.InnerException != null)
                {
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                    }
                }

                switch (exception)
                {
                    case CustomException e:
                        errorResult.StatusCode = (int)e.StatusCode;
                        if (e.ErrorMessages is not null)
                        {
                            errorResult.Messages = e.ErrorMessages;
                        }

                        break;

                    case KeyNotFoundException:
                        errorResult.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        errorResult.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                Log.Error($"{errorResult.Exception} Request failed with Status Code {context.Response.StatusCode} and Error Id {errorId}.");
                var response = context.Response;
                if (!response.HasStarted)
                {
                    response.ContentType = "application/json";
                    response.StatusCode = errorResult.StatusCode;
                    await response.WriteAsync(JsonConvert.SerializeObject(errorResult));
                }
                else
                {
                    Log.Warning("Can't write error response. Response has already started.");
                }
            }
        }
    }
}
