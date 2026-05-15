using NutriPlan.Application.Services;
using NutriPlan.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using NutriPlan.Infrastructure.Repositories;
using NutriPlan.Infrastructure.Interfaces;
using System.IO;
using System.Diagnostics;

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

        // Repository
        private IProductRepository _productRepository;

        // Loaded products
        private List<Product> _availableProducts = new List<Product>();
        public IReadOnlyList<Product> AvailableProducts => _availableProducts.AsReadOnly();

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

        private Dictionary<string, List<Product>> _mealProducts;
        public Dictionary<string, List<Product>> MealProducts
        {
            get => _mealProducts;
            set { _mealProducts = value; OnPropertyChanged(nameof(MealProducts)); }
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

            // Load products from JSON
            try
            {
                var jsonPath = ResolveProductsJsonPath();
                var repo = JsonProductRepository.GetInstance(jsonPath);
                _productRepository = repo;
                _availableProducts = repo.GetAllProducts() ?? new List<Product>();

                Debug.WriteLine($"MainViewModel: products.json path='{jsonPath}'");
                Debug.WriteLine($"MainViewModel: file exists='{File.Exists(jsonPath)}'");
                Debug.WriteLine($"MainViewModel: loaded products count={_availableProducts.Count}");
            }
            catch (Exception ex)
            {
                _availableProducts = new List<Product>();
                Debug.WriteLine($"MainViewModel: error loading products.json: {ex.Message}");
            }

            // Initial generation
            Generate();
        }

        /// <summary>
        /// Attempts to resolve the path to Data/products.json from several likely locations.
        /// </summary>
        private string ResolveProductsJsonPath()
        {
            // Try application base directory relative paths and project-relative Data folder
            var candidates = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "products.json"),
                Path.Combine(Environment.CurrentDirectory, "Data", "products.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json"),
                "Data\\products.json",
                "products.json"
            };

            foreach (var c in candidates)
            {
                var full = Path.GetFullPath(c);
                if (File.Exists(full)) return full;
            }

            // Fallback: return relative path as given (may not exist)
            return "Data\\products.json";
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

            // 5-6. Generate meals and products
            Dictionary<string, List<Product>> mealProducts;
            Meals = _mealDistributor.GenerateMealPlan(_productRepository, MacroResult, NumberOfMeals, out mealProducts);
            MealProducts = mealProducts;
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