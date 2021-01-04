using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODI.Services.ODC;

var OrderCode = "CRD00009";

string Deserialize(string serializedValue)
{
    var client = new OdcHttpClient(null);
    return $"{serializedValue.Substring(0, 2)}-{serializedValue.Substring(3)}";
}

string Deserialize2(string serializedValue) =>
             $"{serializedValue[0..2]}-{serializedValue[3..]}";

Console.WriteLine($"{Deserialize("93222")} {Deserialize2("93222")}");

//var result = GetIncrementedOrderCode();
//Console.WriteLine($"{result}");

//result = Base36GetNextValue(OrderCode);

//var car = new Car { Model = "Audi", Owner = "Heni" };
//var (model, owner) = car;

//Console.WriteLine($"Owner before: {owner}");

//car.Owner = "test";

//Console.WriteLine($"Owner after: {car.Owner}");

//Console.WriteLine($"The {model} of {owner}");
//Console.WriteLine("Hello .net 5");


//Person p = new("Heni", "Fazzani");
//Person p2 = new("heni", "Fazzani");

//var p3 = p with { Lastname = "Fazzani2" };

//var hash1 = HashCode.Combine(p.Firstname, p.Lastname);
//var hash2 = HashCode.Combine(p2.Firstname, p2.Lastname);
//var hash3 = HashCode.Combine(p3.Firstname, p3.Lastname);
//Console.WriteLine($"hash are Equal {hash1.Equals(hash2)}");
//Console.WriteLine($"hash are Equal {hash1.Equals(hash3)}");

//var result = p switch
//{
//    ("heni", _) or ("Heni", _) => "Yes",
//    ("Doudou", _) => "No",
//#pragma warning disable RCS1079 // Throwing of new NotImplementedException.
//    _ => throw new NotImplementedException()
//#pragma warning restore RCS1079 // Throwing of new NotImplementedException.
//};

Console.ReadKey();


string GetIncrementedOrderCode()
{
    long value = 0L;
    char[] characters =
        Enumerable.Range(0, '9' - '0' + 1).Select(i => (char)('0' + i))
        .Union(Enumerable.Range(0, 'Z' - 'A' + 1).Select(i => (char)('A' + i))).ToArray();

    for (int i = 0; i < OrderCode.Length; i++)
    {
        value *= characters.Length;
        var character = OrderCode[i];
        var characterValue = Array.IndexOf(characters, character);
        value += characterValue;
    }

    value++;
    var sb = new StringBuilder(13);
    do
    {
        sb.Insert(0, characters[(int)(value % 36)]);
        value /= 36;
    } while (value != 0);

    return sb.ToString();
}



const string BASE36 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
string Base36GetNextValue(ReadOnlySpan<char> value, int incrementBy = 1)
{
    var input = 0L;

    for (var i = 0; i < value.Length; ++i)
        input += BASE36.IndexOf(value[i]) * (long)Math.Pow(BASE36.Length, value.Length - i - 1);

    input += incrementBy;

    var clistarr = BASE36.AsSpan();
    var result = new Stack<char>();
    while (input != 0)
    {
        var index = input % 36;
        result.Push(clistarr[(int)index]);
        input /= 36;
    }
    return new string(result.ToArray());
}
