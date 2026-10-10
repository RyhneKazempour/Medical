using System;
using Polly;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trying to access Polly v8 types...");
            
            // Try to create instances of v8 types
            var retryOpts = new Polly.Retry.RetryStrategyOptions();
            Console.WriteLine("RetryStrategyOptions created");
            
            var cbOpts = new Polly.CircuitBreaker.CircuitBreakerStrategyOptions();
            Console.WriteLine("CircuitBreakerStrategyOptions created");
            
            var timeoutOpts = new Polly.Timeout.TimeoutStrategyOptions();
            Console.WriteLine("TimeoutStrategyOptions created");
            
            Console.WriteLine("All types found!");
        }
    }
}