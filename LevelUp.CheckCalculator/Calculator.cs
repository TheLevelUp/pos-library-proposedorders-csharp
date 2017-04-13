using LevelUp.CheckCalculator.Models;

namespace LevelUp.CheckCalculator
{
    public static class Calculator
    {
        public static AdjustedCheckValues CalculateAdjustedCheckValues(int totalOutstandingAmountOnCheck,
            int totalTaxAmountOnCheck,
            int spendAmount,
            int? exemptionAmount)
        {
            int adjustedSpendAmount =
                CalculatorHelpers.CalculateAdjustedSpendAmount(totalOutstandingAmountOnCheck, spendAmount);

            int adjustedTaxAmount = CalculatorHelpers.CalculateAdjustedTax(adjustedSpendAmount,
                totalOutstandingAmountOnCheck, totalTaxAmountOnCheck);

            int adjustedExemptionAmount =
                CalculatorHelpers.CalculateAdjustedExemptionAmount(adjustedSpendAmount, exemptionAmount);

            return new AdjustedCheckValues(adjustedSpendAmount, adjustedTaxAmount, adjustedExemptionAmount);
        }
    }
}