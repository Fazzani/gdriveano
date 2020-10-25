using System;

namespace TestApp
{
    public static class Extensions
    {
        public static void Deconstruct(this DateTime data, out int year, out int month, out int day) => (year, month, day) = data;
    }
}
