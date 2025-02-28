using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class AnimalDbRepos
{
    private readonly ILogger<AnimalDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    #region contructors
    public AnimalDbRepos(ILogger<AnimalDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
    #endregion

    public async Task<ResponseItemDto<IAnimal>> ReadItemAsync(Guid id, bool flat)
    {
        IQueryable<AnimalDbM> query;
        if (!flat)
        {
            query = _dbContext.Animals.AsNoTracking()
                .Include(i => i.ZooDbM)
                .Where(i => i.AnimalId == id);
        }
        else
        {
            query = _dbContext.Animals.AsNoTracking()
                .Where(i => i.AnimalId == id);
        }

        var resp = await query.FirstOrDefaultAsync<IAnimal>();
        return new ResponseItemDto<IAnimal>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = resp
        };
    }

    public async Task<ResponsePageDto<IAnimal>> ReadItemsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<AnimalDbM> query;
        if (flat)
        {
            query = _dbContext.Animals.AsNoTracking();
        }
        else
        {
            query = _dbContext.Animals.AsNoTracking()
                .Include(i => i.ZooDbM);
        }

        var ret = new ResponsePageDto<IAnimal>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.Name.ToLower().Contains(filter) ||
                         i.strMood.ToLower().Contains(filter) ||
                         i.strKind.ToLower().Contains(filter) ||
                         i.Age.ToString().Contains(filter) ||
                         i.Description.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                        (i.Name.ToLower().Contains(filter) ||
                         i.strMood.ToLower().Contains(filter) ||
                         i.strKind.ToLower().Contains(filter) ||
                         i.Age.ToString().Contains(filter) ||
                         i.Description.ToLower().Contains(filter)))

            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IAnimal>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<IAnimal>> DeleteItemAsync(Guid id)
    {
        var query1 = _dbContext.Animals
            .Where(i => i.AnimalId == id);

        var item = await query1.FirstOrDefaultAsync<AnimalDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Animals.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IAnimal>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<IAnimal>> UpdateItemAsync(AnimalCuDto itemDto)
    {
        var query1 = _dbContext.Animals
            .Where(i => i.AnimalId == itemDto.AnimalId);
        var item = await query1
                .Include(i => i.ZooDbM)
                .FirstOrDefaultAsync<AnimalDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.AnimalId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties 
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Animals.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.AnimalId, false);    
    }

    public async Task<ResponseItemDto<IAnimal>> CreateItemAsync(AnimalCuDto itemDto)
    {
        if (itemDto.AnimalId != null)
            throw new ArgumentException($"{nameof(itemDto.AnimalId)} must be null when creating a new object");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new AnimalDbM(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Animals.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.AnimalId, false);    
    }

    private async Task navProp_ItemCUdto_to_ItemDbM(AnimalCuDto itemDtoSrc, AnimalDbM itemDst)
    {
        //update zoo nav props
        var zoo = await _dbContext.Zoos.FirstOrDefaultAsync(
            a => (a.ZooId == itemDtoSrc.ZooId));

        if (zoo == null)
            throw new ArgumentException($"Item id {itemDtoSrc.ZooId} not existing");

        itemDst.ZooDbM = zoo;
    }
}
