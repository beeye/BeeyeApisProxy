using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HelloBeeye
{
    /// <summary>
    /// This an http logger.
    /// </summary>
    public class LoggingHandler : DelegatingHandler
    {
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                await request.Content.ReadAsStringAsync();
            }
            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
            return response;
        }
    }
}
