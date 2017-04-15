using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrderCalculator.Tests
{
    [TestClass]
    public class UpdateTaxAmountTests
    {
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIsPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int amountUserIsPaying = 1000;
            int taxAmount = 100;

            Calculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountUserIsPaying)
                .Should()
                .Be(taxAmount);
        }

        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIsNotPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int amountUserIsPaying = 500;
            int taxAmount = 100;

            Calculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountUserIsPaying)
                .Should()
                .Be(0);
        }

        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIsPartiallyPayingIntoTheTax()
        {
            int outstandingTotalOnCheck = 1000;
            int amountUserIsPaying = 950;
            int taxAmount = 100;

            Calculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountUserIsPaying)
                .Should()
                .Be(50);
        }
    }
}