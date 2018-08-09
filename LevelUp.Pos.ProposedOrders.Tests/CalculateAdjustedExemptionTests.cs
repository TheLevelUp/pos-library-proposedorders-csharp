using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class CalculateAdjustedExemptionTests
    {

        [TestMethod]
        public void PaymentZero_ReturnsZero()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 500,
                PaymentAmount = 0
            };

            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(0);

            checkData.ExemptionAmount = 1;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(0);
        }

        [TestMethod]
        public void PaymentLessThanNonExempt_ReturnsZero()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 500,
                PaymentAmount = 1
            };

            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(0);

            checkData.PaymentAmount = 400;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(0);

            checkData.PaymentAmount = 499;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(0);
        }

        [TestMethod]
        public void PaymentGreaterThanNonExemptButLessThanPreTaxSubtotal_ReturnsPartialAmount()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 200,
                PaymentAmount = 801
            };
            // In this case, the partial amount should be payment - 800

            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(1);

            checkData.PaymentAmount = 900;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(100);

            checkData.PaymentAmount = 999;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(199);
        }

        [TestMethod]
        public void PaymentAmountGreaterThanOrEqualToPreTaxSubtotal_ReturnsExemptionUnchanged()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 500,
                PaymentAmount = 1000
            };
            // PreTaxSubtotal = 1000

            // Payment = PreTaxSubtotal
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(500);

            // Payment > PreTaxSubtotal
            checkData.PaymentAmount = 1100;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(500);
        }

        [TestMethod]
        public void ExemptEqualsPreTaxSubtotal_PaymentLessThanPreTaxSubtotal_ReturnPaymentAmount()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 0,
                ExemptionAmount = 1000,
                PaymentAmount = 1
            };

            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(1);

            checkData.PaymentAmount = 500;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(500);

            checkData.PaymentAmount = 999;
            ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData)
                .Should().Be(999);
        }

        [TestMethod]
        public void ExemptAmountGreaterThanPreTaxOutstanding_ThrowsException()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                TaxAmount = 200,
                ExemptionAmount = 900,
                PaymentAmount = 500
            };

            Action action = () => ProposedOrderCalculator.CalculateAdjustedExemptionAmount(checkData);
            action.Should().Throw<Exception>();
        }
    }
}
