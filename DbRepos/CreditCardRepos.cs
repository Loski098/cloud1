using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;
using Configuration;

namespace DbRepos;

public class CreditCardDbRepos
{
    private readonly ILogger<CreditCardDbRepos> _logger;
    private readonly MainDbContext _dbContext;
    private Encryptions _encryptions;

    #region contructors
    public CreditCardDbRepos(ILogger<CreditCardDbRepos> logger, Encryptions encryptions, MainDbContext context)
    {
        _logger = logger;
        _encryptions = encryptions;
        _dbContext = context;
    }
    #endregion

    public async Task<ResponseItemDto<ICreditCard>> ReadItemAsync(Guid id, bool flat)
    {
        IQueryable<CreditCardDbM> query;
        if (!flat)
        {
            query = _dbContext.CreditCards.AsNoTracking()
                .Include(i => i.EmployeeDbM)
                .Where(i => i.CreditCardId == id);
        }
        else
        {
            query = _dbContext.CreditCards.AsNoTracking()
                .Where(i => i.CreditCardId == id);
        }

        var resp = await query.FirstOrDefaultAsync<CreditCard>();
        return new ResponseItemDto<ICreditCard>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = resp
        };
    }

    public async Task<ResponsePageDto<ICreditCard>> ReadItemsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<CreditCardDbM> query;
        if (flat)
        {
            query = _dbContext.CreditCards.AsNoTracking();
        }
        else
        {
            query = _dbContext.CreditCards.AsNoTracking()
                .Include(i => i.EmployeeDbM);
        }

        var ret = new ResponsePageDto<ICreditCard>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.FirstName.ToLower().Contains(filter) ||
                         i.LastName.ToLower().Contains(filter) ||
                         i.strIssuer.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.FirstName.ToLower().Contains(filter) ||
                         i.LastName.ToLower().Contains(filter) ||
                         i.strIssuer.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<ICreditCard>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<ICreditCard>> DeleteItemAsync(Guid id)
    {
        var query1 = _dbContext.CreditCards
            .Where(i => i.CreditCardId == id);

        var item = await query1.FirstOrDefaultAsync<CreditCardDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.CreditCards.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<ICreditCard>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }


    public async Task<ResponseItemDto<ICreditCard>> CreateItemAsync(CreditCardCuDto itemDto)
    {
        if (itemDto.CreditCardId != null)
            throw new ArgumentException($"{nameof(itemDto.CreditCardId)} must be null when creating a new object");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new CreditCardDbM(itemDto);

        item.EnryptAndObfuscate(_encryptions.AesEncryptToBase64);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.CreditCards.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.CreditCardId, false);    
    }

    //CRUD support
    private async Task navProp_ItemCUdto_to_ItemDbM(CreditCardCuDto itemDtoSrc, CreditCardDbM itemDst)
    {
        //update Employee nav props
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(
            a => (a.EmployeeId == itemDtoSrc.EmployeeId));

        if (employee == null)
            throw new ArgumentException($"Item id {itemDtoSrc.EmployeeId} not existing");

        itemDst.EmployeeDbM = employee;
    }

    //Special Non-CRUD repo
    public async Task<ResponsePageDto<IEmployee>> ReadEmployeesWithCCAsync(bool hasCreditCard, int pageNumber, int pageSize)
    {
        var query = _dbContext.Employees.AsNoTracking()
            .Include(i => i.CreditCardDbM);

        var ret = new ResponsePageDto<IEmployee>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

                //Adding filter functionality
                .Where(i => i.CreditCardDbM == null).CountAsync(),

            PageItems = await query

                //Adding filter functionality
                .Where(i =>(hasCreditCard) ?i.CreditCardDbM != null : i.CreditCardDbM == null)

                //Adding paging
                .Skip(pageNumber * pageSize)
                .Take(pageSize)

                .ToListAsync<IEmployee>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<ICreditCard>> ReadDecryptedCCAsync(Guid id)
    {
        IQueryable<CreditCardDbM> query = _dbContext.CreditCards.AsNoTracking()
                .Include(i => i.EmployeeDbM)
                .Where(i => i.CreditCardId == id);

        var resp = await query.FirstOrDefaultAsync<CreditCard>();
        var cc = resp.Decrypt(_encryptions.AesDecryptFromBase64<CreditCard>);

        //Nav props are not set in the decrypted object, set them
        cc.Employee = resp.Employee;

        return new ResponseItemDto<ICreditCard>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = cc
        };
    }
}
