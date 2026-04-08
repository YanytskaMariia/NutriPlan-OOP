using NutriPlan.Domain;
using System;

namespace NutriPlan.Application.Services
{
    /// <summary>
    /// Service to calculate daily calorie requirements using Mifflin-St Jeor Equation
    /// </summary>
    public class CalorieCalculatorService
    {
        /// <summary>
        /// Calculates Basal Metabolic Rate (BMR) using Mifflin-St Jeor Equation
        /// Men: 10 * weight + 6.25 * height - 5 * age + 5
        /// Women: 10 * weight + 6.25 * height - 5 * age - 161
        /// </summary>
        public double CalculateBmr(double weight, double height, int age, Gender gender)
        {
            double bmr = (10 * weight) + (6.25 * height) - (5 * age);
            return gender == Gender.Male ? bmr + 5 : bmr - 161;
        }

        /// <summary>
        /// Returns the activity factor for the given level
        /// </summary>
        public double GetActivityFactor(ActivityLevel level)
        {
            switch (level)
            {
                case ActivityLevel.Sedentary: return 1.2;
                case ActivityLevel.LightlyActive: return 1.375;
                case ActivityLevel.ModeratelyActive: return 1.55;
                case ActivityLevel.VeryActive: return 1.725;
                default: return 1.2;
            }
        }

        /// <summary>
        /// Calculates Total Daily Energy Expenditure (TDEE): BMR * activity factor
        /// </summary>
        public double CalculateTdee(double bmr, ActivityLevel level)
            => bmr * GetActivityFactor(level);

        /// <summary>
        /// Calculates target calories based on goal:
        /// Weight Loss: Deficit -15%
        /// Maintenance: 0%
        /// Muscle Gain: Surplus +10-15% (using 15% for optimal results)
        /// </summary>
        public double CalculateTargetCalories(double tdee, Goal goal)
        {
            switch (goal)
            {
                case Goal.WeightLoss:
                    return tdee * 0.85; // -15%
                case Goal.Maintenance:
                    return tdee;
                case Goal.MuscleGain:
                    return tdee * 1.15; // +15%
                default:
                    return tdee;
            }
        }
    }
} 