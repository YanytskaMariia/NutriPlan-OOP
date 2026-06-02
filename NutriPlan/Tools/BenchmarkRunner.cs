using System;
using System.Diagnostics;
using System.IO;

namespace NutriPlan.Tools
{
    public static class BenchmarkRunner
    {
        /// <summary>
        /// Run an action multiple times and return average elapsed milliseconds.
        /// Writes results to CSV file at output path if csvPath is provided.
        /// </summary>
        public static double Run(string label, Action work, int iterations = 5, string csvPath = null)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));
            if (iterations <= 0) iterations = 1;

            var sw = new Stopwatch();
            long total = 0;
            for (int i = 0; i < iterations; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                sw.Restart();
                work();
                sw.Stop();
                total += sw.ElapsedMilliseconds;
            }

            double avg = (double)total / iterations;

            try
            {
                if (!string.IsNullOrWhiteSpace(csvPath))
                {
                    var dir = Path.GetDirectoryName(csvPath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    var exists = File.Exists(csvPath);
                    using (var w = new StreamWriter(csvPath, true))
                    {
                        if (!exists)
                        {
                            w.WriteLine("Label,Iterations,AvgMs,Timestamp");
                        }
                        w.WriteLine($"{label},{iterations},{avg},{DateTime.UtcNow:O}");
                    }
                }
            }
            catch
            {
                // ignore IO errors for benchmarking
            }

            Debug.WriteLine($"Benchmark '{label}': avg {avg} ms over {iterations} runs");
            return avg;
        }
    }
}
