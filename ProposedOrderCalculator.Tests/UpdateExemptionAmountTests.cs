using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrderCalculator.Tests
{
    [TestClass]
    public class UpdateExemptionAmountTests
    {
        private const int SPEND = 987;

        [TestMethod]
        public void ExemptAmountEqualsSpendAmount()
        {
            const int EXEMPT = SPEND;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, 0, EXEMPT, SPEND).Should().Be(SPEND);
        }

        [TestMethod]
        public void ExemptAmountLessThanSpendAmount()
        {
            const int EXEMPT = SPEND - 300;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, 0, EXEMPT, SPEND).Should().Be(EXEMPT);
        }

        [TestMethod]
        public void ExemptAmountGreaterThanSpendAmount()
        {
            const int EXEMPT = SPEND + 300;

            Calculator.CalculateAdjustedExemptionAmount(SPEND, 0, SPEND, EXEMPT).Should().Be(SPEND);
        }
    }
}