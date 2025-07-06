using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SWEN2_TourPlannerGroupProject.Services
{
    public class OpenRouteServiceClient
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public OpenRouteServiceClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public async Task<JObject> GetRouteAsync(double fromLat, double fromLon, double toLat, double toLon)
        {
            var url = "https://api.openrouteservice.org/v2/directions/driving-car/geojson";

            var body = new
            {
                coordinates = new[]
                {
                    new[] { fromLon, fromLat },
                    new[] { toLon, toLat }
                }
            };

            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            content.Headers.Add("Authorization", _apiKey);

            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            return JObject.Parse(jsonString);
        }

        public async Task<(double lat, double lon)> GeocodeAsync(string location)
        {
            var url = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={Uri.EscapeDataString(location)}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonString);

            var coords = json["features"]?[0]?["geometry"]?["coordinates"];
            if (coords == null || coords.Count() < 2)
                throw new Exception("Could not geocode address");

            double lon = coords[0].Value<double>();
            double lat = coords[1].Value<double>();

            return (lat, lon);
        }
    }
}



