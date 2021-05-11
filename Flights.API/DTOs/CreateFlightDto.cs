using Flights.Domain.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flights.API.DTOs
{
    public record CreateFlightDto
    {
        public string FlightNumber { get; init; }
        public string Airline { get; init; }
        public string Origin { get; init; }
        public string Destination { get; init; }
        public DateTime DepartureDate { get; init; }
        public TravelClass TravelClass { get; init; }
    }
}
