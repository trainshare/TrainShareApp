using System;
using System.Data.Linq;
using System.Diagnostics;
using System.Linq.Expressions;
using TrainShareApp.Model;

namespace TrainShareApp.Data
{
    public class DbDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DbConnectionString = "Data Source=isostore:/Trainshare.sdf";

        /// <summary>
        /// Table for CheckinSections
        /// </summary>
        public Table<CheckinSection> CheckinSections;

        /// <summary>
        /// Table for Checkins
        /// </summary>
        public Table<Checkin> Checkins;

        /// <summary>
        /// Table for Facebook/Twitter/Trainshare tokens
        /// </summary>
        public Table<Token> Tokens;

        public DbDataContext(string connectionString)
            : base(connectionString)
        { }

        public DbDataContext IncludeOne<T>(Expression<Func<T, object>> expr)
        {
            var options = new DataLoadOptions();
            options.LoadWith(expr);
            
            LoadOptions = options;

            return this;
        }
    }
}