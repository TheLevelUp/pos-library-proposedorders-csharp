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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class UpdateTaxAmountTests
    {
        // Paid In Full
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequest_IsPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1000;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(taxAmount);
        }

        // Partial payment, payment requested > subtotal
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PartiallyPayingIntoTheTax()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 950;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(50);
        }

        // Partial payment, payment requested < subtotal
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_NotPaidInFull()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 500;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }

        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PayingOneCent()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 1;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }

        // Zero dollar payment; this order would get rejected by platform
        [TestMethod]
        public void UpdateTax_WhenProposedOrderRequestIs_PayingNothing()
        {
            int outstandingTotalOnCheck = 1000;
            int taxAmount = 100;
            int amountCustomerIsPaying = 0;

            ProposedOrderCalculator.CalculateAdjustedTaxAmount(outstandingTotalOnCheck, taxAmount, amountCustomerIsPaying)
                .Should()
                .Be(0);
        }
    }
}