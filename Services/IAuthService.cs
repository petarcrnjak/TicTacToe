using System.Threading.Tasks;
using TicTacToe.Contracts.Requests.Auth;
using TicTacToe.Contracts.Responses.Auth;

namespace TicTacToe.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}