using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TrainShareApp.Model
{
    [Table]
    public class Token : EntityBase
    {
        /// <summary>
        /// Version column aids update performance.
        /// </summary>
        [Column(IsVersion = true, DbType = "timestamp", IsDbGenerated = true)]
        protected Binary Version { get; set; }

        /// <summary>
        /// Get the PrimaryKey of the database
        /// </summary>
        [Column(IsPrimaryKey = true, IsDbGenerated = true, AutoSync = AutoSync.OnInsert)]
        public int Id { get; set; }

        /// <summary>
        /// Get or Set the screen name
        /// </summary>
        [Column]
        public string ScreenName { get; set; }

        /// <summary>
        /// Get or Set the access token
        /// </summary>
        [Column]
        public string AccessToken { get; set; }

        /// <summary>
        /// Get or Set the access token secret
        /// </summary>
        [Column]
        public string AccessTokenSecret { get; set; }

        /// <summary>
        /// Get or Set the date and time when this token expires
        /// </summary>
        [Column(CanBeNull = true)]
        public DateTime? Expires { get; set; }

        [Column(CanBeNull = false, IsPrimaryKey = true)]
        public string Network { get; set; }
    }
}