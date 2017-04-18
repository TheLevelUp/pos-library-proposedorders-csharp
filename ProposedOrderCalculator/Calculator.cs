using System;

namespace LevelUp.Pos.ProposedOrderCalculator
{
    public class AdjustedCheckValues
    {
        public int SpendAmount { get; }
        public int TaxAmount { get; }
        public int ExemptionAmount { get; }

        public AdjustedCheckValues(int spendAmount, int taxAmount, int exemptionAmount)
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
        /// spend_amount, tax_amount, and exemption_amount to submit a LevelUp API request to either Create or Complete 
        /// a Proposed Order.
        /// </summary>
        /// <param name="totalOutstandingAmount">The current total amount of the check, including tax, in cents.</param>
        /// <param name="totalTaxAmount">The current tax due on the check, in cents.</param>
        /// <param name="totalExemptionAmount">The current total of exempted items on the check, in cents.</param>
        /// <param name="spendAmount">The amount the user would like to spend, in cents.</param>
        /// <returns>LevelUp.Pos.ProposedOrderCalculator.AdjustedCheckValues</returns>
        public static AdjustedCheckValues CalculateAdjustedCheckValues(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int spendAmount
        )
        {
            int adjustedSpendAmount = CalculateAdjustedSpendAmount(spendAmount, totalOutstandingAmount);

            int adjustedTaxAmount = CalculateAdjustedTaxAmount(totalOutstandingAmount, totalTaxAmount, 
                adjustedSpendAmount);

            int adjustedExemptionAmount = CalculateAdjustedExemptionAmount(totalOutstandingAmount, totalTaxAmount,
                totalExemptionAmount, adjustedSpendAmount);

            return new AdjustedCheckValues(adjustedSpendAmount, adjustedTaxAmount, adjustedExemptionAmount);
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
            int adjustedTaxAmount = totalTaxAmount;

            bool wasPartialPaymentRequested = adjustedSpendAmount < totalOutstandingAmount;

            if (wasPartialPaymentRequested)
            {
                int remainingAmountOwedAfterSpend = totalOutstandingAmount - adjustedSpendAmount;

                return Math.Max(0, totalTaxAmount - remainingAmountOwedAfterSpend);
            }

            return adjustedTaxAmount;
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
                // defer the exemption amount to the end
                int totalOutstandingLessTaxAfterPayment = totalOutstandingAmountLessTax - adjustedSpendAmount;

                adjustedExemptionAmount = Math.Max(0, adjustedExemptionAmount - totalOutstandingLessTaxAfterPayment);
            }

            return adjustedExemptionAmount;
        }
    }
}
