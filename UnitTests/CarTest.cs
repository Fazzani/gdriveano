using System.Collections.Concurrent;
using ConsoleApp;
using Xunit;

namespace UnitTests
{
    public class CarTest
    {
        [Fact]
        public void DestructCarTest()
        {
            const string expectedModel = "Audi";
            const string expectedOwner = "Heni";

            var car = new Car { Model = expectedModel, Owner = expectedOwner };
            var (model, owner) = car;

            Assert.Equal(expectedModel, model);
            Assert.Equal(expectedOwner, owner);
        }

        [Fact]
        public void DestructCarTest2()
        {
            const string expectedModel = null;
            var expectedOwner = string.Empty;

            var car = new Car { Model = expectedModel, Owner = expectedOwner };
            var (model, owner) = car;

            Assert.Equal(expectedModel, model);
            Assert.Equal(expectedOwner, owner);
        }

        [Fact]
        public void IsMineTest()
        {
            const string expectedModel = "Heni";
            var expectedOwner = string.Empty;

            var car = new Car { Model = expectedModel, Owner = expectedOwner };
            car.Model = "test";

            Assert.True(car.IsMine(expectedOwner));
        }
    }
}