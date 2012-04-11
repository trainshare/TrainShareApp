namespace TrainShareApp.Model
{
    public class TwitterToken
    {
        /// <summary>
        /// Get or Set the unique twitter user id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Get or Set the twitter screen name
        /// </summary>
        public string ScreenName { get; set; }

        /// <summary>
        /// Get or Set the twitter access token
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Get or Set the twitter access token secret
        /// </summary>
        public string AccessTokenSecret { get; set; }

        public void Clear()
        {
            Id = 0;
            ScreenName = string.Empty;
            AccessToken = string.Empty;
            AccessTokenSecret = string.Empty;
        }
    }
}