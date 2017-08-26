using System.Collections.Generic;
using LevelUp.Pos.ProposedOrders.Tests.Mocks;
using NUnit.Framework;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestFixture]
    public class PermutationTest
    {
        public static IEnumerable<TestCaseData> DataSource()
        {
            var first = new TestData
            {
                CheckTotal = 2200,
                CheckTax = 200,
                ExemptionAmount = 0,
                SpendAmount = 1000,
                AvailableDiscountAmount = 100,
                PaymentAlreadyMade = 0,
                ExpectedProposedValues = new AdjustedCheckValues(1000, 0, 0),
                ExpectedCompleteValues = new AdjustedCheckValues(1000, 0, 0)
            };
            yield return new TestCaseData(first).SetName("LevelUp Then Cash");

            var second = new TestData
            {
                CheckTotal = 2200,
                CheckTax = 200,
                ExemptionAmount = 0,
                SpendAmount = 1200,
                AvailableDiscountAmount = 100,
                PaymentAlreadyMade = 1000,
                ExpectedProposedValues = new AdjustedCheckValues(1200, 200, 0),
                ExpectedCompleteValues = new AdjustedCheckValues(1190, 190, 0)
            };

            yield return new TestCaseData(second).SetName("Second Test");
        }

        [Test,TestCaseSource(nameof(DataSource))]
        public void SplitTenderWorkflow(TestData data)
        {
            Check check = new Check(data.CheckTotal, data.CheckTax);
            if (data.PaymentAlreadyMade > 0)
            {
                check.ApplyTender(data.PaymentAlreadyMade);
            }


            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                data.ExemptionAmount,
                data.SpendAmount);

            check.ApplyDiscount(data.AvailableDiscountAmount);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                data.ExemptionAmount,
                data.SpendAmount,
                data.AvailableDiscountAmount);

            Assert.AreEqual(data.ExpectedProposedValues.SpendAmount, proposedOrderValues.SpendAmount, "Proposed Spend Amount");
            Assert.AreEqual(data.ExpectedProposedValues.TaxAmount, proposedOrderValues.TaxAmount, "Proposed Tax Amount");
            Assert.AreEqual(data.ExpectedProposedValues.ExemptionAmount, proposedOrderValues.ExemptionAmount, "Proposed Exemption Amount");

            Assert.AreEqual(data.ExpectedCompleteValues.SpendAmount, completedOrderValues.SpendAmount, "Complete Spend Amount");
            Assert.AreEqual(data.ExpectedCompleteValues.TaxAmount, completedOrderValues.TaxAmount, "Complete Tax Amount");
            Assert.AreEqual(data.ExpectedCompleteValues.ExemptionAmount, completedOrderValues.ExemptionAmount, "Complete Exemption Amount");
        }

        public class TestData
        {
            public int CheckTotal { get; set; }
            public int CheckTax { get; set; }
            public int ExemptionAmount { get; set; }
            public int SpendAmount { get; set; }
            public int AvailableDiscountAmount { get; set; }
            public int PaymentAlreadyMade { get; set; }
            public AdjustedCheckValues ExpectedProposedValues { get; set; }
            public AdjustedCheckValues ExpectedCompleteValues { get; set; }
        }
    }
}
