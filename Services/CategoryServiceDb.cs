using DbRepos;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;

namespace Services;     

public class CategoryServiceDb : ICategoryService    
{
    private readonly CategoryDbRepos _categoryDbRepos; 
    private readonly ILogger<CategoryServiceDb> _logger;  
    
    public CategoryServiceDb(CategoryDbRepos categoryDbRepos, ILogger<CategoryServiceDb> logger)
    {
        _categoryDbRepos = categoryDbRepos;
        _logger = logger;
    }

    public Task<ResponseItemDto<ICategory>> ReadCategoryAsync(Guid id, bool flat) => _categoryDbRepos.ReadItemAsync(id, flat);
    
    public Task<ResponsePageDto<ICategory>> ReadCategoriesAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) => _categoryDbRepos.ReadItemsAsync(seeded, flat, filter, pageNumber, pageSize);

    public Task<ResponseItemDto<ICategory>> DeleteCategoryAsync(Guid id) => _categoryDbRepos.DeleteItemAsync(id);

    public Task<ResponseItemDto<ICategory>> PostCategoryAsync(CategoryCuDto item) => _categoryDbRepos.CreateItemAsync(item);

    public Task<ResponseItemDto<ICategory>> UpdateCategoryAsync(CategoryCuDto item) => _categoryDbRepos.UpdateItemAsync(item);

}