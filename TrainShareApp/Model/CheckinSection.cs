using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace TrainShareApp.Model
{
    [Table]
    public class CheckinSection
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

        [Column]
        public string TrainId { get; set; }
        [Column]
        public string DepartureStation { get; set; }
        [Column]
        public DateTime DepartureTime { get; set; }
        [Column]
        public string ArrivalStation { get; set; }
        [Column]
        public DateTime ArrivalTime { get; set; }

        // Private column for the associated Checkin ID value
        [Column]
        private int _checkinId;

        // Entity reference, to identify the Checkin "storage" table
        private EntityRef<Checkin> _checkin;

        // Association, to describe the relationship between this key and that "storage" table
        [Association(Storage = "_checkin", ThisKey = "_checkinId", OtherKey = "Id", IsForeignKey = true)]
        public Checkin Checkin
        {
            get { return _checkin.Entity; }
            set
            {
                //OnPropertyChanging("Checkin");
                _checkin.Entity = value;

                if (value != null)
                {
                    _checkinId = value.Id;
                }

                //OnPropertyChanged("Checkin");
            }
        }

        public static bool operator ==(CheckinSection a, CheckinSection b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object) a == null) || ((object) b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }

        public static bool operator !=(CheckinSection a, CheckinSection b)
        {
            return !(a == b);
        }

        public bool Equals(CheckinSection other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                Equals(other.TrainId, TrainId) &&
                Equals(other.DepartureStation, DepartureStation) &&
                other.DepartureTime.TimeOfDay.Equals(DepartureTime.TimeOfDay) &&
                Equals(other.ArrivalStation, ArrivalStation) &&
                other.ArrivalTime.TimeOfDay.Equals(ArrivalTime.TimeOfDay);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CheckinSection)) return false;

            return Equals((CheckinSection) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (TrainId != null ? TrainId.GetHashCode() : 0);
                result = (result*397) ^ (DepartureStation != null ? DepartureStation.GetHashCode() : 0);
                result = (result*397) ^ DepartureTime.GetHashCode();
                result = (result*397) ^ (ArrivalStation != null ? ArrivalStation.GetHashCode() : 0);
                result = (result*397) ^ ArrivalTime.GetHashCode();
                return result;
            }
        }
    }
}