using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Reviews
{

    public class TracedHttpRequestMessage: HttpRequestMessage
    {
        public IHeaderDictionary ParentRequestHeaders { get; set; }
    }
    
    public class TracedHttpMessageHandler: HttpClientHandler
    {
        // HTTP headers to propagate for distributed tracing are documented at
        // https://istio.io/docs/tasks/telemetry/distributed-tracing/overview/#trace-context-propagation
        private static string[] HeadersToForward =
        {
            "x-request-id","x-b3-traceid","x-b3-spanid","x-b3-sampled","x-b3-flags",
            "x-ot-span-context","x-datadog-trace-id","x-datadog-parent-id","x-datadog-sampled", "end-user","user-agent"
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request is TracedHttpRequestMessage tracedRequest)
            {
                foreach (var name in HeadersToForward)
                {
                    if (tracedRequest.ParentRequestHeaders.TryGetValue(name, out var values))
                    {
                        request.Headers.TryAddWithoutValidation(name, values.ToArray());
                    }
                }
            }
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}