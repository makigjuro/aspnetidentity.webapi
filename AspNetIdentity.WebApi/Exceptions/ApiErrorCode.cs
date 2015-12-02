using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspNetIdentity.WebApi.Exceptions
{
    public enum ApiErrorCode
    {
        InvalidToken = 1001,

        ExpiredToken = 1002,
        
        UserDoesNotExist = 1003,
        
        PasswordIsRequired = 1004,

        RequestBodyInvalid = 1005
    }
}