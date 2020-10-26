using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var car = new Car { Model = "Audi", Owner = "Heni" };
            var (model, owner) = car;

            Console.WriteLine($"The {model} of {owner}");
            Console.ReadKey();
        }
    }
}
