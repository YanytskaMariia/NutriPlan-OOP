using System;
using System.Collections.Generic;
using NutriPlan.Domain;

namespace NutriPlan.Domain
{
    /// <summary>
    /// Represents a meal with its constituent products
    /// Extended domain model for meal generation from products
    /// </summary>
    public class MealWithProducts : Meal
    {
        public List<Product> Products { get; set; } = new List<Product>();

        public MealWithProducts(string name, double calories, double protein, double fat, double carbs)
            : base(name, calories, protein, fat, carbs)
        {
        }

        public void AddProduct(Product product)
        {
            Products.Add(product);
        }

        /// <summary>
        /// Calculates actual totals from all products in the meal
        /// </summary>
        public (double Calories, double Protein, double Fat, double Carbs) CalculateActualTotals()
        {
            double calories = 0;
            double protein = 0;
            double fat = 0;
            double carbs = 0;

            foreach (var product in Products)
            {
                calories += product.Calories;
                protein += product.Protein;
                fat += product.Fat;
                carbs += product.Carbs;
            }

            return (calories, protein, fat, carbs);
        }
    }
}
