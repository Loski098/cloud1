using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;


public class AdminServiceDb : IAdminService {

    private readonly AdminDbRepos _adminRepo;
    private readonly ILogger<AdminServiceDb> _logger;    
    
    public AdminServiceDb(AdminDbRepos adminRepo, ILogger<AdminServiceDb> logger)
    {
        _adminRepo = adminRepo;
        _logger = logger;
    }

    public Task<ResponseItemDto<GstUsrInfoAllDto>> InfoAsync() => _adminRepo.InfoAsync();
    public Task<ResponseItemDto<GstUsrInfoAllDto>> SeedAsync(int nrAtt,int nrOfItems) => _adminRepo.SeedAsync(nrAtt, nrOfItems );
    public Task<ResponseItemDto<GstUsrInfoAllDto>> RemoveSeedAsync(bool seeded) => _adminRepo.RemoveSeedAsync(seeded);
    public Task<UsrInfoDto> SeedUsersAsync(int nrOfUsers, int nrOfSuperUsers, int nrOfSysAdmin) => _adminRepo.SeedUsersAsync(nrOfUsers, nrOfSuperUsers, nrOfSysAdmin);
}