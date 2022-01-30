using System;

namespace Users.Models
{
    public class Token
    {
        public string Login { get; private set; }
        public string ExternalId { get; private set; }
        public string AccessToken { get; private set; }

        public Token(string login, string externalId, string accessToken)
        {
            if (login == default || login.Length == 0)
                throw new ArgumentNullException($"Parameter {nameof(login)} passed into {nameof(Token)} constructor is null or empty.");

            if (externalId == default || externalId.Length == 0)
                throw new ArgumentNullException($"Parameter {nameof(externalId)} passed into {nameof(Token)} constructor is null or empty.");

            if (accessToken == default || accessToken.Length == 0)
                throw new ArgumentNullException($"Parameter {nameof(accessToken)} passed into {nameof(Token)} constructor is null or empty.");

            Login = login;
            ExternalId = externalId;
            AccessToken = accessToken;
        }
    }
}
