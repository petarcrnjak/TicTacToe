using Microsoft.AspNetCore.Identity;
using TicTacToe.Authorization;
using TicTacToe.Contracts.Requests.Auth;
using TicTacToe.Contracts.Responses.Auth;

namespace TicTacToe.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required.", nameof(request));

            var existing = await _userManager.FindByNameAsync(request.Username);
            if (existing is not null)
                throw new InvalidOperationException("Username already exists.");

            var user = new AppUser { UserName = request.Username };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

            return user.UserName!;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ArgumentException("Username and password are required.", nameof(request));

            var user = await _userManager.FindByNameAsync(request.Username);
            if (user is null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = _tokenService.GenerateJwt(user.Id, user.UserName ?? string.Empty);

            return new LoginResponse { Token = token, Username = user.UserName ?? string.Empty };
        }
    }
}