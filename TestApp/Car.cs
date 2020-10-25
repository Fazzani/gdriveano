namespace TestApp
{
    public class Car
    {
        public string Model { get; set; }
        public string Owner { get; set; }

        public void Deconstruct(out string model, out string owner)
        {
            model = this.Model;
            owner = this.Owner;
        }
    }
}
