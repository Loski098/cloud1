using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

using Models.DTO;
using Models;
using DbModels;
using DbContext;
using Configuration;
using Microsoft.Identity.Client;

namespace DbRepos;

public class AddressDbRepos
{
    private readonly ILogger<AddressDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AddressDbRepos(ILogger<AddressDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<IAddress>> ReadItemAsync (Guid id, bool flat)
    {
        IQueryable<AddressDbM> query = _dbContext.Address.AsNoTracking()
            .Where(i => i.AddressId == id);

            if (!flat)
            {
                query = _dbContext.Address.AsNoTracking()
                    .Include(a => a.AttractionDbM)
                    .Where(a => a.AddressId == id);  
            }

            var resp =  await query.FirstOrDefaultAsync<IAddress>();
            return new ResponseItemDto<IAddress>()
            {
                DbConnectionKeyUsed = _dbContext.dbConnection,
                Item = resp
            };
    }

    public async Task<ResponsePageDto<IAddress>> ReadItemsAsync (bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        IQueryable<AddressDbM> query = _dbContext.Address.AsNoTracking();

         if (!flat)
         {
            query = _dbContext.Address.AsNoTracking()
                .Include(a => a.AttractionDbM);   
        }

        return new ResponsePageDto<IAddress>
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            

             .Where(i => (i.Seeded == seeded) && 
                    i.City.ToLower().Contains(filter)).CountAsync(),

             PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                    i.City.ToLower().Contains(filter))
                        

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAddress>(),

            PageNr = pageNumber,
            PageSize = pageSize


        };

    } 
     
}