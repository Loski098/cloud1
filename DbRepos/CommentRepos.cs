using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Models;
using Models.DTO;
using DbModels;
using DbContext;

namespace DbRepos;

public class CommentDbRepos
{
    private readonly ILogger<CommentDbRepos> _logger;
    private readonly MainDbContext _dbContext;

    #region contructors
    public CommentDbRepos(ILogger<CommentDbRepos> logger, MainDbContext context)
    {
        _logger = logger;
        _dbContext = context;
    }
    #endregion

    public async Task<ResponseItemDto<IComment>> ReadItemAsync(Guid id, bool flat)
    {
        IQueryable<CommentDbM> query;
        if (!flat)
        {
            query = _dbContext.Comment.AsNoTracking()
                .Include(i => i.AttractionDbM)
                .Where(i => i.CommentId == id);
        }
        else
        {
            query = _dbContext.Comment.AsNoTracking()
                .Where(i => i.CommentId == id);
        }

        var resp = await query.FirstOrDefaultAsync<IComment>();
        return new ResponseItemDto<IComment>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = resp
        };
    }

    public async Task<ResponsePageDto<IComment>> ReadItemsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        filter ??= "";
        IQueryable<CommentDbM> query;
        if (flat)
        {
            query = _dbContext.Comment.AsNoTracking();
        }
        else
        {
            query = _dbContext.Comment.AsNoTracking()
                .Include(i => i.AttractionDbM);
        }

        var ret = new ResponsePageDto<IComment>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            DbItemsCount = await query

            //Adding filter functionality
            .Where(i => (i.Seeded == seeded) && 
                         (i.strSentiment.ToLower().Contains(filter) ||
                         i.Content.ToLower().Contains(filter))).CountAsync(),

            PageItems = await query

            //Adding filter functionality
            /*.Where(i => (i.Seed == seeded) && 
                         (i.strSentiment.ToLower().Contains(filter) ||
                         i.Content.ToLower().Contains(filter)))
*/
            //Adding paging
            .Skip(pageNumber * pageSize)
            .Take(pageSize)

            .ToListAsync<IComment>(),

            PageNr = pageNumber,
            PageSize = pageSize
        };
        return ret;
    }

    public async Task<ResponseItemDto<IComment>> DeleteItemAsync(Guid id)
    {
        var query1 = _dbContext.Comment
            .Where(i => i.CommentId == id);

        var item = await query1.FirstOrDefaultAsync<CommentDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {id} is not existing");

        //delete in the database model
        _dbContext.Comment.Remove(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        return new ResponseItemDto<IComment>()
        {
            DbConnectionKeyUsed = _dbContext.dbConnection,
            Item = item
        };
    }

    public async Task<ResponseItemDto<IComment>> UpdateItemAsync(CommentCuDto itemDto)
    {
        var query1 = _dbContext.Comment
            .Where(i => i.CommentId == itemDto.CommentId);
        var item = await query1
                .Include(i => i.AttractionDbM)
                .FirstOrDefaultAsync<CommentDbM>();

        //If the item does not exists
        if (item == null) throw new ArgumentException($"Item {itemDto.CommentId} is not existing");

        //transfer any changes from DTO to database objects
        //Update individual properties 
        item.UpdateFromDTO(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Comment.Update(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.CommentId, false);    
    }

    public async Task<ResponseItemDto<IComment>> CreateItemAsync(CommentCuDto itemDto)
    {
        if (itemDto.CommentId != null)
            throw new ArgumentException($"{nameof(itemDto.CommentId)} must be null when creating a new object");

        //transfer any changes from DTO to database objects
        //Update individual properties
        var item = new CommentDbM(itemDto);

        //Update navigation properties
        await navProp_ItemCUdto_to_ItemDbM(itemDto, item);

        //write to database model
        _dbContext.Comment.Add(item);

        //write to database in a UoW
        await _dbContext.SaveChangesAsync();

        //return the updated item in non-flat mode
        return await ReadItemAsync(item.CommentId, false);    
    }

    private async Task navProp_ItemCUdto_to_ItemDbM(CommentCuDto itemDtoSrc, CommentDbM itemDst)
    {
        //update attraction nav props
        var attraction = await _dbContext.Attraction.FirstOrDefaultAsync(
            a => (a.AttractionId == itemDtoSrc.AttractionId));

        if (attraction == null)
            throw new ArgumentException($"Item id {itemDtoSrc.AttractionId} not existing");

        itemDst.AttractionDbM = attraction;
    }
}