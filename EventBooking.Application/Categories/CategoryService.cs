using EventBooking.Application.Categories.DTOs;
using EventBooking.Application.Common.Exceptions;
using EventBooking.Application.Common.Interfaces;
using EventBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBooking.Application.Categories
{
    public class CategoryService (IApplicationDbContext _applicationDbContext, IMessageService  _message) : ICategoryService
    {
        public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request)
        {
            var exists = await _applicationDbContext.EventCategories
               .AnyAsync(c => c.Name == request.Name);

            if (exists)
            {
                throw new ValidationException(_message.Get("CategoryAlreadyExists"));
            }
            var category = new EventCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };
            _applicationDbContext.EventCategories.Add(category);
            await _applicationDbContext.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            return await _applicationDbContext.EventCategories
                  .Select(c => new CategoryDto
                  {
                      Id = c.Id,
                      Name = c.Name
                  })
                  .ToListAsync();
        }
    }
}
