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
using LevelUp.Pos.ProposedOrders.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class SplitTenderTests
    {
        [TestMethod]
        public void SplitTenderExample_LevelUp_Then_Cash()
        {
            // Split tender example: partial payment to LevelUp ($10) and remaining balance tendered to cash.
            // Check details (prior to LevelUp Scan)
            //   Item subtotal: $20
            //   tax (10%):      $2
            PointOfSale check = new PointOfSale(total: 2200, tax: 200);

            // User pays $10 towards LevelUp
            // example does not consider exemptions
            int exemptionAmount = 0;
            int spendAmount = 1000;

            // Create proposed order: expected values
            int expectedTaxAmount = 0;
            int expectedSpendAmount = 1000;
            int expectedExemptionAmount = 0;
            AdjustedCheckValues expectedProposedOderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Expected: {expectedProposedOderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies pretax discount to check and updates subtotal and tax
            check.ApplyDiscount(availableDiscountAmount);

            // Create completed order: expected values
            // tax amount unchanged
            // spend amount unchanged
            // exemption amount unchanged

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
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
            PointOfSale check = new PointOfSale(total: 2200, tax: 200);

            // Cashier tenders $10 to cash
            // Updated check:
            //   PAID cash:     $10
            //   TOTAL DUE:     $12
            check.ApplyTender(1000);

            // User pays remaining balance ($12) towards LevelUp
            // example does not consider exemptions
            int exemptionAmount = 0;
            int spendAmount = 1200;

            // Create proposed order: expected values
            int expectedTaxAmount = 200;
            int expectedSpendAmount = 1200;
            int expectedExemptionAmount = 0;
            AdjustedCheckValues expectedProposedOderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOderValues,
                $"Expected: {expectedProposedOderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies pretax discount to check and updates subtotal and tax
            check.ApplyDiscount(availableDiscountAmount);

            // Create completed order: expected values
            // adjusted tax amount: $1.90
            // spend amount unchanged: $11.90
            // exemption amount unchanged
            expectedTaxAmount = 190;
            expectedSpendAmount = 1190;

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(expectedSpendAmount, expectedTaxAmount, expectedExemptionAmount);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }
       
        [TestMethod]
        public void SplitTenderExample_NoChangeInSpendOrTax_AfterLevelUpPaidFirst()
        {
            // $9.90 is owed, $0.90 of that is tax. $3.00 of that is tobacco/alcohol, and the customer wants to pay
            // $9.00 towards the check
            PointOfSale check = new PointOfSale(total: 990, tax: 90);
            int exemptionAmount = 300;
            int spendAmount = 900;

            // Create proposed order: expected values
            AdjustedCheckValues expectedProposedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 900,
                    taxAmount: 0,
                    exemptionAmount: 300);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $1
            int availableDiscountAmount = 100;

            // POS applies $1.00 pretax discount to check and updates subtotal and tax
            check.ApplyDiscount(100);

            // $8.00 (Subtotal)
            // $0.80 (Tax)
            // $8.80 (Total)
            // $9.80 (Total + Applied Discount)

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 900,
                    taxAmount: 0,
                    exemptionAmount: 300);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }
        
        /// <summary>
        /// In this example, cash is applied first. The remaining balance can be covered in full by the LevelUp 
        /// discount credit available.
        /// </summary>
        /// <remarks>
        /// In this example, the second payment will still be responsible for some tax.
        /// </remarks>
        [TestMethod]
        public void SplitTenderExample_SpendAndTaxAreAdjusted_WhenLevelUpDiscountApplied()
        {
            // $22.00 is owed, $2.00 of that is tax. $0.00 of that is tobacco/alcohol, and the customer wants to pay
            // $12.00 towards the check all of which is discountable.
            PointOfSale check = new PointOfSale(total: 2200, tax: 200);
            int exemptionAmount = 0;
            int spendAmount = 1200;

            // Apply a non-LevelUp tender
            check.ApplyTender(1000);

            // Create proposed order: expected values
            AdjustedCheckValues expectedProposedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 1200,
                    taxAmount: 200,
                    exemptionAmount: 0);

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $10.00
            int availableDiscountAmount = 1000;

            // POS applies $10.00 pretax discount to check and updates subtotal and tax
            check.ApplyDiscount(availableDiscountAmount);

            // $ 0.00 (Subtotal Remaining)
            // $ 1.00 (Tax)
            // $ 1.00 (Total)
            // $11.00 (Total + Discount Applied) 

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 1100,      // spendAmount will include appliedDiscount and remaining tax
                    taxAmount: 100,         // the remaining portion of the tax will be paid
                    exemptionAmount: 0);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }

        /// <summary>
        /// This was an example where more than half of the check was discounted. This was added after a bug
        /// was found which could result in the tax_amount being incorrectly adjusted (increasing) on the 
        /// CalculateCompleteOrderValues(); call.
        /// </summary>
        [TestMethod]
        public void SplitTenderExample_NoChangeInSpendOrTax_WhenMostOfCheckIsDiscounted()
        {
            // $22.00 is owed, $2.00 of that is tax. $0.00 of that is tobacco/alcohol, and the customer wants to pay
            // $12.00 towards the check all of which is discountable.
            PointOfSale check = new PointOfSale(total: 1060, tax: 60);
            int exemptionAmount = 0;
            int spendAmount = 530;

            // Create proposed order: expected values
            AdjustedCheckValues expectedProposedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 530,       // amount the customer wants to pay
                    taxAmount: 0,           // no tax will be due for this payment; tax will be paid "at the end"
                    exemptionAmount: 0);    // there are no exemptions

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $5.30
            int availableDiscountAmount = 530;

            check.ApplyDiscount(availableDiscountAmount);

            // $ 4.70 (Subtotal Remaining)
            // $ 0.28 (Tax)
            // $ 4.98 (Total)
            // $10.28 (Total + Discount Applied) 

            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 530,      
                    taxAmount: 0,
                    exemptionAmount: 0);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                check.TotalOutstandingAmount,
                check.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }

        [TestMethod]
        public void SplitTenderExample_OneCentPaid_WhenOneCentOwed()
        {
            // $0.02 is owed, $0.01 of that is tax. $0.00 of that is tobacco/alcohol, and the customer wants to pay
            // $0.01 towards the check none of which is discountable (after a $0.01 cash payment).
            PointOfSale pointOfSale = new PointOfSale(total: 2, tax: 1);
            int exemptionAmount = 0;
            int spendAmount = 1;

            pointOfSale.ApplyTender(1);

            // Create proposed order: expected values
            AdjustedCheckValues expectedProposedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 1,         // amount the customer wants to pay
                    taxAmount: 1,           // tax will be due for this payment; tax will be paid "at the end"
                    exemptionAmount: 0);    // there are no exemptions

            AdjustedCheckValues proposedOrderValues = ProposedOrderCalculator.CalculateCreateProposedOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount);

            proposedOrderValues.ShouldBeEquivalentTo(expectedProposedOrderValues,
                $"Expected: {expectedProposedOrderValues}" + Environment.NewLine +
                $"Actual: {proposedOrderValues}");

            // available discount amount $0.00
            int availableDiscountAmount = 0;

            pointOfSale.ApplyDiscount(availableDiscountAmount);

            // $ 0.00 (Subtotal Remaining)
            // $ 0.01 (Tax)
            // $ 0.01 (Total)
            // $ 0.01 (Total + Discount Applied) 
            AdjustedCheckValues expectedCompletedOrderValues =
                new AdjustedCheckValues(
                    spendAmount: 1,
                    taxAmount: 1,
                    exemptionAmount: 0);

            AdjustedCheckValues completedOrderValues = ProposedOrderCalculator.CalculateCompleteOrderValues(
                pointOfSale.TotalOutstandingAmount,
                pointOfSale.TotalTaxAmount,
                exemptionAmount,
                spendAmount,
                availableDiscountAmount);

            completedOrderValues.ShouldBeEquivalentTo(expectedCompletedOrderValues,
                $"Expected: {expectedCompletedOrderValues}" + Environment.NewLine +
                $"Actual: {completedOrderValues}");
        }
    }
}