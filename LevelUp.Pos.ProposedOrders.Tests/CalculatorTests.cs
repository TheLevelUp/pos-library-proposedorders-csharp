using System;
using FluentAssertions;
using LevelUp.Pos.CheckCalculators.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        [TestMethod]
        public void RunTestBattery()
        {
            RunTestArray(CalculatorTestData.TestBattery);
        }

        private void RunTestArray(int[,] values)
        {
            for (int i = 0; i < values.GetLength(0); i++)
            {
                int totalOutstandingAmount = values[i, 0];
                int totalTaxAmount = values[i, 1];
                int totalExemptionAmount = values[i, 2];
                int spendAmount = values[i, 3];

                int expectedSpendAmount = values[i, 4];
                int expectedTaxAmount = values[i, 5];
                int expectedExemptionAmount = values[i, 6];

                AdjustedCheckValues expectedCheckValues =
                    new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

                AdjustedCheckValues actualCheckValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                    totalOutstandingAmount,
                    totalTaxAmount,
                    totalExemptionAmount,
                    spendAmount);

                actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues,
                    "row={0}; Expected:{1}; Actual: {2}",
                    i, expectedCheckValues, actualCheckValues, Environment.NewLine);
            }
        }

        [TestMethod]
        public void TestTest123()
        {
            int totalOutstandingAmount = 1;
            int totalTaxAmount = 10;
            int totalExemptionAmount = 0;
            int spendAmount = 1;

            AdjustedCheckValues expectedCheckValues =
                new AdjustedCheckValues(spendAmount:1, taxAmount:1, exemptionAmount:0);

            AdjustedCheckValues actualCheckValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues);
        }
    }
}