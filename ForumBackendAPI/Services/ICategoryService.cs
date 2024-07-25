﻿using ForumBackendAPI.Models;

namespace ForumBackendAPI.Services;

public interface ICategoryService
{
    IEnumerable<Category> Get();
    Category Create(Category category);
    
    public string NameFromCategoryId(int categoryId);
    public int CategoryIdFromName(string categoryName);
}