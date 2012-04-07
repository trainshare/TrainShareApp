namespace TrainShareApp.Model
{
    public class TwitterToken
    {
        public string AuthToken { get; set; }
        public string AuthTokenSecret { get; set; }
        public string Verifier { get; set; }
        public string AccessToken { get; set; }
        public string AccessTokenSecret { get; set; }

        public TwitterToken WithAuth(string token, string secret)
        {
            AuthToken = token;
            AuthTokenSecret = secret;

            return this;
        }

        public TwitterToken WithVerifier(string verifier)
        {
            Verifier = verifier;

            return this;
        }

        public TwitterToken WithAccess(string token, string secret)
        {
            AccessToken = token;
            AccessTokenSecret = secret;

            return this;
        }
    }
}