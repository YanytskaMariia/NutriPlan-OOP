using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NutriPlan.Domain;
using NutriPlan.Infrastructure.Interfaces;

namespace NutriPlan.Application.Services
{
    /// <summary>
    /// Parallel implementation of meal plan generation using TPL.
    /// Generates each meal concurrently (one task per meal).
    /// </summary>
    public class ParallelMealGenerationService
    {
        /// <summary>
        /// Generate meal plan in parallel. Returns list of meals and fills mealProducts (mealName -> products).
        /// degreeOfParallelism defaults to Environment.ProcessorCount.
        /// </summary>
        public List<Meal> GenerateMealPlanParallel(
            IProductRepository productRepository,
            MacroResult macros,
            int mealCount,
            out Dictionary<string, List<Product>> mealProducts,
            int degreeOfParallelism = 0)
        {
            mealProducts = new Dictionary<string, List<Product>>();
            var meals = new List<Meal>();

            if (productRepository == null) return meals;
            var availableProducts = productRepository.GetAllProducts() ?? new List<Product>();
            if (!availableProducts.Any() || mealCount <= 0) return meals;

            if (degreeOfParallelism <= 0) degreeOfParallelism = Environment.ProcessorCount;

            double perMealCalories = macros.TotalCalories / mealCount;
            double perMealProtein = macros.ProteinGrams / mealCount;
            double perMealFat = macros.FatGrams / mealCount;
            double perMealCarbs = macros.CarbsGrams / mealCount;

            var mealsArr = new Meal[mealCount];
            var productsArr = new List<Product>[mealCount];

            var rndLocal = new ThreadLocal<Random>(() => new Random(Guid.NewGuid().GetHashCode()));

            Parallel.For(0, mealCount, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, i =>
            {
                // Create meal target
                var meal = new Meal($"Meal {i + 1}", perMealCalories, perMealProtein, perMealFat, perMealCarbs);

                // Use greedy selection per meal (local selection uses the full product list, selection does not mutate source)
                var selected = new List<Product>();
                double currentCalories = 0;
                var shuffled = availableProducts.OrderBy(x => rndLocal.Value.Next()).ToList();

                foreach (var p in shuffled)
                {
                    if (currentCalories >= perMealCalories * 0.9) break;
                    if (currentCalories + p.Calories <= perMealCalories * 1.2)
                    {
                        selected.Add(p);
                        currentCalories += p.Calories;
                    }
                }

                mealsArr[i] = meal;
                productsArr[i] = selected;
            });

            for (int i = 0; i < mealCount; i++)
            {
                meals.Add(mealsArr[i]);
                mealProducts[mealsArr[i].Name] = productsArr[i] ?? new List<Product>();
            }

            return meals;
        }
    }
}
