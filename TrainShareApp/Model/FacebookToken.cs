namespace TrainShareApp.Model
{
    public class FacebookToken
    {
        /// <summary>
        /// Get or Set the facebook id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Get or Set the facebook screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Get or Set the facebook access token
        /// </summary>
        public string AccessToken { get; set; }
    }
}