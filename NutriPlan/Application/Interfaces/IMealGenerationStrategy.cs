using System;
using System.Collections.Generic;
using System.Linq;
using NutriPlan.Domain;

namespace NutriPlan.Application.Services
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
