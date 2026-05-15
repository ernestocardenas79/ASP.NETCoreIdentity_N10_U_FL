using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;
using WebApp_UnderTheHood.Pages.Account;

namespace WebApp_UnderTheHood.Pages
{
    [Authorize(policy:"HRManagerOnly")]
    public class HRManagerModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public List<WeatherForecastDTO>? weatherForecastItems { get; set; }

        public HRManagerModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task OnGet()
        {
            var client = _httpClientFactory.CreateClient("OurWebAPI");
            var response = await client.PostAsJsonAsync("Auth", new Credential { UserName = "admin", Password = "admin" });
            response.EnsureSuccessStatusCode();

            string strJwt = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken??string.Empty);
            weatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
        }
    }
}
