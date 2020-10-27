using System;
using System.Diagnostics.CodeAnalysis;

namespace ConsoleApp
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        private static void Main()
        {
            var car = new Car { Model = "Audi", Owner = "Heni" };
            var (model, owner) = car;

            Console.WriteLine($"The {model} of {owner}");
            Console.ReadKey();
        }
    }
}