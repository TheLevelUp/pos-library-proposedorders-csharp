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
            return new AdjustedCheckValues()
            {
                ExemptionAmount = CalculatorHelpers.CalculateAdjustedExemptionAmount(spendAmount, exemptionAmount),
                SpendAmount = CalculatorHelpers.CalculateAdjustedSpendAmount(totalOutstandingAmountOnCheck, spendAmount),
                TaxAmount = CalculatorHelpers.CalculateAdjustedTax(spendAmount, totalOutstandingAmountOnCheck, totalTaxAmountOnCheck)
            };
        }
    }
}