using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class UpdateTaxAmountTests
    {
        // Paid In Full
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequest_IsPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1000;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(taxAmount);
        }

        // Partial payment, payment requested > subtotal
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PartiallyPayingIntoTheTax()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 950;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(50);
        }

        // Partial payment, payment requested < subtotal
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_NotPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 500;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }

        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PayingOneCent()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }

        // Zero dollar payment; this order would get rejected by platform
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PayingNothing()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 0;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }
    }
}