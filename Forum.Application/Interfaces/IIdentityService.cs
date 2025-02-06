using Forum.Application.Requests.Identity;
using Forum.Domain.Models;

namespace Forum.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<Result> RegisterAsync(RegisterRequest request);
        Task<Result> LoginAsync(LoginRequest request);
        Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request);
        Task<Result> ResetPasswordAsync(ResetPasswordRequest request);
        Task<Result> VerifyEmailAsync(VerifyEmailRequest request);
    }
}
