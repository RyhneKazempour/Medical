using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        var asm = Assembly.LoadFile("C:\\Users\\user\\.nuget\\packages\\polly.core\\8.1.0\\lib\\netstandard2.0\\Polly.Core.dll");
        foreach (var t in asm.GetTypes())
        {
            if (t.Name.Contains("Pipeline") || t.Name.Contains("Resilience"))
            {
                Console.WriteLine($"TYPE: {t.FullName}");
                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance))
                {
                    Console.WriteLine($"  METHOD: {m.Name} -> {m.ReturnType.Name} ({string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name))})");
                }
            }
        }
    }
}