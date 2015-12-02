using System;
using System.IdentityModel.Tokens;
using System.Security.Principal;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Security;
using AspNetIdentity.WebApi.CustomHttpResults;
using AspNetIdentity.WebApi.Exceptions;
using AspNetIdentity.WebApi.Infrastructure.Security;

namespace AspNetIdentity.WebApi.Filters
{
    public class JwtAuthenticationFilter : IAuthenticationFilter
    {
        public bool AllowMultiple { get { return false; } }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            var authHeader = context.Request.Headers.Authorization;
            if (authHeader != null && String.Equals(authHeader.Scheme, "Bearer", StringComparison.InvariantCultureIgnoreCase)
                && authHeader.Parameter != null)
            {
                try
                {
                    var tokenEncrypted = Base64UrlEncoder.DecodeBytes(authHeader.Parameter);
                    var tokenDecrypted = MachineKey.Unprotect(tokenEncrypted);

                    var tokenString = Encoding.Unicode.GetString(tokenDecrypted);
                    var tokenValidationParams = new TokenValidationParameters
                    {
                        IssuerSigningToken = new BinarySecretSecurityToken(Convert.FromBase64String(JsonWebTokenFactory.SymmetricKey)),
                        ValidAudience = JsonWebTokenFactory.Audience,
                        ValidIssuer = JsonWebTokenFactory.TokenIssuerName
                    };

                    var handler = new JwtSecurityTokenHandler();
                    SecurityToken token;
                    IPrincipal principal = handler.ValidateToken(tokenString, tokenValidationParams, out token);

                    context.Principal = principal;
                }
                catch (SecurityTokenExpiredException)
                {
                    context.ErrorResult = new ApiErrorResult(ApiErrorCode.ExpiredToken);
                }
                catch (Exception)
                {
                    context.ErrorResult = new ApiErrorResult(ApiErrorCode.InvalidToken);
                }
            }
            return Task.FromResult(default(object));
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            context.Result = new BearerChallengeResult(context.Result, "AspNet.WebAPI");
            return Task.FromResult(default(object));
        }
    }
}