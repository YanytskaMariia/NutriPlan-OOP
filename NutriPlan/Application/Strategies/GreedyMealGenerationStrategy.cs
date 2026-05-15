using System;
using System.Collections.Generic;
using System.Linq;
using NutriPlan.Domain;
using NutriPlan.Application.Services;

namespace NutriPlan.Application.Strategies
{
    /// <summary>
    /// Greedy algorithm implementation for meal generation
    /// Selects products one-by-one to approach macro targets
    /// Fast but may not produce optimal results
    /// </summary>
    public class GreedyMealGenerationStrategy : IMealGenerationStrategy
    {
        private readonly Random _random = new Random();

        public List<Product> SelectProducts(
            List<Product> availableProducts,
            double calorieTarget,
            double proteinTarget,
            double fatTarget,
            double carbsTarget)
        {
            var selectedProducts = new List<Product>();
            double currentCalories = 0;
            double currentProtein = 0;
            double currentFat = 0;
            double currentCarbs = 0;

            // Shuffle products for variety
            var shuffled = availableProducts.OrderBy(x => _random.Next()).ToList();

            // Greedy selection: keep adding products until targets are approached
            foreach (var product in shuffled)
            {
                // Check if adding this product would exceed targets by more than 20%
                double projectedCalories = currentCalories + product.Calories;
                double projectedProtein = currentProtein + product.Protein;
                double projectedFat = currentFat + product.Fat;
                double projectedCarbs = currentCarbs + product.Carbs;

                // Stop if we've reached most of the calorie target (80%+)
                if (currentCalories >= calorieTarget * 0.8)
                {
                    break;
                }

                // Add product if it doesn't massively overshoot targets
                if (projectedCalories <= calorieTarget * 1.3)
                {
                    selectedProducts.Add(product);
                    currentCalories = projectedCalories;
                    currentProtein = projectedProtein;
                    currentFat = projectedFat;
                    currentCarbs = projectedCarbs;
                }
            }

            return selectedProducts;
        }
    }
}
