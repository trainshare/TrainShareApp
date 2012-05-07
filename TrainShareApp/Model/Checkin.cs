using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace TrainShareApp.Model
{
    [Table]
    public class Checkin
    {
        private readonly EntitySet<CheckinSection> _sections;

        public Checkin()
        {
            _sections = new EntitySet<CheckinSection>(AttachSection, DetachSection);
        }

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
        /// Get or set the position in the train (1 - 10)
        /// </summary>
        [Column]
        public float Position { get; set; }

        /// <summary>
        /// Get or set the name of the departure station
        /// </summary>
        [Column]
        public string DepartureStation { get; set; }

        /// <summary>
        /// Get or set the time of departure
        /// </summary>
        [Column]
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Get or set the name of the arrival station
        /// </summary>
        [Column]
        public string ArrivalStation { get; set; }

        /// <summary>
        /// Get or set the time of arrival
        /// </summary>
        [Column]
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Get or set the check in time
        /// </summary>
        [Column]
        public DateTime CheckinTime { get; set; }

        /// <summary>
        /// Get or set if the user explicitly checked out
        /// </summary>
        [Column(CanBeNull = true)]
        public bool CheckedOut { get; set; }

        /// <summary>
        /// Get or set the sections of the travel
        /// </summary>
        [Association(Storage = "_sections", OtherKey = "_checkinId", ThisKey = "Id")]
        public EntitySet<CheckinSection> Sections
        {
            get { return _sections; }
            set { _sections.Assign(value); }
        }

        /// <summary>
        /// Set the checkin reference of the <paramref name="section"/> to <value>this</value>
        /// </summary>
        /// <param name="section">The section to modify</param>
        private void AttachSection(CheckinSection section)
        {
            //OnPropertyChanging("CheckinSection");
            section.Checkin = this;
        }

        /// <summary>
        /// Remove the checkin reference of the <paramref name="section"/>
        /// </summary>
        /// <param name="section">The section to modify</param>
        private void DetachSection(CheckinSection section)
        {
            //OnPropertyChanging("CheckinSection");
            section.Checkin = null;
        }

        /// <summary>
        /// Create new checkin from <paramref name="connection"/>
        /// </summary>
        /// <param name="connection">The source connection</param>
        /// <returns>The created checkin object</returns>
        public static Checkin FromConnection(Connection connection)
        {
            var checkin =
                new Checkin
                {
                    DepartureStation = connection.From.Station.Name,
                    DepartureTime = connection.From.Departure,
                    ArrivalStation = connection.To.Station.Name,
                    ArrivalTime = connection.To.Arrival,
                    Position = 0.5f
                };

            checkin.Sections.AddRange(
                connection
                    .Sections
                    .Select(
                        section =>
                        new CheckinSection
                        {
                            TrainId = section.Journey.Name,
                            DepartureStation = section.Departure.Station.Name,
                            DepartureTime = section.Departure.Departure,
                            ArrivalStation = section.Arrival.Station.Name,
                            ArrivalTime = section.Arrival.Arrival
                        })
                    .ToList());

            return checkin;
        }
    }
}