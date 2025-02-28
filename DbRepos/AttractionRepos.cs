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
using System.IO.Compression;

namespace DbRepos;

public class AttractionDbRepos
{
    private readonly ILogger<AttractionDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public AttractionDbRepos(ILogger<AttractionDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<IAttraction>> ReadItemAsync (Guid id, bool flat)
    {
        IQueryable<AttractionDbM> query = _dbContext.Attraction.AsNoTracking()
            .Where(i => i.AttractionId == id);

            if (!flat)
            {
                query = _dbContext.Attraction.AsNoTracking()
                    .Include(a => a.AddressDbM)
                    .Include(a => a.CategoryDbM)
                    .Include(x => x.CommentsDbM)
                    .Where(a => a.AttractionId == id); 
            }

            var resp =  await query.FirstOrDefaultAsync<IAttraction>();
            return new ResponseItemDto<IAttraction>()
            {
                DbConnectionKeyUsed = _dbContext.dbConnection,
                Item = resp
            };
    }

    public async Task<ResponsePageDto<IAttraction>> ReadItemsAsync (bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        IQueryable<AttractionDbM> query = _dbContext.Attraction.AsNoTracking();

         if (!flat)
         {
            query = _dbContext.Attraction.AsNoTracking()
            .Include(a => a.AddressDbM)
            .Include(a => a.CategoryDbM)
            .Include(x => x.CommentsDbM); 
         }

        query = query.Where(i => i.Seeded == seeded &&
           (
              i.AttractionTitle.ToLower().Contains(filter) ||
              i.Description.ToLower().Contains(filter) ||
              i.CategoryDbM.Catkind.ToLower().Contains(filter) ||
              i.AddressDbM.City.ToLower().Contains(filter) ||
              i.AddressDbM.Country.ToLower().Contains(filter)
           ));

        return new ResponsePageDto<IAttraction>
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query.CountAsync(),
            PageItems = await query
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync<IAttraction>(),
            PageNr = pageNumber,
            PageSize = pageSize
        };

    } 

    public async Task<ResponseItemDto<IAttraction>> DeleteItemAsync(Guid id)
    {
        var query = _dbContext.Attraction
        .Where(a => a.AttractionId == id);
        var item = await query.FirstOrDefaultAsync();

        if(item == null) throw new ArgumentException($"Item: {id} is not existing");

        _dbContext.Attraction.Remove(item);

        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IAttraction> 
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<IAttraction>> CreateItemAsync(AttractionCuDto itemDto)
    {
        if (itemDto.AttractionId != null) 
          throw new ArgumentException($"{nameof(itemDto.AttractionId)} must be null when creating a new object");

          var item = new AttractionDbM(itemDto);

          await UpdateNavigationProp(itemDto, item);

          _dbContext.Add(item);

          await _dbContext.SaveChangesAsync();

          return await ReadItemAsync(item.AttractionId, true);
    }

    public async Task<ResponseItemDto<IAttraction>> UpdateItemAsync(AttractionCuDto itemDto)
    {
        var query1 = _dbContext.Attraction
            .Where(i => i.AttractionId == itemDto.AttractionId);
        var item = await query1
            .Include(i => i.CommentsDbM)
            .Include(i => i.AddressDbM)
            .Include(i => i.CategoryDbM)
            .FirstOrDefaultAsync<AttractionDbM>();

        if (item == null) throw new ArgumentException($"Item {itemDto.AttractionId} is not existing");

        item.UpdateFromDTO(itemDto);

        await UpdateNavigationProp(itemDto, item);

        _dbContext.Update(item);

        await _dbContext.SaveChangesAsync();

        return await ReadItemAsync(item.AttractionId, true);
    }

    public async Task UpdateNavigationProp(AttractionCuDto itemDto, AttractionDbM item)
    {
       // Update Category
        var updatedCat = await _dbContext.Category
            .FirstOrDefaultAsync(c => c.CategoryId == itemDto.CategoryId);
        if (updatedCat == null)
            throw new ArgumentException($"Category with id {itemDto.CategoryId} does not exist");
        item.CategoryDbM = updatedCat;

        // Update Address
        var updatedAddress = await _dbContext.Address
            .FirstOrDefaultAsync(a => a.AddressId == itemDto.AddressId);
        if (updatedAddress == null)
            throw new ArgumentException($"Address with id {itemDto.AddressId} does not exist");
        item.AddressDbM = updatedAddress;
    }

}