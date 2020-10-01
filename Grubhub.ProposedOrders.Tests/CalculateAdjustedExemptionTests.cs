using NUnit.Framework;
using System;

namespace Grubhub.ProposedOrders.Tests
{
    [TestFixture]
    public class CalculateAdjustedExemptionTests
    {

        [Test]
        [TestCase(500, 0)]
        [TestCase(1, 0)]
        public void CalculateAdjustedExemptionAmount_PaymentZero_ReturnsZero(
            int exemptionAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = exemptionAmount,
                PaymentAmount = 0
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }


        [Test]
        [TestCase(500, 0)]
        [TestCase(400, 0)]
        [TestCase(499, 0)]
        public void CalculateAdjustedExemptionAmount_PaymentLessThanNonExempt_ReturnsZero(
            int exemptionAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = exemptionAmount,
                PaymentAmount = 1
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        [TestCase(801, 1)]
        [TestCase(900, 100)]
        [TestCase(999, 199)]
        public void CalculateAdjustedExemptionAmount_PaymentGreaterThanNonExemptButLessThanPreTaxSubtotal_ReturnsPartialAmount(
            int paymentAmount, int calculatedExemptionAmount)
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 200,
                PaymentAmount = paymentAmount
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        [TestCase(1000, 500)]
        [TestCase(1100, 500)]
        public void CalculateAdjustedExemptionAmount_PaymentAmountGreaterThanOrEqualToPreTaxSubtotal_ReturnsExemptionUnchanged(
            int paymentAmount, int calculatedExemptionAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 500,
                PaymentAmount = paymentAmount
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptionAmount, result);
        }

        [Test]
        [TestCase(1, 1)]
        [TestCase(500, 500)]
        [TestCase(999, 999)]
        public void CalculateAdjustedExemptionAmount_ExemptEqualToSubtotalandPaymentLessThanSubtotal_ReturnsPaymentAmount(
            int paymentAmount, int calculatedExemptedAmount)
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 1000,
                PaymentAmount = paymentAmount
            };

            // Act
            var result = ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.AreEqual(calculatedExemptedAmount, result);
        }

        [Test]
        public void CalculateAdjustedExemptionAmount_ExemptAmountGreaterThanPreTaxOutstanding_ThrowsException()
        {
            // Arrange
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 200,
                ExemptionAmount = 900,
                PaymentAmount = 500
            };

            // Act
            TestDelegate action = () => ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);

            // Assert
            Assert.Throws<Exception>(action);
        }
    }
}
