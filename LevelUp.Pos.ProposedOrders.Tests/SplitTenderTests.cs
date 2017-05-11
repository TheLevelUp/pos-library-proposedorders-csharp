#region Copyright (Apache 2.0)
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// <copyright file="SplitTenderTests.cs" company="SCVNGR, Inc. d/b/a LevelUp">
//   Copyright(c) 2017 SCVNGR, Inc. d/b/a LevelUp. All rights reserved.
// </copyright>
// <license publisher="Apache Software Foundation" date="January 2004" version="2.0">
//   Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except
//   in compliance with the License. You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software distributed under the License
//   is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
//   or implied. See the License for the specific language governing permissions and limitations under
//   the License.
// </license>
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
#endregion

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class SplitTenderTests
    {
        const int TAX_RATE = 10; // Tax rate as a percent

        [TestMethod]
        public void SplitTenderExample_LevelUp_Then_Cash()
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

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Expected: {expectedProposedOderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

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

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }

        [TestMethod]
        public void SplitTenderExample_Cash_Then_LevelUp()
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

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Expected: {expectedProposedOderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

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
            expectedSpendAmount = 1190;

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }
       
        [TestMethod]
        public void SplitTenderExample_LevelUp_Then_Cash_With_Exemptions()
        {
            // $9.90 is owed, $0.90 of that is tax. $3.00 of that is tobacco/alcohol, and the customer wants to pay
            // $9.00 towards the check
            int itemsSubtotalAmount = 900;
            int taxAmountDue = itemsSubtotalAmount / TAX_RATE;
            int totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;
            int exemptionAmount = 300;

            int spendAmount = 900;

            // Create propsed order: expected values
            AdjustedCheckValues expectedProposedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 900,
                    taxAmount: 0,
                    exemptionAmount: 300);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                taxAmountDue,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies $1.00 pretax discount to check and updates subtotal and tax
            itemsSubtotalAmount -= availableDiscountAmount;
            taxAmountDue = itemsSubtotalAmount / TAX_RATE;
            totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;

            // Create completed order: expected values
            // tax amount unchanged
            // spend amount unchanged
            // expemption amount unchanged
            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 900,
                    taxAmount: 80,
                    exemptionAmount: 300);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                totalOutstandingAmount,
                taxAmountDue,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }

        // Adjusted Spend = No, Exemption = Yes, Tax = Yes
        [TestMethod]
        public void SplitTenderExample_LevelUp_Then_Cash_With_Adjusted_Exemption_And_Adjusted_Tax()
        {
            // $11.00 is owed, $1.00 of that is tax. $5.00 of that is tobacco/alcohol, and the customer wants to pay
            // $8.00 towards the check
            int itemsSubtotalAmount = 1000;
            int taxAmountDue = itemsSubtotalAmount / TAX_RATE;
            int totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;
            int exemptionAmount = 500;

            int spendAmount = 800;

            // Create propsed order: expected values
            AdjustedCheckValues expectedProposedOrderValues = 
                new AdjustedCheckValues(
                    spendAmount:800, 
                    taxAmount:0, 
                    exemptionAmount:300);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                taxAmountDue,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies $1.00 pretax discount to check and updates subtotal and tax
            itemsSubtotalAmount -= availableDiscountAmount;
            taxAmountDue = itemsSubtotalAmount / TAX_RATE;
            totalOutstandingAmount = itemsSubtotalAmount + taxAmountDue;

            // Create completed order: expected values
            // spend amount unchanged
            // tax amount unchanged
            // expemption amount *changed*
            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 800,
                    taxAmount: 0,
                    exemptionAmount: 400);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                totalOutstandingAmount,
                taxAmountDue,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }
    }
}