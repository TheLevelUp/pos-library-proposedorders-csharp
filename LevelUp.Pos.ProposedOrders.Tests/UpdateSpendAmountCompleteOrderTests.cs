using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class UpdateSpendAmountCompleteOrderTests
    {
        // Payment requested > total due
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PayingTooMuch_1200()
        {
            int amountCustomerIsPaying = 1200;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(1100);
        }

        // Payment requested > total due, fringe case
        // The customer wanted to pay $11.50, but after applying a discount of $1.00, we see only $11.00 could have 
        // been owed on the check; The `spend_amount` should have been/should be $11.00.
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PayingTooMuch_1150()
        {
            int amountCustomerIsPaying = 1150;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(1100);
        }

        // Paid in full
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PaidInFull()
        {
            int amountCustomerIsPaying = 1100;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(1100);
        }

        // Partial payment, payment requested < subtotal, fringe case
        // The customer wanted to pay $10.50, but after applying a discount of $1.00, we see that the balance was
        // $11.00; This is a partial payment, so the `spend_amount` should be equal to what the customer wanted to pay.
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PartialPayment_1050()
        {
            int amountCustomerIsPaying = 1050;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(1050);
        }

        // Partial payment, payment requested < subtotal
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PartialPayment_1000()
        {
            int amountCustomerIsPaying = 1000;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(1000);
        }

        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PartialPayment_900()
        {
            int amountCustomerIsPaying = 900;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(900);
        }

        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PartialPayment_800()
        {
            int amountCustomerIsPaying = 800;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(800);
        }

        // Zero dollar tender; discount credit only
        [TestMethod]
        public void UpdateSpend_WhenCompleteOrderRequestIs_PartialPayment_AllDiscount()
        {
            int amountCustomerIsPaying = 100;

            int discountAmountApplied = 100;
            int outstandingTotalOnCheck = 1000;

            ProposedOrderCalculator.CalculateAdjustedSpendAmountCompleteOrder(outstandingTotalOnCheck, amountCustomerIsPaying,
                    discountAmountApplied)
                .Should()
                .Be(100);
        }
    }
}
