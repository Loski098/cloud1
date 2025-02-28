using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

using Models.DTO;
using Services;
using Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AppWebApi.Controllers
{
#if !DEBUG    
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
        Policy = null, Roles = "sysadmin")]
#endif
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AdminController : Controller
    {
        readonly DatabaseConnections _dbConnections;
        readonly IAdminService _adminService;
        readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger, DatabaseConnections dbConnections)
        {
            _adminService = adminService;
            _logger = logger;
            _dbConnections = dbConnections;
        }

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(DatabaseConnections.SetupInformation))]
        public IActionResult Info()
        {
            try
            {
                var info = _dbConnections.SetupInfo;

                _logger.LogInformation($"{nameof(Info)}:\n{JsonConvert.SerializeObject(info)}");
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Info)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
         }

#if DEBUG
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
            Policy = null, Roles = "sysadmin")]

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<GstUsrInfoAllDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Seed(string nrAtt = "1000", string nrAtt2 = "100")
        {
            try
            {
                
                int countArg = int.Parse(nrAtt);
                int countArg2 = int.Parse(nrAtt2);

                    if(countArg < 1000 )
                    return BadRequest("Attractions count cant be under 1000 and and adreeses must not be under 100");

                _logger.LogInformation($"{nameof(Seed)}: {nameof(countArg)}: {countArg}");
                var info = await _adminService.SeedAsync(countArg, countArg2);
                return Ok(info);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Seed)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
            Policy = null, Roles = "sysadmin")]

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<GstUsrInfoAllDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> RemoveSeed(string seeded = "true")
        {
            try
            {
                bool seededArg = bool.Parse(seeded);

                _logger.LogInformation($"{nameof(RemoveSeed)}: {nameof(seededArg)}: {seededArg}");
                var info = await _adminService.RemoveSeedAsync(seededArg);
                return Ok(info);        
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(RemoveSeed)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(UsrInfoDto))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> SeedUsers(string countUsr = "47", string countSupUsr = "2", string countSysAdmin = "1")
        {
            try
            {
                int _countUsr = int.Parse(countUsr);
                int _countSupUsr = int.Parse(countSupUsr);
                int _countSysAdmin = int.Parse(countSysAdmin);

                if(_countUsr + _countSupUsr + _countSysAdmin < 50 )
                return BadRequest("The sum of users must be under 50");

                _logger.LogInformation($"{nameof(SeedUsers)}: {nameof(_countUsr)}: {_countUsr}, {nameof(_countSupUsr)}: {_countSupUsr}, {nameof(_countSysAdmin)}: {_countSysAdmin}");

                UsrInfoDto _info = await _adminService.SeedUsersAsync(_countUsr, _countSupUsr, _countSysAdmin);
                return Ok(_info);           
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }       
        }
#endif

        [HttpGet()]
        [ProducesResponseType(200, Type = typeof(IEnumerable<LogMessage>))]
        public async Task<IActionResult> Log([FromServices] ILoggerProvider _loggerProvider)
        {
            //Note the way to get the LoggerProvider, not the logger from Services via DI
            if (_loggerProvider is InMemoryLoggerProvider cl)
            {
                return Ok(await cl.MessagesAsync);
            }
            return Ok("No messages in log");
        }
    }
}

