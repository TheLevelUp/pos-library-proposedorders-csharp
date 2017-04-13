namespace LevelUp.CheckCalculator.Models
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
    }
}
