using System;
using System.Collections.Generic;
using System.Linq;

namespace TrainShareApp.Model
{
    public class Checkin
    {
        public float Position { get; set; }
        public string DepartureStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime ArrivalTime { get; set; }
        public IList<CheckinSection> Sections { get; set; }
        public DateTime CheckinTime { get; set; }

        public void FromCheckin(Checkin checkin)
        {
            DepartureStation = checkin.DepartureStation;
            DepartureTime = checkin.DepartureTime;

            ArrivalStation = checkin.ArrivalStation;
            ArrivalTime = checkin.ArrivalTime;

            Sections = checkin.Sections;
            CheckinTime = checkin.CheckinTime;
        }

        public static Checkin FromConnection(Connection connection)
        {
            return
                new Checkin
                    {
                        DepartureStation = connection.From.Station.Name,
                        DepartureTime = connection.From.Departure,
                        ArrivalStation = connection.To.Station.Name,
                        ArrivalTime = connection.To.Arrival,
                        Position = 0.5f,
                        Sections =
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
                            .ToList()
                    };
        }

        public static bool operator ==(Checkin a, Checkin b)
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

        public static bool operator !=(Checkin a, Checkin b)
        {
            return !(a == b);
        }

        public bool Equals(Checkin other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return
                Equals(other.DepartureStation, DepartureStation) &&
                other.DepartureTime.TimeOfDay.Equals(DepartureTime.TimeOfDay) &&
                Equals(other.ArrivalStation, ArrivalStation) &&
                other.ArrivalTime.TimeOfDay.Equals(ArrivalTime.TimeOfDay) &&
                other.Sections.SequenceEqual(Sections);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Checkin)) return false;

            return Equals((Checkin) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = (DepartureStation != null ? DepartureStation.GetHashCode() : 0);
                result = (result*397) ^ DepartureTime.GetHashCode();
                result = (result*397) ^ (ArrivalStation != null ? ArrivalStation.GetHashCode() : 0);
                result = (result*397) ^ ArrivalTime.GetHashCode();
                result = (result*397) ^ (Sections != null ? Sections.GetHashCode() : 0);
                return result;
            }
        }
    }
}