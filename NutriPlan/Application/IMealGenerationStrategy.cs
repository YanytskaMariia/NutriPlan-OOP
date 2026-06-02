using System.Collections.Generic;
using NutriPlan.Domain;

namespace NutriPlan.Application
{
    public interface IMealGenerationStrategy
    {
        List<Product> SelectProducts(
            List<Product> availableProducts,
            double calorieTarget,
            double proteinTarget,
            double fatTarget,
            double carbsTarget);
    }
}
