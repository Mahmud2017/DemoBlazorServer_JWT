using DemoBlazorServerWithJWTAuth.DTOs;
using static DemoBlazorServerWithJWTAuth.Responses.CustomResponses;

namespace DemoBlazorServerWithJWTAuth.Services
{
    public interface IAccountService
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
        Task<LoginResponse> LoginAsync(LoginDTO model);
        Task<LoginResponse> RefreshToken(UserSession model);
        Task<WeatherForecastDTO[]> GetWeatherForecasts();
    }
}
