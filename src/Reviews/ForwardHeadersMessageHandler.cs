using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Reviews
{
    public class ForwardHeadersMessageHandler: DelegatingHandler
    {
        private readonly IHttpContextAccessor _ctxAccessor;
        private readonly Settings _settings;

        public ForwardHeadersMessageHandler(IHttpContextAccessor ctxAccessor, Settings settings)
        {
            _ctxAccessor = ctxAccessor;
            _settings = settings;
        }

        // HTTP headers to propagate for distributed tracing are documented at
        // https://istio.io/docs/tasks/telemetry/distributed-tracing/overview/#trace-context-propagation
        private static string[] HeadersToForward =
        {
            "x-request-id","x-b3-traceid","x-b3-spanid","x-b3-sampled","x-b3-flags",
            "x-ot-span-context","x-datadog-trace-id","x-datadog-parent-id","x-datadog-sampled", "end-user","user-agent"
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            IHeaderDictionary parentRequestHeaders = null;
            var httpContext = _ctxAccessor.HttpContext;
            if (httpContext != null)
            {
                parentRequestHeaders = httpContext.Request.Headers;
            }

            if (parentRequestHeaders != null)
            {
                var extra = _settings.ExtraHeadersToForward
                    .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim());
                
                var forwards = HeadersToForward.Concat(extra);
                foreach (var name in forwards)
                {
                    if (parentRequestHeaders.TryGetValue(name, out var values))
                    {
                        request.Headers.TryAddWithoutValidation(name, values.ToArray());
                    }
                }
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}