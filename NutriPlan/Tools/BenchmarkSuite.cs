using System;
using System.Collections.Generic;
using System.IO;
using NutriPlan.Infrastructure.Repositories;
using NutriPlan.Application.Services;
using NutriPlan.Domain;

namespace NutriPlan.Tools
{
    /// <summary>
    /// Runs benchmarks for sequential and parallel meal generation across different degrees of parallelism and records CSV results.
    /// </summary>
    public static class BenchmarkSuite
    {
        public static void RunAll(string outputCsvPath)
        {
            var repoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json");
            var repo = new JsonProductRepository(repoPath);

            var macros = new MacroResult { TotalCalories = 2000, ProteinGrams = 150, FatGrams = 70, CarbsGrams = 250 };

            var mealService = new MealDistributionService();
            var parallelService = new ParallelMealGenerationService();

            var degrees = new[] { 1, 2, Environment.ProcessorCount, Math.Max(1, Environment.ProcessorCount * 2) };
            var mealCounts = new[] { 2, 3, 5 };

            // Prepare CSV
            var dir = Path.GetDirectoryName(outputCsvPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (var writer = new StreamWriter(outputCsvPath, false))
            {
                writer.WriteLine("Mode,MealCount,Threads,Iterations,AvgMs,Timestamp");
            }

            foreach (var mealCount in mealCounts)
            {
                // Sequential benchmark
                var seqAvg = BenchmarkRunner.Run($"Sequential_mc{mealCount}", () =>
                {
                    Dictionary<string, List<Domain.Product>> outMap;
                    var m = mealService.GenerateMealPlan(repo, macros, mealCount, out outMap);
                }, iterations: 5, csvPath: outputCsvPath);

                // Parallel benchmarks for various thread counts
                foreach (var d in degrees)
                {
                    var parAvg = BenchmarkRunner.Run($"Parallel_mc{mealCount}_t{d}", () =>
                    {
                        Dictionary<string, List<Domain.Product>> outMap;
                        var m = parallelService.GenerateMealPlanParallel(repo, macros, mealCount, out outMap, degreeOfParallelism: d);
                    }, iterations: 5, csvPath: outputCsvPath);
                }
            }
        }
    }
}
