using System;
using System.Reflection;

namespace PollyInspection
{
    class Program
    {
        static void Main()
        {
            try
            {
                // Load Polly assembly from the actual resolved path
                var asmPath = @"C:\Users\user\.nuget\packages\polly\8.1.0\lib\net6.0\Polly.dll";
                Console.WriteLine($"Loading assembly from: {asmPath}");
                var asm = Assembly.LoadFrom(asmPath);
                
                Console.WriteLine("\n=== Polly Assembly Types ===");
                foreach (var type in asm.GetTypes())
                {
                    if (type.IsPublic && (type.Name.Contains("Strategy") || type.Name.Contains("Pipeline") || type.Name.Contains("Resilience")))
                    {
                        Console.WriteLine($"- {type.FullName}");
                    }
                }
                
                // Also check Polly.Extensions if it's a separate assembly
                var extPath = @"C:\Users\user\.nuget\packages\polly.extensions\8.1.0\lib\net6.0\Polly.Extensions.dll";
                Console.WriteLine($"\nLoading Polly.Extensions from: {extPath}");
                var extAsm = Assembly.LoadFrom(extPath);
                
                Console.WriteLine("\n=== Polly.Extensions Assembly Types ===");
                foreach (var type in extAsm.GetTypes())
                {
                    if (type.IsPublic && (type.Name.Contains("Strategy") || type.Name.Contains("Pipeline") || type.Name.Contains("Resilience")))
                    {
                        Console.WriteLine($"- {type.FullName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}