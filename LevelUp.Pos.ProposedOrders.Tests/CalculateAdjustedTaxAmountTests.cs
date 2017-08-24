using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class CalculateAdjustedTaxAmountTests
    {
        [TestMethod]
        public void PaymentLessThanPreTaxSubtotal_ReturnZero()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = 0
            };

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(0);

            checkData.PaymentAmount = 799;
            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(0);
        }

        [TestMethod]
        public void PaymentGreaterThanPreTaxSubtotalButLessThanOutstanding_ReturnTaxPaid()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = 801
            };

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(1);

            checkData.PaymentAmount = 900;
            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(100);

            checkData.PaymentAmount = 999;
            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(199);
        }

        [TestMethod]
        public void PaymentGreaterThanOrEqualToOutstandingAmount_ReturnsTaxAmount()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 200,
                PaymentAmount = 1000
            };

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(200);

            checkData.PaymentAmount = 1200;
            ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData)
                .Should().Be(200);
        }

        [TestMethod]
        public void TaxGreaterThanOutstanding_ThrowsException()
        {
            var checkData = new CheckData
            {
                OutstandingAmount = 1000,
                ExemptionAmount = 0,
                TaxAmount = 1100,
                PaymentAmount = 500
            };

            Action action = () => ProposedOrderCalculator.CalculateAdjustedTaxAmount(checkData);
            action.ShouldThrow<Exception>();
        }
    }
}
