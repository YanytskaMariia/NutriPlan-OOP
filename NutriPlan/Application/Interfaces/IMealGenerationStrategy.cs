using System.Collections.Generic;
using NutriPlan.Domain;

namespace NutriPlan.Application.Interfaces
{
    /// <summary>
    /// Strategy interface for selecting products to fill macro targets
    /// Allows different algorithms for meal generation
    /// </summary>
    public interface IMealGenerationStrategy
    {
        /// <summary>
        /// Selects products from available list to meet macro targets
        /// </summary>
        List<Product> SelectProducts(
            List<Product> availableProducts,
            double calorieTarget,
            double proteinTarget,
            double fatTarget,
            double carbsTarget);
    }
}
