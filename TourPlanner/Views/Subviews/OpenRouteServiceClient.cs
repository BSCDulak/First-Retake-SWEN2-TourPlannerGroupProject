using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

            var jsonBody = JsonConvert.SerializeObject(body);

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
            };

            
            request.Headers.Add("X-Api-Key", _apiKey);

            System.Diagnostics.Debug.WriteLine($"[OpenRouteServiceClient] Using API Key: {_apiKey}");

            var response = await _httpClient.SendAsync(request);
            System.Diagnostics.Debug.WriteLine($"[OpenRouteServiceClient] StatusCode: {response.StatusCode}");

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



