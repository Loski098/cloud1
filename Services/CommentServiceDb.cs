using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;

public class CommentServiceDb : ICommentServiceDb
{
    private readonly CommentDbRepos _commentDbRepos;
    private readonly ILogger<CommentServiceDb> _logger;    
    
    public CommentServiceDb(CommentDbRepos commentDbRepos, ILogger<CommentServiceDb> logger)
    {
        _commentDbRepos = commentDbRepos;
        _logger = logger;
    }

    public Task<ResponsePageDto<IComment>> ReadCommentsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _commentDbRepos.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);
    public Task<ResponseItemDto<IComment>> ReadCommentAsync(Guid id, bool flat) => _commentDbRepos.ReadItemAsync(id, flat);
    public Task<ResponseItemDto<IComment>> DeleteCommentAsync(Guid id) => _commentDbRepos.DeleteItemAsync(id);
    public Task<ResponseItemDto<IComment>> UpdateCommentAsync(CommentCuDto item) => _commentDbRepos.UpdateItemAsync(item);
    public Task<ResponseItemDto<IComment>> CreateCommentAsync(CommentCuDto item) => _commentDbRepos.CreateItemAsync(item);
}