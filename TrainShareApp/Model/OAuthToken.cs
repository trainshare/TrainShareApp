namespace TrainShareApp.Model
{
    public class OAuthToken
    {
        public string AuthToken { get; set; }
        public string AuthTokenSecret { get; set; }
        public string Verifier { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public OAuthToken WithAuth(string token, string secret)
        {
            AuthToken = token;
            AuthTokenSecret = secret;

            return this;
        }

        public OAuthToken WithVerifier(string verifier)
        {
            Verifier = verifier;

            return this;
        }

        public OAuthToken WithAccess(string token, string secret)
        {
            AccessToken = token;
            AccessTokenSecret = secret;

            return this;
        }
    }
}