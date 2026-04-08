using NutriPlan.Application.Services;
using NutriPlan.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;

namespace NutriPlan.Presentation.ViewModels
{
    /// <summary>
    /// ViewModel for the main application window
    /// Handles macro calculations and meal distribution
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        // Internal services
        private readonly CalorieCalculatorService _calorieCalculator = new CalorieCalculatorService();
        private readonly MacroCalculatorService _macroCalculator = new MacroCalculatorService();
        private readonly MealDistributionService _mealDistributor = new MealDistributionService();

        public event PropertyChangedEventHandler PropertyChanged;

        // Input properties
        private double _weight = 70.0;
        public double Weight
        {
            get => _weight;
            set { _weight = value; OnPropertyChanged(nameof(Weight)); }
        }

        private double _height = 175.0;
        public double Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(nameof(Height)); }
        }

        private int _age = 30;
        public int Age
        {
            get => _age;
            set { _age = value; OnPropertyChanged(nameof(Age)); }
        }

        private Gender _selectedGender = Gender.Male;
        public Gender SelectedGender
        {
            get => _selectedGender;
            set { _selectedGender = value; OnPropertyChanged(nameof(SelectedGender)); }
        }

        private ActivityLevel _selectedActivity = ActivityLevel.ModeratelyActive;
        public ActivityLevel SelectedActivity
        {
            get => _selectedActivity;
            set { _selectedActivity = value; OnPropertyChanged(nameof(SelectedActivity)); }
        }

        private Goal _selectedGoal = Goal.Maintenance;
        public Goal SelectedGoal
        {
            get => _selectedGoal;
            set { _selectedGoal = value; OnPropertyChanged(nameof(SelectedGoal)); }
        }

        private int _numberOfMeals = 3;
        public int NumberOfMeals
        {
            get => _numberOfMeals;
            set { _numberOfMeals = value; OnPropertyChanged(nameof(NumberOfMeals)); }
        }

        // Output properties
        private MacroResult _macroResult;
        public MacroResult MacroResult
        {
            get => _macroResult;
            set { _macroResult = value; OnPropertyChanged(nameof(MacroResult)); }
        }

        private List<Meal> _meals;
        public List<Meal> Meals
        {
            get => _meals;
            set { _meals = value; OnPropertyChanged(nameof(Meals)); }
        }

        // Available enums for UI binding
        public IEnumerable<ActivityLevel> ActivityLevels => Enum.GetValues(typeof(ActivityLevel)).Cast<ActivityLevel>();
        public IEnumerable<Gender> Genders => Enum.GetValues(typeof(Gender)).Cast<Gender>();
        public IEnumerable<Goal> Goals => Enum.GetValues(typeof(Goal)).Cast<Goal>();
        
        // Available meal counts (2-5)
        public IEnumerable<int> MealCounts => new[] { 2, 3, 4, 5 };

        public ICommand GenerateCommand { get; }

        public MainViewModel()
        {
            GenerateCommand = new RelayCommand(_ => Generate());
            
            // Initial generation
            Generate();
        }

        /// <summary>
        /// Validates input and calculates the meal plan
        /// </summary>
        private void Generate()
        {
            // Validate inputs
            if (Weight <= 0 || Height <= 0 || Age <= 0) return;
            if (NumberOfMeals < 2 || NumberOfMeals > 5) return;

            // 1. Calculate BMR
            double bmr = _calorieCalculator.CalculateBmr(Weight, Height, Age, SelectedGender);

            // 2. Calculate TDEE
            double tdee = _calorieCalculator.CalculateTdee(bmr, SelectedActivity);

            // 3. Calculate target calories based on goal
            double targetCalories = _calorieCalculator.CalculateTargetCalories(tdee, SelectedGoal);

            // 4. Calculate daily macro distribution (B/F/C)
            MacroResult = _macroCalculator.CalculateMacros(Weight, targetCalories, SelectedGoal);

            // 5. Evenly distribute macros across the selected number of meals
            Meals = _mealDistributor.DistributeMeals(MacroResult, NumberOfMeals);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Simple Command pattern implementation for WPF bindings
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        public event EventHandler CanExecuteChanged
        {
            add => System.Windows.Input.CommandManager.RequerySuggested += value;
            remove => System.Windows.Input.CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object> execute) => _execute = execute;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _execute(parameter);
    }
}