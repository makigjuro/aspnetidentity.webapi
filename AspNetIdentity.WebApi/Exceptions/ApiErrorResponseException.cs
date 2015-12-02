using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace AspNetIdentity.WebApi.Exceptions
{
    /// <summary>
    /// Wrapper for returning http response exceptions to the user
    /// </summary>
    public class ApiErrorResponseException : HttpResponseException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="errorCode">Specific API error code</param>
        public ApiErrorResponseException(ApiErrorCode errorCode)
            : base(BuildResponse(errorCode))
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelState"></param>
        public ApiErrorResponseException(ModelStateDictionary modelState)
            : base(BuildResponse(modelState))
        { }

        /// <summary>
        /// Helper method for building the error response for the user
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        internal static HttpResponseMessage BuildResponse(ApiErrorCode errorCode)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string message = null;

            switch (errorCode)
            {
                case ApiErrorCode.ExpiredToken:
                    message = ApiErrorMessages.ExpiredToken;
                    statusCode = HttpStatusCode.Unauthorized;
                    break;

                case ApiErrorCode.InvalidToken:
                    message = ApiErrorMessages.InvalidToken;
                    statusCode = HttpStatusCode.Unauthorized;
                    break;
                case ApiErrorCode.UserDoesNotExist:
                    message = ApiErrorMessages.UserNotFound;
                    statusCode = HttpStatusCode.NotModified;
                    break;
                case ApiErrorCode.PasswordIsRequired:
                    message = ApiErrorMessages.PasswordIsRequired;
                    statusCode = HttpStatusCode.BadRequest;
                    break;
            }

            var responseBody = new
            {
                errorCode = errorCode,
                errorMessage = message
            };

            return BuildResponseMessage(statusCode, responseBody);
        }

        /// <summary>
        /// Helper method for building the error response for the user
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        private static HttpResponseMessage BuildResponse(ModelStateDictionary modelState)
        {
            HttpStatusCode statusCode = HttpStatusCode.BadRequest;

            var errors = modelState.ToDictionary(s => StripPrefix(s.Key),
                s => s.Value.Errors.Select(e => e.ErrorMessage));

            var responseBody = new
            {
                errorCode = ApiErrorCode.RequestBodyInvalid,
                errorMessage = ApiErrorMessages.RequestBodyInvalid,
                errors = errors
            };

            return BuildResponseMessage(statusCode, responseBody);
        }

        /// <summary>
        /// Build response based on json message
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="responseBody"></param>
        /// <returns></returns>
        private static HttpResponseMessage BuildResponseMessage(HttpStatusCode statusCode, object responseBody)
        {
            string responseJson = JsonConvert.SerializeObject(responseBody);

            return new HttpResponseMessage
            {
                Content = new StringContent(responseJson, null, JsonMediaTypeFormatter.DefaultMediaType.MediaType),
                StatusCode = statusCode
            };
        }

        private static object StripPrefix(string message)
        {
            int separatorPosition = message.IndexOf('.');
            return message.Substring(separatorPosition + 1);
        }
        
        /// <summary>
        /// Error messages for user
        /// </summary>
        private class ApiErrorMessages
        {
            internal const string ExpiredToken = "Token is expired.";
            internal const string InvalidToken = "Token is invalid.";
            internal const string RequestBodyInvalid = "Request body is invalid.";
            internal const string UserNotFound = "We are sorry, but the user is not found to be updated at this time.";
            internal const string PasswordIsRequired = "Password is required.";
        }
    }
}