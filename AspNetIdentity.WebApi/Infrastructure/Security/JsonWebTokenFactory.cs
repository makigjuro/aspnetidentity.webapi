using System;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Security;
using AspNetIdentity.WebApi.Infrastructure.Interfaces;
using AspNetIdentity.WebApi.Models;

namespace AspNetIdentity.WebApi.Infrastructure.Security
{
    public class JsonWebTokenFactory : ITokenFactory
    {
        #region Properties

        public static string Audience
        {
            get { return ConfigurationManager.AppSettings["Audience"]; }
        }

        public static string SymmetricKey
        {
            get { return ConfigurationManager.AppSettings["SymmetricKey"]; }
        }

        public static string TokenIssuerName
        {
            get { return ConfigurationManager.AppSettings["TokenIssuerName"]; }
        }

        public static string DigestAlgo
        {
            get { return ConfigurationManager.AppSettings["DigestAlgo"]; }
        }

        public static string SignatureAlgo
        {
            get { return ConfigurationManager.AppSettings["SignatureAlgo"]; }
        }

        #endregion

        public string CreateSecurityTokenForUser(UserDataModel user)
        {
            ClaimsIdentity identity = this.CreateClaimsIdentity(user);

            SigningCredentials signingCredentials = this.CreateSigningCredentials();

            String tokenString = this.CreateToken(identity, signingCredentials);
            return this.ProtectToken(tokenString);
        }

        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricKey = Guid.NewGuid().ToByteArray().Concat(Guid.NewGuid().ToByteArray()).ToArray();
            var key = Convert.ToBase64String(symmetricKey);

            return new SigningCredentials(
                new InMemorySymmetricSecurityKey(Convert.FromBase64String(SymmetricKey)),
                SignatureAlgo, DigestAlgo);
        }

        private ClaimsIdentity CreateClaimsIdentity(UserDataModel user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                //here you can create custom claim propertyes
                //new Claim("HasApprovedTermsAndConditions", user.HasApprovedTermsAndConditions.ToString())
            };

            return new ClaimsIdentity(claims);
        }

        private string CreateToken(ClaimsIdentity identity, SigningCredentials signingCredentials)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                AppliesToAddress = Audience,
                Subject = identity,
                TokenIssuerName = TokenIssuerName,
                SigningCredentials = signingCredentials,
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }

        private string ProtectToken(string tokenString)
        {
            var bStr = Encoding.Unicode.GetBytes(tokenString);
            byte[] bytes = MachineKey.Protect(bStr);

            return Base64UrlEncoder.Encode(bytes);
        }
    }
}