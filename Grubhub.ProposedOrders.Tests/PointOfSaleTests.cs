using Grubhub.ProposedOrders.Tests.Mocks;
using NUnit.Framework;

namespace Grubhub.ProposedOrders.Tests
{
    [TestFixture]
    public class PointOfSaleTests
    {
        /// <summary>
        /// Simple test with (somewhat) arbitrary values that was added when a rounding error in the PointOfSale.cs
        /// class was discovered.
        /// </summary>
        [Test]
        public void PointOfSaleTests_RoundingTest1()
        {
            // Arrange
            PointOfSale pointOfSale = new PointOfSale(total: 1131, tax: 74);

            int exemptionAmount = 0;
            int spendAmount = 1131;

            // apply tender(s)
            pointOfSale.ApplyTender(0);

            var expectedProposedOrderValues = new AdjustedCheckValues(
                spendAmount: 1131, taxAmount: 74, exemptionAmount: 0);
            
            // Act
            // prepare Proposed Order call
            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount);
            
            // Assert
            Assert.AreEqual(expectedProposedOrderValues.ExemptionAmount, proposedOrderValues.ExemptionAmount);
            Assert.AreEqual(expectedProposedOrderValues.SpendAmount, proposedOrderValues.SpendAmount);
            Assert.AreEqual(expectedProposedOrderValues.TaxAmount, proposedOrderValues.TaxAmount);
             
            // Arrange
            // apply discount(s)
            int availableDiscountAmount = 0;

            pointOfSale.ApplyDiscount(availableDiscountAmount);

            var expectedCompletedOrderValues = new AdjustedCheckValues(
                spendAmount: 1131, taxAmount: 74, exemptionAmount: 0);
            
            // Act
            // prepare Completed Order call
            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            // Assert
            Assert.AreEqual(expectedCompletedOrderValues.ExemptionAmount, completedOrderValues.ExemptionAmount);
            Assert.AreEqual(expectedCompletedOrderValues.SpendAmount, completedOrderValues.SpendAmount);
            Assert.AreEqual(expectedCompletedOrderValues.TaxAmount, completedOrderValues.TaxAmount);
        }
    }
}
