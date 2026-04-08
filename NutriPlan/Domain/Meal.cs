namespace NutriPlan.Domain
{
    /// <summary>
    /// Represents a single meal with its target macros
    /// </summary>
    public class Meal
    {
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double Fat { get; set; }
        public double Carbs { get; set; }

        public Meal(string name, double calories, double protein, double fat, double carbs)
        {
            Name = name;
            Calories = calories;
            Protein = protein;
            Fat = fat;
            Carbs = carbs;
        }
    }
}