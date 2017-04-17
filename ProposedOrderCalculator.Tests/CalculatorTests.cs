using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace LevelUp.Pos.ProposedOrderCalculator.Tests
{
    public class CalculatorTests
    {
        [TestClass]
        public class AlternativeCalculatorTests
        {
            [TestMethod]
            public void RunTestBattery()
            {
                RunTestArray(Data.CalculatorTestData.TestBattery);
            }

            private void RunTestArray(int[,] values)
            {
                for (int i = 0; i < values.GetLength(0); i++)
                {
                    int totalOutstandingAmount = values[i, 0];
                    int totalOutstandintotalTaxAmountgAmount = values[i, 1];
                    int totalExemptionAmount = values[i, 2];
                    int spendAmount = values[i, 3];

                    int expectedSpendAmount = values[i, 4];
                    int expectedTaxAmount = values[i, 5];
                    int expectedExemptionAmount = values[i, 6];

                    AdjustedCheckValues expectedCheckValues =
                        new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

                    AdjustedCheckValues actualCheckValues = Calculator.CalculateAdjustedCheckValues(
                        totalOutstandingAmount,
                        totalOutstandintotalTaxAmountgAmount,
                        totalExemptionAmount,
                        spendAmount);

                    actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues,
                        "row={0}; {3}Exected:{1}; {3}Actual: {2}",
                        i, expectedCheckValues, actualCheckValues, Environment.NewLine);
                }
            }
        }
    }
}
