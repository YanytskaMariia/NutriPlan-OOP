namespace NutriPlan.Domain
{
    /// <summary>
    /// A product selected for a meal with a specific quantity in grams.
    /// Assumes Product nutritional values are per 100g.
    /// </summary>
    public class MealProduct
    {
        public Product Product { get; }
        public double Grams { get; private set; } // e.g. 150 = 150g

        public MealProduct(Product product, double grams)
        {
            Product = product;
            Grams = grams;
        }

        public void AddGrams(double g) => Grams += g;

        private double Scale => Grams / 100.0;

        public double Calories => Product == null ? 0.0 : Product.Calories * Scale;
        public double Protein => Product == null ? 0.0 : Product.Protein * Scale;
        public double Fat => Product == null ? 0.0 : Product.Fat * Scale;
        public double Carbs => Product == null ? 0.0 : Product.Carbs * Scale;
    }
}
