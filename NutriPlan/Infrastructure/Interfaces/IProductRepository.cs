using System.Collections.Generic;
using NutriPlan.Domain;

namespace NutriPlan.Infrastructure.Interfaces
{
    /// <summary>
    /// Repository interface for products
    /// </summary>
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
    }
}