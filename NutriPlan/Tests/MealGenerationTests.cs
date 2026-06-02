using Microsoft.VisualStudio.TestTools.UnitTesting;
using NutriPlan.Infrastructure.Repositories;
using System.IO;
using System;
using NutriPlan.Application.Services;
using NutriPlan.Domain;
using System.Linq;
using NutriPlan.Infrastructure.Interfaces;
using System.Collections.Generic;

namespace NutriPlan.Tests
{
    [TestClass]
    public class MealGenerationTests
    {
        private string ProductsPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json");

        [TestMethod]
        public void JsonRepository_LoadsProducts()
        {
            var repo = new JsonProductRepository(ProductsPath);
            var products = repo.GetAllProducts();
            Assert.IsNotNull(products);
            Assert.IsTrue(products.Count >= 20, "Expected at least 20 products from products.json");
        }

        [TestMethod]
        public void MacroCalculator_CalculatesTotals()
        {
            var svc = new MacroCalculatorService();
            var res = svc.CalculateMacros(70, 2000, Goal.Maintenance);
            Assert.IsTrue(res.TotalCalories > 0);
            Assert.IsTrue(res.ProteinGrams > 0);
            Assert.IsTrue(res.FatGrams > 0);
            Assert.IsTrue(res.CarbsGrams > 0);
        }

        [TestMethod]
        public void SequentialAndParallel_GenerateSameMealCount()
        {
            var repo = new JsonProductRepository(ProductsPath) as IProductRepository;
            var macros = new MacroResult { TotalCalories = 2000, ProteinGrams = 150, FatGrams = 70, CarbsGrams = 250 };
            int mealCount = 3;

            var mealService = new MealDistributionService();
            Dictionary<string, List<Product>> seqMap;
            var seqMeals = mealService.GenerateMealPlan(repo, macros, mealCount, out seqMap);

            var parallelService = new ParallelMealGenerationService();
            Dictionary<string, List<Product>> parMap;
            var parMeals = parallelService.GenerateMealPlanParallel(repo, macros, mealCount, out parMap);

            Assert.AreEqual(seqMeals.Count, parMeals.Count, "Meal count should match between sequential and parallel implementations");
            CollectionAssert.AreEqual(seqMeals.Select(m => m.Name).ToList(), parMeals.Select(m => m.Name).ToList());
        }

        [TestMethod]
        public void GreedyStrategy_SelectsProducts_NotEmpty()
        {
            var repo = new JsonProductRepository(ProductsPath);
            var products = repo.GetAllProducts();
            var strat = new NutriPlan.Application.Strategies.GreedyMealGenerationStrategy();
            var selected = strat.SelectProducts(products, 500, 30, 20, 50);
            Assert.IsNotNull(selected);
            Assert.IsTrue(selected.Count > 0, "Greedy strategy should select at least one product for 500 kcal target");
        }
    }
}
