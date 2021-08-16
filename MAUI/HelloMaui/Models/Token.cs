namespace MAUI.Models
{
    public class Token
    {
        public string Login { get; set; }
        public string Id { get; set; }
        public string AccessToken { get; set; }
        public Token(string login, string id, string accessToken)
        {
            Login = login;
            Id = id;
            AccessToken = accessToken;
        }
    }
}
