using System;

namespace TrainShareApp.Model
{
    public class CheckinSection
    {
        public string TrainId { get; set; }
        public string DepartureStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime ArrivalTime { get; set; }

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