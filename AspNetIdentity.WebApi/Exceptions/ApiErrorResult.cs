using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AspNetIdentity.WebApi.Exceptions
{
    public class ApiErrorResult : IHttpActionResult
    {
        private readonly ApiErrorCode errorCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">Based on error code</param>
        public ApiErrorResult(ApiErrorCode errorCode)
        {
            this.errorCode = errorCode;
        }

        /// <summary>
        /// Execute the api callback
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(ApiErrorResponseException.BuildResponse(this.errorCode));
        }
    }
}