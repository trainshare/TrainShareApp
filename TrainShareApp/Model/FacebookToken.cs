﻿using System;

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

        /// <summary>
        /// Get or Set the date and time when this token expires
        /// </summary>
        public DateTime Expires { get; set; }

        public void Clear()
        {
            Id = 0;
            ScreenName = string.Empty;
            AccessToken = string.Empty;
            Expires = DateTime.Now;
        }
    }
}