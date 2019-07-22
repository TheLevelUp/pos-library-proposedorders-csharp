#region Copyright (Apache 2.0)
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// <copyright file="UpdateExemptionAmountTests.cs" company="SCVNGR, Inc. d/b/a LevelUp">
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
using NUnit.Framework;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestFixture]
    public class UpdateExemptionAmountTests
    {
        // Too much is exempt
        [Test]
        [TestCase(1200, 900)] // Significantly over total
        [TestCase(1050, 900)] // Slightly over total
        public void CalculateOrderValues_WhenExemptAmountIsMoreThanTotal_ReturnsTotalAsExemptAmount(
            int exemptionAmount, int expectedExemptionAmount)
        {
            // Arrange
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1000;

            // Act
            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            // Assert
            Assert.AreEqual(expectedExemptionAmount, result);
        }

        // Order is fully exempt
        [Test]
        public void CalculateOrderValues_WhenProposedOrderRequestIsFullyExempt_ReturnsPaymentAmountMinusTax()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int exemptionAmount = 1000;
            int amountCustomerIsPaying = 1000;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(900, result);
        }

        // Order not exempt at all
        [Test]
        public void CalculateOrderValues_WhenProposedOrderRequestIsFullyNotExempt_ReturnsZero()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int exemptionAmount = 0;
            int amountCustomerIsPaying = 1000;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(0, result);
        }

        // Order is partially exempt
        [Test]
        [TestCase(100, 100)] // Small Amount Exempt
        [TestCase(899, 899)] // Mostly Exempt
        [TestCase(900, 900)] // Equal to Pre-Tax Subtotal
        [TestCase(901, 900)] // Greater than Pre-Tax Subtotal, less than total.
        public void CalculateOrderValues_WhenProposedOrderRequestIsPartiallyExempt_ReturnsPartOfTotal(
            int exemptionAmount, int expectedExemptionAmount)
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1000;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(expectedExemptionAmount, result);
        }

        // Too much is exempt
        [Test]
        [TestCase(1200, 500)] // More than Outstanding Total
        [TestCase(1050, 500)] // More than Payment Amount
        public void CalculateOrderValues_WhenExemptIsMoreThanSubtotalAndPaymentAmount_ReturnPaymentAmount(
            int exemptionAmount, int expectedExemptionAmount)
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 500;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(expectedExemptionAmount, result);
        }

        // Order is fully exempt
        [Test]
        public void CalculateOrderValues_WhenProposedOrderRequestIsFullyExempt_ReturnsPaymentAmount()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int exemptionAmount = 1000;
            int amountCustomerIsPaying = 500;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(500, result);
        }

        // Order not exempt at all
        [Test]
        public void CalculateOrderValues_WhenProposedOrderRequestIsFullyNotExemptWithPartialPayment_ReturnsZero()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int exemptionAmount = 0;
            int amountCustomerIsPaying = 500;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(0, result);
        }

        // Some of the order is exempt
        [Test]
        [TestCase(100, 0)] // Small amount is exempt

        // of the $9.00 subtotal, only $4.99 is exempt; $9.00 - $4.99 = $4.01 can be paid before claiming responsibility for exemption amounts
        [TestCase(499, 99)] // Most is exempt

        // of the $9.00 subtotal, only $5.00 is exempt; $9.00 - $5.00 = $4.00 can be paid before claiming responsibility for exemption amounts
        [TestCase(500, 100)] // Most is exempt

        // of the $9.00 subtotal, only $5.01 is exempt; $9.00 - $5.01 = $3.99 can be paid before claiming responsibility for exemption amounts
        [TestCase(501, 101)] // Mostly exempt, more than customer payment.

        // of the $9.00 subtotal, only $4.99 is exempt; $9.00 - $3.99 = $5.01 can be paid before claiming responsibility for exemption amounts
        [TestCase(399, 0)]

        // of the $9.00 subtotal, only $5.00 is exempt; $9.00 - $4.00 = $5.00 can be paid before claiming responsibility for exemption amounts
        [TestCase(400, 0)]

        // of the $9.00 subtotal, only $5.01 is exempt; $9.00 - $4.01 = $4.99 can be paid before claiming responsibility for exemption amounts
        [TestCase(401, 1)]
        public void CalculateOrderValues_WhenProposedOrderRequestIsPartiallyExempt(
            int exemptionAmount, int expectedExemptionAmount)
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 500;

            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            Assert.AreEqual(expectedExemptionAmount, result);
        }

        [Test]
        // of the $6.00 subtotal, only $4.99 is exempt; $6.00 - $3.99 = $2.01 can be paid before claiming responsibility for exemption amounts
        [TestCase(399, 399)] // Mostly Exempt

        // of the $6.00 subtotal, only $5.00 is exempt; $6.00 - $4.00 = $2.00 can be paid before claiming responsibility for exemption amounts
        [TestCase(400, 400)]

        // of the $6.00 subtotal, only $5.01 is exempt; $6.00 - $4.01 = $1.99 can be paid before claiming responsibility for exemption amounts
        [TestCase(401, 400)]
        public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_LessThan_CustomerPayment_Fringe(
            int exemptionAmount, int expectedExemptionAmount)
        {
            // Arrange
            int outstandingTotalOnCheck = 600;
            int taxAmount = 200;
            int amountCustomerIsPaying = 500;

            // Act
            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying).ExemptionAmount;

            // Assert
            Assert.AreEqual(expectedExemptionAmount, result);
        }
    }
}