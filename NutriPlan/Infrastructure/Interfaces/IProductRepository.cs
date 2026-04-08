using System.Collections.Generic;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
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