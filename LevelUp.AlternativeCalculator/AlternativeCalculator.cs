using System;

namespace LevelUp.AlternativeCalculator
{
    public static class Calculator
    {
        public class AdjustedCheckValues
        {
            public int SpendAmount { get; set; }
            public int TaxAmount { get; set; }
            public int ExemptionAmount { get; set; }
        }

        public static AdjustedCheckValues CalculateAdjustedCheckValues(
            int totalOutstandingAmount,
            int totalTaxAmount,
            int totalExemptionAmount,
            int spendAmount
            )
        {
            int adjustedSpendAmount = Math.Max(0, Math.Min(spendAmount, totalOutstandingAmount));
            int adjustedTaxAmount = totalTaxAmount;
            int adjustedExemptionAmount = totalExemptionAmount;

            if (adjustedSpendAmount < totalOutstandingAmount)
            {
                int remainingAmountOwedAfterSpend = totalOutstandingAmount - adjustedSpendAmount;

                // if attempting a partial payment, reduce the exemption considered against the payment amount
                // by the difference between that payment amount and the actual total due
                adjustedTaxAmount = Math.Max(0, totalTaxAmount - remainingAmountOwedAfterSpend);
                adjustedExemptionAmount = Math.Max(0, totalExemptionAmount - remainingAmountOwedAfterSpend);
            }

            // finished
            return new AdjustedCheckValues()
            {
                SpendAmount = adjustedSpendAmount,
                TaxAmount = adjustedTaxAmount,
                ExemptionAmount = adjustedExemptionAmount
            };
        }
    }
}
