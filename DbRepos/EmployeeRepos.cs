using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class EmployeeDbRepos
{
    private readonly ILogger<EmployeeDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    #region contructors
    public EmployeeDbRepos(ILogger<EmployeeDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
    #endregion

    public async Task<ResponseItemDto<IEmployee>> ReadItemAsync(Guid id, bool flat)
    {
        IQueryable<EmployeeDbM> query;
        if (!flat)
        {
            query = _dbContext.Employees.AsNoTracking()
                .Include(i => i.ZoosDbM)
                .Include(i => i.CreditCardDbM)
                .Where(i => i.EmployeeId == id);
        }
        else
        {
            query = _dbContext.Employees.AsNoTracking()
                .Where(i => i.EmployeeId == id);
        }

        var resp = await query.FirstOrDefaultAsync<IEmployee>();
        return new ResponseItemDto<IEmployee>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = resp
        };
    }

    public async Task<ResponsePageDto<IEmployee>> ReadItemsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<EmployeeDbM> query;
        if (flat)
        {
            query = _dbContext.Employees.AsNoTracking();
        }
        else
        {
            query = _dbContext.Employees.AsNoTracking()
                .Include(i => i.ZoosDbM)
                .Include(i => i.CreditCardDbM);
        }

        var ret = new ResponsePageDto<IEmployee>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.FirstName.ToLower().Contains(filter) ||
                         i.LastName.ToLower().Contains(filter) ||
                         i.strRole.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.FirstName.ToLower().Contains(filter) ||
                         i.LastName.ToLower().Contains(filter) ||
                         i.strRole.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IEmployee>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<IEmployee>> DeleteItemAsync(Guid id)
    {
        var query1 = _dbContext.Employees
            .Where(i => i.EmployeeId == id);

        var item = await query1.FirstOrDefaultAsync<EmployeeDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Employees.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IEmployee>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<IEmployee>> UpdateItemAsync(EmployeeCuDto itemDto)
    {
        var query1 = _dbContext.Employees
            .Where(i => i.EmployeeId == itemDto.EmployeeId);
        var item = await query1
                .Include(i => i.ZoosDbM)
                .FirstOrDefaultAsync<EmployeeDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.EmployeeId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties 
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Employees.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.EmployeeId, false);    
    }

    public async Task<ResponseItemDto<IEmployee>> CreateItemAsync(EmployeeCuDto itemDto)
    {
        if (itemDto.EmployeeId != null)
            throw new ArgumentException($"{nameof(itemDto.EmployeeId)} must be null when creating a new object");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new EmployeeDbM(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Employees.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.EmployeeId, false);    
    }

    private async Task navProp_ItemCUdto_to_ItemDbM(EmployeeCuDto itemDtoSrc, EmployeeDbM itemDst)
    {
        //update ZooDbM from itemDto.ZooId
        List<ZooDbM> zoos = null;
        if (itemDtoSrc.ZooIds != null)
        {
            zoos = new List<ZooDbM>();
            foreach (var id in itemDtoSrc.ZooIds)
            {
                var z = await _dbContext.Zoos.FirstOrDefaultAsync(i => i.ZooId == id);
                if (z == null)
                    throw new ArgumentException($"Item id {id} not existing");

                zoos.Add(z);
            }
        }
        itemDst.ZoosDbM = zoos;
    }
}
