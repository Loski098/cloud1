using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;

public class AttractionServiceDb : IAttractionService
{
    private readonly AttractionDbRepos _attractionDbRepos;
    private readonly CommentDbRepos _commentDbRepos;
    private readonly ILogger<AttractionServiceDb> _logger;    
    
    public AttractionServiceDb(AttractionDbRepos attractionDbRepos, CommentDbRepos commentDbRepos, ILogger<AttractionServiceDb> logger)
    {
        _attractionDbRepos = attractionDbRepos;
        _commentDbRepos = commentDbRepos;
        _logger = logger;
    }

    public Task<ResponseItemDto<IAttraction>> ReadAttractionAsync(Guid id, bool flat) => _attractionDbRepos.ReadItemAsync(id, flat);
    public Task<ResponsePageDto<IAttraction>> ReadAttractionsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _attractionDbRepos.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IAttraction>> DeleteAttractionAsync(Guid id) => _attractionDbRepos.DeleteItemAsync(id);
    public Task<ResponseItemDto<IAttraction>> PostAttractionAsync(AttractionCuDto item) => _attractionDbRepos.CreateItemAsync(item);
    public Task<ResponseItemDto<IAttraction>> UpdateAttractionAsync(AttractionCuDto item) => _attractionDbRepos.UpdateItemAsync(item);
}