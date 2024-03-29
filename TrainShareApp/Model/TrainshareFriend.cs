﻿
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace TrainShareApp.Model
{
    /*
     * {
     * "name":"Yöda",
     * "trainshare_id":"5eedcdfb-db12-4abd-a46f-694361f3cbb5",
     * "position":9,
     * "upper":true,
     * "message":"a message",
     * "image_url":"https://si0.twimg.com/sticky/default_profile_images/default_profile_3_biger.png",
     * "overlaps":{
     *     "time":"2012-04-09T11:36:09+02:00",
     *     "departure_time":"2012-04-09T11:36:09+02:00",
     *     "departure_station":"Bern",
     *     "arrival_time":"2012-04-09T11:36:09+02:00",
     *     "arrival_station":"Basel SBB"
     * }
     */

    public class TrainshareFriend
    {
        public string Name { get; set; }

        public string Message { get; set; }
        public bool Upper { get; set; }
        public int Position { get; set; }
        [JsonProperty("trainshare_id")]
        public string TrainshareId { get; set; }
        [JsonProperty("image_url")]
        public string ImageUrl { get; set; }
        [JsonProperty("overlaps")]
        public TrainshareOverlap TrainshareOverlaps { get; set; }

        public ImageSource UserImage
        {
            get
            {
                var realImage = new BitmapImage();

                realImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                realImage.ImageFailed +=
                    (sender, args) => realImage.UriSource = new Uri("/icons/user.png", UriKind.Relative);

                realImage.UriSource = new Uri(ImageUrl);

                return realImage;
            }
        }
    }
}
