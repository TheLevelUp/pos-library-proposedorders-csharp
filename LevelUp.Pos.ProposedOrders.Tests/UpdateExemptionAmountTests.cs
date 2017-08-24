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

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LevelUp.Pos.ProposedOrders.Tests
{
    [TestClass]
    public class UpdateExemptionAmountTests
    {
        #region Partial Payments

        [TestClass]
        public class PaidInFull
        {
            // Too much is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_TooMuchIsExempt_MoreThanTotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1200;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(900);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_TooMuchIsExempt_LessThanTotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1050;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(900);
            }

            // Order is fully exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_FullyExempt()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1000;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(900);
            }

            // Order not exempt at all
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_FullyNotExempt()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 0;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(0);
            }

            // Some of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_PartiallyExempt_SmallExemption()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 100;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(100);
            }

            // Most of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_LessThanSubtotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 899;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(899);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_EqualToSubtotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 900;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(900);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_GreaterThanSubtotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 901;
                int amountCustomerIsPaying = 1000;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(900);
            }
        }

        #endregion

        #region Partial Payments

        [TestClass]
        public class PartialPayments
        {
            // Too much is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_TooMuchIsExempt_MoreThanTotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1200;
                int amountCustomerIsPaying = 500;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(500);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_TooMuchIsExempt_LessThanTotal()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1050;
                int amountCustomerIsPaying = 500;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(500);
            }

            // Order is fully exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_FullyExempt()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 1000;
                int amountCustomerIsPaying = 500;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(500);
            }

            // Order not exempt at all
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_FullyNotExempt()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 0;
                int amountCustomerIsPaying = 500;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(0);
            }

            // Some of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_PartiallyExempt_SmallExemption()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 100;
                int amountCustomerIsPaying = 500;

                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(0);
            }

            // Most of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_LessThan_CustomerPayment()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 499;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $4.99 is exempt; $9.00 - $4.99 = $4.01 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(99);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_EqualTo_CustomerPayment()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 500;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $5.00 is exempt; $9.00 - $5.00 = $4.00 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(100);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_GreaterThan_CustomerPayment()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 501;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $5.01 is exempt; $9.00 - $5.01 = $3.99 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(101);
            }

            // Most of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_LessThan_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 399;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $4.99 is exempt; $9.00 - $3.99 = $5.01 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(0);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_EqualTo_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 400;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $5.00 is exempt; $9.00 - $4.00 = $5.00 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(0);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_GreaterThan_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 1000;
                int taxAmount = 100;
                int exemptionAmount = 401;
                int amountCustomerIsPaying = 500;

                // of the $9.00 subtotal, only $5.01 is exempt; $9.00 - $4.01 = $4.99 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(1);
            }
        }

        [TestClass]
        public class CherryPickedPartialPayments
        {
            // Most of the order is exempt
            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_LessThan_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 600;
                int taxAmount = 200;
                int exemptionAmount = 399;
                int amountCustomerIsPaying = 500;

                // of the $6.00 subtotal, only $4.99 is exempt; $6.00 - $3.99 = $2.01 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(399);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_EqualTo_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 600;
                int taxAmount = 200;
                int exemptionAmount = 400;
                int amountCustomerIsPaying = 500;

                // of the $6.00 subtotal, only $5.00 is exempt; $6.00 - $4.00 = $2.00 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(400);
            }

            [TestMethod]
            public void UpdateExemption_WhenProposedOrderRequestIs_MostlyExempt_GreaterThan_CustomerPayment_Fringe()
            {
                int outstandingTotalOnCheck = 600;
                int taxAmount = 200;
                int exemptionAmount = 401;
                int amountCustomerIsPaying = 500;

                // of the $6.00 subtotal, only $5.01 is exempt; $6.00 - $4.01 = $1.99 can be paid before claiming responsibility for exemption amounts
                ProposedOrderCalculator.CalculateOrderValues(outstandingTotalOnCheck, taxAmount, exemptionAmount, amountCustomerIsPaying)
                    .ExemptionAmount.Should()
                    .Be(400);
            }
        }

        #endregion
        }
}