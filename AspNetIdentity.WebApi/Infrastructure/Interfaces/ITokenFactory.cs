using AspNetIdentity.WebApi.Models;

namespace AspNetIdentity.WebApi.Infrastructure.Interfaces
{
    public interface ITokenFactory
    {
        string CreateSecurityTokenForUser(UserDataModel user);
    }
}
