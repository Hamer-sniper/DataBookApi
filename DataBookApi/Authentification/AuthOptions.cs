using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DataBookApi.Authentification
{
    public class AuthOptions
    {
        public const string ISSUER = "DataBookApiServer"; // издатель токена
        public const string AUDIENCE = "DataBookApiClient"; // потребитель токена
        const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
