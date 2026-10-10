using Microsoft.AspNetCore.RateLimiting;
using System.Reflection;
using System.Text;

var asm = typeof(RateLimiterOptions).Assembly;
var sb = new StringBuilder();
sb.AppendLine($"Assembly: {asm.FullName}");

var rateLimiterOptionsType = typeof(RateLimiterOptions);
sb.AppendLine($"\nMethods on RateLimiterOptions:");
foreach (var m in rateLimiterOptionsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
    .OrderBy(m => m.Name))
{
    sb.AppendLine($"  {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})");
}

sb.AppendLine($"\nAll extension methods from {asm.GetName().Name}:");
var extMethods = asm.GetTypes()
    .Where(t => t.IsSealed && !t.IsGenericType && !t.IsNested)
    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
    .Where(m => m.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false))
    .OrderBy(m => m.Name);
foreach (var m in extMethods)
{
    sb.AppendLine($"  {m.Name}({string.Join(", ", m.GetParameters().Select(p => p.ParameterType.Name))})");
}

Console.WriteLine(sb.ToString());