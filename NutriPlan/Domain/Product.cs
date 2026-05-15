namespace NutriPlan.Domain
{
    /// <summary>
    /// Represents a food product with nutritional information
    /// </summary>
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }

        public Product()
        {
        }

        public Product(string name, string category, double calories, double protein, double fat, double carbs)
        {
            Name = name;
            Category = category;
            Calories = calories;
            Protein = protein;
            Fat = fat;
            Carbs = carbs;
        }
    }
}
