using DemoBlazorServerWithJWTAuth.DTOs;
using static DemoBlazorServerWithJWTAuth.Responses.CustomResponses;

namespace DemoBlazorServerWithJWTAuth.Repositories
{
    public interface IAccount
    {
        Task<RegistrationResponse> RegisterAsync(RegisterDTO model);
        Task<LoginResponse> LoginAsync(LoginDTO model);
        LoginResponse RefreshToken(UserSession session);
    }
}
