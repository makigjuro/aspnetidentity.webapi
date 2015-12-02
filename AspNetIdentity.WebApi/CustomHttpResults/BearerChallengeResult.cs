using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace AspNetIdentity.WebApi.CustomHttpResults
{
    public class BearerChallengeResult : IHttpActionResult
    {       
        private readonly IHttpActionResult innerResult;
        private readonly string realm;

        public BearerChallengeResult(IHttpActionResult innerResult, string realm)
        {
            this.innerResult = innerResult;
            this.realm = realm;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage response = await this.innerResult.ExecuteAsync(cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
                response.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue("Bearer", String.Format("realm=\"{0}\"", realm)));
            return response;
        }
    }
}