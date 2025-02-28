using Microsoft.Extensions.Logging;
using DbRepos;
using Models.DTO;
using DbContext;
using System.Security;

namespace Services;

public class LoginServiceDb : ILoginService
{
    private readonly LoginDbRepos _repo;
    private readonly JWTService _jtwService;

    private readonly ILogger<LoginServiceDb> _logger;

    public LoginServiceDb(ILogger<LoginServiceDb> logger, LoginDbRepos repo, JWTService jtwService)
    {
        _repo = repo;
        _jtwService = jtwService;
        _logger = logger;
    }

    public async Task<ResponseItemDto<LoginUserSessionDto>> LoginUserAsync(LoginCredentialsDto usrCreds)
    {
        try
        {
            var usrSession = await _repo.LoginUserAsync(usrCreds);

            //Successful login. Create a JWT token
            usrSession.Item.JwtToken = _jtwService.CreateJwtUserToken(usrSession.Item);

#if DEBUG
            //For test only, decypt the JWT token and compare.
            var tmp = _jtwService.DecodeToken(usrSession.Item.JwtToken.EncryptedToken);
            if (tmp.UserId != usrSession.Item.UserId) throw new SecurityException("JWT Token encryption error");
#endif
            return usrSession;
        }
        catch
        {
            //if there was an error during login, simply pass it on.
            throw;
        }
    }
}

