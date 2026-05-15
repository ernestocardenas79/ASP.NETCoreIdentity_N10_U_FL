using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using WebApp_UnderTheHood.Authorization;
using WebApp_UnderTheHood.DTO;

namespace WebApp_UnderTheHood.Pages;

[Authorize(policy: "HRManagerOnly")]
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
        //ge token from session
        JwtToken token = new JwtToken();

        var strToken = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(strToken))
        {
            token =await Authenticate();
        }
        else
        {
            token = JsonConvert.DeserializeObject<JwtToken>(strToken) ?? new JwtToken();
        }

        if (token == null || string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpiresAt < DateTime.UtcNow)
        {
            token= await Authenticate();
        }

        var client = _httpClientFactory.CreateClient("OurWebAPI");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken ?? string.Empty);
        weatherForecastItems = await client.GetFromJsonAsync<List<WeatherForecastDTO>>("WeatherForecast");
    }

    private async Task<JwtToken> Authenticate()
    {
        var client = _httpClientFactory.CreateClient("OurWebAPI");

        var response = await client.PostAsJsonAsync("Auth", new Credential { UserName = "admin", Password = "admin" });
        response.EnsureSuccessStatusCode();

        string strJwt = await response.Content.ReadAsStringAsync();

        HttpContext.Session.SetString("access_token", strJwt);
        return JsonConvert.DeserializeObject<JwtToken>(strJwt)??new();  

    }
}