using System;
using NUnit.Framework;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestFixture]
    public class CalculateAdjustedTaxAmountTests
    {
        [Test]
        [TestCase(0, 0)]
        [TestCase(799, 0)]
        public void CalculateAdjustedTaxAmount_PaymentLessThanPreTaxSubtotal_ReturnZero(
            int paymentAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = paymentAmount
            };
            
            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        [TestCase(801, 1)]
        [TestCase(900, 100)]
        [TestCase(999, 199)]
        public void CalculateAdjustedTaxAmount_PaymentGreaterThanPreTaxSubtotalButLessThanOutstanding_ReturnTaxPaid(
            int paymentAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = paymentAmount
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        [TestCase(1000, 200)]
        [TestCase(1200, 200)]
        public void CalculateAdjustedTaxAmount_PaymentGreaterThanOrEqualToOutstandingAmount_ReturnsTaxAmount(
            int paymentAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = 1000
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        public void CalculateAdjustedTaxAmount_TaxGreaterThanOutstanding_ThrowsException()
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 1100,
                PaymentAmount = 500
            };

            // Act
            TestDelegate action = () => ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData);

            // Assert
            Assert.Throws<Exception>(action);
        }
    }
}
