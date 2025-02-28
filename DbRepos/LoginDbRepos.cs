using Configuration;
using Models;
using Models.DTO;
using DbModels;
using DbContext;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace DbRepos;

public class LoginDbRepos
{
    private readonly ILogger<LoginDbRepos> _logger;
    private readonly MainDbContext _dbContext;
    private Encryptions _encryptions;

    public LoginDbRepos(ILogger<LoginDbRepos> logger, Encryptions encryptions, MainDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        _encryptions = encryptions;
    }



    public async Task<ResponseItemDto<LoginUserSessionDto>> LoginUserAsync(LoginCredentialsDto usrCreds)
    {
        using (var cmd1 = _dbContext.Database.GetDbConnection().CreateCommand())
        {
            //Notice how I use the efc Command to call sp as I do not return any dataset, only output parameters
            //Notice also how I encrypt the password, no coms to database with open password
            cmd1.CommandType = CommandType.StoredProcedure;
            cmd1.CommandText = "gstusr.spLogin";
            cmd1.Parameters.Add(new SqlParameter("UserNameOrEmail", usrCreds.UserNameOrEmail));
            cmd1.Parameters.Add(new SqlParameter("Password", _encryptions.EncryptPasswordToBase64(usrCreds.Password)));

            int _usrIdIdx = cmd1.Parameters.Add(new SqlParameter("UserId", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output });
            int _usrIdx = cmd1.Parameters.Add(new SqlParameter("UserName", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });
            int _roleIdx = cmd1.Parameters.Add(new SqlParameter("Role", SqlDbType.NVarChar, 100) { Direction = ParameterDirection.Output });


            _dbContext.Database.OpenConnection();
            await cmd1.ExecuteScalarAsync();

            var info = new LoginUserSessionDto
            {
                //Notice the soft cast conversion 'as' it will be null if cast cannot be made
                UserId = cmd1.Parameters[_usrIdIdx].Value as Guid?,
                UserName = cmd1.Parameters[_usrIdx].Value as string,
                UserRole = cmd1.Parameters[_roleIdx].Value as string
            };

            return new ResponseItemDto<LoginUserSessionDto>()
            {
                DbConnectionKeyUsed = _dbContext.dbConnection,
                Item = info
            };
        }
    }
}


