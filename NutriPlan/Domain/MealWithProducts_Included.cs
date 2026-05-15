using System.Collections.Generic;

namespace NutriPlan.Domain
{
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

        public (double Calories, double Protein, double Fat, double Carbs) CalculateActualTotals()
        {
            double calories = 0;
            double protein = 0;
            double fat = 0;
            double carbs = 0;

            foreach (var p in Products)
            {
                calories += p.Calories;
                protein += p.Protein;
                fat += p.Fat;
                carbs += p.Carbs;
            }

            return (calories, protein, fat, carbs);
        }
    }
}