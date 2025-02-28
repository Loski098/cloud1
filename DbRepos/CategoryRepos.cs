using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.SqlClient;

using Models.DTO;
using Models;
using DbModels;
using DbContext;
using Microsoft.Identity.Client;

namespace DbRepos;

public class CategoryDbRepos
{
    private readonly ILogger<CategoryDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    public CategoryDbRepos(ILogger<CategoryDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }

    public async Task<ResponseItemDto<ICategory>> ReadItemAsync (Guid id, bool flat)
    {
        IQueryable<CategoryDbM> query = _dbContext.Category.AsNoTracking()
            .Where(i => i.CategoryId == id);

            if (!flat){
                query = query.Include(a => a.AttractionsDbM);   
            }
            var resp =  await query.FirstOrDefaultAsync<ICategory>();
            return new ResponseItemDto<ICategory>()
            {
                DbConnectionKeyUsed = _dbContext.dbConnection,
                Item = resp
            };

        // var at = await _dbContext.Attractions.Where(at => at.AttractionId == id).FirstAsync();
        // return at;
    }

    public async Task<ResponsePageDto<ICategory>> ReadItemsAsync (bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";

        IQueryable<CategoryDbM> query = _dbContext.Category.AsNoTracking();

         if (!flat){
             query = query.Include(a => a.AttractionsDbM);   
            }

        return new ResponsePageDto<ICategory>
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            

             .Where(i => (i.Seeded == seeded) && 
                    i.Catkind.ToLower().Contains(filter)).CountAsync(),

             PageItems = await query

            //Adding filter functionality
           .Where(i => (i.Seeded == seeded) && 
                    i.Catkind.ToLower().Contains(filter))
                        
            
            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<ICategory>(),

            PageNr = pageNumber,
            PageSize = pageSize


        };

        

        // var at = await _dbContext.Attractions.ToListAsync<IAttraction>();
        // return at;
    } 

    public async Task<ResponseItemDto<ICategory>> DeleteItemAsync(Guid id)
    {
        var query = _dbContext.Category
        .Where(a => a.CategoryId == id);
        var item = await query.FirstOrDefaultAsync();

        if(item == null) throw new ArgumentException($"Item: {id} is not existing");

        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<ICategory> 
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<IAttraction>> DeleteAttraction(Guid id)
    {
        var query = _dbContext.Attraction.Where(a => a.AttractionId == id);
        var item = await query.FirstOrDefaultAsync<AttractionDbM>();

        if(item == null) throw new ArgumentException($"No object linked to id: {id}");

        _dbContext.Remove(item);

        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IAttraction>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<ICategory>> CreateItemAsync(CategoryCuDto itemDto)
    {
        if (itemDto.CategoryId != null) 
          throw new ArgumentException($"{nameof(itemDto.CategoryId)} must be null when creating a new object");

          var item = new CategoryDbM(itemDto);

          await navProp_itemDto_to_ItemDbM(itemDto,item);

          _dbContext.Add(item);

          await _dbContext.SaveChangesAsync();

          return await ReadItemAsync(item.CategoryId, true);
    }

    public async Task<ResponseItemDto<ICategory>> UpdateItemAsync(CategoryCuDto itemDto)
{
    var item = await _dbContext.Category
        .Include(c => c.AttractionsDbM)  
        .FirstOrDefaultAsync(a => a.CategoryId == itemDto.CategoryId);

    if (item == null) 
        throw new ArgumentException($"Category {itemDto.CategoryId} does not exist in the database.");

    item.UpdateFromDTO(itemDto);

  
    await navProp_itemDto_to_ItemDbM(itemDto, item);

    
    await _dbContext.SaveChangesAsync();

    return await ReadItemAsync(item.CategoryId, true);
}




   

private async Task navProp_itemDto_to_ItemDbM(CategoryCuDto itemDtoSrc, CategoryDbM itemDst)
{
    if (itemDtoSrc.AttractionsId == null) return;

    
    var attractions = await _dbContext.Attraction
        .Where(a => itemDtoSrc.AttractionsId.Contains(a.AttractionId))
        .ToListAsync();

  
    foreach (var attraction in attractions)
    {
        if (attraction.CategoryDbM != null)
        {
            attraction.CategoryDbM.AttractionsDbM.Remove(attraction);  
        }

        attraction.CategoryDbM = itemDst;  
    }

    
    itemDst.AttractionsDbM = attractions;

    
    _dbContext.Entry(itemDst).State = EntityState.Modified;
}







     
}