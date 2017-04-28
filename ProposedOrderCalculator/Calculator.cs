using System;

namespace LevelUp.Pos.ProposedOrderCalculator
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
    }

    public static class Calculator
    {
        /// <summary>
        /// Accepts known values from the point-of-sale and gives you an AdjustedCheckValues object containing the 
        /// spend_amount, tax_amount, and exemption_amount to submit a LevelUp Create Proposed Order API request.
        /// </summary>
        /// <param name="totalOutstandingAmount">The current total amount of the check, including tax, in cents.</param>
        /// <param name="totalTaxAmount">The current tax due on the check, in cents.</param>
        /// <param name="totalExemptionAmount">The current total of exempted items on the check, in cents.</param>
        /// <param name="customerPaymentAmount">The amount the customer would like to spend, in cents.</param>
        /// <returns>LevelUp.Pos.ProposedOrderCalculator.CalculateCreateProposedOrderValues</returns>
        public static AdjustedCheckValues CalculateCreateProposedOrderValues(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int customerPaymentAmount
        )
        {
            int adjustedSpendAmount = CalculateAdjustedSpendAmount(customerPaymentAmount, totalOutstandingAmount);

            int adjustedTaxAmount = CalculateAdjustedTaxAmount(totalOutstandingAmount, totalTaxAmount,
                adjustedSpendAmount);

            int adjustedExemptionAmount = CalculateAdjustedExemptionAmount(totalOutstandingAmount, totalTaxAmount,
                totalExemptionAmount, adjustedSpendAmount);

            return new AdjustedCheckValues(adjustedSpendAmount, adjustedTaxAmount, adjustedExemptionAmount);
        }

        /// <summary>
        /// Accepts known values from the point-of-sale and gives you an AdjustedCheckValues object containing the 
        /// spend_amount, tax_amount, and exemption_amount to submit a LevelUp Complete Order API request.
        /// </summary>
        /// <param name="totalOutstandingAmount">The current total amount of the check, including tax, in cents.</param>
        /// <param name="totalTaxAmount">The current tax due on the check, in cents.</param>
        /// <param name="totalExemptionAmount">The current total of exempted items on the check, in cents.</param>
        /// <param name="customerPaymentAmount">The amount the customer would like to spend, in cents.</param>
        /// <param name="appliedDiscountAmount">The discount amount applied to the point of sale for the customer.</param>
        /// <returns>LevelUp.Pos.ProposedOrderCalculator.CalculateCompleteOrderValues</returns>
        public static AdjustedCheckValues CalculateCompleteOrderValues(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int customerPaymentAmount,
            int appliedDiscountAmount
        )
        {
            AdjustedCheckValues values = CalculateCreateProposedOrderValues(
                totalOutstandingAmount,
                totalTaxAmount,
                totalExemptionAmount,
                customerPaymentAmount);

            // For LevelUp Complete Proposed order, the spendAmount is expected to include the discount amount applied.
            // This method re-calculates the taxAmount and exemptionAmount based off known check values, but then
            // artifically adjusts the spendAmount to suit the context of the LevelUp Complete Proposed order.
            values.SpendAmount = CalculateAdjustedSpendAmount(customerPaymentAmount,
                totalOutstandingAmount + Math.Abs(appliedDiscountAmount));

            return values;
        }

        internal static int CalculateAdjustedSpendAmount(int totalOutstandingAmount, int spendAmount)
        {
            return Math.Max(0, Math.Min(spendAmount, totalOutstandingAmount));
        }

        internal static int CalculateAdjustedTaxAmount(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int adjustedSpendAmount)
        {
            bool wasPartialPaymentRequested = adjustedSpendAmount < totalOutstandingAmount;

            if (wasPartialPaymentRequested)
            {
                int remainingAmountOwedAfterSpend = totalOutstandingAmount - adjustedSpendAmount;

                int adjustedTaxAmount = Math.Max(0, totalTaxAmount - remainingAmountOwedAfterSpend);

                return adjustedTaxAmount;
            }

            return totalTaxAmount;
        }

        internal static int CalculateAdjustedExemptionAmount(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int adjustedSpendAmount)
        {
            int totalOutstandingAmountLessTax = totalOutstandingAmount - totalTaxAmount;

            // don't consider exemption amounts < $0
            // don't consider exemption amounts > spend amount
            // don't consider exemption amounts > than subtotal
            int adjustedExemptionAmount =
                Math.Max(0, Math.Min(Math.Min(totalExemptionAmount, totalOutstandingAmountLessTax), adjustedSpendAmount));

            bool wasPartialPaymentRequested = adjustedSpendAmount < totalOutstandingAmount;

            if (wasPartialPaymentRequested)
            {
                // defer the exemption amount to last possible paying customer or customers
                int totalOutstandingLessTaxAfterPayment = totalOutstandingAmountLessTax - adjustedSpendAmount;

                adjustedExemptionAmount = Math.Max(0, adjustedExemptionAmount - totalOutstandingLessTaxAfterPayment);
            }

            return adjustedExemptionAmount;
        }
    }
}
