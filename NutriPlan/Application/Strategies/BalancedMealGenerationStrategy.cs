using System;
using System.Collections.Generic;
using System.Linq;
using NutriPlan.Domain;
using NutriPlan.Application.Services;

namespace NutriPlan.Application.Strategies
{
    /// <summary>
    /// Balanced algorithm for meal generation
    /// Attempts to match macro ratios in addition to calories
    /// More sophisticated than greedy, slower execution
    /// </summary>
    public class BalancedMealGenerationStrategy : IMealGenerationStrategy
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

            var remaining = new List<Product>(availableProducts);

            // Iteratively select products that best match remaining macro needs
            while (remaining.Count > 0 && currentCalories < calorieTarget * 0.9)
            {
                // Calculate remaining targets
                double remainingCalories = Math.Max(0, calorieTarget - currentCalories);
                double remainingProtein = Math.Max(0, proteinTarget - currentProtein);
                double remainingFat = Math.Max(0, fatTarget - currentFat);
                double remainingCarbs = Math.Max(0, carbsTarget - currentCarbs);

                // Find product with best macro fit
                Product bestProduct = null;
                double bestScore = double.MaxValue;

                foreach (var product in remaining)
                {
                    // Skip if product would exceed total calories too much
                    if (currentCalories + product.Calories > calorieTarget * 1.5)
                        continue;

                    // Calculate deviation score (lower is better)
                    double calorieDeviation = Math.Abs((currentCalories + product.Calories) - calorieTarget);
                    double proteinDeviation = Math.Abs((currentProtein + product.Protein) - proteinTarget);
                    double fatDeviation = Math.Abs((currentFat + product.Fat) - fatTarget);
                    double carbsDeviation = Math.Abs((currentCarbs + product.Carbs) - carbsTarget);

                    // Weighted deviation (calorie weight is higher)
                    double score = (calorieDeviation * 0.5) + (proteinDeviation * 0.2) + 
                                   (fatDeviation * 0.15) + (carbsDeviation * 0.15);

                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestProduct = product;
                    }
                }

                if (bestProduct != null)
                {
                    selectedProducts.Add(bestProduct);
                    currentCalories += bestProduct.Calories;
                    currentProtein += bestProduct.Protein;
                    currentFat += bestProduct.Fat;
                    currentCarbs += bestProduct.Carbs;
                    remaining.Remove(bestProduct);
                }
                else
                {
                    break;
                }
            }

            return selectedProducts;
        }
    }
}
