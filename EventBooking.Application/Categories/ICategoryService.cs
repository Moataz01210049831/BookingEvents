using EventBooking.Application.Categories.DTOs;

namespace EventBooking.Application.Categories
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto> CreateAsync(CreateCategoryRequest request);
    }
}