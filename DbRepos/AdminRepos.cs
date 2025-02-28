using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

using Seido.Utilities.csSeedGenerator;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;
using Models;
using System.Security;

namespace DbRepos;

public class AdminDbRepos
{
    private const string _seedSource = "./app-seeds.json";
    private readonly ILogger<AdminDbRepos> _logger;
    private Encryptions _encryptions;
    private readonly MainDbContext _dbContext;

    #region contructors
    public AdminDbRepos(ILogger<AdminDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
    #endregion

    public async Task<ResponseItemDto<GstUsrInfoAllDto>> InfoAsync()
    {
        var info = new GstUsrInfoAllDto();
        info.Db = await _dbContext.InfoDbView.FirstAsync();
        //info.Zoos = await _dbContext.InfoZoosView.ToListAsync();
        //info.Animals = await _dbContext.InfoAnimalsView.ToListAsync();
        //info.Employees = await _dbContext.InfoEmployeesView.ToListAsync();

        return new ResponseItemDto<GstUsrInfoAllDto>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = info
        };
    }

     public async Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrOfAttractions, int nrAddress)
    {
        try 
        {
            await RemoveSeedAsync(true);

        var rnd = new csSeedGenerator();
        var at = rnd.ItemsToList<AttractionDbM>(nrOfAttractions);
        var ad = rnd.ItemsToList<AddressDbM>(nrAddress);
        var comments = rnd.ItemsToList<CommentDbM>(rnd.Next(nrOfAttractions, 20*nrOfAttractions));

        var i = 0;

        var allCategories = await SeedEachCategoryAsync();

        foreach (var item in at){
            item.CategoryDbM = rnd.FromList(allCategories);
            item.AddressDbM = rnd.FromList(ad);
            item.CommentsDbM = rnd.UniqueIndexPickedFromList<CommentDbM>(rnd.Next(0,21), comments);
            i++;
        }

        _dbContext.Attraction.AddRange(at);
    
        await _dbContext.SaveChangesAsync();

            return await InfoAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in SeedAsync: {ex.Message}");
            _logger.LogError($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<List<CategoryDbM>> SeedEachCategoryAsync(){

        var allCategories = new List<CategoryDbM>();

        foreach (CategoryNames category in Enum.GetValues(typeof(CategoryNames)))
        {
            CategoryDbM cat = new();
            cat.CategoryId = Guid.NewGuid();
            cat.Name = category;
            cat.Seeded = true;
            
            allCategories.Add(cat);
        }

        await _dbContext.Category.AddRangeAsync(allCategories);

        return allCategories;

    }
    
    public async Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded)
    {
            var parameters = new List<SqlParameter>();

            var retValue = new SqlParameter("retval", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var seededArg = new SqlParameter("seeded", seeded);

            parameters.Add(retValue);
            parameters.Add(seededArg);

            //there is no FromSqlRawAsync to I make one here
            var _query = await Task.Run(() =>
                _dbContext.InfoDbView.FromSqlRaw($"EXEC @retval = supusr.spDeleteAll @seeded",
                    parameters.ToArray()).AsEnumerable());

            //Execute the query and get the sp result set.
            //Although, I am not using this result set, but it shows how to get it
            GstUsrInfoDbDto result_set = _query.FirstOrDefault();

            //Check the return code
            int retCode = (int)retValue.Value;
            if (retCode != 0) throw new Exception("supusr.spDeleteAll return code error");

            return await InfoAsync();
    }

    public async Task<UsrInfoDto> SeedUsersAsync(int nrOfUsers, int nrOfSuperUsers, int nrOfSysAdmin)
    {
            _logger.LogInformation($"Seeding {nrOfUsers} users and {nrOfSuperUsers} superusers");
            
            //First delete all existing users
            foreach (var u in _dbContext.Users)
                _dbContext.Users.Remove(u);

            //add users
            for (int i = 1; i <= nrOfUsers; i++)
            {
                _dbContext.Users.Add(new UserDbM
                {
                    UserId = Guid.NewGuid(),
                    UserName = $"user{i}",
                    Email = $"user{i}@gmail.com",
                    Password = _encryptions.EncryptPasswordToBase64($"user{i}"),
                    Role = "usr"
                });
            }

            //add super user
            for (int i = 1; i <= nrOfSuperUsers; i++)
            {
                _dbContext.Users.Add(new UserDbM
                {
                    UserId = Guid.NewGuid(),
                    UserName = $"superuser{i}",
                    Email = $"superuser{i}@gmail.com",
                    Password = _encryptions.EncryptPasswordToBase64($"superuser{i}"),
                    Role = "supusr"
                });
            }

            //add system adminitrators
            for (int i = 1; i <= nrOfSysAdmin; i++)
            {
                _dbContext.Users.Add(new UserDbM
                {
                    UserId = Guid.NewGuid(),
                    UserName = $"sysadmin{i}",
                    Email = $"sysadmin{i}@gmail.com",
                    Password = _encryptions.EncryptPasswordToBase64($"sysadmin{i}"),
                    Role = "sysadmin"
                });
            }
            await _dbContext.SaveChangesAsync();

            var _info = new UsrInfoDto
            {
                NrUsers = await _dbContext.Users.CountAsync(i => i.Role == "usr"),
                NrSuperUsers = await _dbContext.Users.CountAsync(i => i.Role == "supusr"),
                NrSystemAdmin = await _dbContext.Users.CountAsync(i => i.Role == "sysadmin")
            };

            return _info;
    }
}
