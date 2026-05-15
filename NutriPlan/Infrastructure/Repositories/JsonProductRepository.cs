using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using NutriPlan.Domain;
using NutriPlan.Infrastructure.Interfaces;
using System;
using System.Diagnostics;

namespace NutriPlan.Infrastructure.Repositories
{
    /// <summary>
    /// Reads products from JSON file
    /// Singleton pattern ensures single instance
    /// Adapter pattern: JSON to Product domain model
    /// </summary>
    public class JsonProductRepository : IProductRepository
    {
        private readonly string _filePath;
        private static JsonProductRepository _instance;

        private JsonProductRepository(string filePath)
        {
            _filePath = filePath;
        }

        public static JsonProductRepository GetInstance(string filePath)
        {
            if (_instance == null)
            {
                _instance = new JsonProductRepository(filePath);
            }
            return _instance;
        }

        public List<Product> GetAllProducts()
        {
            var pathToUse = ResolvePath(_filePath);

            if (!File.Exists(pathToUse))
            {
                Debug.WriteLine($"JsonProductRepository: file not found at resolved path '{pathToUse}'");
                return new List<Product>();
            }

            string json;
            try
            {
                json = File.ReadAllText(pathToUse);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"JsonProductRepository: error reading file '{pathToUse}': {ex.Message}");
                return new List<Product>();
            }

            List<Product> products;
            try
            {
                products = JsonConvert.DeserializeObject<List<Product>>(json);
            }
            catch (JsonException jex)
            {
                Debug.WriteLine($"JsonProductRepository: JSON deserialization error: {jex.Message}");
                // Malformed JSON -> return empty list
                return new List<Product>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"JsonProductRepository: unexpected error during deserialization: {ex.Message}");
                return new List<Product>();
            }

            return products ?? new List<Product>();
        }

        private string ResolvePath(string configuredPath)
        {
            // If configuredPath exists as absolute or relative, prefer it
            try
            {
                if (!string.IsNullOrWhiteSpace(configuredPath))
                {
                    var full = Path.GetFullPath(configuredPath);
                    if (File.Exists(full)) return full;
                }
            }
            catch { /* ignore */ }

            // Try common candidate locations relative to running directory
            var candidates = new[]
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "products.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Data", "products.json"),
                Path.Combine(Environment.CurrentDirectory, "Data", "products.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json"),
                Path.Combine(Directory.GetCurrentDirectory(), "products.json"),
                "Data\\products.json",
                "products.json"
            };

            foreach (var c in candidates)
            {
                try
                {
                    var full = Path.GetFullPath(c);
                    if (File.Exists(full))
                    {
                        Debug.WriteLine($"JsonProductRepository: found products.json at '{full}'");
                        return full;
                    }
                }
                catch { }
            }

            // Fallback to original configured path (may be relative)
            return configuredPath ?? "Data\\products.json";
        }
    }
}