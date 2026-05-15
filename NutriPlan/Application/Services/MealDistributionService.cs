using System.Collections.Generic;
using System.Linq;
using System;
using NutriPlan.Domain;

namespace NutriPlan.Application.Services
{
    /// <summary>
    /// Service to distribute daily macros and calories across meals
    /// </summary>
    public class MealDistributionService
    {
        /// <summary>
        /// Evenly distributes daily totals across the selected number of meals
        /// </summary>
        public List<Meal> DistributeMeals(MacroResult macros, int mealCount)
        {
            var meals = new List<Meal>();
            for (int i = 1; i <= mealCount; i++)
            {
                meals.Add(new Meal(
                    $"Meal {i}",
                    macros.TotalCalories / mealCount,
                    macros.ProteinGrams / mealCount,
                    macros.FatGrams / mealCount,
                    macros.CarbsGrams / mealCount));
            }
            return meals;
        }

        /// <summary>
        /// Generates a meal plan (meals and products per meal) using the repository and an internal default strategy.
        /// Returns list of meals and fills out mealProducts map (meal name -> products).
        /// </summary>
        public List<Meal> GenerateMealPlan(
            NutriPlan.Infrastructure.Interfaces.IProductRepository productRepository,
            MacroResult macros,
            int mealCount,
            out Dictionary<string, List<Product>> mealProducts)
        {
            mealProducts = new Dictionary<string, List<Product>>();
            var meals = new List<Meal>();

            if (productRepository == null) return meals;
            var availableProducts = productRepository.GetAllProducts() ?? new List<Product>();
            if (!availableProducts.Any() || mealCount <= 0) return meals;

            // Use internal default strategy
            var strategy = new DefaultStrategy();

            double perMealCalories = macros.TotalCalories / mealCount;
            double perMealProtein = macros.ProteinGrams / mealCount;
            double perMealFat = macros.FatGrams / mealCount;
            double perMealCarbs = macros.CarbsGrams / mealCount;

            for (int i = 1; i <= mealCount; i++)
            {
                var meal = new Meal($"Meal {i}", perMealCalories, perMealProtein, perMealFat, perMealCarbs);

                // Let strategy pick products for this meal
                var selected = strategy.SelectProducts(new List<Product>(availableProducts), perMealCalories, perMealProtein, perMealFat, perMealCarbs) ?? new List<Product>();

                meals.Add(meal);
                mealProducts[meal.Name] = selected;
            }

            return meals;
        }

        /// <summary>
        /// Simple internal greedy strategy to avoid referencing external strategy implementations.
        /// </summary>
        private class DefaultStrategy
        {
            private readonly Random _random = new Random();

            public List<Product> SelectProducts(List<Product> availableProducts, double calorieTarget, double proteinTarget, double fatTarget, double carbsTarget)
            {
                var selectedProducts = new List<Product>();
                double currentCalories = 0;

                var shuffled = availableProducts.OrderBy(x => _random.Next()).ToList();

                foreach (var product in shuffled)
                {
                    if (currentCalories >= calorieTarget * 0.9)
                        break;

                    if (currentCalories + product.Calories <= calorieTarget * 1.2)
                    {
                        selectedProducts.Add(product);
                        currentCalories += product.Calories;
                    }
                }

                return selectedProducts;
            }
        }
    }
}