using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LevelUp.AlternativeCalculator;

namespace LevelUp.AlternativeCalculator.Tests
{
    [TestClass]
    public class AlternativeCalculatorTests
    {
        [TestMethod]
        public void Test1()
        {
            Calculator.AdjustedCheckValues x = new Calculator.AdjustedCheckValues();

            Assert.AreEqual(AlternativeCalculator.Calculator.CalculateAdjustedCheckValues(1,1,1,1), new Calculator.AdjustedCheckValues(1, 2, 3));
        }
    }
}
