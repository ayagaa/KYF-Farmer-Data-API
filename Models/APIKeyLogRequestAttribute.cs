using Farmer.Data.API.Utils;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Farmer.Data.API.Models
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class APIKeyLogRequestAttribute : ActionFilterAttribute
    {
        // ensure thread-safety when multiple requests hit at once
        private static readonly object _fileLock = new();
        private const string _logFile = "SoilAPIKeyLogs.csv";
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<APIKeyLogRequestAttribute>>();
            var req = context.HttpContext.Request;

            try
            {
                // 1) Get the raw header value
                var rawAuth = req.Headers["Authorization"].FirstOrDefault();

                // 2) Parse out just the token portion (after the scheme)
                //    e.g. rawAuth = "Token abc123" → token = "abc123"
                var token = rawAuth?.Split(' ', StringSplitOptions.RemoveEmptyEntries).ElementAtOrDefault(1);

                if (!string.IsNullOrEmpty(token) && DataAccess.SoilAPIKeys.TryGetValue(token, out string organization))
                {
                    var line = $"{organization},{token},{req.Method},{req.Path}";

                    lock (_fileLock)
                    {
                        File.AppendAllText(_logFile, line + System.Environment.NewLine);
                    }

                }

                //logger.LogInformation(
                //    "Incoming {Method} {Path} — Authorization header present: {HasAuth}. Token={Token}",
                //    req.Method,
                //    req.Path,
                //    !string.IsNullOrEmpty(rawAuth),
                //    token ?? "<none>"
                //);
            }
            catch (Exception)
            {
            }

            base.OnActionExecuting(context);
        }
    }
}
