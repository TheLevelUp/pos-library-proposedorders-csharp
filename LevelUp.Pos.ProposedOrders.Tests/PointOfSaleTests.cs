using FluentAssertions;
using LevelUp.Pos.ProposedOrders.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class PointOfSaleTests
    {
        /// <summary>
        /// Simple test with (somewhat) arbitrary values that was added when a rounding error in the PointOfSale.cs
        /// class was discovered.
        /// </summary>
        [TestMethod]
        public void PointOfSaleTests_RoundingTest1()
        {
            PointOfSale pointOfSale = new PointOfSale(total: 1131, tax: 74);

            int exemptionAmount = 0;
            int spendAmount = 1131;

            // apply tender(s)
            pointOfSale.ApplyTender(0);

            // prepare Proposed Order call
            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(new AdjustedCheckValues(
                spendAmount: 1131,
                taxAmount: 74,
                exemptionAmount: 0));

            // apply discount(s)
            int availableDiscountAmount = 0;

            pointOfSale.ApplyDiscount(availableDiscountAmount);

            // prepare Completed Order call
            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(new AdjustedCheckValues(
                spendAmount: 1131,
                taxAmount: 74,
                exemptionAmount: 0));
        }
    }
}
