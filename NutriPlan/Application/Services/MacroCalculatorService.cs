using NutriPlan.Domain;

namespace NutriPlan.Application.Services
{
    /// <summary>
    /// Сервіс для розрахунку добової норми білків, жирів та вуглеводів (БЖВ).
    /// </summary>
    public class MacroCalculatorService
    {
        /// <summary>
        /// Розраховує макронутрієнти на основі цільових калорій та мети користувача.
        /// Використовує відсотковий розподіл для запобігання критично низькому рівню вуглеводів.
        /// </summary>
        /// <param name="weight">Вага користувача в кілограмах (використовується для довідки у поясненні).</param>
        /// <param name="targetCalories">Загальна кількість калорій, розрахована для користувача.</param>
        /// <param name="goal">Мета користувача (Схуднення, Підтримка, Набір маси).</param>
        /// <returns>Об'єкт <see cref="MacroResult"/> з розрахованими грамами БЖВ.</returns>
        public MacroResult CalculateMacros(double weight, double targetCalories, Goal goal)
        {
            double proteinPercentage;
            double fatPercentage;
            double carbsPercentage;

            // Визначаємо розподіл у відсотках залежно від мети
            switch (goal)
            {
                case Goal.WeightLoss:
                    // Вищий вміст білка для збереження м'язів при дефіциті
                    proteinPercentage = 0.35; // 35%
                    fatPercentage = 0.30;     // 30%
                    carbsPercentage = 0.35;    // 35%
                    break;

                case Goal.MuscleGain:
                    // Більше вуглеводів для енергії під час тренувань
                    proteinPercentage = 0.25; // 25%
                    fatPercentage = 0.25;     // 25%
                    carbsPercentage = 0.50;    // 50%
                    break;

                case Goal.Maintenance:
                default:
                    // Збалансований підхід
                    proteinPercentage = 0.30; // 30%
                    fatPercentage = 0.30;     // 30%
                    carbsPercentage = 0.40;    // 40%
                    break;
            }

            // Розрахунок грамів:
            // Білки: 1г = 4 ккал
            // Жири: 1г = 9 ккал
            // Вуглеводи: 1г = 4 ккал
            double proteinGrams = (targetCalories * proteinPercentage) / 4;
            double fatGrams = (targetCalories * fatPercentage) / 9;
            double carbsGrams = (targetCalories * carbsPercentage) / 4;

            return new MacroResult
            {
                TotalCalories = targetCalories,
                ProteinGrams = proteinGrams,
                FatGrams = fatGrams,
                CarbsGrams = carbsGrams,
                FormulaExplanation = $"Розподіл за метою '{goal}': " +
                                     $"Білки: {proteinPercentage * 100}%, " +
                                     $"Жири: {fatPercentage * 100}%, " +
                                     $"Вуглеводи: {carbsPercentage * 100}%. " +
                                     $"Вага: {weight} кг."
            };
        }
    }
}