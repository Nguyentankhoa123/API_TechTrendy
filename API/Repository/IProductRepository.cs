﻿using API.Model.Entity;

namespace API.Repositories
{
    public interface IProductRepository
    {
        Task<dynamic> GetAllProducts(int pageNumber, int pageSize);

        Task<Product> GetProductById(int id);
    }
}
