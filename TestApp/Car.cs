using System;

namespace ConsoleApp
{
    //public class Car
    //{
    //    public string Model { get; init; }
    //    public string Owner { get; init; }

    //    public void Deconstruct(out string model, out string owner)
    //    {
    //        model = this.Model;
    //        owner = this.Owner;
    //    }

    //    public bool IsMine(string owner)
    //    {
    //        return Owner.Equals(owner, System.StringComparison.InvariantCultureIgnoreCase);
    //    }
    //}

    public record Car
    {
        public string Model { get; set; }
        public string Owner { get; set; }
        public bool IsMine(string owner) =>
            Owner.Equals(owner, System.StringComparison.InvariantCultureIgnoreCase);

        public void Deconstruct(out string model, out string owner)
        {
            model = this.Model;
            owner = this.Owner;
        }
    }

    public record Person(string Firstname, string Lastname);

    class MyClass
    {
        public static int X = 10;
        public static void Show()
        {
            X = 20;
        }
    }
}
