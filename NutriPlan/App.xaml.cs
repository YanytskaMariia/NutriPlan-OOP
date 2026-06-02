using NutriPlan.Presentation.ViewModels;
using NutriPlan.Presentation.Views;
using System.Windows;
using NutriPlan.Tools;
using NutriPlan.Application.Services;
using NutriPlan.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace NutriPlan
{
    public partial class App : System.Windows.Application
    {
        private MainWindow MainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Run optional benchmark if environment variable set to '1'
            try
            {
                var runBench = Environment.GetEnvironmentVariable("BENCHMARK_ON_STARTUP");
                if (!string.IsNullOrEmpty(runBench) && runBench == "1")
                {
                    RunBenchmark();
                }
            }
            catch { }
        }

        private void RunBenchmark()
        {
            try
            {
                // Prepare repo and services
                var jsonPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json");
                var repo = JsonProductRepository.GetInstance(jsonPath);

                var macros = new Domain.MacroResult { TotalCalories = 2000, ProteinGrams = 150, FatGrams = 70, CarbsGrams = 250 };
                int mealCount = 3;

                var mealService = new MealDistributionService();
                var parallelService = new ParallelMealGenerationService();

                string csv = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "benchmarks.csv");

                // Sequential
                BenchmarkRunner.Run("Sequential_GenerateMealPlan", () =>
                {
                    Dictionary<string, List<Domain.Product>> outMap;
                    var meals = mealService.GenerateMealPlan(repo, macros, mealCount, out outMap);
                }, iterations: 5, csvPath: csv);

                // Parallel
                BenchmarkRunner.Run("Parallel_GenerateMealPlan", () =>
                {
                    Dictionary<string, List<Domain.Product>> outMap;
                    var meals = parallelService.GenerateMealPlanParallel(repo, macros, mealCount, out outMap, degreeOfParallelism: Environment.ProcessorCount);
                }, iterations: 5, csvPath: csv);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Benchmark error: " + ex.Message);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            base.Shutdown();
            // Clean up resources if needed
        }
    }
}