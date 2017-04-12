using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.CheckCalculator.Tests
{
    [TestClass]
    public class UpdateExemptionAmountTests
    {
        private const int SPEND = 987;

        [TestMethod]
        public void ExemptAmountEqualsSpendAmount()
        {
            const int EXEMPT = SPEND;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, EXEMPT).Should().Be(SPEND);
        }

        [TestMethod]
        public void ExemptAmountLessThanSpendAmount()
        {
            const int EXEMPT = SPEND - 300;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, EXEMPT).Should().Be(EXEMPT);
        }

        [TestMethod]
        public void ExemptAmountGreaterThanSpendAmount()
        {
            const int EXEMPT = SPEND + 300;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, EXEMPT).Should().Be(SPEND);
        }
    }
}