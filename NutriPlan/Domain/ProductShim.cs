namespace NutriPlan.Domain
{
    /// <summary>
    /// Product shim included in project to ensure type is available to the compiler.
    /// If original Domain\Product.cs is added to the project file later, this shim can be removed.
    /// </summary>
    public class Product
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }

        public Product() { }

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