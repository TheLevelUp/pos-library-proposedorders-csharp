using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class UpdateSpendAmountTests
    {
        // Payment requested > total due
        [TestMethod]
        public void UpdateSpend_WhenProposedOrderRequestIs_PayingTooMuch()
        {
            int outstandingTotalOnCheck = 1000;
            int amountCustomerIsPaying = 1200;

            ProposedOrderCalculator.CalculateAdjustedCustomerPaymentAmount(outstandingTotalOnCheck, amountCustomerIsPaying)
                .Should()
                .Be(outstandingTotalOnCheck);
        }

        // Paid In Full
        [TestMethod]
        public void UpdateSpend_WhenProposedOrderRequestIs_PaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int amountCustomerIsPaying = 1000;

            ProposedOrderCalculator.CalculateAdjustedCustomerPaymentAmount(outstandingTotalOnCheck, amountCustomerIsPaying)
                .Should()
                .Be(amountCustomerIsPaying);
        }

        // Partial payment, payment requested < subtotal
        [TestMethod]
        public void UpdateSpend_WhenProposedOrderRequestIs_PartialPayment()
        {
            int outstandingTotalOnCheck = 1000;
            int amountCustomerIsPaying = 800;

            ProposedOrderCalculator.CalculateAdjustedCustomerPaymentAmount(outstandingTotalOnCheck, amountCustomerIsPaying)
                .Should()
                .Be(amountCustomerIsPaying);
        }

        // Zero dollar payment
        [TestMethod]
        public void UpdateSpend_WhenProposedOrderRequestIs_PayingZero()
        {
            int outstandingTotalOnCheck = 1000;
            int amountCustomerIsPaying = 0;

            ProposedOrderCalculator.CalculateAdjustedCustomerPaymentAmount(outstandingTotalOnCheck, amountCustomerIsPaying)
                .Should()
                .Be(amountCustomerIsPaying);
        }
    }
}
