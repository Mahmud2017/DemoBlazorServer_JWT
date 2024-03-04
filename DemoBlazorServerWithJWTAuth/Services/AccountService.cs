using DemoBlazorServerWithJWTAuth.DTOs;
using DemoBlazorServerWithJWTAuth.States;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using static DemoBlazorServerWithJWTAuth.Responses.CustomResponses;

namespace DemoBlazorServerWithJWTAuth.Services
{
    public class AccountService : IAccountService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/account";

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherForecastDTO[]> GetWeatherForecasts()
        {
            GetProtectedClient();
            var response = await _httpClient.GetAsync($"{BaseUrl}/weather");
            bool isUnauthorized = CheckIfUnauthorized(response);

            if(isUnauthorized)
            {
                await GetNewToken();
                return await GetWeatherForecasts();
            }

            WeatherForecastDTO[]? weatherForecastDTOs = await response.Content.ReadFromJsonAsync<WeatherForecastDTO[]>();
            return weatherForecastDTOs!;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", model);
            var result = await response.Content.ReadFromJsonAsync<RegistrationResponse>();
            return result!;
        }

        public async Task<LoginResponse> RefreshToken(UserSession model)
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", model);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return result!;
        }

        private void GetProtectedClient()
        {
            if (Constants.JwtToken == "")
                return;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.JwtToken);
        }

        private bool CheckIfUnauthorized(HttpResponseMessage httpResponseMessage)
        {
            if(httpResponseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return true;

            return false;
        }

        private async Task GetNewToken()
        {
            var currentUserSession = new UserSession() { JwtToken = Constants.JwtToken };
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/refresh-token", currentUserSession);
            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            Constants.JwtToken = result!.JWTToken;
        }
    }
}
