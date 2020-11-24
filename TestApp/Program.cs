using System;
using ConsoleApp;

var car = new Car { Model = "Audi", Owner = "Heni" };
var (model, owner) = car;

Console.WriteLine($"Owner before: {owner}");

car.Owner = "test";

Console.WriteLine($"Owner after: {car.Owner}");

Console.WriteLine($"The {model} of {owner}");
Console.WriteLine("Hello .net 5");


Person p = new("Heni", "Fazzani");
Person p2 = new("heni", "Fazzani");

var p3 = p with { Lastname = "Fazzani2" };

var hash1 = HashCode.Combine(p.Firstname, p.Lastname);
var hash2 = HashCode.Combine(p2.Firstname, p2.Lastname);
var hash3 = HashCode.Combine(p3.Firstname, p3.Lastname);
Console.WriteLine($"hash are Equal {hash1.Equals(hash2)}");
Console.WriteLine($"hash are Equal {hash1.Equals(hash3)}");

var result = p switch
{
    ("heni", _) or ("Heni", _) => "Yes",
    ("Doudou", _) => "No",
#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
    _ => throw new NotImplementedException()
#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
};
Console.WriteLine($"{result}");
Console.ReadKey();
