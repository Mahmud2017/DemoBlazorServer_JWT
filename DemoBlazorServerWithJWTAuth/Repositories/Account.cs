using DemoBlazorServerWithJWTAuth.Data;
using DemoBlazorServerWithJWTAuth.DTOs;
using DemoBlazorServerWithJWTAuth.Models;
using DemoBlazorServerWithJWTAuth.States;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static DemoBlazorServerWithJWTAuth.Responses.CustomResponses;

namespace DemoBlazorServerWithJWTAuth.Repositories
{
    public class Account : IAccount
    {
        private readonly AppDbContext _appDbContext;
        private readonly IConfiguration _config;

        public Account(AppDbContext appDbContext, IConfiguration config)
        {
            _appDbContext = appDbContext;
            _config = config;
        }

        public async Task<LoginResponse> LoginAsync(LoginDTO model)
        {
            var findUser = await GetUser(model.Email);

            if (findUser == null)
                return new LoginResponse(false, "User not found");

            if (!BCrypt.Net.BCrypt.Verify(model.Password, findUser!.Password))
                return new LoginResponse(false, "Password not valid");

            string jwtToken = GenerateToken(findUser);
            return new LoginResponse(true, "Login Successful", jwtToken);
        }

        public LoginResponse RefreshToken(UserSession session)
        {
            CustomUserClaims customUserClaims = DecryptJwtService.DecryptToken(session.JwtToken);

            if(customUserClaims is null)
                return new LoginResponse(false, "Incorrect Token");

            string newToken = GenerateToken(new ApplicationUser()
            {
                FirstName = customUserClaims.Name.Split(" ")[0],
                LastName = customUserClaims.Name.Split(" ")[1],
                Email = customUserClaims.Email
            });

            return new LoginResponse(true, "New Token", newToken);
        }

        public async Task<RegistrationResponse> RegisterAsync(RegisterDTO model)
        {
            var findUser = await GetUser(model.Email);
            
            if (findUser != null)
                return new RegistrationResponse(false, "User already exists");

            _appDbContext.Users.Add(
                new ApplicationUser()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = model.Role
                });

            await _appDbContext.SaveChangesAsync();
            return new RegistrationResponse(true, "Registration Successful");
        }

        public async Task<ApplicationUser> GetUser(string email)
            => await _appDbContext.Users.FirstOrDefaultAsync(x => x.Email == email);

        private string GenerateToken(ApplicationUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var userClaims = new[]
            {
                new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role!)
            };

            var dtn = DateTime.UtcNow.AddHours(1);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: userClaims,
                expires: dtn.AddMinutes(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
