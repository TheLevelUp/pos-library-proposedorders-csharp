#region Copyright (Apache 2.0)
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
// <copyright file="ProposedOrderCalculator.cs" company="SCVNGR, Inc. d/b/a LevelUp">
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

namespace LevelUp.Pos.ProposedOrders
{
    public class AdjustedCheckValues
    {
        public int SpendAmount { get; }
        public int TaxAmount { get; }
        public int ExemptionAmount { get; }

        internal AdjustedCheckValues(int spendAmount, int taxAmount, int exemptionAmount)
        {
            SpendAmount = spendAmount;
            TaxAmount = taxAmount;
            ExemptionAmount = exemptionAmount;
        }

        public override string ToString()
        {
            return $"SpendAmount={SpendAmount};TaxAmount={TaxAmount};ExemptionAmount={ExemptionAmount};";
        }
    }

    internal class CheckData
    {
        public int PaymentAmount { get; set; }
        public int TaxAmount { get; set; }
        public int ExemptionAmount { get; set; }
        public int OutstandingAmount { get; set; }
        public int PreTaxSubtotal => OutstandingAmount - TaxAmount;
        public int NonExemptSubtotal => PreTaxSubtotal - ExemptionAmount;
    }

    public static class ProposedOrderCalculator
    {
        /// <summary>
        /// Accepts known values from the point-of-sale and gives you an AdjustedCheckValuesModel object containing the 
        /// spend_amount, tax_amount, and exemption_amount to submit a LevelUp Create Proposed Order API request.
        /// </summary>
        /// <param name="outstandingAmount">The current total amount of the check, including tax, in cents.</param>
        /// <param name="taxAmount">The current tax due on the check, in cents.</param>
        /// <param name="exemptAmount">The current total of exempted items on the check, in cents.</param>
        /// <param name="paymentAmount">The amount the customer would like to spend, in cents.</param>
        /// <returns>LevelUp.Pos.ProposedOrderCalculator.CalculateCreateProposedOrderValues</returns>
        public static AdjustedCheckValues CalculateCreateProposedOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount
        )
        {
            return CalculateOrderValues(
                outstandingAmount,
                taxAmount,
                exemptAmount,
                paymentAmount);
        }

        /// <summary>
        /// Accepts known values from the point-of-sale and gives you an AdjustedCheckValuesModel object containing the 
        /// spend_amount, tax_amount, and exemption_amount to submit a LevelUp Complete Order API request.
        /// </summary>
        /// <param name="outstandingAmount">The current total amount of the check, including tax, in cents.</param>
        /// <param name="taxAmount">The current tax due on the check, in cents.</param>
        /// <param name="exemptAmount">The current total of exempted items on the check, in cents.</param>
        /// <param name="paymentAmount">The amount the customer would like to spend, in cents.</param>
        /// <param name="appliedDiscountAmount">The discount amount applied to the point of sale for the customer.</param>
        /// <returns>LevelUp.Pos.ProposedOrderCalculator.CalculateCompleteOrderValues</returns>
        public static AdjustedCheckValues CalculateCompleteOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptAmount,
            int paymentAmount,
            int appliedDiscountAmount
        )
        {
            int outstandingAmountWithDiscount = outstandingAmount + Math.Abs(appliedDiscountAmount);

            return CalculateOrderValues(
                outstandingAmountWithDiscount,
                taxAmount,
                exemptAmount,
                paymentAmount);
        }

        internal static AdjustedCheckValues CalculateOrderValues(
            int outstandingAmount,
            int taxAmount,
            int exemptionAmount,
            int paymentAmount)
        {
            CheckData checkData = SanitizeData(outstandingAmount, taxAmount, exemptionAmount, paymentAmount);

            var adjustedTaxAmount = CalculateAdjustedTaxAmount(checkData);

            var adjustedExemptionAmount = CalculateAdjustedExemptionAmount(checkData);

            return new AdjustedCheckValues(checkData.PaymentAmount, adjustedTaxAmount, adjustedExemptionAmount);
        }

        internal static CheckData SanitizeData(int outstandingAmount, int taxAmount, int exemptionAmount, int paymentAmount)
        {
            var checkData = new CheckData
            {
                ExemptionAmount = exemptionAmount,
                OutstandingAmount = outstandingAmount,
                PaymentAmount = paymentAmount,
                TaxAmount = taxAmount
            };

            checkData.PaymentAmount = PaymentAmountCannotBeGreaterThanOutstandingAmount(
                checkData.OutstandingAmount, 
                checkData.PaymentAmount);

            checkData.TaxAmount = TaxAmountCannotBeGreaterThanOutstandingAmount(
                checkData.OutstandingAmount, 
                checkData.TaxAmount);

            checkData.ExemptionAmount = ExemptionAmountCannotBeGreaterThanPreTaxSubtotal(
                checkData.ExemptionAmount, 
                checkData.PreTaxSubtotal);

            return checkData;
        }

        /// <summary>
        /// For LevelUp Proposed/Complete Order, with partial payments, the last user to pay is responsible for paying
        /// the tax.
        /// </summary>
        /// <param name="checkData"></param>
        internal static int CalculateAdjustedTaxAmount(CheckData checkData)
        {
            if (checkData.TaxAmount > checkData.OutstandingAmount)
            {
                throw new Exception("Tax amount cannot be greater than total outstanding amount.");
            }

            bool isTaxFullyPaid = checkData.PaymentAmount >= checkData.OutstandingAmount;
            if (isTaxFullyPaid)
            {
                return checkData.TaxAmount;
            }

            return ZeroIfNegative(checkData.PaymentAmount - checkData.PreTaxSubtotal);
        }

        /// <summary>
        /// For LevelUp Proposed/Complete Order, with partial payments, the last user to pay bears the burden of
        /// exemption amounts. We allow anyone paying to use discount credit until the remaining amount owed is
        /// less than or equal to the amount of exempted items on the check.
        /// </summary>
        /// <param name="checkData"></param>
        internal static int CalculateAdjustedExemptionAmount(CheckData checkData)
        {
            if (checkData.ExemptionAmount > checkData.PreTaxSubtotal)
            {
                throw new Exception("Exemption amount cannot be greater that the pre-tax total on the check.");
            }

            bool isExemptFullyPaid = checkData.PaymentAmount >= checkData.PreTaxSubtotal;
            if (isExemptFullyPaid)
            {
                return checkData.ExemptionAmount;
            }

            return ZeroIfNegative(checkData.PaymentAmount - checkData.NonExemptSubtotal);
        }

        private static int PaymentAmountCannotBeGreaterThanOutstandingAmount(
            int outstandingAmount,
            int paymentAmount)
            => SmallerOrZeroIfNegative(outstandingAmount, paymentAmount);

        private static int TaxAmountCannotBeGreaterThanOutstandingAmount(int outstandingAmount, int taxAmount)
            => SmallerOrZeroIfNegative(taxAmount, outstandingAmount);

        private static int ExemptionAmountCannotBeGreaterThanPreTaxSubtotal(
            int exemptAmount,
            int outstandingAmount)
            => SmallerOrZeroIfNegative(exemptAmount, outstandingAmount);

        internal static int SmallerOrZeroIfNegative(int a, int b) => ZeroIfNegative(Math.Min(a, b));

        private static int ZeroIfNegative(int val) => Math.Max(0, val);
    }
}
