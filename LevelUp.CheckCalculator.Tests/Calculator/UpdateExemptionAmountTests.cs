using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.CheckCalculator.Tests.Calculator
{
    [TestClass]
    public class UpdateExemptionAmountTests
    {
        private const int SPEND = 987;

        [TestMethod]
        public void ExemptAmountEqualsSpendAmount()
        {
            const int exempt = SPEND;

            CheckCalculator.Calculator.CalculateAdjustedExemptionAmount(SPEND, exempt).Should().Be(SPEND);
        }

        [TestMethod]
        public void ExemptAmountLessThanSpendAmount()
        {
            const int exempt = SPEND - 300;

            CheckCalculator.Calculator.CalculateAdjustedExemptionAmount(SPEND, exempt).Should().Be(exempt);
        }

        [TestMethod]
        public void ExemptAmountGreaterThanSpendAmount()
        {
            const int exempt = SPEND + 300;

            CheckCalculator.Calculator.CalculateAdjustedExemptionAmount(SPEND, exempt).Should().Be(SPEND);
        }
    }
}
