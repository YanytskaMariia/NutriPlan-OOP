using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using Newtonsoft.Json;
using NutriPlan.Domain;
using NutriPlan.Infrastructure.Interfaces;

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
            if (!File.Exists(_filePath))
                return new List<Product>();

            string json = File.ReadAllText(_filePath);

            List<Product> products;
            try
            {
                products = JsonConvert.DeserializeObject<List<Product>>(json);
            }
            catch (JsonException)
            {
                // Malformed JSON -> return empty list
                return new List<Product>();
            }

            return products ?? new List<Product>();
        }
    }
}