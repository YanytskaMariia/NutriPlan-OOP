using NutriPlan.Domain;
using System.Collections.Generic;

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
    }
}