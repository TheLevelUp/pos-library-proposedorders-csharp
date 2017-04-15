using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrderCalculator.Tests
{
    [TestClass]
    public class UpdateSpendAmountTests
    {
        private const int SPEND = 555;

        [TestMethod]
        public void SpendAmountEqualsPayRequested()
        {
            const int PAYMENT_AMOUNT_REQUESTED = SPEND;

            Calculator.CalculateAdjustedSpendAmount(SPEND, PAYMENT_AMOUNT_REQUESTED).Should().Be(SPEND);
        }

        [TestMethod]
        public void SpendAmountLessThanPayRequested()
        {
            const int PAYMENT_AMOUNT_REQUESTED = SPEND + 200;

            Calculator.CalculateAdjustedSpendAmount(SPEND, PAYMENT_AMOUNT_REQUESTED).Should().Be(SPEND);
        }

        [TestMethod]
        public void SpendAmountGreaterThanPayRequested()
        {
            const int PAYMENT_AMOUNT_REQUESTED = SPEND - 200;

            Calculator.CalculateAdjustedSpendAmount(SPEND, PAYMENT_AMOUNT_REQUESTED)
                .Should()
                .Be(PAYMENT_AMOUNT_REQUESTED);
        }
    }
}
