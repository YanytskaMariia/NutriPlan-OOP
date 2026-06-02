# NutriPlan-OOP
Веб-застосунок, що автоматично рахує добову норму калорій і БЖВ на основі фізичних параметрів користувача та його цілей.
# Реалізовані патерни в NutriPlan

| Патерн                  | Клас / Місце реалізації                     | Роль / Як працює                                                                 |
|--------------------------|---------------------------------------------|----------------------------------------------------------------------------------|
| **Repository**           | `IProductRepository` + `JsonProductRepository` | Абстрагує доступ до джерела даних (`products.json`), повертає список продуктів.   |
| **Singleton**            | `JsonProductRepository.GetInstance(...)`    | Один екземпляр репозиторію для всього додатка.                                   |
| **Adapter**              | `JsonProductRepository`                     | Читає JSON та мапить у доменну модель `Product` (адаптація зовнішнього формату). |
| **Strategy**             | `IMealGenerationStrategy` + `GreedyMealGenerationStrategy`, `BalancedMealGenerationStrategy` | Логіка підбору продуктів інкапсульована у стратегіях; можна підмінити алгоритм. |
| **Service / Facade**     | `MealDistributionService`                   | Агрегує кроки: розподіл макросів → виклик репозиторію → підбір продуктів → план. |
| **MVVM**                 | `MainViewModel` + `MainWindow.xaml`         | ViewModel керує логікою, забезпечує дані для View.                               |
| **Command**              | `RelayCommand` у ViewModel                  | Командна взаємодія з UI (наприклад, `GenerateCommand`).                          |
| **Observer (Binding)**   | `INotifyPropertyChanged` у `MainViewModel`  | Оповіщення View про оновлення даних.                                             |
| **Domain Model / Inheritance** | `MealWithProducts` наслідує `Meal`    | Розширення доменної моделі для конкретної задачі.                                |
| **Internal Strategy fallback** | Усередині `MealDistributionService`   | Має внутрішню просту `DefaultStrategy` (greedy) як запасний варіант.             |
