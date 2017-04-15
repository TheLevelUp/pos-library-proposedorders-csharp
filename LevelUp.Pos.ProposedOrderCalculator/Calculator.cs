using System;

namespace LevelUp.Pos.ProposedOrderCalculator
{
    public class AdjustedCheckValues
    {
        public int SpendAmount { get; set; }
        public int TaxAmount { get; set; }
        public int ExemptionAmount { get; set; }

        public AdjustedCheckValues(int spendAmount, int taxAmount, int exemptionAmount)
        {
            SpendAmount = spendAmount;
            TaxAmount = taxAmount;
            ExemptionAmount = exemptionAmount;
        }

        public override string ToString() =>
            $"SpendAmount={SpendAmount};TaxAmount={TaxAmount};ExemptionAmount={ExemptionAmount}";
    }

    public class Calculator
    {
        public static AdjustedCheckValues CalculateAdjustedCheckValues(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int spendAmount
        )
        {
            int adjustedSpendAmount = CalculateAdjustedSpendAmount(spendAmount, totalOutstandingAmount);

            int adjustedTaxAmount =
                CalculateAdjustedTaxAmount(totalOutstandingAmount, totalTaxAmount, adjustedSpendAmount);

            int adjustedExemptionAmount =
                CalculateAdjustedExemptionAmount(totalOutstandingAmount, totalTaxAmount, totalExemptionAmount,
                    adjustedSpendAmount);

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
