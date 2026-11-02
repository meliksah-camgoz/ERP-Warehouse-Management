namespace MiniERP.Application.Authentication;

public interface IAuthService
{
    Task<string?> LoginAsync(LoginRequest request);
}