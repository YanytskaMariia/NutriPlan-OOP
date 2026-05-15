# NutriPlan - Meal Generation System Documentation

## Overview
The meal generation system generates personalized meal plans from a product database (JSON file) based on user parameters and macro targets, while maintaining OOP principles.

## Architecture

### Domain Layer (`Domain/`)
- **Meal**: Base class representing a meal with name and macro targets
- **MealWithProducts**: Extended meal class containing actual products and their nutritional values
- **Product**: Food item with nutritional information (calories, protein, fat, carbs)
- **MacroResult**: Daily macro targets (proteins, fats, carbs, calories)

### Repository Layer (`Infrastructure/`)
- **IProductRepository**: Interface for product data access
- **JsonProductRepository**: Singleton implementation reading from JSON file

### Application Layer (`Application/`)

#### Services
- **CalorieCalculatorService**: Calculates BMR and TDEE based on user metrics
- **MacroCalculatorService**: Determines daily macro targets based on fitness goal
- **MealDistributionService**: Distributes daily macros across meals and generates meals with products

#### Strategy Pattern
- **IMealGenerationStrategy**: Interface for product selection algorithms
- **GreedyMealGenerationStrategy**: Fast greedy algorithm for product selection
- **BalancedMealGenerationStrategy**: Optimized algorithm matching macro ratios

### Presentation Layer (`Presentation/`)
- **MainViewModel**: Orchestrates calculations and meal generation, exposes data to UI

## Workflow

1. **User Input**: User provides weight, height, age, gender, activity level, fitness goal, and meal count
2. **Calorie Calculation**: BMR is calculated using Mifflin-St Jeor formula, then adjusted by activity level
3. **Goal Adjustment**: Calories are modified based on fitness goal (lose/maintain/gain)
4. **Macro Distribution**: Daily calories are split into protein, fat, and carbs percentages
5. **Meal Generation**: Macros are distributed evenly across the selected number of meals
6. **Product Selection**: The configured strategy selects products from the repository that best match macro targets
7. **Result**: Each meal contains actual products with their nutritional information

## OOP Principles Applied

### 1. **Dependency Injection**
- Services receive dependencies through constructors
- Easy to test and swap implementations

### 2. **Repository Pattern**
- Abstraction for data access via `IProductRepository`
- Can easily switch between JSON, SQL, or other data sources

### 3. **Strategy Pattern**
- `IMealGenerationStrategy` allows different algorithms
- Easy to add new selection strategies without modifying existing code

### 4. **Separation of Concerns**
- Calculator ? Meal Distribution ? Product Selection
- Each layer has a specific responsibility

### 5. **Inheritance**
- `MealWithProducts` extends `Meal` with product-specific functionality

## Configuration

### Data Source
Products are loaded from `Data/products.json`. Example format:

```json
[
  {
    "Name": "Chicken Breast",
    "Category": "Protein",
    "Calories": 165,
    "Protein": 31,
    "Fat": 3.6,
    "Carbs": 0
  }
]
```

### Strategy Selection
In `App.xaml.cs`, you can switch strategies:

```csharp
// Use BalancedMealGenerationStrategy (default)
var mealDistributor = new MealDistributionService(repo);

// Or use GreedyMealGenerationStrategy
var mealDistributor = new MealDistributionService(
    repo,
    new GreedyMealGenerationStrategy());
```

## Usage

### From ViewModel
```csharp
// MealsWithProducts contains the generated meal plan
var mealsWithProducts = _mealDistributor.GenerateMealsWithProducts(macroResult, mealCount);

foreach (var meal in mealsWithProducts)
{
    Console.WriteLine($"Meal: {meal.Name}");
    foreach (var product in meal.Products)
    {
        Console.WriteLine($"  - {product.Name}: {product.Calories} kcal");
    }
}
```

## Extending the System

### Adding a New Strategy
1. Create a class implementing `IMealGenerationStrategy`
2. Implement `SelectProducts()` method
3. Pass to `MealDistributionService` constructor

### Changing Data Source
1. Create a class implementing `IProductRepository`
2. Implement `GetAllProducts()` method
3. Register in `App.xaml.cs`

### Adding New Meal Types
Extend `MealWithProducts` or create specialized meal classes for specific meal types (Breakfast, Lunch, Dinner).

## Key Features

? Personalized macro calculations based on user profile
? Multiple meal generation strategies
? Product-based meal generation from JSON
? Clean OOP architecture with DI
? Repository pattern for data abstraction
? Strategy pattern for algorithm flexibility
? Extensible design for future enhancements
