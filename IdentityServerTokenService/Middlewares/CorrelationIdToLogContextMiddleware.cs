using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CorrelationId;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace IdentityServerTokenService.Middlewares
{
    public class CorrelationIdToLogContextMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly ICorrelationContextAccessor _correlationContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationIdToLogContextMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next task in the pipeline.</param>
        /// <param name="loggerFactory"></param>
        public CorrelationIdToLogContextMiddleware(RequestDelegate next, ICorrelationContextAccessor correlationContext, ILoggerFactory loggerFactory)
        {
            _next = next;
            _correlationContext = correlationContext;
            _logger = loggerFactory.CreateLogger<CorrelationIdToLogContextMiddleware>();
        }

        /// <summary>
        /// Invokes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Task.</returns>
        public Task Invoke(HttpContext context)
        {
            _logger.LogDebug($"Middleware: {typeof(CorrelationIdToLogContextMiddleware)}");
            _logger.LogInformation($"CorrelationId: {_correlationContext.CorrelationContext.CorrelationId}");
            return _next(context);
        }
    }
}
