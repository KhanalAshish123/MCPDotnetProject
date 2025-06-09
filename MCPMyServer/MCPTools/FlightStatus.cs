using System.Text.Json;
using System.ComponentModel;
using ModelContextProtocol.Server;

namespace MCPServer.MCPTools
{
    [McpServerToolType]
    public static class FlightStatus
    {
        static readonly HttpClient httpClient = new();

        [McpServerTool, Description("Get the current status, departure/arrival airports and times of a flight using the flight IATA code (e.g., 'EK202')")]
        public static async Task<string> GetFlightStatusAsync(string flightIATA)
        {
            var apiKey = "094512cb4ff112701a745fb84ea5060b";  
            var url = $"http://api.aviationstack.com/v1/flights?access_key={apiKey}&flight_iata={Uri.EscapeDataString(flightIATA)}";

            try
            {
                var json = await httpClient.GetStringAsync(url);
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var data = root.GetProperty("data");
                if (data.GetArrayLength() == 0)
                    return $"No flight status found for flight {flightIATA}.";

                var flight = data[0]; 
                var departureAirport = flight.GetProperty("departure").GetProperty("airport").GetString();
                var arrivalAirport = flight.GetProperty("arrival").GetProperty("airport").GetString();
                var airline = flight.GetProperty("airline").GetProperty("name").GetString();
                var status = flight.GetProperty("flight_status").GetString();

                var departureTime = flight.GetProperty("departure").GetProperty("scheduled").GetString();
                var arrivalTime = flight.GetProperty("arrival").GetProperty("scheduled").GetString();

                return $"Flight {flightIATA} ({airline}) is currently {status}.\n" +
                       $"Departure: {departureAirport} at {departureTime}\n" +
                       $"Arrival: {arrivalAirport} at {arrivalTime}";
            }
            catch (Exception ex)
            {
                return $"Sorry, I couldn't fetch the flight status for {flightIATA}. ({ex.Message})";
            }
        }
    }
}
