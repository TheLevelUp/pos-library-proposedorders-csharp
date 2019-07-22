#region Copyright (Apache 2.0)
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// <copyright file="UpdateTaxAmountTests.cs" company="SCVNGR, Inc. d/b/a LevelUp">
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
    public class UpdateTaxAmountTests
    {
        // Paid In Full
        [Test]
        public void CalculateOrderValues_WhenProposedOrderRequestIsPaidInFull_ReturnsFullTaxAmount()
        {
            // Arrange
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1000;
            
            // Act
            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, 0, amountCustomerIsPaying).TaxAmount;

            // Assert
            Assert.AreEqual(taxAmount, result);
        }

        [Test]
        [TestCase(950, 50)]  // Partial payment, payment requested > subtotal
        [TestCase(500, 0)] // Partial payment, payment requested < subtotal
        [TestCase(1, 0)]
        [TestCase(0, 0)] // Zero dollar payment; this order would get rejected by platform
        public void CalculateOrderValues_WhenProposedOrderRequestIsNotPaidInFull_UpdatesTaxAmount(
            int amountCustomerIsPaying, int expectedTax)
        {
            // Arrange
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;

            // Act
            var result = ProposedOrderCalculator.CalculateOrderValues(
                outstandingTotalOnCheck, taxAmount, 0, amountCustomerIsPaying).TaxAmount;

            // Assert
            Assert.AreEqual(expectedTax, result);
        }
    }
}