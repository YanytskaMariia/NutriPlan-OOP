namespace NutriPlan.Domain
{
    /// <summary>
    /// Model to store total daily calories and macros (grams and kcal)
    /// </summary>
    public class MacroResult
    {
        public double TotalCalories { get; set; }
        public double ProteinGrams { get; set; }
        public double FatGrams { get; set; }
        public double CarbsGrams { get; set; }
        
        // Calculate kcal values based on grams
        public double ProteinKcal => ProteinGrams * 4;
        public double FatKcal => FatGrams * 9;
        public double CarbsKcal => CarbsGrams * 4;
        
        /// <summary>
        /// Explanation of the calculation formulas used
        /// </summary>
        public string FormulaExplanation { get; set; }
    }
}