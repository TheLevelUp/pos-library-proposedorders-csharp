using LevelUp.Pos.ProposedOrders.DiTalk.Models;
using LevelUp.Pos.ProposedOrders.DiTalk.Services;
using NSubstitute;
using NUnit.Framework;

namespace LevelUp.Pos.ProposedOrders.Tests.DiTalk
{
    [TestFixture]
    public class AdjustCheckServiceTests
    {
        [Test]
        public void AdjustProposedOrder_CallsThreeServices()
        {
            // Arrange
            var _d = Substitute.For<AdjustCheckService.IDependencies>();
            _d.DataSanitizerService.SanitizeData(Arg.Any<CheckModel>())
                .Returns(x => x[0]);
            var classUnderTest = new AdjustCheckService(_d);

            var check = new CheckModel
            {
                OutstandingAmount = 10,
                TaxAmount = 1,
                ExemptionAmount = 2,
                PaymentAmount = 5
            };

            // Act
            classUnderTest.AdjustProposedOrder(
                check.OutstandingAmount,
                check.TaxAmount,
                check.ExemptionAmount,
                check.PaymentAmount);

            // Assert
            _d.DataSanitizerService.Received(1)
                .SanitizeData(Arg.Is<CheckModel>(x => AreChecksEqual(check, x)));
            _d.AdjustTaxService.Received(1)
                .CalculateAdjustedTaxAmount(Arg.Is<CheckModel>(x => AreChecksEqual(check, x)));
            _d.AdjustExemptService.Received(1)
                .CalculateAdjustedExemptionAmount(Arg.Is<CheckModel>(x => AreChecksEqual(check, x)));
        }

        [Test]
        public void AdjustCompleteOrder_CalculateOutstandingAmount_ThenCallsThreeServices()
        {
            // Arrange
            var _d = Substitute.For<AdjustCheckService.IDependencies>();
            _d.MathWrapper.Abs(Arg.Any<int>()).Returns(x => x[0]);
            _d.DataSanitizerService.SanitizeData(Arg.Any<CheckModel>()).Returns(x => x[0]);
            var classUnderTest = new AdjustCheckService(_d);

            var preCalcOutstandingAmount = 10;
            var checkPostCalc = new CheckModel
            {
                OutstandingAmount = 12,
                TaxAmount = 1,
                ExemptionAmount = 2,
                PaymentAmount = 5
            };

            // Act
            classUnderTest.AdjustCompleteOrder(
                preCalcOutstandingAmount,
                checkPostCalc.TaxAmount,
                checkPostCalc.ExemptionAmount,
                checkPostCalc.PaymentAmount,
                2);

            // Assert
            _d.DataSanitizerService.Received(1)
                .SanitizeData(Arg.Is<CheckModel>(x => AreChecksEqual(checkPostCalc, x)));
            _d.AdjustTaxService.Received(1)
                .CalculateAdjustedTaxAmount(Arg.Is<CheckModel>(x => AreChecksEqual(checkPostCalc, x)));
            _d.AdjustExemptService.Received(1)
                .CalculateAdjustedExemptionAmount(Arg.Is<CheckModel>(x => AreChecksEqual(checkPostCalc, x)));
        }

        private bool AreChecksEqual(CheckModel a, CheckModel b)
        {
            return a.OutstandingAmount == b.OutstandingAmount
                   && a.ExemptionAmount == b.ExemptionAmount
                   && a.TaxAmount == b.TaxAmount
                   && a.PaymentAmount == b.PaymentAmount;
        }
    }
}
