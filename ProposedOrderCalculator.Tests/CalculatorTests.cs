using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;

namespace LevelUp.Pos.ProposedOrderCalculator.Tests
{
    [TestClass]
    public class CalculatorTests
    {
        const int TAX_RATE = 10; // used for examples

        [TestMethod]
        public void RunTestBattery()
        {
            RunTestArray(Data.CalculatorTestData.TestBattery);
        }

        [TestMethod]
        public void SplitTenderExample1()
        {
            // Split tender example: partial payment to LevelUp ($10) and remaining balance tendered to cash.
            // Check details (prior to LevelUp Scan)
            //   Item subtotal: $20
            //   tax (10%):      $2

            int itemsSubtotalAmount = 2000;
            int taxAmountDue = itemsSubtotalAmount / TAX_RATE;

            // User pays $10 towards LevelUp

            // Create propsed order: expected values
            int expectedTaxAmount = 0;
            int expectedSpendAmount = 1000;
            int expectedExemptionAmount = 0;
            AdjustedCheckValues expectedProposedOderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            // Total amount due on check, including tax
            int totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;

            // The current tax due on the check
            int totalTaxAmount = taxAmountDue;

            // example does not consider exemptions
            int totalExemptionAmount = 0;

            // The amount the user would like to spend
            int spendAmount = 1000;

            AdjustedCheckValues proposedOrderValues = Calculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Exected: {expectedProposedOderValues};\nActual: {proposedOrderValues}");


            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies pretax discount to check and updates subtotal and tax
            itemsSubtotalAmount -= availableDiscountAmount;
            taxAmountDue = itemsSubtotalAmount / TAX_RATE;
            totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;

            // Create completed order: expected values
            // tax amount unchanged
            // spend amount unchanged
            // expemption amount unchanged

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues completedOrderValues = Calculator.CalculateCompleteOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
               $"Exected: {expectedCompletedOrderValues};\nActual: {completedOrderValues}");
        }
        [TestMethod]
        public void SplitTenderExample2()
        {
            // Split tender example: partial payment to cash ($10) and remaining balance tendered to LevelUp.
            // Check details (prior to LevelUp Scan)
            //   Item subtotal: $20
            //   tax (10%):      $2
            //   TOTAL DUE:     $22

            int itemsSubtotalAmount = 2000;
            int taxAmountDue = itemsSubtotalAmount / TAX_RATE;

            // Cashier tenders $10 to cash
            // Updated check:
            //   PAID cash:     $10
            //   TOTAL DUE:     $12

            // User pays remaining balance ($12) towards LevelUp   

            // Create propsed order: expected values
            int expectedTaxAmount = 200;
            int expectedSpendAmount = 1200;
            int expectedExemptionAmount = 0;
            AdjustedCheckValues expectedProposedOderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            // Total amount due on check, including tax
            int paidCashAmount = 1000;
            int totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue - paidCashAmount;

            // The current tax due on the check
            int totalTaxAmount = taxAmountDue;

            // example does not consider exemptions
            int totalExemptionAmount = 0;

            // The amount the user would like to spend (remaining balance)
            int spendAmount = totalOutstandingAmount;

            AdjustedCheckValues proposedOrderValues = Calculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Exected: {expectedProposedOderValues};\nActual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies pretax discount to check and updates subtotal and tax
            itemsSubtotalAmount -= availableDiscountAmount;
            totalTaxAmount = itemsSubtotalAmount / TAX_RATE;
            totalOutstandingAmount = itemsSubtotalAmount + totalTaxAmount - paidCashAmount;
            spendAmount = totalOutstandingAmount + availableDiscountAmount;

            // Create completed order: expected values
            // adjusted tax amount: $1.90
            // spend amount unchanged: $11.90
            // expemption amount unchanged

            expectedTaxAmount = 190;
            expectedSpendAmount = 1190; /// CONFUSION: 1190 or 1090???

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues completedOrderValues = Calculator.CalculateCompleteOrderValues(
               totalOutstandingAmount,
               totalTaxAmount,
               totalExemptionAmount,
               spendAmount,
               availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
               $"Exected: {expectedCompletedOrderValues};\nActual: {completedOrderValues}");
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

                AdjustedCheckValues actualCheckValues = Calculator.CalculateCreateProposedOrderValues(
                    totalOutstandingAmount,
                    totalTaxAmount,
                    totalExemptionAmount,
                    spendAmount);

                actualCheckValues.ShouldBeEquivalentTo(expectedCheckValues,
                    "row={0}; {3}Exected:{1}; {3}Actual: {2}",
                    i, expectedCheckValues, actualCheckValues, Environment.NewLine);
            }
        }
    }
}
