using System;
using ConsoleApp;
using Xunit;

namespace UnitTests
{
    public class CarTest
    {
        [Fact]
        public void DestructCarTest()
        {
            var expectedModel = "Audi";
            var expectedOwner = "Heni";

            var car = new Car { Model = expectedModel, Owner = expectedOwner };
            var (model, owner) = car;

            Assert.Equal(expectedModel, model);
            Assert.Equal(expectedOwner, owner);
        }

        [Fact]
        public void DestructCarTest2()
        {
            string expectedModel = null;
            var expectedOwner = string.Empty;

            var car = new Car { Model = expectedModel, Owner = expectedOwner };
            var (model, owner) = car;

            Assert.Equal(expectedModel, model);
            Assert.Equal(expectedOwner, owner);
        }

        [Fact]
        public void IsMineTest()
        {
            string expectedModel = "Heni";
            var expectedOwner = string.Empty;

            var car = new Car { Model = expectedModel, Owner = expectedOwner };

            Assert.True(car.IsMine(expectedOwner));
        }
    }
}
