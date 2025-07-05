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
    }
}

