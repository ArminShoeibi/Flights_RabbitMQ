using Flights.Domain.Enumerations;
using System;

namespace Flights.Domain
{
    public class Flight
    {
        public long FlightId { get; set; }
        public string FlightNumber { get; set; }
        public string Airline { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTimeOffset DateCreated { get; set; }
        public TravelClass TravelClass { get; set; }
    }
}
