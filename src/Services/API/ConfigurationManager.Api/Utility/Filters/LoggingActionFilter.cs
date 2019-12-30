using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Utility.Filters
{
    public class LoggingActionFilter : IAsyncActionFilter
    {
        ILogger _logger;
        private readonly ILoggerFactory loggerFactory;
        public LoggingActionFilter(
        ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<LoggingActionFilter>();
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            // Enable seeking
            request.EnableBuffering();
            // Read the stream as text
            var bodyAsText = await new System.IO.StreamReader(request.Body).ReadToEndAsync();
            // Set the position of the stream to 0 to enable rereading
            request.Body.Position = 0;
            var Url = $"{request.Host}{request.Path}{request.QueryString.Value}";
            var RemoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            // do something before the action executes
            _logger.LogInformation("Action {DescriptorName} executing\nBody: {body}\nUrl: {url}\nIP: {IP}", context.ActionDescriptor.DisplayName, bodyAsText, Url, RemoteIpAddress);
            var resultContext = await next();
        }
    }
}