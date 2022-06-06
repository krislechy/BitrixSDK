using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yolva.Bitrix.OAuth2
{
    internal sealed class Variables
    {
        public readonly static string PHPSESSIDName = "PHPSESSID";
        public readonly static string codeQueryName = "code";
        public readonly static string refreshTokenName = "refresh_token";

        public readonly static string grantTypeName = "grant_type";
        public readonly static string grantTypeRefreshTokenName = "refresh_token";
        public readonly static string grantTypeAuthorizationCodeName = "authorization_code";

        public readonly static string clientIdName = "client_id";
        public readonly static string clientSecretName = "client_secret";
        public readonly static string stateName = "state";
    }
}
